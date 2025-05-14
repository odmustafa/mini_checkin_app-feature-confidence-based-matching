/**
 * Test script to query all members without filters
 * This helps verify if the Wix SDK is working correctly and if there are any members in the database
 */

// Import required modules
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { members } = require('@wix/members');
const { items } = require('@wix/data');
const fs = require('fs');
const path = require('path');

// Read the Wix configuration file
const configPath = path.join(__dirname, '../wix.config.json');
const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));

async function testQueryAllMembers() {
  console.log('Starting test: Query all members without filters');
  
  try {
    // Initialize the Wix SDK client directly
    console.log('Initializing Wix SDK client with API Key strategy');
    
    // Configure authentication with API Key strategy
    console.log(`- API Key: ${config.apiKey.substring(0, 20)}...`);
    console.log(`- Site ID: ${config.siteId}`);
    console.log('- Including Account ID in headers for account-level API access');
    
    // Account ID for account-level API access
    const accountId = '11a11ce3-d0da-46c7-b4e4-48c17df562f0';
    
    // Create the client
    const client = createClient({
      modules: { items, members },
      auth: ApiKeyStrategy({
        apiKey: config.apiKey,
        siteId: config.siteId
      }),
      // Add headers for account-level API access
      headers: {
        'wix-account-id': accountId
      }
    });
    
    // Create a simple query with no filters
    const query = {
      paging: { limit: 50 } // Increased limit to get more results
    };
    
    console.log('Executing query for all members:', JSON.stringify(query, null, 2));
    
    // Execute the query using the queryMembers method
    const response = await members.queryMembers(query);
    
    // Get the results
    const results = response.items || [];
    console.log(`Found ${results.length} total members`);
    
    // Create the result object
    const result = {
      success: true,
      items: results,
      total: results.length,
      source: 'wix-sdk'
    };
    
    // Log the result
    console.log('Query result:', JSON.stringify(result, null, 2));
    
    // If successful, log details about the members found
    if (result.success && result.items && result.items.length > 0) {
      console.log(`Found ${result.items.length} members:`);
      
      // Log some basic info about each member
      result.items.forEach((member, index) => {
        console.log(`\nMember ${index + 1}:`);
        console.log(`- ID: ${member._id || 'N/A'}`);
        console.log(`- Name: ${member.contact?.firstName || 'N/A'} ${member.contact?.lastName || 'N/A'}`);
        console.log(`- Email: ${member.loginEmail || member.contact?.emails?.[0] || 'N/A'}`);
        console.log(`- Status: ${member.privacyStatus || 'Unknown'}`);
      });
    } else if (result.success) {
      console.log('No members found in the database.');
    } else {
      console.error('Error querying members:', result.error);
    }
  } catch (error) {
    console.error('Unexpected error during test:', error);
  }
}

// Run the test
testQueryAllMembers()
  .then(() => console.log('Test completed'))
  .catch(err => console.error('Test failed:', err));
