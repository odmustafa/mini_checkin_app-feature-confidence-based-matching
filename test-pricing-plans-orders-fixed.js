/**
 * test-pricing-plans-orders-fixed.js
 * A test script to list all orders for Pricing Plans using the Wix JavaScript SDK
 * with proper field access according to the SDK documentation
 */

const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders } = require('@wix/pricing-plans');

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
  modules: { orders },
  auth: ApiKeyStrategy({
    apiKey: WIX_CONFIG.apiKey,
    siteId: WIX_CONFIG.siteId
  }),
  // Add headers for account-level API access
  headers: {
    'wix-account-id': accountId
  }
});

// Function to list all pricing plan orders
async function listAllPricingPlanOrders() {
  console.log('🔍 Listing all pricing plan orders...');
  
  try {
    // Use the managementListOrders method as per Wix JavaScript SDK documentation
    const response = await client.orders.managementListOrders({
      // No filter to get all orders
      // Sort by created date in descending order (newest first)
      sort: {
        fieldName: 'createdDate',
        order: 'DESC'
      },
      // Include additional order details
      limit: 50 // Maximum allowed by the API
    });
    
    console.log(`✅ Found ${response.orders?.length || 0} total orders`);
    
    // Process and display order details
    if (response.orders && response.orders.length > 0) {
      console.log('\n📋 Order Details:');
      
      response.orders.forEach((order, index) => {
        console.log(`\n--- Order ${index + 1} ---`);
        console.log(`ID: ${order._id || 'N/A'}`);
        console.log(`Plan Name: ${order.planName || 'Unnamed Plan'}`);
        console.log(`Plan ID: ${order.planId || 'N/A'}`);
        console.log(`Status: ${order.status || 'Unknown'}`);
        
        // Properly access buyer information according to the SDK documentation
        if (order.buyer) {
          console.log(`Buyer Member ID: ${order.buyer.memberId || 'N/A'}`);
          console.log(`Buyer Contact ID: ${order.buyer.contactId || 'N/A'}`);
        } else {
          console.log('Buyer Information: Not available');
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
        
        if (order.paymentDetails) {
          console.log(`Payment Method: ${order.paymentDetails.paymentMethod || 'Unknown'}`);
          if (order.paymentDetails.paymentDate) {
            console.log(`Payment Date: ${new Date(order.paymentDetails.paymentDate).toLocaleString()}`);
          }
        }
        
        // Check if plan is recurring
        if (order.recurring) {
          console.log('Recurring: Yes');
          console.log(`Cycle: ${order.recurring.cycle || 'Unknown'}`);
        } else {
          console.log('Recurring: No');
        }
        
        // Display additional useful information
        if (order.orderMethod) {
          console.log(`Order Method: ${order.orderMethod}`);
        }
        
        if (order.pausePeriods && order.pausePeriods.length > 0) {
          console.log(`Pause Periods: ${order.pausePeriods.length}`);
        }
      });
    } else {
      console.log('\n❌ No orders found');
    }
    
    // Return the raw response for further inspection
    return response;
  } catch (error) {
    console.error('❌ Error listing pricing plan orders:', error);
    
    // Provide detailed error information
    if (error.response) {
      console.error('Response status:', error.response.status);
      console.error('Response data:', JSON.stringify(error.response.data, null, 2));
    }
    
    // Check for specific error types
    if (error.message && error.message.includes('permission')) {
      console.error('\n⚠️ This might be a permissions issue. Make sure your API key has access to the Pricing Plans module.');
    } else if (error.message && error.message.includes('not found')) {
      console.error('\n⚠️ The Pricing Plans module might not be installed or enabled on your site.');
    }
    
    return {
      success: false,
      error: error.message || 'Unknown error',
      details: error.stack || 'No stack trace available'
    };
  }
}

// Run the test
(async () => {
  console.log('🚀 Starting Pricing Plans Orders test...');
  console.log(`📦 SDK Versions:`);
  console.log(`- SDK Version: ${require('@wix/sdk/package.json').version}`);
  console.log(`- Pricing Plans Version: ${require('@wix/pricing-plans/package.json').version}`);
  
  console.log(`\n🔑 Using API Key: ${WIX_CONFIG.apiKey.substring(0, 15)}...`);
  console.log(`🌐 Site ID: ${WIX_CONFIG.siteId}`);
  
  const result = await listAllPricingPlanOrders();
  
  // Save the raw response to a file for inspection
  fs.writeFileSync(
    path.join(__dirname, 'pricing-plans-orders-response-fixed.json'),
    JSON.stringify(result, null, 2)
  );
  
  console.log('\n💾 Raw response saved to pricing-plans-orders-response-fixed.json');
  console.log('✅ Test completed');
})();
