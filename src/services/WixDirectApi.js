/**
 * WixDirectApi.js
 * 
 * DEPRECATED: This file is kept as a placeholder to prevent import errors.
 * According to the application's global rules, we should strictly follow Wix documentation
 * and not implement alternative methods or fallbacks.
 * 
 * All functionality has been migrated to use the official Wix SDK via WixSdkAdapter.js
 */

class WixDirectApi {
  constructor() {
    this.initialized = false;
    console.log('Direct API usage has been deprecated. Using Wix SDK instead.');
  }

  /**
   * Placeholder for compatibility
   */
  getHeaders() {
    return {};
  }

  /**
   * Placeholder for compatibility
   */
  async queryCollection(collectionId, query = {}) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api'
    };
  }

  /**
   * Placeholder for compatibility
   */
  async getItem(collectionId, itemId) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api'
    };
  }

  /**
   * Query members - Placeholder for compatibility
   */
  async queryMembers(query = {}) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api',
      message: 'This functionality has been migrated to use the official Wix SDK via WixSdkAdapter.js'
    };
  }

  /**
   * Placeholder for compatibility
   */
  async queryContacts(query = {}) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api'
    };
  }

  /**
   * Placeholder for compatibility
   */
  async searchMemberByNameOrDOB(name, dob) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api'
    };
  }

  /**
   * Placeholder for compatibility
   */
  async getMemberPricingPlans(memberId) {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api'
    };
  }
}

// Create a singleton instance
const directApi = new WixDirectApi();

module.exports = {
  /**
   * Placeholder for compatibility
   */
  async testDirectApi(endpoint = "members") {
    console.log('Direct API usage is deprecated. Please use the Wix SDK Adapter instead.');
    return {
      success: false,
      error: 'Direct API usage is deprecated. Please use the Wix SDK Adapter instead.',
      source: 'deprecated-direct-api',
      message: 'This functionality has been migrated to use the official Wix SDK via WixSdkAdapter.js'
    };
  },
  
  // Export the direct API instance for compatibility
  directApi
};
