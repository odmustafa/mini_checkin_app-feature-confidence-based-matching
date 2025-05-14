/**
 * Web server for mini check-in app
 * Serves the application in a browser instead of as a standalone Electron app
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for APIs
 * - Separation of Concerns: Maintaining clear boundaries between components
 * - Fail Fast and Learn: Using fallback mechanisms and detailed error reporting
 */
const express = require('express');
const path = require('path');
const fs = require('fs');
const { parse } = require('csv-parse/sync');
const axios = require('axios');
const http = require('http');
const socketIo = require('socket.io');
const ScanIDWatcher = require('./services/ScanIDWatcher');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../wix.config.json');
let WIX_CONFIG = {};

if (fs.existsSync(CONFIG_PATH)) {
  try {
    WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
    console.log('Loaded Wix config for web server');
  } catch (e) {
    console.error('Error loading Wix config:', e.message);
  }
}

const app = express();
const PORT = process.env.PORT || 3000;

// Create HTTP server and Socket.io instance
const server = http.createServer(app);

// Increase the maximum number of listeners to prevent memory leak warnings
// Following the Ethereal Engineering Technical Codex principle of Fail Fast and Learn
server.setMaxListeners(20);

const io = socketIo(server);

// Initialize the ScanID watcher
const scanIDPath = path.join(__dirname, 'assets/scan-id-export.csv');
const scanIDWatcher = new ScanIDWatcher(scanIDPath);

// Add request logging
app.use((req, res, next) => {
  console.log(`${new Date().toISOString()} - ${req.method} ${req.url}`);
  next();
});

// Serve static files from the renderer directory with proper MIME types
app.use(express.static(path.join(__dirname, 'renderer'), {
  setHeaders: (res, path) => {
    // Ensure JavaScript files are served with the correct MIME type
    if (path.endsWith('.js')) {
      res.setHeader('Content-Type', 'application/javascript');
    }
  }
}));
app.use(express.json());

// Check if renderer directory exists
const rendererPath = path.join(__dirname, 'renderer');
if (!fs.existsSync(rendererPath)) {
  console.error(`ERROR: Renderer directory not found at ${rendererPath}`);
} else {
  console.log(`Renderer directory found at ${rendererPath}`);
  console.log('Files in renderer directory:', fs.readdirSync(rendererPath));
}

// API endpoint to get the latest scan from Scan-ID CSV
app.get('/api/scanid/latest', async (req, res) => {
  console.log('[API] /api/scanid/latest called');
  try {
    const csvPath = path.join(__dirname, 'assets/scan-id-export.csv');
    console.log('Looking for Scan-ID CSV at:', csvPath);
    
    if (!fs.existsSync(csvPath)) {
      return res.status(404).json({ error: 'Scan-ID CSV file not found' });
    }
    
    const csvContent = fs.readFileSync(csvPath, 'utf8');
    console.log('CSV content loaded, parsing...');
    
    const records = parse(csvContent, {
      columns: true,
      skip_empty_lines: true
    });
    
    if (!records || records.length === 0) {
      return res.status(404).json({ error: 'No scan records found in CSV' });
    }
    
    // Sort by scan time (most recent first) and return the latest
    records.sort((a, b) => {
      const dateA = new Date(a.CREATED ? a.CREATED.split(' ')[0].replace(/\//g, '-') : 0);
      const dateB = new Date(b.CREATED ? b.CREATED.split(' ')[0].replace(/\//g, '-') : 0);
      return dateB - dateA;
    });
    
    // Get the latest record
    const latestRecord = records[0];
    
    // Map the Scan-ID CSV fields to our expected format
    // This follows the same mapping as in ScanIDService.js
    const mappedRecord = {
      FirstName: latestRecord['FIRST NAME'],
      LastName: latestRecord['LAST NAME'],
      FullName: latestRecord['FULL NAME'],
      DateOfBirth: latestRecord['BIRTHDATE'],
      Age: latestRecord['AGE'],
      IDNumber: latestRecord['DRV LC NO'],
      IDExpiration: latestRecord['EXPIRES ON'],
      IDIssued: latestRecord['ISSUED ON'],
      ScanTime: latestRecord['CREATED'],
      PhotoPath: latestRecord['Image1']
    };
    
    console.log(`Found ${records.length} records, returning the latest one:`, mappedRecord);
    res.json(mappedRecord);
  } catch (err) {
    console.error('Error getting latest scan:', err);
    console.error('Error in /api/scanid/latest:', err);
    res.status(500).json({ error: 'Failed to get latest scan', details: err.message });
  }
});

// API endpoint to search for a Wix member
app.post('/api/wix/search-member', async (req, res) => {
  console.log('[API] /api/wix/search-member called');
  console.log('Request body:', req.body);
  if (req.body && req.body.dob) {
    console.log('DOB received:', req.body.dob);
  }
  try {
    const { name, dob } = req.body;
    console.log(`Searching for member with name: ${name || 'N/A'} or DOB: ${dob || 'N/A'}`);
    
    // Try Members API first
    const membersUrl = `https://www.wixapis.com/members/v1/members/query`;
    const headers = {
      'Authorization': WIX_CONFIG.apiKey,
      'wix-site-id': WIX_CONFIG.siteId,
      'Content-Type': 'application/json'
    };
    
    // Build filter based on provided parameters
    const filters = [];
    if (name) {
      // Split the name into parts and search for each part individually
      const nameParts = name.split(' ').filter(part => part.trim().length > 0);
      
      // Create a more flexible search by looking for any name part in any name field
      nameParts.forEach(part => {
        // Try to match the part against first name
        filters.push({
          filter: { fieldName: "profile.firstName", operator: "contains", value: part }
        });
        
        // Try to match the part against last name
        filters.push({
          filter: { fieldName: "profile.lastName", operator: "contains", value: part }
        });
        
        // Try to match against the full name field if it exists
        filters.push({
          filter: { fieldName: "profile.name", operator: "contains", value: part }
        });
        
        // Try to match against any custom name fields that might exist
        filters.push({
          filter: { fieldName: "extendedFields.fullName", operator: "contains", value: part }
        });
      });
      
      // Also try the full name as one search term
      filters.push({
        filter: { fieldName: "profile.name", operator: "contains", value: name }
      });
    }
    
    if (dob) {
      // Format DOB if needed (assuming input is MM-DD-YYYY and API needs YYYY-MM-DD)
      let formattedDob = dob;
      if (dob.includes('-')) {
        const parts = dob.split('-');
        if (parts.length === 3 && parts[2].length === 4) {
          // Convert from MM-DD-YYYY to YYYY-MM-DD
          formattedDob = `${parts[2]}-${parts[0]}-${parts[1]}`;
        }
      }
      
      // Try both standard and extended fields for DOB
      filters.push({
        filter: { fieldName: "extendedFields.dob", operator: "eq", value: formattedDob }
      });
      filters.push({
        filter: { fieldName: "extendedFields.birthdate", operator: "eq", value: formattedDob }
      });
      filters.push({
        filter: { fieldName: "extendedFields.dateOfBirth", operator: "eq", value: formattedDob }
      });
    }
    
    // If no search criteria provided, return empty results
    if (filters.length === 0) {
      return res.json({
        success: true,
        source: 'none',
        results: []
      });
    }
    
    // Properly structure the query according to Wix API requirements
    const data = {
      // The query object needs to be directly in the request body
      filter: {
        or: filters
      },
      paging: { limit: 10 },
      fields: [
        "profile",
        "privacyStatus",
        "status",
        "activityStatus",
        "extendedFields",
        "membershipStatus"
      ]
    };
    
    // Make the request
    let membersResponse;
    try {
      membersResponse = await axios.post(membersUrl, data, { headers });
      console.log('Members API search successful');
      
      // Log the first member to debug the structure
      if (membersResponse.data.members && membersResponse.data.members.length > 0) {
        console.log('Found member structure:', JSON.stringify(membersResponse.data.members[0], null, 2));
        
        // Ensure each member has an _id property
        const members = membersResponse.data.members.map(member => {
          // If the member doesn't have an _id property but has an id property, use that
          if (!member._id && member.id) {
            member._id = member.id;
          }
          return member;
        });
        
        return res.json({
          success: true,
          source: 'members',
          results: members
        });
      }
    } catch (error) {
      console.warn('Members API search failed:', error.message);
    }
    
    // Fallback to Contacts API if no members found
    console.log('No members found, trying Contacts API');
    const contactsUrl = `https://www.wixapis.com/contacts/v4/contacts/query`;
    
    // Rebuild filters for contacts API
    const contactFilters = [];
    if (name) {
      // Split the name into parts and search for each part individually
      const nameParts = name.split(' ').filter(part => part.trim().length > 0);
      
      // Create a more flexible search by looking for any name part in any name field
      nameParts.forEach(part => {
        // Try to match the part against first name
        contactFilters.push({
          filter: { fieldName: "info.name.first", operator: "contains", value: part }
        });
        
        // Try to match the part against last name
        contactFilters.push({
          filter: { fieldName: "info.name.last", operator: "contains", value: part }
        });
        
        // Try to match against the display name if it exists
        contactFilters.push({
          filter: { fieldName: "info.name.full", operator: "contains", value: part }
        });
        
        // Try to match against any custom name fields that might exist
        contactFilters.push({
          filter: { fieldName: "customFields.fullName", operator: "contains", value: part }
        });
      });
      
      // Also try the full name as one search term
      contactFilters.push({
        filter: { fieldName: "info.name.full", operator: "contains", value: name }
      });
    }
    
    if (dob) {
      // Format DOB if needed
      let formattedDob = dob;
      if (dob.includes('-')) {
        const parts = dob.split('-');
        if (parts.length === 3 && parts[2].length === 4) {
          // Convert from MM-DD-YYYY to YYYY-MM-DD
          formattedDob = `${parts[2]}-${parts[0]}-${parts[1]}`;
        }
      }
      
      contactFilters.push({
        filter: { fieldName: "extendedFields.dob", operator: "eq", value: formattedDob }
      });
      contactFilters.push({
        filter: { fieldName: "extendedFields.birthdate", operator: "eq", value: formattedDob }
      });
      contactFilters.push({
        filter: { fieldName: "extendedFields.dateOfBirth", operator: "eq", value: formattedDob }
      });
    }
    
    // Properly structure the contacts query according to Wix API requirements
    const contactsData = {
      // The query object needs to be directly in the request body
      filter: {
        or: contactFilters
      },
      paging: { limit: 10 },
      fields: [
        "info",
        "customFields",
        "extendedFields"
      ]
    };
    
    let contactsResponse;
    try {
      contactsResponse = await axios.post(contactsUrl, contactsData, { headers });
      console.log('Contacts API search successful');
      return res.json({
        success: true,
        source: 'contacts',
        results: contactsResponse.data.contacts || []
      });
    } catch (error) {
      console.error('Contacts API search failed:', error.message);
      return res.json({
        success: false,
        error: `Error searching for contacts: ${error.message}`
      });
    }
  } catch (err) {
    console.error('Wix API Search Error:', err.message);
    res.status(500).json({
      success: false,
      error: `Error searching for member: ${err.message}`
    });
  }
});

// API endpoint to find a Wix member by firstName, lastName, and dateOfBirth
app.post('/api/wix/find-member', async (req, res) => {
  console.log('[API] /api/wix/find-member called');
  console.log('Request body:', req.body);
  
  // Validate that we have at least some search criteria
  const { firstName, lastName, dateOfBirth } = req.body;
  
  if ((!firstName || firstName.trim() === '') && 
      (!lastName || lastName.trim() === '') && 
      (!dateOfBirth || dateOfBirth.trim() === '')) {
    console.error('Missing search parameters');
    return res.status(400).json({
      success: false,
      error: 'At least one search parameter (firstName, lastName, or dateOfBirth) is required'
    });
  }
  
  try {
    console.log(`Looking up member: ${firstName} ${lastName}, DOB: ${dateOfBirth}`);
    
    // Use the WixService to find the member
    const WixService = require('./services/WixService');
    const result = await WixService.findMember({ firstName, lastName, dateOfBirth });
    
    res.json(result);
  } catch (err) {
    console.error('Wix API Find Member Error:', err.message);
    res.status(500).json({
      success: false,
      error: `Error finding member: ${err.message}`
    });
  }
});

// API endpoint to get pricing plans for a member
app.post('/api/wix/pricing-plans', async (req, res) => {
  console.log('[API] /api/wix/pricing-plans called');
  console.log('Request body:', req.body);
  
  // Validate memberId
  const memberId = req.body?.memberId;
  if (!memberId) {
    console.error('Missing or invalid memberId in request');
    return res.status(400).json({
      success: false,
      error: 'Member ID is required'
    });
  }
  
  console.log('memberId received:', memberId);
  try {
    // Use the WixService to get pricing plans
    const WixService = require('./services/WixService');
    const result = await WixService.getMemberPricingPlans(memberId);
    
    // Return the result directly from the WixService
    res.json(result);
  } catch (err) {
    console.error('Wix API Pricing Plans Error:', err.message);
    
    // Extract error details from Axios error
    const errorDetails = err.response ? {
      status: err.response.status,
      statusText: err.response.statusText,
      data: err.response.data
    } : { message: err.message };
    
    res.status(500).json({
      success: false,
      error: `Error getting pricing plans: ${err.message}`,
      details: errorDetails
    });
  }
});

// API endpoint for Wix API Explorer available endpoints
app.get('/api/wix-explorer/endpoints', async (req, res) => {
  console.log('[API] /api/wix-explorer/endpoints called');
  try {
    // Load the WixApiExplorer module
    const WixApiExplorer = require('./services/WixApiExplorer');
    const endpoints = WixApiExplorer.getAvailableEndpoints();
    res.json(endpoints);
  } catch (err) {
    console.error('Error getting Wix API Explorer endpoints:', err.message);
    res.status(500).json({
      success: false,
      error: `Error getting endpoints: ${err.message}`
    });
  }
});

// API endpoint for testing Wix API calls
app.post('/api/wix-explorer/test-api', async (req, res) => {
  console.log('[API] /api/wix-explorer/test-api called');
  console.log('Request body:', req.body);
  try {
    const { endpointId, searchParams } = req.body;
    // Load the WixApiExplorer module
    const WixApiExplorer = require('./services/WixApiExplorer');
    const result = await WixApiExplorer.testApiCall(endpointId, searchParams);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix API:', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing API: ${err.message}`
    });
  }
});

// Endpoint to serve ID photos from the new location
app.get('/api/photos/:filename', (req, res) => {
  console.log('[API] Photo request for:', req.params.filename);
  
  // Security check - allow jpg and jpeg files
  const filename = req.params.filename.toLowerCase();
  if (!filename.endsWith('.jpg') && !filename.endsWith('.jpeg') && !filename.endsWith('.jpeg.jpeg')) {
    return res.status(400).send('Invalid file type');
  }
  
  // Build the path to the photo in the new location
  let photoPath = path.join(__dirname, 'assets/scan-id-export-scan-demo', req.params.filename);
  
  // If the filename doesn't already have .jpeg.jpeg extension and we're looking for a jpg file
  if (!req.params.filename.includes('.jpeg.jpeg') && req.params.filename.toLowerCase().endsWith('.jpg')) {
    // Try with .jpeg.jpeg extension instead
    photoPath = path.join(__dirname, 'assets/scan-id-export-scan-demo', 
      req.params.filename.replace(/\.jpg$/i, '.jpeg.jpeg'));
  }
  
  console.log('Looking for photo at:', photoPath);
  
  // Check if file exists
  if (!fs.existsSync(photoPath)) {
    console.error('Photo not found:', photoPath);
    return res.status(404).send('Photo not found');
  }
  
  // Send the file
  res.sendFile(photoPath);
});

// API endpoint for testing Wix SDK
app.post('/api/wix-sdk/test', async (req, res) => {
  console.log('[API] /api/wix-sdk/test called');
  console.log('Request body:', req.body);
  try {
    const { collectionId } = req.body;
    // Load the WixSdkTest module
    const WixSdkTest = require('./services/WixSdkTest');
    const result = await WixSdkTest.testSdk(collectionId);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix SDK:', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing SDK: ${err.message}`
    });
  }
});

// API endpoint for testing Wix SDK (simple version)
app.post('/api/wix-sdk/test-simple', async (req, res) => {
  console.log('[API] /api/wix-sdk/test-simple called');
  console.log('Request body:', req.body);
  try {
    const { collectionId } = req.body;
    // Load the WixSdkTestSimple module
    const WixSdkTestSimple = require('./services/WixSdkTestSimple');
    const result = await WixSdkTestSimple.testSdkSimple(collectionId);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix SDK (simple):', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing SDK: ${err.message}`
    });
  }
});

// API endpoint for inspecting Wix SDK
app.get('/api/wix-sdk/inspect', async (req, res) => {
  console.log('[API] /api/wix-sdk/inspect called');
  try {
    // Load the WixSdkInspector module
    const WixSdkInspector = require('./services/WixSdkInspector');
    const result = await WixSdkInspector.inspectSdk();
    res.json(result);
  } catch (err) {
    console.error('Error inspecting Wix SDK:', err.message);
    res.status(500).json({
      success: false,
      error: `Error inspecting SDK: ${err.message}`
    });
  }
});

// API endpoint for testing Wix SDK Adapter
app.post('/api/wix-sdk/adapter-test', async (req, res) => {
  console.log('[API] /api/wix-sdk/adapter-test called');
  console.log('Request body:', req.body);
  try {
    const { collectionId } = req.body;
    // Load the WixSdkAdapter module
    const WixSdkAdapter = require('./services/WixSdkAdapter');
    const result = await WixSdkAdapter.testAdapter(collectionId);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix SDK Adapter:', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing SDK Adapter: ${err.message}`
    });
  }
});

// API endpoint for testing Wix Direct API
app.post('/api/wix-direct/test', async (req, res) => {
  console.log('[API] /api/wix-direct/test called');
  console.log('Request body:', req.body);
  try {
    const { endpoint } = req.body;
    // Load the WixSdkAdapter module
    const WixSdkAdapter = require('./services/WixSdkAdapter');
    const result = await WixSdkAdapter.testAdapter(endpoint);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix Direct API:', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing Direct API: ${err.message}`
    });
  }
});

// API endpoint for testing Wix SDK Compatibility Adapter
app.post('/api/wix-sdk/compat-test', async (req, res) => {
  console.log('[API] /api/wix-sdk/compat-test called');
  console.log('Request body:', req.body);
  try {
    const { collectionId } = req.body;
    // Load the WixSdkCompatAdapter module
    const WixSdkCompatAdapter = require('./services/WixSdkCompatAdapter');
    const result = await WixSdkCompatAdapter.testCompatAdapter(collectionId);
    res.json(result);
  } catch (err) {
    console.error('Error testing Wix SDK Compatibility Adapter:', err.message);
    res.status(500).json({
      success: false,
      error: `Error testing SDK Compatibility Adapter: ${err.message}`
    });
  }
});

// API endpoint to list pricing plan orders
app.post('/api/wix/list-orders', async (req, res) => {
  console.log('[API] /api/wix/list-orders called');
  console.log('Request body:', req.body);
  
  try {
    // Use the WixService to list pricing plan orders
    const WixService = require('./services/WixService');
    
    // Extract filter, sort, and paging options from the request
    const options = {
      filter: req.body.filter || {},
      sort: req.body.sort || { fieldName: 'createdDate', direction: 'DESC' },
      paging: req.body.paging || { limit: 50, offset: 0 }
    };
    
    console.log('Listing orders with options:', JSON.stringify(options, null, 2));
    const result = await WixService.listOrders(options);
    
    // Return the result directly from the WixService
    res.json(result);
  } catch (err) {
    console.error('Wix API List Orders Error:', err.message);
    
    // Extract error details from Axios error
    const errorDetails = err.response ? {
      status: err.response.status,
      statusText: err.response.statusText,
      data: err.response.data
    } : { message: err.message };
    
    res.status(500).json({
      success: false,
      error: `Error listing orders: ${err.message}`
    });
  }
});

// Endpoint to serve ID photos
app.get('/api/photos/:filename', (req, res) => {
  console.log('[API] Photo request for:', req.params.filename);
  
  // Security check - only allow jpg files
  if (!req.params.filename.toLowerCase().endsWith('.jpg')) {
    return res.status(400).send('Invalid file type');
  }
  
  // Build the path to the photo
  const scanIDFolder = path.dirname(scanIDPath);
  const photoPath = path.join(scanIDFolder, req.params.filename);
  
  console.log('Looking for photo at:', photoPath);
  
  // Check if file exists
  if (!fs.existsSync(photoPath)) {
    console.error('Photo not found:', photoPath);
    return res.status(404).send('Photo not found');
  }
  
  // Send the file
  res.sendFile(photoPath);
});

// Serve the main HTML file for all other routes
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'renderer', 'index.html'));
});

// Socket.io connection handling
io.on('connection', (socket) => {
  console.log('New client connected');
  
  // Send initial status
  socket.emit('scanWatchStatus', { watching: scanIDWatcher.watching });
  // Send initial status
  socket.emit('scanWatchStatus', { watching: scanIDWatcher.watching });
  
  // Handle start watching request
  socket.on('startWatching', () => {
    console.log('Client requested to start watching Scan-ID');
    if (!scanIDWatcher.watching) {
      scanIDWatcher.startWatching();
    }
  });
  
  // Handle stop watching request
  socket.on('stopWatching', () => {
    console.log('Client requested to stop watching Scan-ID');
    if (scanIDWatcher.watching) {
      scanIDWatcher.stopWatching();
    }
  });
  
  // Handle disconnect
  socket.on('disconnect', () => {
    console.log('Client disconnected');
  });
});

// Set up ScanIDWatcher event handlers
scanIDWatcher.on('watching', (data) => {
  console.log('ScanIDWatcher started watching:', data.path);
  io.emit('scanWatchStatus', { watching: true });
});

scanIDWatcher.on('stopped', () => {
  console.log('ScanIDWatcher stopped watching');
  io.emit('scanWatchStatus', { watching: false });
});

scanIDWatcher.on('newscan', (scan) => {
  console.log('New scan detected:', scan.FullName);
  io.emit('newScan', scan);
});

scanIDWatcher.on('error', (error) => {
  console.error('ScanIDWatcher error:', error.message);
  io.emit('scanWatchError', { error: error.message });
});

// Start the server with port fallback mechanism
const startServer = (port) => {
  server.listen(port)
    .on('listening', () => {
      console.log(`Web server running at http://localhost:${port}`);
      console.log('Open this URL in your browser to use the mini check-in app');
      
      // Set the actual port in case we're using a different one
      process.env.ACTUAL_PORT = port;
      
      // Open the browser automatically
      const { exec } = require('child_process');
      exec(`open http://localhost:${port}`, (error) => {
        if (error) {
          console.log('Could not open browser automatically. Please open the URL manually.');
        }
      });
    })
    .on('error', (err) => {
      if (err.code === 'EADDRINUSE') {
        console.log(`Port ${port} is already in use, trying ${port + 1}...`);
        startServer(port + 1);
      } else {
        console.error('Server error:', err);
      }
    });
  
  // Handle graceful shutdown
  process.on('SIGINT', () => {
    console.log('Shutting down server...');
    // Stop the ScanID watcher
    if (scanIDWatcher.watching) {
      scanIDWatcher.stopWatching();
    }
    server.close(() => {
      console.log('Server closed');
      process.exit(0);
    });
  });
};

// Error handling middleware - must be after all routes
app.use((err, req, res, next) => {
  console.error('Express error:', err.stack);
  res.status(500).send('Something broke! Check server logs for details.');
});

// Start the server with the initial port
try {
  startServer(PORT);
} catch (err) {
  console.error('Failed to start server:', err);
}