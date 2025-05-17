const { contextBridge, ipcRenderer } = require('electron');

// Expose Scan ID and Wix member lookup APIs
contextBridge.exposeInMainWorld('scanidAPI', {
  getLatestScan: () => ipcRenderer.invoke('scanid:get-latest'),
  findWixMember: (firstName, lastName, dateOfBirth) => ipcRenderer.invoke('wix:find-member', { firstName, lastName, dateOfBirth }),
  searchMemberByNameOrDOB: (name, dob) => ipcRenderer.invoke('wix-direct:search-member', { name, dob }),
  getMemberPricingPlans: (memberId) => ipcRenderer.invoke('wix-sdk:list-pricing-plan-orders', { filter: { buyerIds: [memberId] } }),
  listPricingPlanOrders: (filter) => ipcRenderer.invoke('wix-sdk:list-pricing-plan-orders', { filter }),
  // Add watch functionality
  getWatchStatus: () => ipcRenderer.invoke('scanid:get-watch-status'),
  startWatching: () => ipcRenderer.invoke('scanid:start-watching'),
  stopWatching: () => ipcRenderer.invoke('scanid:stop-watching'),
  onScanWatchEvent: (callback) => {
    // Create a listener for scan watch events
    const listener = (event, eventType, data) => callback(eventType, data);
    ipcRenderer.on('scanid:watch-event', listener);
    
    // Return a function to remove the listener
    return () => {
      ipcRenderer.removeListener('scanid:watch-event', listener);
    };
  }
});

// Expose Wix API Explorer functionality
contextBridge.exposeInMainWorld('wixExplorer', {
  getConfig: () => ipcRenderer.invoke('wix-explorer:get-config'),
  getEndpoints: () => ipcRenderer.invoke('wix-explorer:get-endpoints'),
  testApiCall: (endpointId, searchParams) => ipcRenderer.invoke('wix-explorer:test-api', { endpointId, searchParams })
});

// Expose Wix SDK test functionality
contextBridge.exposeInMainWorld('wixSdk', {
  testSdk: (collectionId) => ipcRenderer.invoke('wix-sdk:test', { collectionId }),
  testSdkSimple: (collectionId) => ipcRenderer.invoke('wix-sdk:test-simple', { collectionId }),
  inspectSdk: () => ipcRenderer.invoke('wix-sdk:inspect'),
  testAdapter: (collectionId) => ipcRenderer.invoke('wix-sdk:adapter-test', { collectionId }),
  testCompatAdapter: (collectionId) => ipcRenderer.invoke('wix-sdk:compat-test', { collectionId }),
  searchMember: (params) => ipcRenderer.invoke('wix-sdk:search-member', params),
  queryAllMembers: () => ipcRenderer.invoke('wix-sdk:query-all-members')
});

// Expose Wix Direct API functionality
contextBridge.exposeInMainWorld('wixDirect', {
  testApi: (endpoint) => ipcRenderer.invoke('wix-direct:test', { endpoint })
});

// Expose Electron API functionality
contextBridge.exposeInMainWorld('electronAPI', {
  restartApp: () => ipcRenderer.invoke('app:restart')
});

// Expose Webhook Server API functionality
contextBridge.exposeInMainWorld('webhookAPI', {
  initialize: (options) => ipcRenderer.invoke('webhook:initialize', options),
  start: (options) => ipcRenderer.invoke('webhook:start', options),
  stop: () => ipcRenderer.invoke('webhook:stop'),
  getStatus: () => ipcRenderer.invoke('webhook:status'),
  generateSecret: () => ipcRenderer.invoke('webhook:generate-secret'),
  setAppId: (appId) => ipcRenderer.invoke('webhook:set-app-id', appId),
  setPublicKey: (publicKey) => ipcRenderer.invoke('webhook:set-public-key', publicKey),
  onWebhookEvent: (callback) => {
    // Create a listener for webhook events
    const listener = (event, data) => callback(data);
    ipcRenderer.on('webhook:event', listener);
    
    // Return a function to remove the listener
    return () => {
      ipcRenderer.removeListener('webhook:event', listener);
    };
  }
});

// Anviz SDK API functionality has been removed
