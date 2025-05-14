/**
 * WixService.js
 * A comprehensive Wix member verification system using the Wix SDK
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for the Wix API
 * - Separation of Concerns: Maintaining clear boundaries between components
 */
const fs = require('fs');
const path = require('path');
// Import the adapter module but don't call any methods yet
const wixSdkAdapterModule = require('./WixSdkAdapter');
const WixSdkAdapter = wixSdkAdapterModule.adapter;
// Import the Pricing Plans service
const WixPricingPlansService = require('./WixPricingPlansService');

// Read config from file (for security, do NOT hardcode in source)
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CLIENT_ID = '';

if (fs.existsSync(CONFIG_PATH)) {
  try {
    const config = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    WIX_CLIENT_ID = config.clientId;
    console.log('Loaded Wix config with Client ID for SDK usage');
  } catch (e) {
    console.error('Error loading Wix config:', e.message);
  }
}

// For debugging
const logOperation = (operation, params) => {
  console.log(`Performing Wix SDK operation: ${operation}`);
  console.log('Params:', JSON.stringify(params, null, 2));
}

/**
 * Convert a string to title case (first letter of each word capitalized, rest lowercase)
 * This helps with matching names from Scan-ID (all caps) to Wix (proper case)
 */
const toTitleCase = (str) => {
  if (!str) return '';
  return str.toLowerCase()
    .split(' ')
    .map(word => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');
}

module.exports = {
  /**
   * Lookup Wix member by first name, last name, and DOB
   * Returns an array of matching profiles with source information
   * 
   * Following the Ethereal Engineering Technical Codex principles:
   * - Boundary Protection: Implementing strict interface contracts for the Wix API
   * - Separation of Concerns: Maintaining clear boundaries between components
   */
  async findMember({ firstName, lastName, dateOfBirth }) {
    // Validate that we have at least some search criteria
    if ((!firstName || firstName.trim() === '') && 
        (!lastName || lastName.trim() === '') && 
        (!dateOfBirth || dateOfBirth.trim() === '')) {
      return { 
        success: false, 
        error: 'At least one search parameter (firstName, lastName, or dateOfBirth) is required' 
      };
    }
    
    try {
      // Convert names to title case for better matching with Wix
      const formattedFirstName = toTitleCase(firstName);
      const formattedLastName = toTitleCase(lastName);
      
      logOperation('searchMember', { firstName: formattedFirstName, lastName: formattedLastName, dateOfBirth });
      
      // Use the SDK adapter to search for members
      const result = await WixSdkAdapter.searchMember({
        firstName: formattedFirstName,
        lastName: formattedLastName,
        dateOfBirth
      });
      
      return result;
    } catch (err) {
      console.error('Member lookup error:', err);
      
      return { 
        success: false, 
        error: `Error looking up member: ${err.message}`,
        details: err.stack || '',
        source: 'wix-service'
      };
    }
  },
  
  /**
   * Get pricing plans for a member
   * Returns pricing plan information for the specified member
   * 
   * Following the Ethereal Engineering Technical Codex principles:
   * - Boundary Protection: Implementing strict interface contracts for the Wix API
   * - Separation of Concerns: Maintaining clear boundaries between components
   */
  async getMemberPricingPlans(memberId) {
    if (!memberId) {
      return { success: false, error: 'Member ID is required' };
    }
    
    try {
      console.log(`Getting pricing plans for member: ${memberId}`);
      logOperation('getMemberPricingPlans', { memberId });
      
      // Use the SDK adapter to get pricing plans for the member
      const result = await WixSdkAdapter.getMemberPricingPlans(memberId);
      
      return result;
    } catch (err) {
      console.error('Pricing Plans error:', err);
      
      return { 
        success: false, 
        error: `Error getting pricing plans: ${err.message}`,
        details: err.stack || '',
        source: 'wix-service'
      };
    }
  },
  
  /**
   * List pricing plan orders
   * Retrieves a list of pricing plan orders with optional filtering and sorting
   * @param {Object} options - Options for filtering and sorting orders
   * @param {Object} options.filter - Filter criteria for orders
   * @param {Object} options.sort - Sorting options
   * @param {Object} options.paging - Pagination options
   * @returns {Promise<Object>} - List of orders and pagination metadata
   */
  async listOrders(options = {}) {
    try {
      console.log('[WixService] Listing pricing plan orders with options:', JSON.stringify(options, null, 2));
      logOperation('listOrders', options);
      
      // Use the WixPricingPlansService to list orders
      const result = await WixPricingPlansService.listOrders(options);
      
      return result;
    } catch (err) {
      console.error('[WixService] Orders error:', err);
      
      return { 
        success: false, 
        error: `Error listing orders: ${err.message}`,
        details: err.stack || '',
        source: 'wix-service'
      };
    }
  }
};
