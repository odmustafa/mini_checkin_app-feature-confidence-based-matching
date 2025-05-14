/**
 * WixSdkTestSimple.js
 * A version-specific implementation for testing the Wix JavaScript SDK
 * Compatible with @wix/sdk@1.15.18 and @wix/data@1.0.222
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Fail Fast and Learn: Implementing clear error reporting
 * - Boundary Protection: Using strict interface contracts for the Wix API
 */
const fs = require('fs');
const path = require('path');
const { items } = require('@wix/data');
const { createClient, OAuthStrategy } = require('@wix/sdk');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let CLIENT_ID = '8efc3d0c-9cfb-4d5d-a596-91c4eaa38bb9'; // Default fallback

if (fs.existsSync(CONFIG_PATH)) {
  try {
    const config = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    if (config.clientId) {
      CLIENT_ID = config.clientId;
    }
    console.log('Loaded Wix SDK Simple Test config, client ID:', CLIENT_ID);
  } catch (e) {
    console.error('Error loading Wix config for simple SDK Test:', e.message);
  }
}

module.exports = {
  /**
   * Test the Wix JavaScript SDK with version-specific code
   * Compatible with @wix/sdk@1.15.18 and @wix/data@1.0.222
   */
  async testSdkSimple(collectionId = "BannedNames") {
    try {
      console.log('Running version-specific SDK test with client ID:', CLIENT_ID);
      
      // Create the client with the correct module structure for your SDK version
      const myWixClient = createClient({
        modules: { items },
        auth: OAuthStrategy({ clientId: CLIENT_ID }),
      });
      
      console.log('Simple Wix client created, querying collection:', collectionId);
      
      // Try multiple API patterns to find the one that works with your SDK version
      let dataItems = null;
      let method = '';
      
      try {
        // Method 1: Direct query
        method = 'query';
        dataItems = await myWixClient.items.query({
          dataCollectionId: collectionId
        });
        console.log('Query method succeeded');
      } catch (err1) {
        console.log('Query method failed:', err1.message);
        
        try {
          // Method 2: Find
          method = 'find';
          dataItems = await myWixClient.items.find({
            dataCollectionId: collectionId
          });
          console.log('Find method succeeded');
        } catch (err2) {
          console.log('Find method failed:', err2.message);
          
          try {
            // Method 3: List
            method = 'list';
            dataItems = await myWixClient.items.list(collectionId);
            console.log('List method succeeded');
          } catch (err3) {
            console.log('List method failed:', err3.message);
            throw new Error(`All API methods failed. Last error: ${err3.message}`);
          }
        }
      }
      
      console.log('Simple query completed successfully using method:', method);
      
      // Format the results based on the response structure
      let items = [];
      let total = 0;
      
      if (dataItems.items) {
        // Standard response format
        items = dataItems.items;
        total = items.length;
      } else if (Array.isArray(dataItems)) {
        // List response format
        items = dataItems;
        total = items.length;
      } else {
        // Unknown format - try to extract items
        items = dataItems.results || dataItems.data || [];
        total = items.length;
      }
      
      const result = {
        method: method,
        total: total,
        items: items,
        rawData: dataItems
      };
      
      return {
        success: true,
        result: result,
        message: `Successfully retrieved ${result.total} items from collection "${collectionId}"`
      };
    } catch (err) {
      console.error('Simple Wix SDK Test Error:', err);
      
      // Try to provide more detailed diagnostics
      let details = err.stack || '';
      try {
        details += '\n\nSDK Version Info:';
        details += `\n@wix/sdk version: ${require('@wix/sdk/package.json').version}`;
        details += `\n@wix/data version: ${require('@wix/data/package.json').version}`;
      } catch (versionErr) {
        details += '\nCould not determine SDK versions: ' + versionErr.message;
      }
      
      return {
        success: false,
        error: err.message,
        details: details
      };
    }
  }
};
