// Minimal Electron app
const { app, BrowserWindow } = require('electron');
const path = require('path');

console.log('Starting minimal Electron app...');
console.log('Electron version:', process.versions.electron);
console.log('Node version:', process.versions.node);

const createWindow = () => {
  // Create the browser window
  const mainWindow = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
      preload: path.join(__dirname, 'src/preload.js'),
      contextIsolation: true,
      nodeIntegration: false
    },
  });

  // Load the index.html of the app
  mainWindow.loadFile(path.join(__dirname, 'src/renderer/index.html'));

  // Open the DevTools for debugging
  mainWindow.webContents.openDevTools();
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

// Quit when all windows are closed, except on macOS.
// There, it's common for applications and their menu bar
// to stay active until the user quits explicitly with Cmd + Q.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
