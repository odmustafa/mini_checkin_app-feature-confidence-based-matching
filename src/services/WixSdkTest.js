/**
 * WixSdkTest.js
 * Tests the Wix JavaScript SDK with headless client
 * Following the Ethereal Engineering Technical Codex principles:
 * - Fail Fast and Learn: Implement early failure detection and clear error reporting
 * - Boundary Protection: Implement strict interface contracts for external APIs
 */
const fs = require('fs');
const path = require('path');
const { createClient, OAuthStrategy } = require('@wix/sdk');
const { items: wixItems } = require('@wix/data');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix SDK Test config');
  } catch (e) {
    console.error('Error loading Wix config for SDK Test:', e.message);
  }
}

module.exports = {
  /**
   * Test the Wix JavaScript SDK with headless client
   */
  async testSdk(collectionId = "BannedNames") {
    try {
      console.log('Testing Wix SDK with client ID:', WIX_CONFIG.clientId);
      
      // Create the Wix client with the SDK
      const myWixClient = createClient({
        modules: { items: wixItems },
        auth: OAuthStrategy({ 
          clientId: WIX_CONFIG.clientId || '8efc3d0c-9cfb-4d5d-a596-91c4eaa38bb9' 
        }),
      });
      
      console.log('Wix client created, querying data collection:', collectionId);
      
      // Use the correct method from the latest Wix SDK
      // First try the new method structure
      try {
        // Query the data items using the current API
        const dataItemsList = await myWixClient.items.query({
          dataCollectionId: collectionId
        });
        
        console.log('Data items query completed');
        
        // Format the results
        const result = {
          total: dataItemsList.items?.length || 0,
          items: dataItemsList.items || [],
          itemIds: (dataItemsList.items || []).map(item => item._id || item.id || 'unknown')
        };
        
        return {
          success: true,
          result: result,
          message: `Successfully retrieved ${result.total} items from collection "${collectionId}"`
        };
      } catch (innerErr) {
        console.log('First query method failed, trying alternative method:', innerErr.message);
        
        // Try alternative method
        const dataItems = await myWixClient.items.find({
          dataCollectionId: collectionId
        });
        
        console.log('Alternative query method completed');
        
        // Format the results
        const result = {
          total: dataItems?.length || 0,
          items: dataItems || [],
          itemIds: (dataItems || []).map(item => item._id || item.id || 'unknown')
        };
        
        return {
          success: true,
          result: result,
          message: `Successfully retrieved ${result.total} items from collection "${collectionId}" (alt method)`
        };
      }
    } catch (err) {
      console.error('Wix SDK Test Error:', err);
      return {
        success: false,
        error: err.message,
        details: err.stack
      };
    }
  }
};
