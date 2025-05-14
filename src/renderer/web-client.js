/**
 * Web client API for mini check-in app
 * Provides browser-compatible API for accessing server endpoints
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for APIs
 * - Separation of Concerns: Maintaining clear boundaries between components
 * - Fail Fast and Learn: Using fallback mechanisms and detailed error reporting
 */

// Initialize Socket.io connection
let socket;
let scanWatchCallbacks = [];

// Connect to Socket.io when the document is ready
document.addEventListener('DOMContentLoaded', () => {
  // Load the Socket.io client script dynamically
  const script = document.createElement('script');
  script.src = '/socket.io/socket.io.js';
  script.onload = () => {
    // Initialize Socket.io connection
    socket = io();
    
    // Set up event handlers
    socket.on('connect', () => {
      console.log('Connected to server via Socket.io');
    });
    
    socket.on('scanWatchStatus', (status) => {
      console.log('Scan watch status:', status);
      // Notify all registered callbacks
      scanWatchCallbacks.forEach(callback => {
        try {
          callback('status', status);
        } catch (err) {
          console.error('Error in scan watch callback:', err);
        }
      });
    });
    
    socket.on('newScan', (scan) => {
      console.log('New scan received:', scan);
      // Notify all registered callbacks
      scanWatchCallbacks.forEach(callback => {
        try {
          callback('newscan', scan);
        } catch (err) {
          console.error('Error in scan watch callback:', err);
        }
      });
    });
    
    socket.on('scanWatchError', (error) => {
      console.error('Scan watch error:', error);
      // Notify all registered callbacks
      scanWatchCallbacks.forEach(callback => {
        try {
          callback('error', error);
        } catch (err) {
          console.error('Error in scan watch callback:', err);
        }
      });
    });
    
    socket.on('disconnect', () => {
      console.log('Disconnected from server');
    });
  };
  
  document.head.appendChild(script);
});

// Web client API for Wix API Explorer
window.wixExplorerAPI = {
  // Get API Explorer configuration
  getConfig: async () => {
    try {
      const response = await fetch('/api/wix-explorer/config');
      if (!response.ok) {
        const errorData = await response.json();
        return { error: errorData.error || 'Failed to get configuration' };
      }
      return await response.json();
    } catch (error) {
      console.error('Error getting API Explorer config:', error);
      return { error: error.message };
    }
  },
  
  // Get available API endpoints
  getEndpoints: async () => {
    try {
      const response = await fetch('/api/wix-explorer/endpoints');
      if (!response.ok) {
        const errorData = await response.json();
        return { error: errorData.error || 'Failed to get endpoints' };
      }
      return await response.json();
    } catch (error) {
      console.error('Error getting API endpoints:', error);
      return { error: error.message };
    }
  },
  
  // Test an API call
  testApiCall: async (endpointId, searchParams) => {
    try {
      const response = await fetch('/api/wix-explorer/test-api', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ endpointId, searchParams })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test API call' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing API call:', error);
      return { success: false, error: error.message };
    }
  }
};

// Web client API for Wix SDK testing
window.wixSdkAPI = {
  // Test the Wix SDK
  testSdk: async (collectionId) => {
    try {
      const response = await fetch('/api/wix-sdk/test', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ collectionId })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test SDK' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing SDK:', error);
      return { success: false, error: error.message };
    }
  },
  
  // Test the simple Wix SDK
  testSdkSimple: async (collectionId) => {
    try {
      const response = await fetch('/api/wix-sdk/test-simple', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ collectionId })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test SDK (simple)' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing SDK (simple):', error);
      return { success: false, error: error.message };
    }
  },
  
  // Inspect the Wix SDK
  inspectSdk: async () => {
    try {
      const response = await fetch('/api/wix-sdk/inspect');
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to inspect SDK' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error inspecting SDK:', error);
      return { success: false, error: error.message };
    }
  },
  
  // Test the Wix SDK adapter
  testAdapter: async (collectionId) => {
    try {
      const response = await fetch('/api/wix-sdk/adapter-test', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ collectionId })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test SDK adapter' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing SDK adapter:', error);
      return { success: false, error: error.message };
    }
  },
  
  // Test the Wix SDK compatibility adapter
  testCompatAdapter: async (collectionId) => {
    try {
      const response = await fetch('/api/wix-sdk/compat-test', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ collectionId })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test SDK compatibility adapter' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing SDK compatibility adapter:', error);
      return { success: false, error: error.message };
    }
  }
};

// Web client API for Wix Direct API testing
window.wixDirectAPI = {
  // Test the Wix Direct API
  testDirectApi: async (endpoint) => {
    try {
      const response = await fetch('/api/wix-direct/test', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ endpoint })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to test Direct API' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error testing Direct API:', error);
      return { success: false, error: error.message };
    }
  }
};

// Web client API for ScanID functionality
window.scanidAPI = {
  // Get the latest scan from Scan-ID CSV
  getLatestScan: async () => {
    try {
      const response = await fetch('/api/scanid/latest');
      if (!response.ok) {
        const errorData = await response.json();
        return { error: errorData.error || 'Failed to get latest scan' };
      }
      return await response.json();
    } catch (error) {
      console.error('Error getting latest scan:', error);
      return { error: error.message };
    }
  },
  
  // Search for a member by name or DOB
  searchMemberByNameOrDOB: async (name, dob) => {
    try {
      const response = await fetch('/api/wix/search-member', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ name, dob })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to search for member' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error searching for member:', error);
      return { success: false, error: error.message };
    }
  },
  
  // Get pricing plans for a member
  getMemberPricingPlans: async (memberId) => {
    try {
      const response = await fetch('/api/wix/pricing-plans', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ memberId })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to get pricing plans' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error getting pricing plans:', error);
      return { success: false, error: error.message };
    }
  },
  
  // Method to find Wix member by firstName, lastName, and dateOfBirth
  findWixMember: async (firstName, lastName, dateOfBirth) => {
    try {
      const response = await fetch('/api/wix/find-member', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ firstName, lastName, dateOfBirth })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to find member' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error finding member:', error);
      return { success: false, error: error.message };
    }
  },
  
  /**
   * List pricing plan orders with optional filtering and sorting
   * @param {Object} options - Options for filtering and sorting orders
   * @param {Object} options.filter - Filter criteria for orders
   * @param {Object} options.sort - Sorting options (fieldName: 'createdDate'|'endDate', direction: 'ASC'|'DESC')
   * @param {Object} options.paging - Pagination options (limit, offset)
   * @returns {Promise<Object>} - List of orders and pagination metadata
   */
  listPricingPlanOrders: async (options = {}) => {
    try {
      const response = await fetch('/api/wix/list-orders', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(options)
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        return { success: false, error: errorData.error || 'Failed to list orders' };
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error listing orders:', error);
      return { success: false, error: error.message };
    }
  },
  
  /**
   * Start watching for Scan-ID changes
   * @returns {Promise<Object>} - Status of the watch operation
   */
  startWatching: async () => {
    if (!socket) {
      return { success: false, error: 'Socket.io not connected' };
    }
    
    socket.emit('startWatching');
    return { success: true };
  },
  
  /**
   * Stop watching for Scan-ID changes
   * @returns {Promise<Object>} - Status of the watch operation
   */
  stopWatching: async () => {
    if (!socket) {
      return { success: false, error: 'Socket.io not connected' };
    }
    
    socket.emit('stopWatching');
    return { success: true };
  },
  
  /**
   * Register a callback for Scan-ID watch events
   * @param {Function} callback - Function to call when events occur
   * @returns {Function} - Function to unregister the callback
   */
  onScanWatchEvent: (callback) => {
    if (typeof callback !== 'function') {
      console.error('onScanWatchEvent requires a function callback');
      return () => {};
    }
    
    scanWatchCallbacks.push(callback);
    
    // Return a function to unregister the callback
    return () => {
      const index = scanWatchCallbacks.indexOf(callback);
      if (index !== -1) {
        scanWatchCallbacks.splice(index, 1);
      }
    };
  },
  
  /**
   * Get the current watching status
   * @returns {Promise<Object>} - Current status of the watch operation
   */
  getWatchStatus: async () => {
    if (!socket) {
      return { success: false, error: 'Socket.io not connected' };
    }
    
    return new Promise((resolve) => {
      socket.emit('getWatchStatus');
      socket.once('scanWatchStatus', (status) => {
        resolve({ success: true, watching: status.watching });
      });
      
      // Timeout after 2 seconds
      setTimeout(() => {
        resolve({ success: false, error: 'Timeout getting watch status' });
      }, 2000);
    });
  }
};
