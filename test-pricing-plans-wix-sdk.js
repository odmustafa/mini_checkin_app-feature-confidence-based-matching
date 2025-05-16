/**
 * Test script to look up pricing plans for a specific member ID
 * Strictly follows the official Wix JavaScript SDK documentation
 */

const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders } = require('@wix/pricing-plans');

// Member ID to look up
const MEMBER_ID = '44f217de-93f1-453a-adb8-90bb1a0e740f';

// Load configuration
const fs = require('fs');
const path = require('path');
const CONFIG_PATH = path.join(__dirname, 'wix-config.json');
const wixConfig = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));

async function testPricingPlanLookup() {
  try {
    console.log(`Looking up pricing plans for member ID: ${MEMBER_ID}`);
    console.log(`Using Wix site ID: ${wixConfig.siteId}`);
    
    // Initialize the Wix client according to official documentation
    const wixClient = createClient({
      modules: { orders },
      auth: ApiKeyStrategy({
        apiKey: wixConfig.apiKey,
        siteId: wixConfig.siteId
      })
    });
    
    // Set up the filter to search for orders by buyer ID
    const filter = {
      buyerIds: [MEMBER_ID]
    };
    
    console.log('Querying orders with filter:', JSON.stringify(filter, null, 2));
    
    // According to Wix documentation, use managementListOrders to get orders
    const ordersResponse = await wixClient.orders.managementListOrders({
      filter: filter
    });
    
    // Check if we got any results
    if (ordersResponse && ordersResponse.orders && ordersResponse.orders.length > 0) {
      console.log(`Found ${ordersResponse.orders.length} pricing plan orders for this member:`);
      
      // Display each order with relevant details
      ordersResponse.orders.forEach((order, index) => {
        console.log(`\nOrder #${index + 1}:`);
        console.log(`  ID: ${order._id}`);
        console.log(`  Plan ID: ${order.planId}`);
        console.log(`  Status: ${order.status}`);
        
        if (order._createdDate) {
          console.log(`  Created: ${new Date(order._createdDate).toLocaleString()}`);
        }
        
        if (order.startDate) {
          console.log(`  Start Date: ${new Date(order.startDate).toLocaleString()}`);
        }
        
        if (order.endDate) {
          console.log(`  End Date: ${new Date(order.endDate).toLocaleString()}`);
        }
        
        // Show pricing details if available
        if (order.pricing) {
          console.log('  Pricing:');
          console.log(`    Price: ${order.pricing.price} ${order.pricing.currency}`);
          if (order.pricing.pricingModelName) {
            console.log(`    Model: ${order.pricing.pricingModelName}`);
          }
        }
        
        // Show buyer info if available
        if (order.buyerInfo) {
          console.log('  Buyer Info:');
          console.log(`    ID: ${order.buyerInfo.memberId || order.buyerInfo.contactId || 'N/A'}`);
          console.log(`    Email: ${order.buyerInfo.email || 'N/A'}`);
        }
      });
      
      return {
        success: true,
        orders: ordersResponse.orders
      };
    } else {
      console.log('No pricing plan orders found for this member ID.');
      return {
        success: true,
        orders: []
      };
    }
  } catch (error) {
    console.error('Error looking up pricing plans:', error);
    if (error.response) {
      console.error('Response error details:', error.response.data);
    }
    return {
      success: false,
      error: error.message
    };
  }
}

// Run the test
testPricingPlanLookup()
  .then(result => {
    if (result.success) {
      console.log('\nTest completed successfully');
    } else {
      console.error('\nTest failed:', result.error);
    }
  })
  .catch(err => {
    console.error('Unhandled error:', err);
  });
