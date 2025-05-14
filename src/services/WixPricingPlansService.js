/**
 * WixPricingPlansService.js
 * A comprehensive service for interacting with Wix Pricing Plans and Orders
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for the Wix API
 * - Separation of Concerns: Maintaining clear boundaries between components
 * - Fail Fast and Learn: Implementing early failure detection and clear error reporting
 */
const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders, plans } = require('@wix/pricing-plans');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix config for Pricing Plans Service');
  } catch (e) {
    console.error('Error loading Wix config for Pricing Plans Service:', e.message);
  }
}

class WixPricingPlansService {
  constructor() {
    this.client = null;
    this.initialized = false;
  }

  /**
   * Initialize the Wix client
   */
  /**
   * Get available methods on a module
   * @param {string} moduleName - Name of the module to inspect
   * @returns {Array<string>} - Array of available method names
   */
  getAvailableMethods(moduleName) {
    if (!this.client || !this.client[moduleName]) {
      return [];
    }
    
    // Get all properties including methods
    const allProps = Object.getOwnPropertyNames(this.client[moduleName]);
    
    // Filter to only include methods
    const methods = allProps.filter(prop => {
      return typeof this.client[moduleName][prop] === 'function';
    });
    
    return methods;
  }
  
  /**
   * Initialize the Wix client
   */
  async initialize() {
    if (this.initialized) return;
    
    try {
      console.log('Initializing Wix SDK client for Pricing Plans');
      
      // Configure authentication with API Key strategy
      this.client = createClient({
        modules: { orders, plans },
        auth: ApiKeyStrategy({
          apiKey: WIX_CONFIG.apiKey,
          siteId: WIX_CONFIG.siteId
        })
      });
      
      this.initialized = true;
      return true;
    } catch (err) {
      console.error('Error initializing Wix Pricing Plans Service:', err);
      throw err;
    }
  }

  /**
   * Get all pricing plans
   * @returns {Promise<Object>} - List of pricing plans
   */
  async getAllPricingPlans() {
    await this.initialize();
    
    try {
      console.log('Retrieving all pricing plans...');
      
      // Query all pricing plans
      // Check which method is available
      if (typeof this.client.plans.queryPublicPlans === 'function') {
        const response = await this.client.plans.queryPublicPlans();
        console.log(`Found ${response.plans?.length || 0} pricing plans`);
        return {
          success: true,
          plans: response.plans || [],
          total: response.plans?.length || 0,
          source: 'wix-pricing-plans'
        };
      } else if (typeof this.client.plans.query === 'function') {
        const response = await this.client.plans.query();
        console.log(`Found ${response.items?.length || 0} pricing plans`);
        return {
          success: true,
          plans: response.items || [],
          total: response.items?.length || 0,
          source: 'wix-pricing-plans'
        };
      } else {
        // Log available methods for debugging
        const methods = this.getAvailableMethods('plans');
        console.log('Available methods on plans:', methods);
        return {
          success: false,
          error: 'No suitable method found to query pricing plans',
          availableMethods: methods,
          source: 'wix-pricing-plans'
        };
      }
    } catch (err) {
      console.error('Error retrieving pricing plans:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get pricing plan by ID
   * @param {string} planId - ID of the pricing plan to retrieve
   * @returns {Promise<Object>} - Pricing plan details
   */
  async getPricingPlanById(planId) {
    await this.initialize();
    
    try {
      console.log(`Retrieving pricing plan with ID: ${planId}`);
      
      // Get the pricing plan by ID
      const response = await this.client.plans.getPlan({ planId });
      
      return {
        success: true,
        plan: response,
        source: 'wix-pricing-plans'
      };
    } catch (err) {
      console.error(`Error retrieving pricing plan with ID ${planId}:`, err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get all orders with optional filtering
   * @param {Object} options - Options for filtering and sorting orders
   * @param {Object} options.filter - Filter criteria for orders
   * @param {Object} options.sort - Sorting options
   * @param {Object} options.paging - Pagination options
   * @returns {Promise<Object>} - List of orders and pagination metadata
   */
  async listOrders(options = {}) {
    await this.initialize();
    
    try {
      console.log('Retrieving pricing plan orders with options:', JSON.stringify(options, null, 2));
      
      // Build the query parameters
      const queryParams = {
        // Default to 50 items per page if not specified
        limit: options.paging?.limit || 50,
        offset: options.paging?.offset || 0,
        
        // Include the filter if provided
        filter: options.filter || undefined,
        
        // Include sorting if provided
        sort: options.sort || undefined
      };
      
      // Check which method is available for listing orders
      let response;
      if (typeof this.client.orders.listOrders === 'function') {
        response = await this.client.orders.listOrders(queryParams);
      } else if (typeof this.client.orders.query === 'function') {
        response = await this.client.orders.query(queryParams);
      } else if (typeof this.client.orders.list === 'function') {
        response = await this.client.orders.list(queryParams);
      } else {
        // Log available methods for debugging
        const methods = this.getAvailableMethods('orders');
        console.log('Available methods on orders:', methods);
        return {
          success: false,
          error: 'No suitable method found to list orders',
          availableMethods: methods,
          source: 'wix-pricing-plans'
        };
      }
      
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

  /**
   * Get orders by buyer ID
   * @param {string} buyerId - ID of the buyer to retrieve orders for
   * @param {Object} options - Additional options for filtering and sorting
   * @returns {Promise<Object>} - List of orders for the specified buyer
   */
  async getOrdersByBuyerId(buyerId, options = {}) {
    try {
      console.log(`Retrieving orders for buyer ID: ${buyerId}`);
      
      // Create a filter for the buyer ID
      const filter = {
        buyerId: {
          $eq: buyerId
        }
      };
      
      // Merge with any additional filters
      const mergedOptions = {
        ...options,
        filter: {
          ...filter,
          ...(options.filter || {})
        }
      };
      
      return await this.listOrders(mergedOptions);
    } catch (err) {
      console.error('Error retrieving orders by buyer ID:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get orders by plan ID
   * @param {string} planId - ID of the plan to retrieve orders for
   * @param {Object} options - Additional options for filtering and sorting
   * @returns {Promise<Object>} - List of orders for the specified plan
   */
  async getOrdersByPlanId(planId, options = {}) {
    try {
      console.log(`Retrieving orders for plan ID: ${planId}`);
      
      // Create a filter for the plan ID
      const filter = {
        planId: {
          $eq: planId
        }
      };
      
      // Merge with any additional filters
      const mergedOptions = {
        ...options,
        filter: {
          ...filter,
          ...(options.filter || {})
        }
      };
      
      return await this.listOrders(mergedOptions);
    } catch (err) {
      console.error('Error retrieving orders by plan ID:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get orders by date range
   * @param {string|Date} startDate - Start date for the range
   * @param {string|Date} endDate - End date for the range
   * @param {Object} options - Additional options for filtering and sorting
   * @returns {Promise<Object>} - List of orders within the specified date range
   */
  async getOrdersByDateRange(startDate, endDate, options = {}) {
    try {
      console.log(`Retrieving orders between ${startDate} and ${endDate}`);
      
      // Create a filter for the date range
      const filter = {
        _createdDate: {
          $gte: new Date(startDate).toISOString(),
          $lte: new Date(endDate).toISOString()
        }
      };
      
      // Merge with any additional filters
      const mergedOptions = {
        ...options,
        filter: {
          ...filter,
          ...(options.filter || {})
        }
      };
      
      return await this.listOrders(mergedOptions);
    } catch (err) {
      console.error('Error retrieving orders by date range:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get orders by status
   * @param {string} status - Status to filter orders by
   * @param {Object} options - Additional options for filtering and sorting
   * @returns {Promise<Object>} - List of orders with the specified status
   */
  async getOrdersByStatus(status, options = {}) {
    try {
      console.log(`Retrieving orders with status: ${status}`);
      
      // Create a filter for the status
      const filter = {
        status: {
          $eq: status
        }
      };
      
      // Merge with any additional filters
      const mergedOptions = {
        ...options,
        filter: {
          ...filter,
          ...(options.filter || {})
        }
      };
      
      return await this.listOrders(mergedOptions);
    } catch (err) {
      console.error('Error retrieving orders by status:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get order by ID
   * @param {string} orderId - ID of the order to retrieve
   * @returns {Promise<Object>} - Order details
   */
  async getOrderById(orderId) {
    await this.initialize();
    
    try {
      console.log(`Retrieving order with ID: ${orderId}`);
      
      // Check which method is available for getting an order
      let response;
      if (typeof this.client.orders.getOrder === 'function') {
        response = await this.client.orders.getOrder({ orderId });
      } else if (typeof this.client.orders.get === 'function') {
        response = await this.client.orders.get({ orderId });
      } else {
        // Log available methods for debugging
        const methods = this.getAvailableMethods('orders');
        console.log('Available methods on orders:', methods);
        return {
          success: false,
          error: 'No suitable method found to get order by ID',
          availableMethods: methods,
          source: 'wix-pricing-plans'
        };
      }
      
      return {
        success: true,
        order: response,
        source: 'wix-pricing-plans'
      };
    } catch (err) {
      console.error(`Error retrieving order with ID ${orderId}:`, err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }

  /**
   * Get active orders for a member
   * @param {string} memberId - ID of the member to retrieve active orders for
   * @param {Object} options - Additional options for filtering and sorting
   * @returns {Promise<Object>} - List of active orders for the specified member
   */
  async getActiveMemberOrders(memberId, options = {}) {
    try {
      console.log(`Retrieving active orders for member ID: ${memberId}`);
      
      // Create a filter for active orders for the member
      const filter = {
        buyerId: {
          $eq: memberId
        },
        status: {
          $eq: 'ACTIVE'
        }
      };
      
      // Merge with any additional filters
      const mergedOptions = {
        ...options,
        filter: {
          ...filter,
          ...(options.filter || {})
        }
      };
      
      return await this.listOrders(mergedOptions);
    } catch (err) {
      console.error('Error retrieving active orders for member:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-pricing-plans'
      };
    }
  }
}

// Create a singleton instance
const pricingPlansService = new WixPricingPlansService();

module.exports = pricingPlansService;
