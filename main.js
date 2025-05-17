// Import required modules
const electron = require('electron');
const path = require('path');
const url = require('url');

// Get app and BrowserWindow instances
const app = electron.app;
const BrowserWindow = electron.BrowserWindow;
const ipcMain = electron.ipcMain;

console.log('Starting Electron app...');
console.log('Electron version:', process.versions.electron);
console.log('Node version:', process.versions.node);

// Keep a global reference of the window object to avoid garbage collection
let mainWindow;

function createWindow() {
  // Create the browser window
  mainWindow = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
      preload: path.join(__dirname, 'src/preload.js'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  // Load the index.html file
  mainWindow.loadFile(path.join(__dirname, 'src/renderer/index.html'));

  // Open DevTools for debugging
  mainWindow.webContents.openDevTools();

  // Handle window close event
  mainWindow.on('closed', function() {
    mainWindow = null;
  });
}

// Create window when app is ready
app.on('ready', createWindow);

// Quit when all windows are closed
app.on('window-all-closed', function() {
  // On macOS, applications stay active until explicitly quit
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', function() {
  // On macOS, re-create window when dock icon is clicked and no windows are open
  if (mainWindow === null) {
    createWindow();
  }
});
