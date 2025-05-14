/**
 * WixApiExplorer.js
 * A service for testing and exploring Wix API endpoints and data structures
 */
const axios = require('axios');
const fs = require('fs');
const path = require('path');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix API Explorer config');
  } catch (e) {
    console.error('Error loading Wix config for API Explorer:', e.message);
  }
}

// Wix API endpoints
const WIX_API_BASE = 'https://www.wixapis.com';
const CONTACTS_API = `${WIX_API_BASE}/contacts/v4/contacts`;
const MEMBERS_API = `${WIX_API_BASE}/members/v1/members`;

/**
 * Helper to log API calls for debugging
 */
const logApiCall = (endpoint, method, params) => {
  console.log(`[API Explorer] Calling: ${method} ${endpoint}`);
  console.log('[API Explorer] Params:', JSON.stringify(params, null, 2));
};

/**
 * Wix API Explorer Service
 */
module.exports = {
  /**
   * Get API configuration
   */
  getConfig() {
    return {
      apiKey: WIX_CONFIG.apiKey ? '***' + WIX_CONFIG.apiKey.substr(-6) : 'Not set',
      siteId: WIX_CONFIG.siteId || 'Not set',
      clientId: WIX_CONFIG.clientId || 'Not set'
    };
  },

  /**
   * List all available API endpoints for testing
   */
  getAvailableEndpoints() {
    return [
      { id: 'members-list', name: 'Members - List All', endpoint: `${MEMBERS_API}/query` },
      { id: 'members-search', name: 'Members - Search by Name', endpoint: `${MEMBERS_API}/search` },
      { id: 'contacts-list', name: 'Contacts - List All', endpoint: `${CONTACTS_API}/query` },
      { id: 'contacts-search', name: 'Contacts - Search by Name', endpoint: `${CONTACTS_API}/search` },
      { id: 'contacts-extended', name: 'Contacts - Extended Fields', endpoint: `${CONTACTS_API}/query` }
    ];
  },

  /**
   * Execute a test API call to explore data
   */
  async testApiCall(endpointId, searchParams = {}) {
    if (!WIX_CONFIG.apiKey) {
      return { error: 'API key not configured' };
    }

    try {
      let endpoint, method, data, headers;
      
      // Common headers
      headers = {
        'Authorization': WIX_CONFIG.apiKey,
        'Content-Type': 'application/json'
      };
      
      // Add site ID if available
      if (WIX_CONFIG.siteId) {
        headers['wix-site-id'] = WIX_CONFIG.siteId;
      }

      // Configure endpoint-specific parameters
      switch (endpointId) {
        case 'members-list':
          endpoint = `${MEMBERS_API}/query`;
          method = 'POST';
          data = { 
            query: {
              paging: { limit: 10 },
              sort: [{ fieldName: 'createdDate', order: 'DESC' }]
            }
          };
          break;
          
        case 'members-search':
          endpoint = `${MEMBERS_API}/search`;
          method = 'POST';
          data = { 
            search: { 
              name: searchParams.name || '' 
            }
          };
          break;
          
        case 'contacts-list':
          endpoint = `${CONTACTS_API}/query`;
          method = 'POST';
          data = {
            query: {
              paging: { limit: 10 },
              sort: [{ fieldName: 'info.name.last', order: 'ASC' }]
            },
            fields: ['info', 'customFields', 'picture', 'source']
          };
          break;
          
        case 'contacts-search':
          endpoint = `${CONTACTS_API}/search`;
          method = 'POST';
          data = {
            search: {
              freeText: searchParams.name || ''
            },
            fields: ['info', 'customFields', 'picture', 'source']
          };
          break;
          
        case 'contacts-extended':
          endpoint = `${CONTACTS_API}/query`;
          method = 'POST';
          data = {
            query: {
              paging: { limit: 10 }
            },
            fields: ['info', 'customFields', 'picture', 'source', 'extendedFields']
          };
          break;
          
        default:
          return { error: 'Unknown endpoint ID' };
      }

      // Log the API call
      logApiCall(endpoint, method, data);

      // Make the API call
      const response = await axios({
        method,
        url: endpoint,
        headers,
        data
      });

      // Return the full response for exploration
      return {
        success: true,
        data: response.data,
        endpoint: endpoint,
        requestData: data,
        // Add suggested field mappings based on response structure
        suggestedMappings: this.suggestFieldMappings(response.data, endpointId)
      };
    } catch (err) {
      console.error('[API Explorer] Error:', err);
      return { 
        error: err.message,
        details: err.response?.data || 'No additional details'
      };
    }
  },

  /**
   * Analyze response data and suggest field mappings to Scan-ID fields
   */
  suggestFieldMappings(responseData, endpointId) {
    const mappings = [];
    
    // Extract data based on endpoint type
    let items = [];
    if (endpointId.startsWith('members')) {
      items = responseData.members || [];
    } else if (endpointId.startsWith('contacts')) {
      items = responseData.contacts || [];
    }
    
    if (items.length === 0) {
      return [{ note: 'No items found to analyze' }];
    }
    
    // Sample the first item
    const sample = items[0];
    
    // Common mappings based on endpoint type
    if (endpointId.startsWith('members')) {
      mappings.push(
        { scanIdField: 'FIRST NAME', wixField: 'profile.firstName', example: sample.profile?.firstName || 'N/A' },
        { scanIdField: 'LAST NAME', wixField: 'profile.lastName', example: sample.profile?.lastName || 'N/A' },
        { scanIdField: 'FULL NAME', wixField: 'profile.firstName + profile.lastName', example: `${sample.profile?.firstName || ''} ${sample.profile?.lastName || ''}` },
        { scanIdField: 'BIRTHDATE', wixField: 'profile.birthdate', example: sample.profile?.birthdate || 'N/A' }
      );
    } else if (endpointId.startsWith('contacts')) {
      mappings.push(
        { scanIdField: 'FIRST NAME', wixField: 'info.name.first', example: sample.info?.name?.first || 'N/A' },
        { scanIdField: 'LAST NAME', wixField: 'info.name.last', example: sample.info?.name?.last || 'N/A' },
        { scanIdField: 'FULL NAME', wixField: 'info.name.full', example: sample.info?.name?.full || 'N/A' }
      );
      
      // Check if birthdate exists
      if (sample.info?.birthdate) {
        mappings.push({ scanIdField: 'BIRTHDATE', wixField: 'info.birthdate', example: sample.info.birthdate });
      }
      
      // Check for custom fields that might contain ID or birthdate info
      if (sample.customFields) {
        Object.keys(sample.customFields).forEach(key => {
          const value = sample.customFields[key];
          const fieldName = key.toLowerCase();
          
          if (fieldName.includes('id') || fieldName.includes('license')) {
            mappings.push({ scanIdField: 'DRV LC NO', wixField: `customFields.${key}`, example: value });
          }
          
          if (fieldName.includes('birth') || fieldName.includes('dob')) {
            mappings.push({ scanIdField: 'BIRTHDATE', wixField: `customFields.${key}`, example: value });
          }
        });
      }
    }
    
    return mappings;
  }
};
