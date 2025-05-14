/**
 * WixSdkInspector.js
 * Inspects the Wix SDK structure to determine available methods
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Fail Fast and Learn: Implementing early failure detection
 * - Reflective Engineering: Building self-auditing capabilities
 */
const fs = require('fs');
const path = require('path');
const { createClient, OAuthStrategy } = require('@wix/sdk');
const { items } = require('@wix/data');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix SDK Inspector config');
  } catch (e) {
    console.error('Error loading Wix config for SDK Inspector:', e.message);
  }
}

module.exports = {
  /**
   * Inspect the Wix SDK structure
   */
  async inspectSdk() {
    try {
      console.log('Inspecting Wix SDK structure');
      
      // Get SDK version info
      const sdkInfo = {
        sdkVersion: require('@wix/sdk/package.json').version,
        dataVersion: require('@wix/data/package.json').version
      };
      
      console.log('SDK Versions:', sdkInfo);
      
      // Create the Wix client
      const myWixClient = createClient({
        modules: { items },
        auth: OAuthStrategy({ 
          clientId: WIX_CONFIG.clientId || '8efc3d0c-9cfb-4d5d-a596-91c4eaa38bb9' 
        }),
      });
      
      // Inspect the client structure
      const clientStructure = {
        hasItems: !!myWixClient.items,
        itemsMethods: Object.getOwnPropertyNames(
          Object.getPrototypeOf(myWixClient.items)
        ).filter(name => typeof myWixClient.items[name] === 'function'),
        itemsProperties: Object.keys(myWixClient.items)
      };
      
      // Inspect the items module structure
      const itemsModuleStructure = {
        exportedProperties: Object.keys(items),
        exportedMethods: Object.getOwnPropertyNames(
          Object.getPrototypeOf(items)
        ).filter(name => typeof items[name] === 'function')
      };
      
      return {
        success: true,
        sdkInfo,
        clientStructure,
        itemsModuleStructure,
        message: 'Successfully inspected Wix SDK structure'
      };
    } catch (err) {
      console.error('Wix SDK Inspector Error:', err);
      return {
        success: false,
        error: err.message,
        stack: err.stack
      };
    }
  }
};
