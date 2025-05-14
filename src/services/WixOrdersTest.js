/**
 * WixOrdersTest.js
 * A proof of concept for retrieving Wix Pricing Plans and Orders
 * 
 * This standalone script demonstrates how to:
 * 1. Connect to the Wix API using the JavaScript SDK
 * 2. Retrieve all pricing plans
 * 3. Retrieve all orders
 * 4. Filter orders by various criteria
 */
const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders, plans } = require('@wix/pricing-plans');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

// Check if config file exists, if not, create a template
if (!fs.existsSync(CONFIG_PATH)) {
  console.log('Wix config file not found. Creating template...');
  WIX_CONFIG = {
    apiKey: "YOUR_API_KEY_HERE",
    siteId: "YOUR_SITE_ID_HERE",
    accountId: "YOUR_ACCOUNT_ID_HERE" // Optional for account-level API access
  };
  
  // Write template config file
  fs.writeFileSync(CONFIG_PATH, JSON.stringify(WIX_CONFIG, null, 2));
  console.error('Please edit the wix.config.json file with your actual credentials before running this script.');
  process.exit(1);
} else {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix config for Orders Test');
  } catch (e) {
    console.error('Error loading Wix config:', e.message);
    process.exit(1);
  }
}

// Initialize the Wix client
async function initializeWixClient() {
  console.log('Initializing Wix SDK client with API Key strategy');
  
  // Configure authentication with API Key strategy
  const client = createClient({
    modules: { orders, plans },
    auth: ApiKeyStrategy({
      apiKey: WIX_CONFIG.apiKey,
      siteId: WIX_CONFIG.siteId
    }),
    // Add headers for account-level API access if accountId is provided
    headers: WIX_CONFIG.accountId ? {
      'wix-account-id': WIX_CONFIG.accountId
    } : undefined
  });
  
  return client;
}

// Get all pricing plans
async function getAllPricingPlans(client) {
  try {
    console.log('Retrieving all pricing plans...');
    
    // Query all pricing plans
    const response = await client.plans.queryPublicPlans();
    
    console.log(`Found ${response.plans?.length || 0} pricing plans`);
    
    return {
      success: true,
      plans: response.plans || [],
      total: response.plans?.length || 0,
      source: 'wix-pricing-plans'
    };
  } catch (err) {
    console.error('Error retrieving pricing plans:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Get all orders with optional filtering
async function getAllOrders(client, options = {}) {
  try {
    console.log('Retrieving pricing plan orders with options:', JSON.stringify(options, null, 2));
    
    // Build the query parameters
    const queryParams = {
      // Default to 50 items per page if not specified
      limit: options.limit || 50,
      
      // Include the filter if provided
      filter: options.filter || undefined,
      
      // Include sorting if provided
      sort: options.sort || undefined
    };
    
    // Use the managementListOrders method with the provided parameters
    const response = await client.orders.listOrders(queryParams);
    
    console.log(`Found ${response.orders?.length || 0} orders`);
    console.log(`Pagination info: ${response.metadata?.count || 0} total, ${response.metadata?.offset || 0} offset`);
    
    return {
      success: true,
      orders: response.orders || [],
      metadata: response.metadata || {},
      source: 'wix-pricing-plans'
    };
  } catch (err) {
    console.error('Error listing pricing plan orders:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Filter orders by buyer ID
async function getOrdersByBuyerId(client, buyerId) {
  try {
    console.log(`Retrieving orders for buyer ID: ${buyerId}`);
    
    // Create a filter for the buyer ID
    const filter = {
      buyerId: {
        $eq: buyerId
      }
    };
    
    return await getAllOrders(client, { filter });
  } catch (err) {
    console.error('Error retrieving orders by buyer ID:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Filter orders by plan ID
async function getOrdersByPlanId(client, planId) {
  try {
    console.log(`Retrieving orders for plan ID: ${planId}`);
    
    // Create a filter for the plan ID
    const filter = {
      planId: {
        $eq: planId
      }
    };
    
    return await getAllOrders(client, { filter });
  } catch (err) {
    console.error('Error retrieving orders by plan ID:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Filter orders by date range
async function getOrdersByDateRange(client, startDate, endDate) {
  try {
    console.log(`Retrieving orders between ${startDate} and ${endDate}`);
    
    // Create a filter for the date range
    const filter = {
      _createdDate: {
        $gte: new Date(startDate).toISOString(),
        $lte: new Date(endDate).toISOString()
      }
    };
    
    return await getAllOrders(client, { filter });
  } catch (err) {
    console.error('Error retrieving orders by date range:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Filter orders by status
async function getOrdersByStatus(client, status) {
  try {
    console.log(`Retrieving orders with status: ${status}`);
    
    // Create a filter for the status
    const filter = {
      status: {
        $eq: status
      }
    };
    
    return await getAllOrders(client, { filter });
  } catch (err) {
    console.error('Error retrieving orders by status:', err);
    return {
      success: false,
      error: err.message,
      source: 'wix-pricing-plans'
    };
  }
}

// Main function to run all tests
async function runTests() {
  try {
    console.log('Starting Wix Orders Test...');
    
    // Initialize the Wix client
    const client = await initializeWixClient();
    
    // Test 1: Get all pricing plans
    console.log('\n=== Test 1: Get All Pricing Plans ===');
    const plansResult = await getAllPricingPlans(client);
    console.log('Plans result:', JSON.stringify(plansResult, null, 2));
    
    // Test 2: Get all orders
    console.log('\n=== Test 2: Get All Orders ===');
    const ordersResult = await getAllOrders(client);
    console.log('Orders result:', JSON.stringify(ordersResult, null, 2));
    
    // If we have orders, run more specific tests
    if (ordersResult.success && ordersResult.orders.length > 0) {
      const sampleOrder = ordersResult.orders[0];
      
      // Test 3: Get orders by buyer ID
      if (sampleOrder.buyerId) {
        console.log(`\n=== Test 3: Get Orders by Buyer ID (${sampleOrder.buyerId}) ===`);
        const buyerOrdersResult = await getOrdersByBuyerId(client, sampleOrder.buyerId);
        console.log('Buyer orders result:', JSON.stringify(buyerOrdersResult, null, 2));
      }
      
      // Test 4: Get orders by plan ID
      if (sampleOrder.planId) {
        console.log(`\n=== Test 4: Get Orders by Plan ID (${sampleOrder.planId}) ===`);
        const planOrdersResult = await getOrdersByPlanId(client, sampleOrder.planId);
        console.log('Plan orders result:', JSON.stringify(planOrdersResult, null, 2));
      }
      
      // Test 5: Get orders by status
      if (sampleOrder.status) {
        console.log(`\n=== Test 5: Get Orders by Status (${sampleOrder.status}) ===`);
        const statusOrdersResult = await getOrdersByStatus(client, sampleOrder.status);
        console.log('Status orders result:', JSON.stringify(statusOrdersResult, null, 2));
      }
    }
    
    // Test 6: Get orders by date range (last 30 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    
    console.log(`\n=== Test 6: Get Orders by Date Range (${startDate.toISOString()} to ${endDate.toISOString()}) ===`);
    const dateRangeOrdersResult = await getOrdersByDateRange(client, startDate, endDate);
    console.log('Date range orders result:', JSON.stringify(dateRangeOrdersResult, null, 2));
    
    console.log('\nAll tests completed!');
  } catch (err) {
    console.error('Error running tests:', err);
  }
}

// Run the tests
runTests().catch(console.error);
