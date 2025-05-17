const electron = require('electron');
const { app, BrowserWindow } = electron;
const path = require('path');

let mainWindow;

function createWindow() {
  console.log('Creating Electron window...');
  
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
  
  console.log('Loading HTML file...');
  mainWindow.loadFile(path.join(__dirname, 'src/renderer/index.html'));
  
  mainWindow.on('closed', () => { 
    console.log('Window closed');
    mainWindow = null; 
  });
  
  // Open DevTools for debugging
  mainWindow.webContents.openDevTools();
}

console.log('Electron app starting...');
console.log('Electron version:', process.versions.electron);
console.log('Node version:', process.versions.node);

app.on('ready', () => {
  console.log('Electron app ready');
  createWindow();
});

app.on('window-all-closed', () => { 
  console.log('All windows closed');
  if (process.platform !== 'darwin') app.quit(); 
});

app.on('activate', () => { 
  console.log('App activated');
  if (mainWindow === null) createWindow(); 
});
