/**
 * Test script for searching contacts using the Wix CRM Contacts API
 * 
 * This script tests the searchMember function which now uses the Wix CRM Contacts API
 * instead of the Members API. It strictly follows Wix documentation for authentication
 * and querying contacts.
 */

const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { contacts } = require('@wix/crm');

// Load configuration
const CONFIG_PATH = path.join(__dirname, '../wix.config.json');
let config = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    config = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix config');
  } catch (e) {
    console.error('Error loading Wix config:', e.message);
    process.exit(1);
  }
} else {
  console.error('Config file not found at:', CONFIG_PATH);
  process.exit(1);
}

// Account ID for account-level API access
const accountId = '11a11ce3-d0da-46c7-b4e4-48c17df562f0';

async function testContactSearch() {
  try {
    console.log('Initializing Wix SDK client with API Key strategy');
    console.log(`- API Key: ${config.apiKey.substring(0, 10)}...`);
    console.log(`- Site ID: ${config.siteId}`);
    
    // Create the client with the contacts module
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
    
    // Test search parameters
    const searchParams = {
      firstName: process.argv[2] || '',
      lastName: process.argv[3] || ''
    };
    
    console.log(`\nSearching for contacts with:`);
    console.log(`- First Name: ${searchParams.firstName || '(any)'}`);
    console.log(`- Last Name: ${searchParams.lastName || '(any)'}`);
    
    // Create an array to store all search results
    let allResults = [];
    
    // Search by first name if provided
    if (searchParams.firstName) {
      console.log('\nSearching by first name...');
      const firstNameQuery = client.contacts.queryContacts()
        .startsWith('info.name.first', searchParams.firstName)
        .limit(50);
      
      const firstNameResults = await firstNameQuery.find();
      console.log(`Found ${firstNameResults.items ? firstNameResults.items.length : 0} contacts by first name`);
      
      if (firstNameResults.items && firstNameResults.items.length > 0) {
        allResults = allResults.concat(firstNameResults.items);
      }
    }
    
    // Search by last name if provided
    if (searchParams.lastName) {
      console.log('\nSearching by last name...');
      const lastNameQuery = client.contacts.queryContacts()
        .startsWith('info.name.last', searchParams.lastName)
        .limit(50);
      
      const lastNameResults = await lastNameQuery.find();
      console.log(`Found ${lastNameResults.items ? lastNameResults.items.length : 0} contacts by last name`);
      
      if (lastNameResults.items && lastNameResults.items.length > 0) {
        allResults = allResults.concat(lastNameResults.items);
      }
    }
    
    // If no search parameters provided, get all contacts
    if (!searchParams.firstName && !searchParams.lastName) {
      console.log('\nNo search parameters provided, retrieving all contacts...');
      const allContactsQuery = client.contacts.queryContacts()
        .limit(50);
      
      const allContactsResults = await allContactsQuery.find();
      console.log(`Found ${allContactsResults.items ? allContactsResults.items.length : 0} total contacts`);
      
      if (allContactsResults.items && allContactsResults.items.length > 0) {
        allResults = allResults.concat(allContactsResults.items);
      }
    }
    
    // Remove duplicates by ID
    const uniqueContacts = new Map();
    allResults.forEach(contact => {
      if (contact && contact._id) {
        uniqueContacts.set(contact._id, contact);
      }
    });
    
    // Convert back to array
    const uniqueResults = Array.from(uniqueContacts.values());
    
    // Display results
    console.log(`\nFound ${uniqueResults.length} unique contacts`);
    
    // Display contact details
    uniqueResults.forEach((contact, index) => {
      console.log(`\nContact #${index + 1}:`);
      console.log(`- ID: ${contact._id}`);
      console.log(`- Name: ${contact.info?.name?.first || ''} ${contact.info?.name?.last || ''}`);
      console.log(`- Email: ${contact.info?.emails?.[0]?.email || 'N/A'}`);
      console.log(`- Phone: ${contact.info?.phones?.[0]?.phone || 'N/A'}`);
      console.log(`- Created: ${new Date(contact._createdDate).toLocaleString()}`);
    });
    
    console.log('\nTest completed successfully!');
  } catch (error) {
    console.error('Error during contact search test:', error);
  }
}

// Run the test
testContactSearch();
