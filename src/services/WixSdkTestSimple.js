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
const wixData = require('@wix/data');
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
  /**
   * Test the Wix JavaScript SDK with a specific collection
   * @param {string} collectionId - The ID or name of the collection to query
   * @returns {Promise<Object>} - The query results
   */
  async testSdkSimple(collectionId = "BannedNames") {
    try {
      console.log('Running version-specific SDK test with client ID:', CLIENT_ID);
      
      // Create the client with the correct module structure for your SDK version
      const myWixClient = createClient({
        modules: { items: wixData.items },
        auth: OAuthStrategy({ clientId: CLIENT_ID }),
      });
      
      console.log('Simple Wix client created, querying collection:', collectionId);
      
      // Try multiple API patterns to find the one that works with your SDK version
      let dataItems = null;
      let method = '';
      
      // According to Wix SDK documentation, try multiple methods with different parameter formats
      // Try both collection ID and collection name approaches
      
      // First, log SDK versions for debugging
      try {
        console.log('SDK Version Info:');
        console.log(`@wix/sdk version: ${require('@wix/sdk/package.json').version}`);
        console.log(`@wix/data version: ${require('@wix/data/package.json').version}`);
      } catch (versionErr) {
        console.log('Could not determine SDK versions:', versionErr.message);
      }
      
      try {
        // Method 1: Query with dataCollectionId
        method = 'query with dataCollectionId';
        console.log('Trying method:', method);
        dataItems = await myWixClient.items.query({
          dataCollectionId: collectionId
        });
        console.log('Query with dataCollectionId succeeded');
      } catch (err1) {
        console.log('Query with dataCollectionId failed:', err1.message);
        
        try {
          // Method 2: Query with collectionName
          method = 'query with collectionName';
          console.log('Trying method:', method);
          dataItems = await myWixClient.items.query({
            collectionName: collectionId
          });
          console.log('Query with collectionName succeeded');
        } catch (err2) {
          console.log('Query with collectionName failed:', err2.message);
          
          try {
            // Method 3: Find with dataCollectionId
            method = 'find with dataCollectionId';
            console.log('Trying method:', method);
            dataItems = await myWixClient.items.find({
              dataCollectionId: collectionId
            });
            console.log('Find with dataCollectionId succeeded');
          } catch (err3) {
            console.log('Find with dataCollectionId failed:', err3.message);
            
            try {
              // Method 4: Find with collectionName
              method = 'find with collectionName';
              console.log('Trying method:', method);
              dataItems = await myWixClient.items.find({
                collectionName: collectionId
              });
              console.log('Find with collectionName succeeded');
            } catch (err4) {
              console.log('Find with collectionName failed:', err4.message);
              
              try {
                // Method 5: Direct list with ID
                method = 'list';
                console.log('Trying method:', method);
                dataItems = await myWixClient.items.list(collectionId);
                console.log('List method succeeded');
              } catch (err5) {
                console.log('List method failed:', err5.message);
                
                try {
                  // Method 6: Try with "Ban List" as the collection name
                  const alternativeName = "Ban List";
                  method = 'query with alternative name';
                  console.log('Trying method:', method, 'with name:', alternativeName);
                  dataItems = await myWixClient.items.query({
                    collectionName: alternativeName
                  });
                  console.log('Query with alternative name succeeded');
                } catch (err6) {
                  console.log('Query with alternative name failed:', err6.message);
                  throw new Error(`All API methods failed. Last error: ${err6.message}`);
                }
              }
            }
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
      
      // Create a serializable result object with only the data we need
      // Avoid including the raw dataItems which may contain non-serializable content
      const result = {
        method: method,
        total: total,
        // Ensure items are serializable by converting to simple objects
        items: items.map(item => {
          // Create a clean, serializable version of each item
          try {
            return JSON.parse(JSON.stringify(item));
          } catch (e) {
            // If item can't be stringified, return a simplified version
            return { 
              id: item._id || item.id || 'unknown',
              error: 'Item contains non-serializable data'
            };
          }
        })
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
