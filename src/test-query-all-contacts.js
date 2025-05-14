/**
 * Test script to query contacts using the Wix CRM Contacts API
 * This strictly follows the Wix JavaScript SDK documentation for querying contacts
 */

// Import required modules
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { contacts } = require('@wix/crm');
const fs = require('fs');
const path = require('path');

// Read the Wix configuration file
const configPath = path.join(__dirname, '../wix.config.json');
const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));

async function testQueryContacts() {
  console.log('Starting test: Query all contacts without filters');
  
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
      modules: { contacts },
      auth: ApiKeyStrategy({
        apiKey: config.apiKey,
        siteId: config.siteId
      }),
      // Add headers for account-level API access
      headers: {
        'wix-account-id': accountId
      }
    });
    
    console.log('Creating query for contacts using queryContacts()');
    
    // Create a query using the queryContacts method
    // This strictly follows the Wix documentation for the Contacts API
    const queryBuilder = client.contacts.queryContacts();
    
    // Execute the query by calling find()
    console.log('Executing find() on the query builder');
    const response = await queryBuilder.find();
    
    // Get the results
    const results = response.items || [];
    console.log(`Found ${results.length} total contacts`);
    
    // Create the result object
    const result = {
      success: true,
      items: results,
      total: results.length,
      source: 'wix-sdk-contacts'
    };
    
    // Log the result
    console.log('Query result:', JSON.stringify(result, null, 2));
    
    // If successful, log details about the contacts found
    if (result.success && result.items && result.items.length > 0) {
      console.log(`Found ${result.items.length} contacts:`);
      
      // Log some basic info about each contact
      result.items.forEach((contact, index) => {
        console.log(`\nContact ${index + 1}:`);
        console.log(`- ID: ${contact._id || 'N/A'}`);
        console.log(`- Name: ${contact.info?.name?.first || 'N/A'} ${contact.info?.name?.last || 'N/A'}`);
        console.log(`- Email: ${contact.primaryInfo?.email || 'N/A'}`);
        console.log(`- Phone: ${contact.primaryInfo?.phone || 'N/A'}`);
        console.log(`- Company: ${contact.info?.company || 'N/A'}`);
      });
    } else if (result.success) {
      console.log('No contacts found in the database.');
    } else {
      console.error('Error querying contacts:', result.error);
    }
  } catch (error) {
    console.error('Unexpected error during test:', error);
  }
}

// Run the test
testQueryContacts()
  .then(() => console.log('Test completed'))
  .catch(err => console.error('Test failed:', err));
