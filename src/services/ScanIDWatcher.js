/**
 * ScanIDWatcher.js
 * Watches for changes to the Scan-ID export CSV file and processes new scans
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts
 * - Fail Fast and Learn: Implementing early failure detection with fallback mechanisms
 * - Separation of Concerns: Maintaining clear boundaries between components
 */
const fs = require('fs');
const path = require('path');
const { parse } = require('csv-parse/sync');
const EventEmitter = require('events');

class ScanIDWatcher extends EventEmitter {
  constructor(csvPath) {
    super();
    this.csvPath = csvPath || path.join(__dirname, '../assets/scan-id-export.csv');
    this.watching = false;
    this.watcher = null;
    this.lastModified = null;
    this.lastScan = null;
    this.checkInterval = null;
    this.intervalMs = 2000; // Check every 2 seconds
    
    console.log('ScanIDWatcher initialized with path:', this.csvPath);
  }
  
  /**
   * Start watching the Scan-ID CSV file for changes
   */
  startWatching() {
    if (this.watching) {
      console.log('Already watching Scan-ID CSV file');
      return;
    }
    
    console.log('Starting to watch Scan-ID CSV file at:', this.csvPath);
    
    // Check if the file exists first
    if (!fs.existsSync(this.csvPath)) {
      const error = new Error(`Scan-ID CSV file not found at: ${this.csvPath}`);
      this.emit('error', error);
      return;
    }
    
    try {
      // Get the initial file stats
      const stats = fs.statSync(this.csvPath);
      this.lastModified = stats.mtime;
      
      // Set up the file watcher
      this.watcher = fs.watch(this.csvPath, (eventType) => {
        if (eventType === 'change') {
          this.checkForChanges();
        }
      });
      
      // Also set up an interval check as a backup mechanism
      // This helps on systems where file watching might be unreliable
      this.checkInterval = setInterval(() => {
        this.checkForChanges();
      }, this.intervalMs);
      
      this.watching = true;
      this.emit('watching', { path: this.csvPath });
      
      // Do an initial check for the latest scan
      this.checkForChanges();
    } catch (err) {
      console.error('Error starting Scan-ID watcher:', err);
      this.emit('error', err);
    }
  }
  
  /**
   * Stop watching the Scan-ID CSV file
   */
  stopWatching() {
    if (!this.watching) {
      return;
    }
    
    console.log('Stopping Scan-ID watcher');
    
    if (this.watcher) {
      this.watcher.close();
      this.watcher = null;
    }
    
    if (this.checkInterval) {
      clearInterval(this.checkInterval);
      this.checkInterval = null;
    }
    
    this.watching = false;
    this.emit('stopped');
  }
  
  /**
   * Check if the CSV file has changed and process any new scans
   */
  async checkForChanges() {
    try {
      // Check if the file exists
      if (!fs.existsSync(this.csvPath)) {
        console.log('Scan-ID CSV file not found during change check');
        return;
      }
      
      // Get the current file stats
      const stats = fs.statSync(this.csvPath);
      
      // Check if the file has been modified
      if (this.lastModified && stats.mtime <= this.lastModified) {
        // File hasn't changed
        return;
      }
      
      console.log('Scan-ID CSV file has changed, processing new scan');
      this.lastModified = stats.mtime;
      
      // Read and parse the CSV file
      const csvContent = fs.readFileSync(this.csvPath, 'utf8');
      const records = parse(csvContent, {
        columns: true,
        skip_empty_lines: true
      });
      
      if (!records || records.length === 0) {
        console.log('No scan records found in CSV');
        return;
      }
      
      // Sort by scan time (most recent first) and get the latest
      records.sort((a, b) => {
        const dateA = new Date(a.ScanTime || 0);
        const dateB = new Date(b.ScanTime || 0);
        return dateB - dateA;
      });
      
      const latestScan = records[0];
      
      // Check if this is a new scan
      if (this.lastScan && 
          this.lastScan.ScanTime === latestScan.ScanTime && 
          this.lastScan.IDNumber === latestScan.IDNumber) {
        // Same scan as before, no need to emit an event
        return;
      }
      
      // New scan detected
      this.lastScan = latestScan;
      console.log('New scan detected:', latestScan.FullName);
      
      // Emit the new scan event
      this.emit('newscan', latestScan);
    } catch (err) {
      console.error('Error checking for Scan-ID changes:', err);
      this.emit('error', err);
    }
  }
  
  /**
   * Get the latest scan from the CSV file
   */
  async getLatestScan() {
    try {
      // Check if the file exists
      if (!fs.existsSync(this.csvPath)) {
        throw new Error(`Scan-ID CSV file not found at: ${this.csvPath}`);
      }
      
      // Read and parse the CSV file
      const csvContent = fs.readFileSync(this.csvPath, 'utf8');
      const records = parse(csvContent, {
        columns: true,
        skip_empty_lines: true
      });
      
      if (!records || records.length === 0) {
        throw new Error('No scan records found in CSV');
      }
      
      // Sort by scan time (most recent first) and return the latest
      records.sort((a, b) => {
        const dateA = new Date(a.ScanTime || 0);
        const dateB = new Date(b.ScanTime || 0);
        return dateB - dateA;
      });
      
      return records[0];
    } catch (err) {
      console.error('Error getting latest scan:', err);
      throw err;
    }
  }
}

module.exports = ScanIDWatcher;
