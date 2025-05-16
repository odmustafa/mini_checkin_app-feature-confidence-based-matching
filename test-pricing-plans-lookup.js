/**
 * Test script to look up pricing plans for a specific member ID
 * Uses the Wix JavaScript SDK as required by the project standards
 * 
 * IMPORTANT: Before running this script, make sure to update the wix-config.json file
 * with your actual Wix API key and site ID.
 */

const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { pricingPlans } = require('@wix/pricing-plans');
const fs = require('fs');
const path = require('path');

// Member ID to look up
const MEMBER_ID = '44f217de-93f1-453a-adb8-90bb1a0e740f';

// Load Wix configuration from config file
function loadWixConfig() {
  try {
    const configPath = path.join(__dirname, 'wix-config.json');
    if (fs.existsSync(configPath)) {
      const configData = fs.readFileSync(configPath, 'utf8');
      return JSON.parse(configData);
    } else {
      console.error('Wix configuration file not found. Please create a wix-config.json file with apiKey and siteId.');
      return null;
    }
  } catch (error) {
    console.error('Error loading Wix configuration:', error.message);
    return null;
  }
}

async function testPricingPlanLookup() {
  try {
    console.log(`Looking up pricing plans for member ID: ${MEMBER_ID}`);
    
    // Load Wix configuration
    const wixConfig = loadWixConfig();
    if (!wixConfig || !wixConfig.apiKey || !wixConfig.siteId) {
      console.error('Invalid Wix configuration. Please ensure apiKey and siteId are set in wix-config.json.');
      return;
    }
    
    console.log(`Using Wix site ID: ${wixConfig.siteId}`);
    
    // Initialize the Wix SDK client with proper authentication
    const wixClient = createClient({
      modules: { pricingPlans },
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
    
    // Query orders using the Wix Pricing Plans SDK
    const ordersResponse = await wixClient.pricingPlans.orders.query({
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
          const planResponse = await wixClient.pricingPlans.plans.get(planId);
          
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
    } else {
      console.log('No pricing plan orders found for this member ID.');
    }
    
  } catch (error) {
    console.error('Error looking up pricing plans:', error);
    if (error.response) {
      console.error('Response error details:', error.response.data);
    }
  }
}

// Run the test
testPricingPlanLookup();
