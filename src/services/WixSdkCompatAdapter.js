/**
 * WixSdkCompatAdapter.js
 * A compatibility adapter for the Wix JavaScript SDK that works with the specific methods
 * available in version 1.15.18
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for external APIs
 * - Fail Fast and Learn: Implementing early failure detection and clear error reporting
 * - Reflective Engineering: Building self-auditing capabilities
 */
const fs = require('fs');
const path = require('path');
const { createClient, OAuthStrategy } = require('@wix/sdk');
const { items } = require('@wix/data');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix SDK Compat Adapter config');
  } catch (e) {
    console.error('Error loading Wix config for SDK Compat Adapter:', e.message);
  }
}

// SDK Version detection
const SDK_VERSION = require('@wix/sdk/package.json').version;
const DATA_VERSION = require('@wix/data/package.json').version;

console.log(`Wix SDK Compat Adapter initialized with SDK v${SDK_VERSION}, Data v${DATA_VERSION}`);

/**
 * Directly use the items module from @wix/data instead of trying to use it through the client
 */
class WixSdkCompatAdapter {
  constructor() {
    this.clientId = WIX_CONFIG.clientId || '8efc3d0c-9cfb-4d5d-a596-91c4eaa38bb9';
    this.initialized = false;
    this.sdkVersion = SDK_VERSION;
    this.dataVersion = DATA_VERSION;
    this.dataModule = items;
    
    // Detect available methods in the data module
    this.availableMethods = Object.getOwnPropertyNames(
      Object.getPrototypeOf(this.dataModule)
    ).filter(name => typeof this.dataModule[name] === 'function');
    
    console.log('Available data module methods:', this.availableMethods);
  }

  /**
   * Query items from a data collection using direct module access
   */
  async queryCollection(collectionId) {
    console.log(`Querying collection "${collectionId}" with SDK Compat Adapter`);
    
    try {
      // Try to use the data module directly
      if (typeof this.dataModule.query === 'function') {
        console.log('Using direct data module query() method');
        const result = await this.dataModule.query({
          dataCollectionId: collectionId
        });
        return result;
      }
      
      // Try alternative methods if available
      if (typeof this.dataModule.find === 'function') {
        console.log('Using direct data module find() method');
        const result = await this.dataModule.find({
          dataCollectionId: collectionId
        });
        return result;
      }
      
      // Try to use the module as a function if it is one
      if (typeof this.dataModule === 'function') {
        console.log('Using data module as a function');
        const dataInstance = this.dataModule(collectionId);
        
        if (dataInstance && typeof dataInstance.find === 'function') {
          const result = await dataInstance.find();
          return result;
        }
      }
      
      // If we get here, we couldn't find a compatible method
      throw new Error(`No compatible query method found in data module v${this.dataVersion}`);
    } catch (err) {
      console.error('Error in SDK Compat Adapter:', err);
      throw err;
    }
  }
}

// Create a singleton instance
const compatAdapter = new WixSdkCompatAdapter();

module.exports = {
  /**
   * Test the Wix SDK Compatibility Adapter
   */
  async testCompatAdapter(collectionId = "BannedNames") {
    try {
      console.log('Testing Wix SDK Compat Adapter with collection:', collectionId);
      
      // Try to get the raw module structure
      const moduleStructure = {
        type: typeof items,
        isFunction: typeof items === 'function',
        hasPrototype: !!Object.getPrototypeOf(items),
        properties: Object.keys(items),
        methods: compatAdapter.availableMethods,
        sdkVersion: compatAdapter.sdkVersion,
        dataVersion: compatAdapter.dataVersion
      };
      
      // Try to use the module directly
      let result = null;
      let success = false;
      let message = '';
      
      try {
        if (typeof items === 'function') {
          // Try using the module as a constructor or factory
          console.log('Trying to use data module as a function');
          const dataInstance = items(collectionId);
          
          if (dataInstance && typeof dataInstance.find === 'function') {
            result = await dataInstance.find();
            success = true;
            message = `Successfully used data module as a function for collection "${collectionId}"`;
          } else {
            throw new Error('Data instance does not have a find method');
          }
        } else {
          // Try direct query on the collection
          try {
            // Try the compatibility adapter
            result = await compatAdapter.queryCollection(collectionId);
            success = true;
            message = `Successfully queried collection "${collectionId}" using compatibility adapter`;
          } catch (adapterErr) {
            console.error('Adapter query failed:', adapterErr.message);
            
            // Try direct module access as a last resort
            if (typeof items.query === 'function') {
              result = await items.query({ dataCollectionId: collectionId });
              success = true;
              message = `Successfully queried collection "${collectionId}" using direct module access`;
            } else {
              throw new Error('No compatible query method available');
            }
          }
        }
      } catch (queryErr) {
        console.error('All query attempts failed:', queryErr.message);
        return {
          success: false,
          error: queryErr.message,
          details: queryErr.stack || '',
          moduleStructure
        };
      }
      
      return {
        success,
        message,
        result,
        moduleStructure
      };
    } catch (err) {
      console.error('Wix SDK Compat Adapter Test Error:', err);
      return {
        success: false,
        error: err.message,
        details: err.stack || '',
        moduleStructure: {
          sdkVersion: compatAdapter.sdkVersion,
          dataVersion: compatAdapter.dataVersion,
          availableMethods: compatAdapter.availableMethods || []
        }
      };
    }
  },
  
  // Export the adapter instance for direct use
  compatAdapter
};
