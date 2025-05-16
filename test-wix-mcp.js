/**
 * test-wix-mcp.js
 * A simple script to test the Wix MCP connection
// [MermaidChart: fcd78d41-f9d6-4f09-b250-f2381937aa6e]
 * Using the same approach as WixSdkAdapter.js
 */

const { createClient, OAuthStrategy } = require('@wix/sdk');
const { contacts } = require('@wix/crm');
const fs = require('fs');
const path = require('path');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, 'wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('✅ Loaded Wix config');
  } catch (e) {
    console.error('❌ Error loading Wix config:', e.message);
    process.exit(1);
  }
} else {
  console.error('❌ wix.config.json not found');
  process.exit(1);
}

// Create a Wix client with OAuth strategy
const client = createClient({
  auth: OAuthStrategy({
    clientId: WIX_CONFIG.clientId,
    tokens: {
      accessToken: WIX_CONFIG.apiKey,
      refreshToken: WIX_CONFIG.refreshToken || '',
    },
  }),
  modules: {
    contacts,
  },
  baseURL: `https://www.wixapis.com/`,
});

// Test the connection by querying contacts
async function testWixConnection() {
  console.log('🔍 Testing Wix MCP connection...');
  
  try {
    // First, let's check if we can access the contacts API
    console.log('\n🔍 Testing Contacts API access...');
    
    // Try to list contacts (limited to 1 for testing)
    const result = await client.contacts.queryContacts({
      query: {},
      fields: ['_id', 'name.first', 'name.last'],
      limit: 1
    });
    
    console.log('✅ Wix MCP connection successful!');
    
    if (result && result.items && result.items.length > 0) {
      console.log('📋 Sample contact:', JSON.stringify(result.items[0], null, 2));
    } else {
      console.log('ℹ️ No contacts found in the account');
    }
    
    // Test Wix SDK version
    console.log('\n📦 Wix SDK Info:');
    console.log(`- SDK Version: ${require('@wix/sdk/package.json').version}`);
    console.log(`- CRM Version: ${require('@wix/crm/package.json').version}`);
    console.log(`- Data Version: ${require('@wix/data/package.json').version}`);
    
    // Test data API access using the correct import
    console.log('\n🔍 Testing Data API access...');
    try {
      // Import data module correctly
      const { items } = require('@wix/data');
      
      // Create a client with the data module
      const dataClient = createClient({
        auth: OAuthStrategy({
          clientId: WIX_CONFIG.clientId,
          tokens: {
            accessToken: WIX_CONFIG.apiKey,
            refreshToken: WIX_CONFIG.refreshToken || '',
          },
        }),
        modules: { dataItems: items },
        baseURL: 'https://www.wixapis.com/'
      });
      
      // List data collections using the correct method
      console.log('Fetching data collections...');
      
      // Since we can't list collections directly, we'll try to query a known collection
      // or demonstrate the data API usage with a sample query
      console.log('✅ Data API client initialized successfully');
      
      // Example of how to use the data client
      console.log('\n📋 Example data query (using a placeholder collection name):');
      try {
        // This is a placeholder - replace 'YourCollectionName' with an actual collection name
        const sampleCollection = 'Members';
        console.log(`Querying collection: ${sampleCollection}`);
        
        const queryResult = await dataClient.dataItems.query(sampleCollection).limit(1).find();
        console.log(`✅ Query successful! Found ${queryResult.items.length} items`);
        
        if (queryResult.items.length > 0) {
          console.log('Sample item:', JSON.stringify(queryResult.items[0], null, 2));
        } else {
          console.log('No items found in the collection');
        }
        
      } catch (queryError) {
        console.error('❌ Query failed:', queryError.message);
        console.error('This is expected if the collection does not exist.');
        console.error('To test with a real collection, replace the collection name in the code.');
      }
      
    } catch (dataError) {
      console.error('❌ Data API access failed:', dataError.message);
      console.error('Error details:', JSON.stringify({
        name: dataError.name,
        code: dataError.code,
        stack: dataError.stack?.split('\n').slice(0, 3).join('\n')
      }, null, 2));
      
      if (dataError.response) {
        console.error('Response status:', dataError.response.status);
        if (dataError.response.data) {
          console.error('Response data:', JSON.stringify(dataError.response.data, null, 2));
        }
      }
    }
    
  } catch (error) {
    console.error('❌ Wix MCP connection failed:', error.message);
    console.error('Error details:', JSON.stringify(error, null, 2));
    
    if (error.response) {
      console.error('Response data:', JSON.stringify(error.response.data, null, 2));
      console.error('Status:', error.response.status);
      console.error('Headers:', JSON.stringify(error.response.headers, null, 2));
    }
    
    if (error.request) {
      console.error('Request details:', {
        method: error.request.method,
        url: error.request.url,
        headers: error.request.headers,
        data: error.request.data
      });
    }
  }
}

// Run the test
testWixConnection();
