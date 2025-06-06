const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const ScanIDService = require('./services/ScanIDService');
const WixService = require('./services/WixService');
const WixApiExplorer = require('./services/WixApiExplorer');
const WixSdkTest = require('./services/WixSdkTest');
const WixSdkTestSimple = require('./services/WixSdkTestSimple');
const WixSdkInspector = require('./services/WixSdkInspector');
const WixSdkAdapter = require('./services/WixSdkAdapter');
const WixSdkCompatAdapter = require('./services/WixSdkCompatAdapter');
const WebhookService = require('./services/WebhookService');
const AnvizService = require('./services/AnvizService');

let mainWindow;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
      // Disable disk cache to avoid permission issues
      webSecurity: true,
      partition: 'nopersist'
    }
  });
  mainWindow.loadFile(path.join(__dirname, 'renderer/index.html'));
  mainWindow.on('closed', () => { mainWindow = null; });
}

app.whenReady().then(createWindow);
app.on('window-all-closed', () => { if (process.platform !== 'darwin') app.quit(); });
app.on('activate', () => { if (mainWindow === null) createWindow(); });

// IPC: Read latest Scan-ID CSV entry
ipcMain.handle('scanid:get-latest', async () => {
  return await ScanIDService.getLatestScan();
});

// IPC: Get watch status
ipcMain.handle('scanid:get-watch-status', async () => {
  return ScanIDService.getWatchStatus();
});

// IPC: Start watching for new scans
ipcMain.handle('scanid:start-watching', async () => {
  return ScanIDService.startWatching();
});

// IPC: Stop watching for new scans
ipcMain.handle('scanid:stop-watching', async () => {
  return ScanIDService.stopWatching();
});

// Set up watch callback to forward events to the renderer process
ScanIDService.registerWatchCallback((eventType, data) => {
  if (mainWindow) {
    mainWindow.webContents.send('scanid:watch-event', eventType, data);
  }
});

// IPC: Lookup Wix member by Scan-ID data
ipcMain.handle('wix:find-member', async (event, { firstName, lastName, dateOfBirth }) => {
  return await WixService.findMember({ firstName, lastName, dateOfBirth });
});

// IPC: Wix API Explorer handlers
ipcMain.handle('wix-explorer:get-config', async () => {
  return WixApiExplorer.getConfig();
});

ipcMain.handle('wix-explorer:get-endpoints', async () => {
  return WixApiExplorer.getAvailableEndpoints();
});

ipcMain.handle('wix-explorer:test-api', async (event, { endpointId, searchParams }) => {
  return await WixApiExplorer.testApiCall(endpointId, searchParams);
});

// IPC: Test Wix JavaScript SDK
ipcMain.handle('wix-sdk:test', async (event, { collectionId }) => {
  return await WixSdkTest.testSdk(collectionId);
});

// Simple Wix SDK Test handler
ipcMain.handle('wix-sdk:test-simple', async (event, { collectionId }) => {
  return await WixSdkTestSimple.testSdkSimple(collectionId);
});

// Wix SDK Inspector handler
ipcMain.handle('wix-sdk:inspect', async () => {
  return await WixSdkInspector.inspectSdk();
});

// Wix SDK Adapter handler
ipcMain.handle('wix-sdk:adapter-test', async (event, { collectionId }) => {
  return await WixSdkAdapter.testAdapter(collectionId);
});

// Direct API handlers removed - using only SDK as per requirements

// Wix SDK Compatibility Adapter handler
ipcMain.handle('wix-sdk:compat-test', async (event, { collectionId }) => {
  return await WixSdkCompatAdapter.testCompatAdapter(collectionId);
});

// Wix SDK Member Search handler
ipcMain.handle('wix-sdk:search-member', async (event, { firstName, lastName, dateOfBirth }) => {
  return await WixSdkAdapter.searchMember({ firstName, lastName, dateOfBirth });
});

// Wix SDK Query All Members handler
ipcMain.handle('wix-sdk:query-all-members', async () => {
  const adapter = new WixSdkAdapter();
  return await adapter.queryAllMembers();
});

// Wix SDK List Pricing Plan Orders handler
ipcMain.handle('wix-sdk:list-pricing-plan-orders', async (event, { filter }) => {
  console.log('Received request to list pricing plan orders with filter:', filter);
  return await WixSdkAdapter.listPricingPlanOrders(filter);
});

// App restart handler
ipcMain.handle('app:restart', () => {
  app.relaunch();
  app.exit();
});

// Webhook Server handlers

// Initialize webhook server
ipcMain.handle('webhook:initialize', async (event, options = {}) => {
  return await WebhookService.initialize(options);
});

// Start webhook server
ipcMain.handle('webhook:start', async (event, options = {}) => {
  return await WebhookService.start(options);
});

// Stop webhook server
ipcMain.handle('webhook:stop', async () => {
  return await WebhookService.stop();
});

// Get webhook server status
ipcMain.handle('webhook:status', () => {
  return WebhookService.getStatus();
});

// Generate new webhook secret
ipcMain.handle('webhook:generate-secret', () => {
  return { secret: WebhookService.generateWebhookSecret() };
});

// Set Wix App ID
ipcMain.handle('webhook:set-app-id', async (event, appId) => {
  return await WebhookService.setAppId(appId);
});

// Set Wix Public Key
ipcMain.handle('webhook:set-public-key', async (event, publicKey) => {
  return await WebhookService.setPublicKey(publicKey);
});

// Set up webhook event forwarding to the renderer process
WebhookService.on('webhook', (data) => {
  if (mainWindow) {
    mainWindow.webContents.send('webhook:event', data);
  }
});

// Anviz SDK handlers

// Initialize Anviz SDK
ipcMain.handle('anviz:initialize', async () => {
  return await AnvizService.initialize();
});

// Connect to Anviz device
ipcMain.handle('anviz:connect-device', async (event, options) => {
  return await AnvizService.connectDevice(options);
});

// Disconnect from Anviz device
ipcMain.handle('anviz:disconnect-device', async () => {
  return await AnvizService.disconnectDevice();
});

// Add user to Anviz device
ipcMain.handle('anviz:add-user', async (event, userData) => {
  return await AnvizService.addUser(userData);
});

// Start fingerprint enrollment
ipcMain.handle('anviz:start-finger-enrollment', async (event, enrollmentData) => {
  return await AnvizService.startFingerEnrollment(enrollmentData);
});

// Start event listener
ipcMain.handle('anviz:start-event-listener', async () => {
  return await AnvizService.startEventListener();
});

// Stop event listener
ipcMain.handle('anviz:stop-event-listener', async () => {
  return await AnvizService.stopEventListener();
});

// Set up Anviz event forwarding to the renderer process
AnvizService.registerCallback('all', (event) => {
  if (mainWindow) {
    mainWindow.webContents.send('anviz:event', event);
  }
});

// Clean up Anviz resources on app quit
app.on('will-quit', async () => {
  await AnvizService.cleanup();
});
