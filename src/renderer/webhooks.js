/**
 * webhooks.js
 * 
 * This file handles the UI interactions for the webhook management page.
 * It follows the Wix JavaScript SDK documentation for webhook handling.
 */

document.addEventListener('DOMContentLoaded', () => {
  // UI Elements
  const serverStatus = document.getElementById('server-status');
  const serverPort = document.getElementById('server-port');
  const serverUrl = document.getElementById('server-url');
  const initializeButton = document.getElementById('initialize-server');
  const startButton = document.getElementById('start-server');
  const stopButton = document.getElementById('stop-server');
  const webhookSecret = document.getElementById('webhook-secret');
  const generateSecretButton = document.getElementById('generate-secret');
  const copySecretButton = document.getElementById('copy-secret');
  const webhookEndpoint = document.getElementById('webhook-endpoint');
  const copyEndpointButton = document.getElementById('copy-endpoint');
  const eventsList = document.getElementById('events-list');
  const clearEventsButton = document.getElementById('clear-events');
  
  // Wix SDK Configuration Elements
  const wixAppIdInput = document.getElementById('wix-app-id');
  const saveAppIdButton = document.getElementById('save-app-id');
  const wixPublicKeyInput = document.getElementById('wix-public-key');
  const savePublicKeyButton = document.getElementById('save-public-key');
  const sdkStatusText = document.getElementById('sdk-status-text');

  // Track webhook events
  const webhookEvents = [];
  const maxEvents = 10;

  // Initialize the page
  initPage();

  // Event Listeners
  initializeButton.addEventListener('click', initializeServer);
  startButton.addEventListener('click', startServer);
  stopButton.addEventListener('click', stopServer);
  generateSecretButton.addEventListener('click', generateSecret);
  copySecretButton.addEventListener('click', () => copyToClipboard(webhookSecret));
  copyEndpointButton.addEventListener('click', () => copyToClipboard(webhookEndpoint));
  clearEventsButton.addEventListener('click', clearEvents);
  
  // Wix SDK Configuration Event Listeners
  saveAppIdButton.addEventListener('click', saveWixAppId);
  savePublicKeyButton.addEventListener('click', saveWixPublicKey);

  // Set up webhook event listener
  const removeWebhookListener = window.webhookAPI.onWebhookEvent((data) => {
    console.log('Received webhook event:', data);
    addWebhookEvent(data);
  });

  /**
   * Initialize the page
   */
  async function initPage() {
    try {
      // Get current webhook server status
      const status = await window.webhookAPI.getStatus();
      updateStatusDisplay(status);
      
      // Update SDK status display
      updateSdkStatus(status);
    } catch (error) {
      console.error('Error initializing page:', error);
      showError('Failed to initialize page: ' + error.message);
    }
  }
  
  /**
   * Update SDK status display
   */
  function updateSdkStatus(status) {
    if (status.wixClientInitialized) {
      sdkStatusText.textContent = 'Configured and initialized';
      sdkStatusText.className = 'status-running';
    } else if (status.appId && status.publicKeyConfigured) {
      sdkStatusText.textContent = 'Configured but not initialized';
      sdkStatusText.className = 'status-initialized';
    } else if (status.appId) {
      sdkStatusText.textContent = 'App ID set, missing Public Key';
      sdkStatusText.className = 'status-warning';
    } else if (status.publicKeyConfigured) {
      sdkStatusText.textContent = 'Public Key set, missing App ID';
      sdkStatusText.className = 'status-warning';
    } else {
      sdkStatusText.textContent = 'Not configured';
      sdkStatusText.className = 'status-stopped';
    }
    
    // Update input fields if values are available
    if (status.appId) {
      wixAppIdInput.value = status.appId;
    }
  }
  
  /**
   * Save Wix App ID
   */
  async function saveWixAppId() {
    try {
      const appId = wixAppIdInput.value.trim();
      if (!appId) {
        showError('App ID cannot be empty');
        return;
      }
      
      setLoading(true, 'Saving App ID...');
      const result = await window.webhookAPI.setAppId(appId);
      
      if (result.success) {
        showSuccess('App ID saved successfully');
        
        // Get updated status
        const status = await window.webhookAPI.getStatus();
        updateSdkStatus(status);
      } else {
        showError('Failed to save App ID: ' + result.error);
      }
    } catch (error) {
      console.error('Error saving App ID:', error);
      showError('Error saving App ID: ' + error.message);
    } finally {
      setLoading(false);
    }
  }
  
  /**
   * Save Wix Public Key
   */
  async function saveWixPublicKey() {
    try {
      const publicKey = wixPublicKeyInput.value.trim();
      if (!publicKey) {
        showError('Public Key cannot be empty');
        return;
      }
      
      // Validate that it looks like a public key
      if (!publicKey.includes('-----BEGIN PUBLIC KEY-----')) {
        showWarning('The Public Key does not appear to be in the correct format. It should start with "-----BEGIN PUBLIC KEY-----"');
        // Continue anyway as the user might know what they're doing
      }
      
      setLoading(true, 'Saving Public Key...');
      const result = await window.webhookAPI.setPublicKey(publicKey);
      
      if (result.success) {
        showSuccess('Public Key saved successfully');
        
        // Get updated status
        const status = await window.webhookAPI.getStatus();
        updateSdkStatus(status);
      } else {
        showError('Failed to save Public Key: ' + result.error);
      }
    } catch (error) {
      console.error('Error saving Public Key:', error);
      showError('Error saving Public Key: ' + error.message);
    } finally {
      setLoading(false);
    }
  }

  /**
   * Initialize the webhook server
   */
  async function initializeServer() {
    try {
      setLoading(true, 'Initializing server...');
      const result = await window.webhookAPI.initialize();
      
      if (result.success) {
        console.log('Server initialized:', result);
        updateStatusDisplay({
          isRunning: false,
          port: result.port
        });
        
        // Update webhook secret
        if (result.webhookSecret) {
          webhookSecret.value = result.webhookSecret;
        }
        
        // Enable start button
        startButton.disabled = false;
        
        showSuccess('Server initialized successfully');
      } else {
        console.error('Failed to initialize server:', result.error);
        showError('Failed to initialize server: ' + result.error);
      }
    } catch (error) {
      console.error('Error initializing server:', error);
      showError('Error initializing server: ' + error.message);
    } finally {
      setLoading(false);
    }
  }

  /**
   * Start the webhook server
   */
  async function startServer() {
    try {
      setLoading(true, 'Starting server...');
      
      // Add checkbox for ngrok option
      const useNgrok = document.getElementById('use-ngrok')?.checked ?? true;
      
      const result = await window.webhookAPI.start({ useNgrok });
      
      if (result.success) {
        console.log('Server started:', result);
        updateStatusDisplay({
          isRunning: true,
          port: result.port,
          url: result.url,
          localUrl: result.localUrl,
          webhookEndpoint: result.webhookEndpoint,
          isLocalOnly: result.isLocalOnly
        });
        
        // Update UI
        startButton.disabled = true;
        stopButton.disabled = false;
        
        if (result.isLocalOnly) {
          showWarning('Server started in local-only mode. External webhooks will not be able to reach this server unless you configure port forwarding or use a different tunneling service.');
        } else {
          showSuccess('Server started successfully with public URL');
        }
      } else {
        console.error('Failed to start server:', result.error);
        showError('Failed to start server: ' + result.error);
      }
    } catch (error) {
      console.error('Error starting server:', error);
      showError('Error starting server: ' + error.message);
    } finally {
      setLoading(false);
    }
  }

  /**
   * Stop the webhook server
   */
  async function stopServer() {
    try {
      setLoading(true, 'Stopping server...');
      const result = await window.webhookAPI.stop();
      
      if (result.success) {
        console.log('Server stopped:', result);
        updateStatusDisplay({
          isRunning: false,
          port: null,
          url: null,
          webhookEndpoint: null
        });
        
        // Update UI
        startButton.disabled = false;
        stopButton.disabled = true;
        
        showSuccess('Server stopped successfully');
      } else {
        console.error('Failed to stop server:', result.error);
        showError('Failed to stop server: ' + result.error);
      }
    } catch (error) {
      console.error('Error stopping server:', error);
      showError('Error stopping server: ' + error.message);
    } finally {
      setLoading(false);
    }
  }

  /**
   * Generate a new webhook secret
   */
  async function generateSecret() {
    try {
      setLoading(true, 'Generating secret...');
      const result = await window.webhookAPI.generateSecret();
      
      if (result && result.secret) {
        webhookSecret.value = result.secret;
        showSuccess('New secret generated');
      } else {
        showError('Failed to generate secret');
      }
    } catch (error) {
      console.error('Error generating secret:', error);
      showError('Error generating secret: ' + error.message);
    } finally {
      setLoading(false);
    }
  }

  /**
   * Update the status display
   */
  function updateStatusDisplay(status) {
    // Update status text and class
    if (status.isRunning) {
      if (status.isLocalOnly) {
        serverStatus.textContent = 'Running (Local Only)';
        serverStatus.className = 'status-running-local';
      } else {
        serverStatus.textContent = 'Running';
        serverStatus.className = 'status-running';
      }
    } else if (status.port) {
      serverStatus.textContent = 'Initialized';
      serverStatus.className = 'status-initialized';
    } else {
      serverStatus.textContent = 'Not initialized';
      serverStatus.className = 'status-stopped';
    }
    
    // Update port and URL
    serverPort.textContent = status.port || 'N/A';
    
    // Create URL display with both public and local URLs if available
    if (status.url) {
      serverUrl.innerHTML = `Public: <a href="${status.url}" target="_blank">${status.url}</a><br>`;
      if (status.localUrl) {
        serverUrl.innerHTML += `Local: <a href="${status.localUrl}" target="_blank">${status.localUrl}</a>`;
      }
    } else if (status.localUrl) {
      serverUrl.innerHTML = `Local only: <a href="${status.localUrl}" target="_blank">${status.localUrl}</a>`;
    } else {
      serverUrl.textContent = 'N/A';
    }
    
    // Update webhook endpoint
    webhookEndpoint.value = status.webhookEndpoint || '';
    
    // Update button states
    initializeButton.disabled = status.isRunning || (status.port !== null);
    startButton.disabled = status.isRunning || (status.port === null);
    stopButton.disabled = !status.isRunning;
  }

  /**
   * Add a webhook event to the list
   */
  function addWebhookEvent(data) {
    // Add to the beginning of the array
    webhookEvents.unshift({
      timestamp: new Date(),
      eventType: data.eventType || 'unknown',
      payload: data.payload,
      headers: data.headers || {}
    });
    
    // Limit the number of events
    if (webhookEvents.length > maxEvents) {
      webhookEvents.pop();
    }
    
    // Update the UI
    renderEvents();
  }

  /**
   * Render the events list
   */
  function renderEvents() {
    // Clear the placeholder text
    eventsList.innerHTML = '';
    
    if (webhookEvents.length === 0) {
      const placeholder = document.createElement('p');
      placeholder.className = 'placeholder-text';
      placeholder.textContent = 'No events received yet.';
      eventsList.appendChild(placeholder);
      return;
    }
    
    // Create event elements
    webhookEvents.forEach((event, index) => {
      const eventElement = document.createElement('div');
      eventElement.className = 'event-item';
      
      const header = document.createElement('div');
      header.className = 'event-header';
      
      const title = document.createElement('h3');
      title.textContent = `Event: ${event.eventType || 'Unknown'}`;
      
      const timestamp = document.createElement('span');
      timestamp.className = 'event-timestamp';
      timestamp.textContent = new Date(event.timestamp).toLocaleString();
      
      header.appendChild(title);
      header.appendChild(timestamp);
      
      // Add headers section if available
      if (event.headers) {
        const headersTitle = document.createElement('h4');
        headersTitle.textContent = 'Headers:';
        eventElement.appendChild(header);
        eventElement.appendChild(headersTitle);
        
        const headersContent = document.createElement('pre');
        headersContent.className = 'event-headers';
        
        // Only show relevant headers
        const relevantHeaders = {};
        const headerKeys = ['content-type', 'x-wix-signature', 'user-agent'];
        headerKeys.forEach(key => {
          if (event.headers[key]) {
            relevantHeaders[key] = event.headers[key];
          }
        });
        
        headersContent.textContent = JSON.stringify(relevantHeaders, null, 2);
        eventElement.appendChild(headersContent);
      }
      
      // Format payload based on content
      const payloadTitle = document.createElement('h4');
      payloadTitle.textContent = 'Payload:';
      eventElement.appendChild(payloadTitle);
      
      const content = document.createElement('pre');
      content.className = 'event-content';
      
      // Handle different payload types
      if (event.payload) {
        // Special handling for JWT tokens
        if (event.payload.jwt) {
          // Create a structured display for JWT tokens
          const jwtSection = document.createElement('div');
          jwtSection.className = 'jwt-section';
          
          // JWT Header
          const headerTitle = document.createElement('h5');
          headerTitle.textContent = 'JWT Header:';
          jwtSection.appendChild(headerTitle);
          
          const headerContent = document.createElement('pre');
          headerContent.className = 'jwt-header';
          headerContent.textContent = JSON.stringify(event.payload.jwt.header, null, 2);
          jwtSection.appendChild(headerContent);
          
          // JWT Payload
          const payloadTitle = document.createElement('h5');
          payloadTitle.textContent = 'JWT Payload:';
          jwtSection.appendChild(payloadTitle);
          
          const payloadContent = document.createElement('pre');
          payloadContent.className = 'jwt-payload';
          payloadContent.textContent = JSON.stringify(event.payload.jwt.payload, null, 2);
          jwtSection.appendChild(payloadContent);
          
          // Extracted Data (if available)
          if (event.payload.data) {
            const dataTitle = document.createElement('h5');
            dataTitle.textContent = 'Extracted Data:';
            jwtSection.appendChild(dataTitle);
            
            const dataContent = document.createElement('pre');
            dataContent.className = 'jwt-data';
            dataContent.textContent = JSON.stringify(event.payload.data, null, 2);
            jwtSection.appendChild(dataContent);
          }
          
          // Raw JWT (truncated)
          const rawTitle = document.createElement('h5');
          rawTitle.textContent = 'Raw JWT Token:';
          jwtSection.appendChild(rawTitle);
          
          const rawContent = document.createElement('pre');
          rawContent.className = 'jwt-raw';
          if (event.payload.rawJwt && event.payload.rawJwt.length > 100) {
            rawContent.textContent = event.payload.rawJwt.substring(0, 100) + '... (truncated)';
          } else if (event.payload.rawJwt) {
            rawContent.textContent = event.payload.rawJwt;
          } else {
            rawContent.textContent = 'Raw JWT not available';
          }
          jwtSection.appendChild(rawContent);
          
          // Add the JWT section to the event element
          eventElement.appendChild(jwtSection);
          
          // Don't add the regular content element
          return;
        } else if (typeof event.payload === 'string') {
          // For string payloads
          if (event.payload.length > 1000) {
            content.textContent = event.payload.substring(0, 1000) + '... (truncated)';
          } else {
            content.textContent = event.payload;
          }
        } else if (event.payload.rawContent) {
          // For non-JSON content that was captured as raw
          if (event.payload.rawContent.length > 1000) {
            content.textContent = event.payload.rawContent.substring(0, 1000) + '... (truncated)';
          } else {
            content.textContent = event.payload.rawContent;
          }
        } else {
          // For JSON objects
          try {
            content.textContent = JSON.stringify(event.payload, null, 2);
          } catch (err) {
            content.textContent = `Error formatting payload: ${err.message}`;
          }
        }
      } else {
        content.textContent = 'No payload data';
      }
      
      eventElement.appendChild(header);
      eventElement.appendChild(content);
      eventsList.appendChild(eventElement);
      
      // Add separator except for the last item
      if (index < webhookEvents.length - 1) {
        const separator = document.createElement('hr');
        eventsList.appendChild(separator);
      }
    });
  }

  /**
   * Clear all webhook events
   */
  function clearEvents() {
    webhookEvents.length = 0;
    renderEvents();
  }

  /**
   * Copy text to clipboard
   */
  function copyToClipboard(inputElement) {
    if (!inputElement.value) {
      showError('Nothing to copy');
      return;
    }
    
    inputElement.select();
    document.execCommand('copy');
    showSuccess('Copied to clipboard');
  }

  /**
   * Show a success message
   */
  function showSuccess(message) {
    // You can implement a toast notification system here
    console.log('Success:', message);
    alert('Success: ' + message);
  }

  /**
   * Show a warning message
   */
  function showWarning(message) {
    // You can implement a toast notification system here
    console.warn('Warning:', message);
    alert('Warning: ' + message);
  }

  /**
   * Show an error message
   */
  function showError(message) {
    // You can implement a toast notification system here
    console.error('Error:', message);
    alert('Error: ' + message);
  }

  /**
   * Set loading state
   */
  function setLoading(isLoading, message = '') {
    // You can implement a loading indicator here
    if (isLoading) {
      console.log('Loading:', message);
    } else {
      console.log('Loading complete');
    }
  }

  // Clean up when navigating away
  window.addEventListener('beforeunload', () => {
    removeWebhookListener();
  });
});
