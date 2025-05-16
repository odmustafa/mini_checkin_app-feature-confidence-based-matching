/**
 * WixSdkAdapter.js
 * A version-adaptive implementation for the Wix JavaScript SDK
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for external APIs
 * - Fail Fast and Learn: Implementing early failure detection and clear error reporting
 * - Reflective Engineering: Building self-auditing capabilities
 */
const fs = require('fs');
const path = require('path');
const { createClient, OAuthStrategy } = require('@wix/sdk');
const { contacts } = require('@wix/crm');
const { items } = require('@wix/data');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix SDK Adapter config');
  } catch (e) {
    console.error('Error loading Wix config for SDK Adapter:', e.message);
  }
}

// SDK Version detection
const SDK_VERSION = require('@wix/sdk/package.json').version;
const CRM_VERSION = require('@wix/crm/package.json').version;

console.log(`Wix SDK Adapter initialized with SDK v${SDK_VERSION}, CRM v${CRM_VERSION}`);

class WixSdkAdapter {
  constructor() {
    this.client = null;
    this.clientId = WIX_CONFIG.clientId;
    this.appSecret = WIX_CONFIG.appSecret;
    this.publicKey = WIX_CONFIG.publicKey;
    this.siteId = WIX_CONFIG.siteId;
    this.apiKey = WIX_CONFIG.apiKey;
    this.initialized = false;
    this.sdkVersion = SDK_VERSION;
    this.crmVersion = CRM_VERSION;
    this.dataVersion = require('@wix/data/package.json').version;
    this.availableMethods = [];
    this.contactsModule = null;
  }

  /**
   * Initialize the Wix client
   */
  async initialize() {
    if (this.initialized) return;
    
    try {
      console.log('Initializing Wix SDK client with API Key strategy');
      
      // Import the modules
      const { createClient, ApiKeyStrategy } = require('@wix/sdk');
      const { contacts } = require('@wix/crm');
      const { items } = require('@wix/data');
      
      // Store the modules for later use
      this.contactsModule = contacts;
      
      // Configure authentication with API Key strategy
      console.log('Authentication configured with API Key strategy');
      console.log(`- API Key: ${this.apiKey.substring(0, 20)}...`);
      console.log(`- Site ID: ${this.siteId}`);
      console.log('- Including Account ID in headers for account-level API access');
      
      // Account ID for account-level API access
      const accountId = '11a11ce3-d0da-46c7-b4e4-48c17df562f0';
      
      this.client = createClient({
        modules: { items, contacts },
        auth: ApiKeyStrategy({
          apiKey: this.apiKey,
          siteId: this.siteId
        }),
        // Add headers for account-level API access
        headers: {
          'wix-account-id': accountId
        }
      });
      
      // Detect available methods
      this.availableMethods = Object.getOwnPropertyNames(
        Object.getPrototypeOf(this.client.contacts)
      ).filter(name => typeof this.client.contacts[name] === 'function');
      
      console.log('Available contacts methods:', this.availableMethods);
      
      this.initialized = true;
      return true;
    } catch (err) {
      console.error('Error initializing Wix SDK Adapter:', err);
      throw err;
    }
  }

  /**
   * Query items from a data collection using the appropriate method for the SDK version
   */
  async queryCollection(collectionId) {
    await this.initialize();
    
    console.log(`Querying collection "${collectionId}" with SDK Adapter`);
    
    // Try different methods based on availability
    if (this.availableMethods.includes('query')) {
      console.log('Using query() method');
      return await this.client.items.query({
        dataCollectionId: collectionId
      });
    } 
    else if (this.availableMethods.includes('find')) {
      console.log('Using find() method');
      return await this.client.items.find({
        dataCollectionId: collectionId
      });
    }
    else {
      console.log('Using list() method');
      return await this.client.items.list(collectionId);
    }
  }

  /**
   * Get a specific item by ID
   */
  async getItem(collectionId, itemId) {
    await this.initialize();
    
    console.log(`Getting item "${itemId}" from collection "${collectionId}"`);
    
    return await this.client.items.getItem({
      dataCollectionId: collectionId,
      itemId: itemId
    });
  }

  /**
   * Create a new item in a collection
   */
  async createItem(collectionId, data) {
    await this.initialize();
    
    console.log(`Creating item in collection "${collectionId}"`);
    
    return await this.client.items.createItem({
      dataCollectionId: collectionId,
      item: data
    });
  }

  /**
   * Search for members by name and date of birth
   * Following the Ethereal Engineering Technical Codex principles:
   * - Boundary Protection: Implementing strict interface contracts for the Wix API
   * - Separation of Concerns: Maintaining clear boundaries between components
   */
  async searchMember({ firstName, lastName, dateOfBirth }) {
    try {
      await this.initialize();
      
      console.log(`Searching for member with name: ${firstName} ${lastName}, DOB: ${dateOfBirth}`);
      
      // Create a query builder for contacts
      const queryBuilder = this.client.contacts.queryContacts();
      
      // Parse the name parts for more flexible matching
      const firstNameParts = firstName ? firstName.toLowerCase().split(/\s+/) : [];
      const formattedLastName = lastName ? lastName.toLowerCase() : '';
      
      // Build the query
      if (firstName) {
        // Use a more flexible approach for first name
        // This handles cases where the first name might be stored in different fields
        queryBuilder.or(
          queryBuilder.filter('info.name.first', 'contains', firstName),
          queryBuilder.filter('info.name.first', 'startsWith', firstName)
        );
      }
      
      if (lastName) {
        // Use a more flexible approach for last name
        queryBuilder.and(
          queryBuilder.or(
            queryBuilder.filter('info.name.last', 'contains', lastName),
            queryBuilder.filter('info.name.last', 'startsWith', lastName)
          )
        );
      }
      
      // Add date of birth filter if provided
      if (dateOfBirth) {
        // Format the date of birth for comparison
        const dobDate = new Date(dateOfBirth);
        const formattedDob = dobDate.toISOString().split('T')[0]; // YYYY-MM-DD format
        
        queryBuilder.and(
          queryBuilder.filter('info.birthdate', 'eq', formattedDob)
        );
      }
      
      // Set a reasonable limit
      queryBuilder.limit(10);
      
      console.log('Executing contact search query');
      
      // Execute the query
      const response = await queryBuilder.find();
      
      // Get the results
      const contacts = response.items || [];
      console.log(`Found ${contacts.length} contacts matching criteria`);
      
      // Calculate confidence scores for each contact
      const contactsWithScores = this.calculateContactConfidenceScores(contacts, {
        firstNameParts,
        formattedLastName,
        dateOfBirth
      });
      
      // Sort by confidence score (highest first)
      const sortedContacts = contactsWithScores.sort((a, b) => b.confidenceScore - a.confidenceScore);
      
      // Return the results
      return {
        success: true,
        contacts: sortedContacts,
        total: sortedContacts.length,
        source: 'wix-crm-contacts'
      };
    } catch (err) {
      console.error('Error searching for member with CRM Contacts API:', err);
      
      return {
        success: false,
        error: `Error searching for member: ${err.message}`,
        details: err.stack || '',
        source: 'wix-crm-contacts'
      };
    }
  }

  /**
   * Calculate confidence scores for contacts based on how well they match the search criteria
   * @param {Array} contacts - Array of contact objects from Wix CRM Contacts API
   * @param {Object} searchCriteria - Object containing search criteria (firstNameParts, formattedLastName, dateOfBirth)
   * @returns {Array} - Array of contact objects with confidence scores, sorted by confidence
   */
  calculateContactConfidenceScores(contacts, searchCriteria) {
    const { firstNameParts, formattedLastName, dateOfBirth } = searchCriteria;
    
    return contacts.map(contact => {
      // Start with a base confidence score
      let confidenceScore = 0;
      let matchDetails = [];
      
      // Extract contact information
      const contactFirstName = (contact.info?.name?.first || '').toLowerCase();
      const contactLastName = (contact.info?.name?.last || '').toLowerCase();
      const contactDob = contact.info?.birthdate || '';
      
      // Calculate first name similarity
      if (contactFirstName && firstNameParts.length > 0) {
        // Check for exact match
        if (firstNameParts.some(part => contactFirstName === part)) {
          confidenceScore += 0.4;
          matchDetails.push('First name exact match');
        } 
        // Check for partial match
        else if (firstNameParts.some(part => contactFirstName.includes(part) || part.includes(contactFirstName))) {
          const bestMatchPart = firstNameParts.reduce((best, part) => {
            const similarity = this.calculateStringSimilarity(contactFirstName, part);
            return similarity > best.similarity ? { part, similarity } : best;
          }, { part: '', similarity: 0 });
          
          confidenceScore += 0.3 * bestMatchPart.similarity;
          matchDetails.push(`First name partial match (${Math.round(bestMatchPart.similarity * 100)}%)`);
        }
      }
      
      // Calculate last name similarity
      if (contactLastName && formattedLastName) {
        const lastNameSimilarity = this.calculateStringSimilarity(contactLastName, formattedLastName);
        
        // Exact match
        if (lastNameSimilarity > 0.9) {
          confidenceScore += 0.4;
          matchDetails.push('Last name exact match');
        }
        // Strong partial match
        else if (lastNameSimilarity > 0.7) {
          confidenceScore += 0.3 * lastNameSimilarity;
          matchDetails.push(`Last name strong match (${Math.round(lastNameSimilarity * 100)}%)`);
        }
        // Weak partial match
        else if (lastNameSimilarity > 0.5) {
          confidenceScore += 0.2 * lastNameSimilarity;
          matchDetails.push(`Last name weak match (${Math.round(lastNameSimilarity * 100)}%)`);
        }
      }
      
      // Calculate date of birth match
      if (contactDob && dateOfBirth) {
        // Format dates for comparison
        const contactDobDate = new Date(contactDob);
        const searchDobDate = new Date(dateOfBirth);
        
        // Check if dates are valid
        if (!isNaN(contactDobDate.getTime()) && !isNaN(searchDobDate.getTime())) {
          // Format as YYYY-MM-DD for comparison
          const contactDobFormatted = contactDobDate.toISOString().split('T')[0];
          const searchDobFormatted = searchDobDate.toISOString().split('T')[0];
          
          // Exact match
          if (contactDobFormatted === searchDobFormatted) {
            confidenceScore += 0.2;
            matchDetails.push('Date of birth exact match');
          }
          // Year and month match
          else if (contactDobDate.getFullYear() === searchDobDate.getFullYear() && 
                   contactDobDate.getMonth() === searchDobDate.getMonth()) {
            confidenceScore += 0.1;
            matchDetails.push('Date of birth year and month match');
          }
          // Only year matches
          else if (contactDobDate.getFullYear() === searchDobDate.getFullYear()) {
            confidenceScore += 0.05;
            matchDetails.push('Date of birth year match');
          }
        }
      }
      
      // Calculate confidence level based on score
      let confidenceLevel = 'Low';
      if (confidenceScore > 0.8) confidenceLevel = 'Very High';
      else if (confidenceScore > 0.6) confidenceLevel = 'High';
      else if (confidenceScore > 0.4) confidenceLevel = 'Medium';
      else if (confidenceScore > 0.2) confidenceLevel = 'Low';
      else confidenceLevel = 'Very Low';
      
      // Add confidence information to the contact object
      return {
        ...contact,
        confidenceScore,
        confidenceLevel,
        matchDetails
      };
    });
  }

  /**
   * Calculate string similarity between two strings (0-1 scale)
   * @param {string} str1 - First string
   * @param {string} str2 - Second string
   * @returns {number} - Similarity score between 0 and 1
   */
  calculateStringSimilarity(str1, str2) {
    // If either string is empty, return 0
    if (!str1 || !str2) return 0;
    
    // Convert both strings to lowercase for case-insensitive comparison
    const s1 = str1.toLowerCase();
    const s2 = str2.toLowerCase();
    
    // Calculate Levenshtein distance
    const m = s1.length;
    const n = s2.length;
    
    // Create a matrix of size (m+1) x (n+1)
    const d = Array(m + 1).fill().map(() => Array(n + 1).fill(0));
    
    // Initialize the first row and column
    for (let i = 0; i <= m; i++) d[i][0] = i;
    for (let j = 0; j <= n; j++) d[0][j] = j;
    
    // Fill the matrix
    for (let i = 1; i <= m; i++) {
      for (let j = 1; j <= n; j++) {
        const cost = s1[i - 1] === s2[j - 1] ? 0 : 1;
        d[i][j] = Math.min(
          d[i - 1][j] + 1,      // deletion
          d[i][j - 1] + 1,      // insertion
          d[i - 1][j - 1] + cost // substitution
        );
      }
    }
    
    // Calculate similarity as 1 - normalized distance
    const distance = d[m][n];
    const maxLength = Math.max(m, n);
    
    // Return similarity score between 0 and 1
    return maxLength ? 1 - distance / maxLength : 1;
  }
  
  /**
   * Get pricing plans for a member
   * Following the Ethereal Engineering Technical Codex principles:
   * - Boundary Protection: Implementing strict interface contracts for the Wix API
   * - Separation of Concerns: Maintaining clear boundaries between components
   * 
   * This implementation strictly follows the Wix JavaScript SDK documentation for pricing plans
   */
  async getMemberPricingPlans(memberId) {
    try {
      await this.initialize();
      
      console.log(`Getting pricing plans for member: ${memberId} with SDK Adapter`);
      
      if (!memberId) {
        throw new Error('Member ID is required');
      }
      
      // Import the pricing plans module
      const { orders } = require('@wix/pricing-plans');
      
      // Add the pricing plans module to the client
      if (!this.client.orders) {
        console.log('Adding pricing-plans orders module to the client');
        this.client.orders = orders;
      }
      
      // Log the request details for debugging
      console.log(`Requesting pricing plan orders for buyerId: ${memberId}`);
      
      // Use the managementListOrders method according to Wix JavaScript SDK documentation
      // https://dev.wix.com/docs/sdk/backend-modules/pricing-plans/orders/management-list-orders
      const response = await this.client.orders.managementListOrders({
        // Filter by buyerId using the proper parameter structure
        buyerIds: [memberId],
        // Sort by created date in descending order (newest first)
        sort: {
          fieldName: 'createdDate',
          order: 'DESC'
        },
        // Get maximum allowed orders
        limit: 50
      });
      
      // Log the response for debugging
      console.log(`Found ${response.orders?.length || 0} orders for member ${memberId}`);
      
      // Process orders to extract plan details according to the SDK documentation structure
      const processedOrders = (response.orders || []).map(order => {
        // Extract buyer information from the correct location in the response
        const buyerId = order.buyer?.memberId || 'N/A';
        const contactId = order.buyer?.contactId || 'N/A';
        
        // Extract pricing information from the correct location in the response
        const priceAmount = order.planPrice?.amount;
        const priceCurrency = order.planPrice?.currency;
        
        // Get creation date from the correct field (_createdDate)
        const createdDate = order._createdDate || null;
        
        // Get start date from the appropriate fields based on the SDK structure
        const startDate = order.startDate || 
                         (order.currentCycle && order.currentCycle.startedDate) || 
                         createdDate;
        
        // Format payment amount based on available data
        const paymentAmount = priceAmount !== undefined ? 
          `${priceAmount} ${priceCurrency || ''}` : 
          'Free';
        
        // Calculate if plan is active based on status and dates
        const isActive = order.status === 'ACTIVE' && 
                        (!order.endDate || new Date(order.endDate) > new Date());
        
        // Return a comprehensive processed order object with all available information
        return {
          // Original order data
          _id: order._id,
          planId: order.planId,
          
          // Plan information
          planName: order.planName || 'Unnamed Plan',
          planDescription: order.planDescription || '',
          status: order.status || 'UNKNOWN',
          
          // Buyer information
          buyerId: buyerId,
          contactId: contactId,
          
          // Dates
          createdDate: createdDate,
          startDate: startDate,
          endDate: order.endDate,
          
          // Pricing information
          price: priceAmount,
          currency: priceCurrency,
          paymentStatus: order.lastPaymentStatus || 'UNKNOWN',
          orderMethod: order.orderMethod || 'UNKNOWN',
          
          // Recurring information
          isRecurring: !!order.recurring,
          recurringCycle: order.recurring?.cycle,
          recurringPeriod: order.recurring?.period,
          
          // Payment details
          paymentMethod: order.paymentDetails?.paymentMethod || 'Unknown',
          paymentAmount: paymentAmount,
          paymentDate: order.paymentDetails?.paymentDate,
          
          // Formatted dates for display
          formattedStartDate: startDate ? new Date(startDate).toLocaleDateString() : 'N/A',
          formattedEndDate: order.endDate ? new Date(order.endDate).toLocaleDateString() : 'No expiration',
          formattedCreatedDate: createdDate ? new Date(createdDate).toLocaleString() : 'Unknown',
          
          // Status indicators
          isActive: isActive,
          isExpired: order.status === 'ENDED' || (order.endDate && new Date(order.endDate) < new Date()),
          isPaused: order.pausePeriods && order.pausePeriods.length > 0
        };
      });
      
      // Separate active and inactive plans for better organization
      const activePlans = processedOrders.filter(plan => plan.isActive);
      const inactivePlans = processedOrders.filter(plan => !plan.isActive);
      
      // Sort plans by creation date (newest first within each category)
      const sortedActivePlans = activePlans.sort((a, b) => {
        const dateA = a.createdDate ? new Date(a.createdDate) : new Date(0);
        const dateB = b.createdDate ? new Date(b.createdDate) : new Date(0);
        return dateB - dateA;
      });
      
      const sortedInactivePlans = inactivePlans.sort((a, b) => {
        const dateA = a.createdDate ? new Date(a.createdDate) : new Date(0);
        const dateB = b.createdDate ? new Date(b.createdDate) : new Date(0);
        return dateB - dateA;
      });
      
      // Combine sorted plans with active plans first
      const sortedPlans = [...sortedActivePlans, ...sortedInactivePlans];
      
      return {
        success: true,
        plans: sortedPlans,
        orders: processedOrders, // For backward compatibility
        activePlans: sortedActivePlans,
        inactivePlans: sortedInactivePlans,
        total: processedOrders.length,
        source: 'wix-pricing-plans'
      };
    } catch (err) {
      console.error('Error getting pricing plans with SDK:', err);
      
      // Ensure we have a valid error message even if err is empty or undefined
      const errorMessage = err && err.message ? err.message : 'Unknown error occurred while retrieving pricing plans';
      
      // Create a detailed error response
      return {
        success: false,
        error: `Error getting pricing plans: ${errorMessage}`,
        details: err && err.stack ? err.stack : 'No stack trace available',
        errorObject: err || {},
        source: 'wix-sdk',
        timestamp: new Date().toISOString()
      };
    }
  }
}

// Create an instance of the adapter
const adapter = new WixSdkAdapter();

// Export the adapter methods
module.exports = {
  /**
   * Search for a contact using the Wix CRM Contacts API
   * Note: Method name kept as searchMember for backward compatibility
   */
  searchMember: async function({ firstName, lastName, dateOfBirth }) {
    try {
      return await adapter.searchMember({ firstName, lastName, dateOfBirth });
    } catch (err) {
      console.error('CRM Contacts API searchMember error:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-crm-contacts-adapter'
      };
    }
  },
  
  /**
   * Get pricing plans for a member
   * Following the Ethereal Engineering Technical Codex principles:
   * - Boundary Protection: Implementing strict interface contracts for the Wix API
   * - Separation of Concerns: Maintaining clear boundaries between components
   * 
   * This implementation strictly follows the Wix JavaScript SDK documentation for pricing plans
   */
  getMemberPricingPlans: async function(memberId) {
    try {
      return await adapter.getMemberPricingPlans(memberId);
    } catch (err) {
      console.error('Error getting pricing plans with SDK:', err);
      return {
        success: false,
        error: `Error getting pricing plans: ${err.message}`,
        details: err && err.stack ? err.stack : 'No stack trace available',
        source: 'wix-sdk'
      };
    }
  },
  
  /**
   * List pricing plan orders filtered by buyerId
   */
  listPricingPlanOrders: async function(filter) {
    try {
      if (!adapter.initialized) {
        await adapter.initialize();
      }
      
      // Import the pricing plans module
      const { orders } = require('@wix/pricing-plans');
      
      // Add the pricing plans module to the client
      if (!adapter.client.orders) {
        console.log('Adding pricing-plans orders module to the client');
        adapter.client.orders = orders;
      }
      
      console.log('Listing pricing plan orders with filter:', filter);
      
      // Use the managementListOrders method with the provided filter
      const response = await adapter.client.orders.managementListOrders({
        filter: filter
      });
      
      console.log(`Found ${response.orders?.length || 0} orders matching filter`);
      
      return {
        success: true,
        orders: response.orders || [],
        total: response.orders?.length || 0,
        source: 'wix-pricing-plans'
      };
    } catch (err) {
      console.error('Error listing pricing plan orders:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-sdk-adapter'
      };
    }
  },
  
  /**
   * Test the Wix SDK Adapter
   */
  async testAdapter(collectionId = "BannedNames") {
    try {
      console.log('Testing Wix SDK Adapter with collection:', collectionId);
      
      // Initialize the adapter
      await adapter.initialize();
      
      // Query the collection
      const dataItems = await adapter.queryCollection(collectionId);
      
      console.log('SDK Adapter query completed successfully');
      
      // Format the results
      let items = [];
      let total = 0;
      
      if (dataItems.items) {
        // Standard response format
        items = dataItems.items;
        total = items.length;
      } else if (Array.isArray(dataItems)) {
        // List response format
        items = dataItems;
        total = items.length;
      } else {
        // Unknown format - try to extract items
        items = dataItems.results || dataItems.data || [];
        total = items.length;
      }
      
      const result = {
        sdkVersion: adapter.sdkVersion,
        dataVersion: adapter.dataVersion,
        availableMethods: adapter.availableMethods,
        total: total,
        items: items,
        rawData: dataItems
      };
      
      return {
        success: true,
        result: result,
        message: `Successfully retrieved ${total} items from collection "${collectionId}" using SDK Adapter`
      };
    } catch (err) {
      console.error('Wix SDK Adapter Test Error:', err);
      return {
        success: false,
        error: err.message,
        details: err.stack || '',
        sdkVersion: adapter.sdkVersion,
        dataVersion: adapter.dataVersion,
        availableMethods: adapter.availableMethods || []
      };
    }
  },
  
  /**
   * Query all contacts without any filters (exported as a module function)
   */
  queryAllContacts: async function() {
    try {
      return await adapter.queryAllContacts();
    } catch (err) {
      console.error('CRM Contacts API queryAllContacts error:', err);
      return {
        success: false,
        error: err.message,
        source: 'wix-crm-contacts-adapter'
      };
    }
  },
  
  /**
   * Method to query all contacts without any filters (exported as a module function)
   */
  queryAllContactsStatic: async function() {
    try {
      console.log('Static method: Querying all contacts without filters');
      
      // Create a new adapter instance
      const adapter = new WixSdkAdapter();
      await adapter.initialize();
      
      // Create a query builder for contacts with no filters
      const queryBuilder = adapter.client.contacts.queryContacts()
        .limit(50); // Increased limit to get more results
      
      console.log('Executing query for all contacts');
      
      // Execute the query by calling find() on the query builder
      const response = await queryBuilder.find();
      
      // Get the results
      const results = response.items || [];
      console.log(`Found ${results.length} total contacts`);
      
      return {
        success: true,
        items: results,
        total: results.length,
        source: 'wix-crm-contacts'
      };
    } catch (err) {
      console.error('Error querying all contacts:', err);
      return {
        success: false,
        error: err.message,
        stack: err.stack
      };
    }
  }
};
