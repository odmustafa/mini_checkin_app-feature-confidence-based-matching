/**
 * Test script to look up pricing plans for a specific member ID
 * Uses the Wix JavaScript SDK as required by the project standards
 * 
 * This script follows the same approach as the WixPricingPlansService.js file
 * in the project to ensure compatibility with the Wix SDK.
 */

const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders, plans } = require('@wix/pricing-plans');

// Member ID to look up
const MEMBER_ID = '44f217de-93f1-453a-adb8-90bb1a0e740f';

// Configuration path
const CONFIG_PATH = path.join(__dirname, 'wix-config.json');

// Load configuration
let WIX_CONFIG = {};
try {
  console.log('Loading Wix configuration from:', CONFIG_PATH);
  WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
  console.log('Configuration loaded successfully');
} catch (error) {
  console.error('Error loading configuration:', error.message);
  process.exit(1);
}

async function initializeWixClient() {
  try {
    console.log('Initializing Wix SDK client for Pricing Plans');
    
    // Configure authentication with API Key strategy
    const client = createClient({
      modules: { orders, plans },
      auth: ApiKeyStrategy({
        apiKey: WIX_CONFIG.apiKey,
        siteId: WIX_CONFIG.siteId
      })
    });
    
    return client;
  } catch (err) {
    console.error('Error initializing Wix client:', err);
    throw err;
  }
}

async function lookupMemberPricingPlans(memberId) {
  try {
    console.log(`Looking up pricing plans for member ID: ${memberId}`);
    
    // Initialize the Wix client
    const client = await initializeWixClient();
    
    // Set up the filter to search for orders by buyer ID
    const filter = {
      buyerIds: [memberId]
    };
    
    console.log('Querying orders with filter:', JSON.stringify(filter, null, 2));
    
    // Query orders using the Wix Pricing Plans SDK
    const ordersResponse = await client.orders.query({
      filter: filter
    });
    
    // Check if we got any results
    if (ordersResponse && ordersResponse.items && ordersResponse.items.length > 0) {
      console.log(`Found ${ordersResponse.items.length} pricing plan orders for this member:`);
      
      // Display each order with relevant details
      ordersResponse.items.forEach((order, index) => {
        console.log(`\nOrder #${index + 1}:`);
        console.log(`  ID: ${order._id}`);
        console.log(`  Plan ID: ${order.planId}`);
        console.log(`  Status: ${order.status}`);
        console.log(`  Created: ${new Date(order._createdDate).toLocaleString()}`);
        
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
      });
      
      // Fetch the plan details for each unique plan ID
      const uniquePlanIds = [...new Set(ordersResponse.items.map(order => order.planId))];
      console.log(`\nFetching details for ${uniquePlanIds.length} unique pricing plans...`);
      
      for (const planId of uniquePlanIds) {
        try {
          const planResponse = await client.plans.get(planId);
          
          if (planResponse) {
            console.log(`\nPlan Details for ID: ${planId}`);
            console.log(`  Name: ${planResponse.name}`);
            console.log(`  Description: ${planResponse.description || 'N/A'}`);
            console.log(`  Type: ${planResponse.planType}`);
            
            if (planResponse.pricing) {
              console.log('  Pricing:');
              console.log(`    Price: ${planResponse.pricing.price} ${planResponse.pricing.currency}`);
              console.log(`    Duration: ${planResponse.pricing.duration || 'N/A'}`);
            }
          }
        } catch (planError) {
          console.error(`Error fetching plan details for ID ${planId}:`, planError.message);
        }
      }
      
      return {
        success: true,
        orders: ordersResponse.items
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
lookupMemberPricingPlans(MEMBER_ID)
  .then(result => {
    if (result.success) {
      console.log('Test completed successfully');
    } else {
      console.error('Test failed:', result.error);
    }
  })
  .catch(err => {
    console.error('Unhandled error:', err);
  });
