/**
 * WebhookService.js
 * 
 * This service implements an Express server to handle Wix webhooks within the Electron application.
 * It follows the Wix JavaScript SDK documentation for webhook handling.
 * 
 * Based on the official Wix SDK webhook processing example:
 * https://dev.wix.com/docs/sdk/api-reference/events/introduction
 */

const express = require('express');
const { EventEmitter } = require('events');
const portfinder = require('portfinder');
const ngrok = require('ngrok');
const fs = require('fs');
const path = require('path');

// Import Wix SDK for webhook processing
const { createClient, AppStrategy } = require('@wix/sdk');
const { orders } = require('@wix/pricing-plans');
const { members } = require('@wix/members');
const { contacts } = require('@wix/crm');

class WebhookService extends EventEmitter {
  constructor() {
    super();
    this.app = express();
    this.server = null;
    this.port = null;
    this.ngrokUrl = null;
    this.isRunning = false;
    this.webhookSecret = null;
    this.publicKey = null;
    this.appId = null;
    this.wixClient = null;
    this.configPath = path.join(__dirname, '..', '..', 'webhook-config.json');
    
    // Set up webhook endpoint - using express.text() middleware as required by Wix SDK
    this.app.post('/webhook', express.text(), this.handleWebhook.bind(this));
    
    // Health check endpoint
    this.app.get('/webhook/health', (req, res) => {
      res.status(200).send({
        status: 'ok',
        timestamp: new Date().toISOString(),
        service: 'Mini Check-In App Webhook Server'
      });
    });
  }

  /**
   * Initialize the webhook service
   * @param {Object} options - Options for initializing the service
   * @param {string} options.appId - Wix App ID
   * @param {string} options.publicKey - Wix Public Key
   */
  async initialize(options = {}) {
    try {
      // Load or create webhook config
      await this.loadConfig();
      
      // Update configuration with provided options
      if (options.appId) this.appId = options.appId;
      if (options.publicKey) this.publicKey = options.publicKey;
      
      // Find an available port
      this.port = await portfinder.getPortPromise({
        port: 3000,     // start at port 3000
        stopPort: 3100  // try up to port 3100
      });
      
      // Initialize Wix client if we have the necessary credentials
      let wixClientInitialized = false;
      if (this.appId && this.publicKey) {
        wixClientInitialized = this.initializeWixClient();
      }
      
      return {
        success: true,
        port: this.port,
        webhookSecret: this.webhookSecret,
        appId: this.appId,
        publicKeyConfigured: !!this.publicKey,
        wixClientInitialized: wixClientInitialized
      };
    } catch (err) {
      console.error('Failed to initialize webhook service:', err);
      return {
        success: false,
        error: err.message
      };
    }
  }

  /**
   * Start the webhook server
   * @param {Object} options - Options for starting the server
   * @param {boolean} options.useNgrok - Whether to use ngrok for tunneling
   */
  async start(options = { useNgrok: true }) {
    if (this.isRunning) {
      return {
        success: true,
        message: 'Webhook server is already running',
        url: this.ngrokUrl,
        localUrl: `http://localhost:${this.port}`,
        port: this.port
      };
    }

    try {
      // Initialize if not already done
      if (!this.port) {
        await this.initialize();
      }

      // Start the Express server
      this.server = this.app.listen(this.port, () => {
        console.log(`Webhook server listening on port ${this.port}`);
      });

      // Set local URL
      const localUrl = `http://localhost:${this.port}`;
      
      // Start ngrok tunnel if requested
      if (options.useNgrok) {
        try {
          // Try to start ngrok with a random subdomain
          this.ngrokUrl = await ngrok.connect({
            addr: this.port
          });
          console.log(`Webhook server accessible at: ${this.ngrokUrl}/webhook`);
        } catch (ngrokErr) {
          console.warn('Failed to start ngrok tunnel:', ngrokErr.message);
          console.log('Continuing with local-only server');
          // Continue without ngrok - local server only
        }
      }

      this.isRunning = true;
      
      // Save the configuration
      await this.saveConfig();

      // Determine the webhook endpoint URL
      const webhookEndpoint = this.ngrokUrl 
        ? `${this.ngrokUrl}/webhook` 
        : `${localUrl}/webhook`;

      return {
        success: true,
        message: this.ngrokUrl 
          ? 'Webhook server started successfully with public URL' 
          : 'Webhook server started in local-only mode',
        url: this.ngrokUrl,
        localUrl: localUrl,
        port: this.port,
        webhookEndpoint: webhookEndpoint,
        isLocalOnly: !this.ngrokUrl
      };
    } catch (err) {
      console.error('Failed to start webhook server:', err);
      
      // Try to clean up if server was partially started
      if (this.server) {
        try {
          this.server.close();
          this.server = null;
        } catch (closeErr) {
          console.error('Error closing server after failed start:', closeErr);
        }
      }
      
      return {
        success: false,
        error: err.message
      };
    }
  }

  /**
   * Stop the webhook server
   */
  async stop() {
    if (!this.isRunning) {
      return {
        success: true,
        message: 'Webhook server is not running'
      };
    }

    try {
      // Disconnect ngrok
      if (this.ngrokUrl) {
        await ngrok.disconnect();
        this.ngrokUrl = null;
      }

      // Close the server
      if (this.server) {
        await new Promise((resolve, reject) => {
          this.server.close((err) => {
            if (err) reject(err);
            else resolve();
          });
        });
        this.server = null;
      }

      this.isRunning = false;
      console.log('Webhook server stopped');

      return {
        success: true,
        message: 'Webhook server stopped successfully'
      };
    } catch (err) {
      console.error('Failed to stop webhook server:', err);
      return {
        success: false,
        error: err.message
      };
    }
  }

  /**
   * Get the current status of the webhook server
   */
  getStatus() {
    // Determine webhook endpoint URL based on available URLs
    let webhookEndpoint = null;
    if (this.isRunning) {
      if (this.ngrokUrl) {
        webhookEndpoint = `${this.ngrokUrl}/webhook`;
      } else if (this.port) {
        webhookEndpoint = `http://localhost:${this.port}/webhook`;
      }
    }
    
    return {
      isRunning: this.isRunning,
      port: this.port,
      url: this.ngrokUrl,
      localUrl: this.port ? `http://localhost:${this.port}` : null,
      webhookEndpoint: webhookEndpoint,
      isLocalOnly: this.isRunning && !this.ngrokUrl,
      // Wix SDK configuration status
      appId: this.appId,
      publicKeyConfigured: !!this.publicKey,
      wixClientInitialized: !!this.wixClient
    };
  }

  /**
   * Generate a new webhook secret
   */
  generateWebhookSecret() {
    const crypto = require('crypto');
    this.webhookSecret = crypto.randomBytes(32).toString('hex');
    return this.webhookSecret;
  }
  
  /**
   * Set the Wix App ID
   * @param {string} appId - The Wix App ID
   */
  setAppId(appId) {
    if (!appId) {
      return {
        success: false,
        error: 'App ID is required'
      };
    }
    
    this.appId = appId;
    this.saveConfig();
    
    // Re-initialize the Wix client if we have both app ID and public key
    if (this.appId && this.publicKey) {
      this.initializeWixClient();
    }
    
    return {
      success: true,
      appId: this.appId
    };
  }
  
  /**
   * Set the Wix Public Key
   * @param {string} publicKey - The Wix Public Key
   */
  setPublicKey(publicKey) {
    if (!publicKey) {
      return {
        success: false,
        error: 'Public Key is required'
      };
    }
    
    this.publicKey = publicKey;
    this.saveConfig();
    
    // Re-initialize the Wix client if we have both app ID and public key
    if (this.appId && this.publicKey) {
      this.initializeWixClient();
    }
    
    return {
      success: true,
      publicKeyConfigured: true
    };
  }

  /**
   * Handle incoming webhook requests using the Wix SDK
   * This follows the official Wix SDK webhook processing example
   */
  async handleWebhook(req, res) {
    try {
      console.log('Received webhook request');
      console.log('Content-Type:', req.headers['content-type']);
      
      // Initialize Wix client if not already done
      if (!this.wixClient && this.appId && this.publicKey) {
        this.initializeWixClient();
      }

      // Get the raw body content for debugging
      const rawBody = req.body;
      console.log('Raw body type:', typeof rawBody);
      console.log('Raw body preview:', typeof rawBody === 'string' ? rawBody.substring(0, 100) : 'Not a string');

      if (this.wixClient) {
        // Use Wix SDK to process the webhook
        console.log('Processing webhook with Wix SDK');
        try {
          await this.wixClient.webhooks.process(rawBody);
          console.log('Webhook processed successfully with Wix SDK');
        } catch (sdkError) {
          console.error('Error processing webhook with Wix SDK:', sdkError);
          // Fall back to basic processing if SDK fails
          await this.processWebhookBasic(rawBody, req.headers);
        }
      } else {
        console.warn('Wix client not initialized. Using basic webhook processing.');
        // Fall back to basic webhook processing if Wix client is not initialized
        await this.processWebhookBasic(rawBody, req.headers);
      }

      // Acknowledge receipt of the webhook
      res.status(200).send('Webhook received');
    } catch (err) {
      console.error('Error handling webhook:', err);
      res.status(500).send(`Webhook error: ${err instanceof Error ? err.message : err}`);
    }
  }
  
  /**
   * Process webhook using basic method without the Wix SDK
   * @param {string|object} body - The webhook request body
   * @param {object} headers - The request headers
   */
  async processWebhookBasic(body, headers) {
    try {
      let payload;
      let eventType = 'unknown';
      
      // Check if this looks like a JWT token (starts with eyJ)
      if (typeof body === 'string' && body.startsWith('eyJ')) {
        console.log('Detected JWT token format, attempting to decode...');
        try {
          // Try to decode the JWT token (without verification)
          const jwtParts = body.split('.');
          if (jwtParts.length >= 2) {
            // Decode the JWT header and payload
            const header = JSON.parse(Buffer.from(jwtParts[0], 'base64').toString());
            const jwtPayload = JSON.parse(Buffer.from(jwtParts[1], 'base64').toString());
            
            console.log('JWT Header:', header);
            console.log('JWT Payload preview:', JSON.stringify(jwtPayload).substring(0, 200) + '...');
            
            // Extract data from JWT payload
            if (jwtPayload.data) {
              try {
                // The data might be a JSON string that needs to be parsed
                const dataObj = typeof jwtPayload.data === 'string' 
                  ? JSON.parse(jwtPayload.data) 
                  : jwtPayload.data;
                
                // Extract event type from the data
                if (dataObj.eventType) {
                  eventType = dataObj.eventType;
                  console.log(`Extracted event type from JWT: ${eventType}`);
                }
                
                // Create a structured payload
                payload = {
                  jwt: {
                    header,
                    payload: jwtPayload
                  },
                  data: dataObj,
                  rawJwt: body
                };
              } catch (dataParseError) {
                console.warn('Failed to parse JWT data field:', dataParseError.message);
                payload = {
                  jwt: {
                    header,
                    payload: jwtPayload
                  },
                  rawJwt: body
                };
              }
            } else {
              payload = {
                jwt: {
                  header,
                  payload: jwtPayload
                },
                rawJwt: body
              };
            }
          } else {
            console.warn('JWT token does not have the expected format');
            payload = { rawContent: body };
          }
        } catch (jwtError) {
          console.warn('Failed to decode JWT token:', jwtError.message);
          payload = { rawContent: body };
        }
      } else {
        // Try to parse the body if it's a string and not a JWT
        if (typeof body === 'string') {
          try {
            payload = JSON.parse(body);
            // Try to determine the event type from JSON
            eventType = payload.eventType || payload.type || eventType;
          } catch (parseError) {
            console.warn('Failed to parse webhook body as JSON:', parseError.message);
            // If it's not valid JSON, use the raw body
            payload = { rawContent: body };
          }
        } else {
          // If it's already an object, use it directly
          payload = body;
          // Try to determine the event type from the object
          eventType = payload.eventType || payload.type || eventType;
        }
      }
      
      console.log(`Processed webhook event: ${eventType}`);
      
      // Log a preview of the payload
      const payloadPreview = JSON.stringify(payload).substring(0, 500) + 
        (JSON.stringify(payload).length > 500 ? '...' : '');
      console.log('Webhook payload preview:', payloadPreview);
      
      // Emit an event for the application to handle
      this.emit('webhook', {
        eventType,
        payload: payload,
        headers: headers
      });
      
      return true;
    } catch (err) {
      console.error('Error in basic webhook processing:', err);
      return false;
    }
  }

  /**
   * Initialize the Wix client with the app ID and public key
   */
  initializeWixClient() {
    try {
      if (!this.appId || !this.publicKey) {
        console.warn('Cannot initialize Wix client: missing appId or publicKey');
        return false;
      }

      // Create the Wix client using the AppStrategy
      this.wixClient = createClient({
        auth: AppStrategy({
          appId: this.appId,
          publicKey: this.publicKey,
        }),
        modules: { orders, members, contacts }
      });

      // Set up event handlers for different webhook types
      this.setupWebhookEventHandlers();

      console.log('Wix client initialized successfully');
      return true;
    } catch (err) {
      console.error('Error initializing Wix client:', err);
      return false;
    }
  }

  /**
   * Set up event handlers for different webhook types
   */
  setupWebhookEventHandlers() {
    if (!this.wixClient) return;

    // Pricing Plans - Orders events
    this.wixClient.orders.onOrderPurchased((event) => {
      console.log('Order purchased event received:', event);
      this.emit('webhook', {
        eventType: 'order/purchased',
        payload: event
      });
    });

    // Members events
    this.wixClient.members.onMemberCreated((event) => {
      console.log('Member created event received:', event);
      this.emit('webhook', {
        eventType: 'member/created',
        payload: event
      });
    });

    this.wixClient.members.onMemberUpdated((event) => {
      console.log('Member updated event received:', event);
      this.emit('webhook', {
        eventType: 'member/updated',
        payload: event
      });
    });

    // Contacts events (if available)
    if (this.wixClient.contacts && typeof this.wixClient.contacts.onContactCreated === 'function') {
      this.wixClient.contacts.onContactCreated((event) => {
        console.log('Contact created event received:', event);
        this.emit('webhook', {
          eventType: 'contact/created',
          payload: event
        });
      });

      this.wixClient.contacts.onContactUpdated((event) => {
        console.log('Contact updated event received:', event);
        this.emit('webhook', {
          eventType: 'contact/updated',
          payload: event
        });
      });
    }
  }

  /**
   * Load webhook configuration from file
   */
  async loadConfig() {
    try {
      if (fs.existsSync(this.configPath)) {
        const configData = fs.readFileSync(this.configPath, 'utf8');
        const config = JSON.parse(configData);
        
        // Load webhook configuration
        this.webhookSecret = config.webhookSecret;
        
        // Load Wix SDK configuration if available
        if (config.appId) this.appId = config.appId;
        if (config.publicKey) this.publicKey = config.publicKey;
        
        console.log('Loaded webhook configuration');
      } else {
        // Generate a new webhook secret if none exists
        this.generateWebhookSecret();
        await this.saveConfig();
        console.log('Created new webhook configuration');
      }
    } catch (err) {
      console.error('Error loading webhook config:', err);
      // Generate a new webhook secret if there's an error
      this.generateWebhookSecret();
    }
  }

  /**
   * Save webhook configuration to file
   */
  async saveConfig() {
    try {
      const config = {
        webhookSecret: this.webhookSecret,
        appId: this.appId,
        publicKey: this.publicKey,
        lastUpdated: new Date().toISOString()
      };
      
      fs.writeFileSync(this.configPath, JSON.stringify(config, null, 2), 'utf8');
      console.log('Saved webhook configuration');
    } catch (err) {
      console.error('Error saving webhook config:', err);
    }
  }
}

// Create a singleton instance
const webhookService = new WebhookService();

module.exports = webhookService;
