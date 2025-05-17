/**
 * Entry point for the Mini Check-In App
 * This file handles platform-specific initialization for both Windows and macOS
 */

// Check if we're running in development mode on macOS
const isDevelopment = process.platform !== 'win32';

// Check if we're being run directly with 'npm start' or 'electron .'
const isElectronStart = process.argv.includes('.');

if (isDevelopment && !isElectronStart) {
  console.log('Running in development mode on macOS');
  console.log('Starting web server instead of Electron app...');
  
  // Start the web server instead of the Electron app
  require('./src/web-server.js');
} else {
  // Start the Electron app
  console.log(`Running Electron app on ${process.platform}`);
  // Use the electron module directly to avoid issues with app initialization
  const electron = require('electron');
  const path = require('path');
  
  const app = electron.app;
  const BrowserWindow = electron.BrowserWindow;
  
  let mainWindow;
  
  function createWindow() {
    mainWindow = new BrowserWindow({
      width: 900,
      height: 700,
      webPreferences: {
        preload: path.join(__dirname, 'src/preload.js'),
        contextIsolation: true,
        nodeIntegration: false,
        webSecurity: true,
        partition: 'nopersist'
      }
    });
    mainWindow.loadFile(path.join(__dirname, 'src/renderer/index.html'));
    mainWindow.on('closed', () => { mainWindow = null; });
    mainWindow.webContents.openDevTools();
  }
  
  app.on('ready', createWindow);
  app.on('window-all-closed', () => { if (process.platform !== 'darwin') app.quit(); });
  app.on('activate', () => { if (mainWindow === null) createWindow(); });
}
