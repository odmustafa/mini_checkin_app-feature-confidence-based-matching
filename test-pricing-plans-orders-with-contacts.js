/**
 * test-pricing-plans-orders-with-contacts.js
 * A test script to list orders for Pricing Plans since April 1, 2025
 * and display contact information for each buyer using the Wix JavaScript SDK
 */

const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders } = require('@wix/pricing-plans');
const { contacts } = require('@wix/crm');

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

// Account ID for account-level API access
const accountId = '11a11ce3-d0da-46c7-b4e4-48c17df562f0';

// Create a Wix client with API Key strategy
const client = createClient({
  modules: { orders, contacts },
  auth: ApiKeyStrategy({
    apiKey: WIX_CONFIG.apiKey,
    siteId: WIX_CONFIG.siteId
  }),
  // Add headers for account-level API access
  headers: {
    'wix-account-id': accountId
  }
});

// Cache for contact details to avoid redundant API calls
const contactCache = new Map();

/**
 * Get contact details by ID
 * @param {string} contactId - The contact ID to look up
 * @returns {Object} - Contact details or null if not found
 */
async function getContactById(contactId) {
  // Check cache first
  if (contactCache.has(contactId)) {
    return contactCache.get(contactId);
  }
  
  try {
    // Get contact details using the Wix CRM Contacts API
    console.log(`📞 Looking up contact: ${contactId}`);
    const contact = await client.contacts.getContact(contactId);
    
    // Cache the result
    contactCache.set(contactId, contact);
    
    return contact;
  } catch (error) {
    console.error(`❌ Error getting contact ${contactId}:`, error.message);
    
    // Cache the null result to avoid repeated failed lookups
    contactCache.set(contactId, null);
    
    return null;
  }
}

/**
 * Format contact name from contact info
 * @param {Object} contact - Contact object from Wix CRM
 * @returns {string} - Formatted name or 'Unknown'
 */
function formatContactName(contact) {
  if (!contact) return 'Unknown';
  
  const info = contact.info || {};
  const name = info.name || {};
  
  // Build name from available parts
  const firstName = name.first || '';
  const lastName = name.last || '';
  
  if (firstName || lastName) {
    return `${firstName} ${lastName}`.trim();
  }
  
  // If no name parts, try to use display name or email
  return info.displayName || info.emails?.[0]?.email || 'Unknown';
}

// Function to list pricing plan orders since a specific date
async function listOrdersSinceDate(sinceDate) {
  console.log(`🔍 Listing pricing plan orders since ${sinceDate.toISOString()}...`);
  
  try {
    // Use the managementListOrders method as per Wix JavaScript SDK documentation
    const response = await client.orders.managementListOrders({
      // Sort by created date in descending order (newest first)
      sort: {
        fieldName: 'createdDate',
        order: 'DESC'
      },
      // Get maximum allowed orders
      limit: 50
    });
    
    console.log(`✅ Found ${response.orders?.length || 0} total orders`);
    
    // Filter orders by date
    const filteredOrders = response.orders.filter(order => {
      const orderDate = new Date(order._createdDate || order._updatedDate || 0);
      return orderDate >= sinceDate;
    });
    
    console.log(`✅ Found ${filteredOrders.length} orders since ${sinceDate.toLocaleDateString()}`);
    
    // Process and display order details
    if (filteredOrders.length > 0) {
      console.log('\n📋 Order Details:');
      
      // Process orders sequentially to avoid rate limiting
      for (let index = 0; index < filteredOrders.length; index++) {
        const order = filteredOrders[index];
        console.log(`\n--- Order ${index + 1} ---`);
        console.log(`ID: ${order._id || 'N/A'}`);
        console.log(`Plan Name: ${order.planName || 'Unnamed Plan'}`);
        console.log(`Plan ID: ${order.planId || 'N/A'}`);
        console.log(`Status: ${order.status || 'Unknown'}`);
        
        // Properly access buyer information according to the SDK documentation
        if (order.buyer) {
          const buyerId = order.buyer.memberId || 'N/A';
          const contactId = order.buyer.contactId || 'N/A';
          
          console.log(`Buyer Member ID: ${buyerId}`);
          console.log(`Buyer Contact ID: ${contactId}`);
          
          // Look up contact details
          if (contactId && contactId !== 'N/A') {
            const contact = await getContactById(contactId);
            const contactName = formatContactName(contact);
            console.log(`Contact Name: ${contactName}`);
            
            // Add additional contact details if available
            if (contact && contact.info) {
              if (contact.info.emails && contact.info.emails.length > 0) {
                console.log(`Email: ${contact.info.emails[0].email || 'N/A'}`);
              }
              if (contact.info.phones && contact.info.phones.length > 0) {
                console.log(`Phone: ${contact.info.phones[0].phone || 'N/A'}`);
              }
            }
          } else {
            console.log('Contact Name: Not available (No Contact ID)');
          }
        } else {
          console.log('Buyer Information: Not available');
          console.log('Contact Name: Not available (No Buyer Info)');
        }
        
        // Access creation date from _createdDate field
        console.log(`Created: ${order._createdDate ? new Date(order._createdDate).toLocaleString() : 'Unknown'}`);
        
        // Access start and end dates
        if (order.startDate) {
          console.log(`Start Date: ${new Date(order.startDate).toLocaleString()}`);
        } else if (order.currentCycle && order.currentCycle.startedDate) {
          console.log(`Start Date (from cycle): ${new Date(order.currentCycle.startedDate).toLocaleString()}`);
        } else {
          console.log('Start Date: Not available');
        }
        
        console.log(`End Date: ${order.endDate ? new Date(order.endDate).toLocaleString() : 'No expiration'}`);
        
        // Properly access pricing information according to the SDK documentation
        if (order.planPrice) {
          console.log(`Price: ${order.planPrice.amount || 0} ${order.planPrice.currency || ''}`);
        } else if (order.pricing) {
          console.log(`Price: ${order.pricing.price || 0} ${order.pricing.currency || ''}`);
        } else {
          console.log('Price: Not available');
        }
        
        // Display payment details
        console.log(`Payment Status: ${order.lastPaymentStatus || order.paymentStatus || 'Unknown'}`);
      }
    } else {
      console.log('\n❌ No orders found since the specified date');
    }
    
    // Return the filtered orders for further processing
    return filteredOrders;
  } catch (error) {
    console.error('❌ Error listing pricing plan orders:', error);
    
    // Provide detailed error information
    if (error.response) {
      console.error('Response status:', error.response.status);
      console.error('Response data:', JSON.stringify(error.response.data, null, 2));
    }
    
    return {
      success: false,
      error: error.message || 'Unknown error',
      details: error.stack || 'No stack trace available'
    };
  }
}

// Run the test with orders since April 1, 2025
(async () => {
  console.log('🚀 Starting Pricing Plans Orders test with contact details...');
  console.log(`📦 SDK Versions:`);
  console.log(`- SDK Version: ${require('@wix/sdk/package.json').version}`);
  console.log(`- Pricing Plans Version: ${require('@wix/pricing-plans/package.json').version}`);
  console.log(`- CRM Version: ${require('@wix/crm/package.json').version}`);
  
  console.log(`\n🔑 Using API Key: ${WIX_CONFIG.apiKey.substring(0, 15)}...`);
  console.log(`🌐 Site ID: ${WIX_CONFIG.siteId}`);
  
  // Set the date to April 1, 2025
  const sinceDate = new Date('2025-04-01T00:00:00Z');
  console.log(`📅 Filtering orders since: ${sinceDate.toLocaleDateString()}`);
  
  const result = await listOrdersSinceDate(sinceDate);
  
  // Save the filtered orders to a file for inspection
  fs.writeFileSync(
    path.join(__dirname, 'pricing-plans-orders-with-contacts.json'),
    JSON.stringify(result, null, 2)
  );
  
  console.log('\n💾 Orders with contact details saved to pricing-plans-orders-with-contacts.json');
  console.log(`✅ Test completed - Processed ${contactCache.size} unique contacts`);
})();
