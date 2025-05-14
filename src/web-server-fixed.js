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
      Gender: latestRecord['GENDER'],
      Address: latestRecord['ADDRESS'],
      City: latestRecord['CITY'],
      State: latestRecord['STATE'],
      ZipCode: latestRecord['ZIPCODE'],
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
      
      // For each name part, try first name and last name
      nameParts.forEach(part => {
        filters.push({
          filter: { fieldName: "profile.firstName", operator: "contains", value: part }
        });
        
        filters.push({
          filter: { fieldName: "profile.lastName", operator: "contains", value: part }
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
      
      // Try to find by DOB in extended fields
      filters.push({
        filter: { fieldName: "extendedFields.dateOfBirth", operator: "eq", value: formattedDob }
      });
      
      // Also try to find by DOB in custom fields
      filters.push({
        filter: { fieldName: "customFields", operator: "hasSome", value: [
          { name: "dateOfBirth", value: formattedDob }
        ]}
      });
    }
    
    // Prepare the request data
    const data = {
      query: {
        filter: {
          operator: "or",
          filters: filters
        }
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
        console.log('First member found:', membersResponse.data.members[0].profile.name);
      } else {
        console.log('No members found in Members API response');
      }
      
      return res.json({
        success: true,
        source: 'members',
        results: membersResponse.data.members || []
      });
    } catch (err) {
      console.error('Members API search failed:', err.message);
      // Continue to try Contacts API
    }
    
    // If we get here, the Members API failed or returned no results
    console.log('No members found, trying Contacts API');
    const contactsUrl = `https://www.wixapis.com/contacts/v4/contacts/query`;
    
    // Rebuild filters for contacts API
    const contactFilters = [];
    if (name) {
      // Split the name into parts and search for each part individually
      const nameParts = name.split(' ').filter(part => part.trim().length > 0);
      
      // For each name part, try first name and last name
      nameParts.forEach(part => {
        contactFilters.push({
          filter: { fieldName: "info.name.first", operator: "contains", value: part }
        });
        
        contactFilters.push({
          filter: { fieldName: "info.name.last", operator: "contains", value: part }
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
      
      // Try to find by DOB in extended fields
      contactFilters.push({
        filter: { fieldName: "info.extendedFields.dateOfBirth", operator: "eq", value: formattedDob }
      });
      
      // Also try to find by DOB in custom fields
      contactFilters.push({
        filter: { fieldName: "info.customFields", operator: "hasSome", value: [
          { name: "dateOfBirth", value: formattedDob }
        ]}
      });
    }
    
    // Prepare the request data for contacts
    const contactsData = {
      query: {
        filter: {
          operator: "or",
          filters: contactFilters
        }
      },
      paging: { limit: 10 }
    };
    
    // Make the request to Contacts API
    try {
      const contactsResponse = await axios.post(contactsUrl, contactsData, { headers });
      console.log('Contacts API search successful');
      return res.json({
        success: true,
        source: 'contacts',
        results: contactsResponse.data.contacts || []
      });
    } catch (err) {
      console.error('Contacts API search failed:', err.message);
      return res.status(500).json({
        success: false,
        error: `Failed to search for members: ${err.message}`
      });
    }
  } catch (err) {
    console.error('Error in search-member endpoint:', err);
    return res.status(500).json({
      success: false,
      error: `Error searching for members: ${err.message}`
    });
  }
});

// API endpoint to get member details by ID
app.get('/api/wix/member/:id', async (req, res) => {
  console.log('[API] /api/wix/member/:id called with ID:', req.params.id);
  try {
    const memberId = req.params.id;
    const memberUrl = `https://www.wixapis.com/members/v1/members/${memberId}`;
    const headers = {
      'Authorization': WIX_CONFIG.apiKey,
      'wix-site-id': WIX_CONFIG.siteId
    };
    
    const response = await axios.get(memberUrl, { headers });
    console.log('Member details retrieved successfully');
    
    res.json({
      success: true,
      member: response.data.member
    });
  } catch (err) {
    console.error('Error getting member details:', err.message);
    res.status(500).json({
      success: false,
      error: `Failed to get member details: ${err.message}`
    });
  }
});

// API endpoint to get pricing plans
app.get('/api/wix/pricing-plans', async (req, res) => {
  console.log('[API] /api/wix/pricing-plans called');
  try {
    const plansUrl = `https://www.wixapis.com/pricing-plans/v2/plans`;
    const headers = {
      'Authorization': WIX_CONFIG.apiKey,
      'wix-site-id': WIX_CONFIG.siteId
    };
    
    const response = await axios.get(plansUrl, { headers });
    console.log('Pricing plans retrieved successfully');
    
    res.json({
      success: true,
      plans: response.data.plans
    });
  } catch (err) {
    console.error('Error getting pricing plans:', err.message);
    res.status(500).json({
      success: false,
      error: `Failed to get pricing plans: ${err.message}`
    });
  }
});

// API endpoint to get member orders
app.get('/api/wix/member/:id/orders', async (req, res) => {
  console.log('[API] /api/wix/member/:id/orders called with ID:', req.params.id);
  try {
    const memberId = req.params.id;
    const ordersUrl = `https://www.wixapis.com/ecom/v1/orders/query`;
    const headers = {
      'Authorization': WIX_CONFIG.apiKey,
      'wix-site-id': WIX_CONFIG.siteId,
      'Content-Type': 'application/json'
    };
    
    // Query orders for this member
    const data = {
      query: {
        filter: {
          filter: { fieldName: "buyerInfo.memberId", operator: "eq", value: memberId }
        },
        sort: [{ fieldName: "dateCreated", order: "DESC" }]
      },
      paging: { limit: 10 }
    };
    
    const response = await axios.post(ordersUrl, data, { headers });
    console.log('Member orders retrieved successfully');
    
    res.json({
      success: true,
      orders: response.data.orders || []
    });
  } catch (err) {
    console.error('Error getting member orders:', err.message);
    res.status(500).json({
      success: false,
      error: `Error listing orders: ${err.message}`,
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
