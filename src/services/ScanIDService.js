const fs = require('fs');
const path = require('path');
const { parse } = require('csv-parse/sync');
const { execSync } = require('child_process');
const os = require('os');

// Use the local scan-id-export.csv file in the src/assets directory
const SCAN_ID_CSV_PATH = path.join(__dirname, '../assets/scan-id-export.csv');
const PYTHON_SORT_SCRIPT = path.join(__dirname, '../../src/sort_scan_id.py');

// Track the watching state and last scan time
let isWatching = false;
let watchInterval = null;
let lastScanTime = null;
let lastScanData = null;
let watchCallbacks = [];

module.exports = {
  getWatchStatus: function() {
    return { success: true, watching: isWatching };
  },
  
  startWatching: function() {
    if (isWatching) {
      return { success: true, watching: true, message: 'Already watching' };
    }
    
    try {
      // Get the current scan data to establish a baseline
      const currentScan = this.getLatestScan();
      if (!currentScan.error) {
        lastScanData = currentScan;
        lastScanTime = currentScan.ScanTime ? new Date(currentScan.ScanTime.split(' ')[0].replace(/\//g, '-')) : new Date();
      }
      
      // Set up the interval to check for new scans
      watchInterval = setInterval(() => {
        this.checkForNewScans();
      }, 5000); // Check every 5 seconds
      
      isWatching = true;
      
      // Notify all callbacks about the status change
      this.notifyCallbacks('status', { watching: true });
      
      return { success: true, watching: true };
    } catch (err) {
      console.error('Error starting watch:', err);
      return { success: false, error: err.message };
    }
  },
  
  stopWatching: function() {
    if (!isWatching) {
      return { success: true, watching: false, message: 'Not watching' };
    }
    
    try {
      // Clear the interval
      if (watchInterval) {
        clearInterval(watchInterval);
        watchInterval = null;
      }
      
      isWatching = false;
      
      // Notify all callbacks about the status change
      this.notifyCallbacks('status', { watching: false });
      
      return { success: true, watching: false };
    } catch (err) {
      console.error('Error stopping watch:', err);
      return { success: false, error: err.message };
    }
  },
  
  registerWatchCallback: function(callback) {
    // Add the callback to the list
    watchCallbacks.push(callback);
    
    // Return a function to unregister the callback
    return () => {
      const index = watchCallbacks.indexOf(callback);
      if (index !== -1) {
        watchCallbacks.splice(index, 1);
      }
    };
  },
  
  notifyCallbacks: function(eventType, data) {
    // Call all registered callbacks with the event type and data
    watchCallbacks.forEach(callback => {
      try {
        callback(eventType, data);
      } catch (err) {
        console.error('Error in watch callback:', err);
      }
    });
  },
  
  checkForNewScans: function() {
    try {
      // Get the latest scan
      const latestScan = this.getLatestScan();
      
      // If there was an error getting the scan, notify callbacks
      if (latestScan.error) {
        this.notifyCallbacks('error', { error: latestScan.error });
        return;
      }
      
      // If we don't have a last scan time, set it and return
      if (!lastScanTime) {
        lastScanData = latestScan;
        lastScanTime = latestScan.ScanTime ? new Date(latestScan.ScanTime.split(' ')[0].replace(/\//g, '-')) : new Date();
        return;
      }
      
      // Parse the scan time from the latest scan
      const scanTime = latestScan.ScanTime ? new Date(latestScan.ScanTime.split(' ')[0].replace(/\//g, '-')) : new Date();
      
      // If the scan time is newer than the last scan time, notify callbacks
      if (scanTime > lastScanTime) {
        console.log('New scan detected:', latestScan.FullName);
        lastScanTime = scanTime;
        lastScanData = latestScan;
        
        // Notify all callbacks about the new scan
        this.notifyCallbacks('newscan', latestScan);
      }
    } catch (err) {
      console.error('Error checking for new scans:', err);
      this.notifyCallbacks('error', { error: err.message });
    }
  },
  
  getLatestScan: function() {
    try {
      console.log('Looking for Scan-ID CSV at:', SCAN_ID_CSV_PATH);
      if (!fs.existsSync(SCAN_ID_CSV_PATH)) {
        console.error('Scan-ID CSV file not found at:', SCAN_ID_CSV_PATH);
        return { error: 'Scan-ID CSV file not found' };
      }
      
      // Create a temporary file path for the sorted copy
      const tempSortedFile = path.join(os.tmpdir(), `scan_id_sorted_${Date.now()}.csv`);
      
      // Run the Python script to create a sorted copy of the CSV file
      console.log('Creating sorted copy of CSV file...');
      let sortedFilePath = null;
      
      try {
        // Call the Python script to create a sorted copy
        const output = execSync(`python "${PYTHON_SORT_SCRIPT}" "${SCAN_ID_CSV_PATH}" "${tempSortedFile}"`, { encoding: 'utf8' });
        console.log(output);
        sortedFilePath = tempSortedFile;
      } catch (sortError) {
        console.error('Error creating sorted copy:', sortError.message);
        // If sorting fails, use the original file
        sortedFilePath = SCAN_ID_CSV_PATH;
      }
      
      // Read from the sorted file if available, otherwise from the original
      const csvContent = fs.readFileSync(sortedFilePath, 'utf8');
      console.log('CSV content loaded, parsing...');
      // Parse with standard comma delimiter
      const records = parse(csvContent, { 
        columns: true, 
        skip_empty_lines: true
      });
      if (!records.length) {
        console.warn('No records found in Scan-ID CSV');
        return { error: 'No records found in CSV file' };
      }
      console.log(`Found ${records.length} records, returning the latest one`);
      
      // Get the latest record (by created date/time)
      // Sort records by CREATED field in descending order
      records.sort((a, b) => {
        try {
          // Safely extract date parts with proper error handling
          const dateAStr = a.CREATED ? a.CREATED.split(' ')[0] : '';
          const dateBStr = b.CREATED ? b.CREATED.split(' ')[0] : '';
          
          if (!dateAStr || !dateBStr) {
            console.warn('Missing date in CREATED field');
            return 0; // Keep original order if dates are missing
          }
          
          const dateA = new Date(dateAStr.replace(/\//g, '-'));
          const dateB = new Date(dateBStr.replace(/\//g, '-'));
          
          return dateB - dateA;
        } catch (err) {
          console.error('Error sorting by date:', err);
          return 0; // Keep original order on error
        }
      });
      
      const latestRecord = records[0];
      
      // Map the Scan-ID CSV fields to our expected format
      return {
        FirstName: latestRecord['FIRST NAME'],
        LastName: latestRecord['LAST NAME'],
        FullName: latestRecord['FULL NAME'],
        DateOfBirth: latestRecord['BIRTHDATE'],
        Age: latestRecord['AGE'],
        IDNumber: latestRecord['DRV LC NO'],
        IDExpiration: latestRecord['EXPIRES ON'],
        IDIssued: latestRecord['ISSUED ON'],
        ScanTime: latestRecord['CREATED'],
        // Update the photo path to point to the new location with correct file extension
        PhotoPath: latestRecord['Image1'] ? path.join(__dirname, '../assets/scan-id-export-scan-demo', path.basename(latestRecord['Image1']).replace('.jpg', '.jpeg.jpeg')) : ''
      };
    } catch (err) {
      console.error('Error processing Scan-ID CSV:', err);
      return { error: err.message };
    }
  }
};
