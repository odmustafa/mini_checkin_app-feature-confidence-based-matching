const { app, BrowserWindow } = require('electron');

console.log('Electron app:', typeof app);
console.log('BrowserWindow:', typeof BrowserWindow);

function createWindow() {
  const win = new BrowserWindow({
    width: 800,
    height: 600
  });
  win.loadFile('index.html');
}

app.whenReady().then(() => {
  console.log('App is ready');
  createWindow();
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
