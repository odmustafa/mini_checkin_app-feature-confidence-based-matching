/**
 * InspectWixSDK.js
 * A utility script to inspect the Wix SDK and discover available methods
 * for working with Pricing Plans and Orders
 */
const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders, plans } = require('@wix/pricing-plans');
const { members } = require('@wix/members');
const { currentMember } = require('@wix/members-current-member');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix config for SDK inspection');
  } catch (e) {
    console.error('Error loading Wix config:', e.message);
    process.exit(1);
  }
}

/**
 * Initialize the Wix client with all available modules
 */
async function initializeWixClient() {
  console.log('Initializing Wix SDK client with API Key strategy');
  
  // Configure authentication with API Key strategy
  const client = createClient({
    modules: { 
      orders, 
      plans,
      members,
      currentMember
    },
    auth: ApiKeyStrategy({
      apiKey: WIX_CONFIG.apiKey,
      siteId: WIX_CONFIG.siteId
    })
  });
  
  return client;
}

/**
 * Get available methods on a module
 */
function getAvailableMethods(client, moduleName) {
  if (!client || !client[moduleName]) {
    return {
      error: `Module '${moduleName}' not found in client`
    };
  }
  
  try {
    // Get all properties
    const properties = Object.getOwnPropertyNames(client[moduleName]);
    
    // Filter to only include methods
    const methods = properties.filter(prop => {
      return typeof client[moduleName][prop] === 'function';
    });
    
    // Get method signatures where possible
    const methodDetails = methods.map(method => {
      try {
        const funcStr = client[moduleName][method].toString();
        return {
          name: method,
          signature: funcStr.split('{')[0].trim()
        };
      } catch (e) {
        return {
          name: method,
          signature: 'Could not determine signature'
        };
      }
    });
    
    return {
      moduleName,
      methods: methodDetails,
      count: methods.length
    };
  } catch (err) {
    return {
      error: `Error inspecting module '${moduleName}': ${err.message}`
    };
  }
}

/**
 * Try to call a method and see what it returns
 */
async function tryMethod(client, moduleName, methodName, params = {}) {
  if (!client || !client[moduleName] || typeof client[moduleName][methodName] !== 'function') {
    return {
      error: `Method '${methodName}' not found in module '${moduleName}'`
    };
  }
  
  try {
    console.log(`Trying method ${moduleName}.${methodName}() with params:`, params);
    const result = await client[moduleName][methodName](params);
    return {
      success: true,
      result
    };
  } catch (err) {
    return {
      error: `Error calling ${moduleName}.${methodName}(): ${err.message}`,
      stack: err.stack
    };
  }
}

/**
 * Inspect the Wix SDK and run tests
 */
async function inspectSDK() {
  try {
    console.log('Starting Wix SDK inspection...');
    
    // Initialize the Wix client
    const client = await initializeWixClient();
    
    // Inspect available modules
    console.log('\n=== Available Modules ===');
    const modules = Object.keys(client);
    console.log('Modules:', modules);
    
    // Inspect plans module
    console.log('\n=== Plans Module Methods ===');
    const plansMethods = getAvailableMethods(client, 'plans');
    console.log(JSON.stringify(plansMethods, null, 2));
    
    // Inspect orders module
    console.log('\n=== Orders Module Methods ===');
    const ordersMethods = getAvailableMethods(client, 'orders');
    console.log(JSON.stringify(ordersMethods, null, 2));
    
    // Try to query plans
    console.log('\n=== Testing Plans Query ===');
    if (plansMethods.methods.some(m => m.name === 'query')) {
      const plansResult = await tryMethod(client, 'plans', 'query');
      console.log('Plans query result:', JSON.stringify(plansResult, null, 2));
    } else if (plansMethods.methods.some(m => m.name === 'list')) {
      const plansResult = await tryMethod(client, 'plans', 'list');
      console.log('Plans list result:', JSON.stringify(plansResult, null, 2));
    } else if (plansMethods.methods.some(m => m.name === 'queryPublicPlans')) {
      const plansResult = await tryMethod(client, 'plans', 'queryPublicPlans');
      console.log('Plans queryPublicPlans result:', JSON.stringify(plansResult, null, 2));
    } else {
      console.log('No suitable method found to query plans');
    }
    
    // Try to query orders
    console.log('\n=== Testing Orders Query ===');
    if (ordersMethods.methods.some(m => m.name === 'query')) {
      const ordersResult = await tryMethod(client, 'orders', 'query');
      console.log('Orders query result:', JSON.stringify(ordersResult, null, 2));
    } else if (ordersMethods.methods.some(m => m.name === 'list')) {
      const ordersResult = await tryMethod(client, 'orders', 'list');
      console.log('Orders list result:', JSON.stringify(ordersResult, null, 2));
    } else if (ordersMethods.methods.some(m => m.name === 'listOrders')) {
      const ordersResult = await tryMethod(client, 'orders', 'listOrders');
      console.log('Orders listOrders result:', JSON.stringify(ordersResult, null, 2));
    } else {
      console.log('No suitable method found to query orders');
    }
    
    // Try to get current member orders
    console.log('\n=== Testing Current Member Orders ===');
    if (ordersMethods.methods.some(m => m.name === 'getCurrentMemberOrders')) {
      const memberOrdersResult = await tryMethod(client, 'orders', 'getCurrentMemberOrders');
      console.log('Current member orders result:', JSON.stringify(memberOrdersResult, null, 2));
    } else {
      console.log('No getCurrentMemberOrders method found');
    }
    
    console.log('\nSDK inspection completed!');
  } catch (err) {
    console.error('Error during SDK inspection:', err);
  }
}

// Run the inspection
inspectSDK().catch(console.error);
