/**
 * Configure the webhook server with Wix App ID and Public Key
 * This script sets up the webhook server to use the Wix JavaScript SDK for webhook processing
 */

const fs = require('fs');
const path = require('path');
const WebhookService = require('./src/services/WebhookService');

// Replace with your actual Wix App ID
const WIX_APP_ID = 'e9e875a1-18a7-44cb-a330-266dd17b46aa';

// Path to the public key file
const PUBLIC_KEY_PATH = path.join(__dirname, 'pubic.pem');

async function configureWebhookSdk() {
  try {
    console.log('Reading public key from:', PUBLIC_KEY_PATH);
    
    // Read the public key from file
    const publicKey = fs.readFileSync(PUBLIC_KEY_PATH, 'utf8');
    
    console.log('Public key loaded successfully');
    console.log('Configuring webhook service with App ID and Public Key...');
    
    // Initialize the webhook service
    await WebhookService.initialize();
    
    // Set the App ID
    const appIdResult = await WebhookService.setAppId(WIX_APP_ID);
    console.log('App ID set result:', appIdResult);
    
    // Set the Public Key
    const publicKeyResult = await WebhookService.setPublicKey(publicKey);
    console.log('Public Key set result:', publicKeyResult);
    
    // Get the current status
    const status = WebhookService.getStatus();
    console.log('Webhook service status:', status);
    
    console.log('Configuration complete!');
    
    if (status.appId && status.publicKeyConfigured) {
      console.log('The webhook server is now configured to use the Wix JavaScript SDK.');
      console.log('You can now start the webhook server from the application UI or using the test scripts.');
    } else {
      console.log('Configuration incomplete. Please check the error messages above.');
    }
  } catch (error) {
    console.error('Error configuring webhook SDK:', error);
  }
}

// Run the configuration
configureWebhookSdk();
