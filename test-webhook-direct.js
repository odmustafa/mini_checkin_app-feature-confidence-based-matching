/**
 * Test script to directly test the WebhookService
 * This bypasses the Electron app and directly tests the webhook server
 */

const WebhookService = require('./src/services/WebhookService');
const http = require('http');

async function runTest() {
  try {
    console.log('Initializing webhook service...');
    const initResult = await WebhookService.initialize();
    console.log('Initialization result:', initResult);

    if (!initResult.success) {
      console.error('Failed to initialize webhook service:', initResult.error);
      return;
    }

    console.log('Starting webhook server...');
    const startResult = await WebhookService.start({ useNgrok: false });
    console.log('Start result:', startResult);

    if (!startResult.success) {
      console.error('Failed to start webhook server:', startResult.error);
      return;
    }

    const port = startResult.port;
    const webhookUrl = `http://localhost:${port}/webhook`;
    console.log(`Webhook server running at ${webhookUrl}`);

    // Wait a moment for the server to fully start
    await new Promise(resolve => setTimeout(resolve, 1000));

    // Test the webhook endpoint with a JWT-like token
    console.log('Sending test webhook request...');
    const jwtToken = 'eyJraWQiOiJQb3pIV1ZJaFwvdlVNRWxFUlpXdGdUbWdQbz0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI3M2RiZDBkMy1kNDRlLTRkNmMtODg4Zi0xNmEyMzYwZDQ4MzgiLCJpc3MiOiJodHRwczpcL1wvd3d3Lndpei5jb20iLCJleHAiOjE3MTU4NzY2NTUsImlhdCI6MTcxNTg3MzA1NX0.example';

    // Set up event listener for webhook events
    WebhookService.on('webhook', (data) => {
      console.log('Webhook event received:', data);
    });

    // Send the test request
    await sendWebhookRequest(webhookUrl, jwtToken);

    // Keep the process running to see the webhook event
    console.log('Waiting for webhook event...');
    await new Promise(resolve => setTimeout(resolve, 3000));

    // Stop the webhook server
    console.log('Stopping webhook server...');
    const stopResult = await WebhookService.stop();
    console.log('Stop result:', stopResult);

    console.log('Test completed');
  } catch (error) {
    console.error('Error running test:', error);
  }
}

async function sendWebhookRequest(url, body) {
  return new Promise((resolve, reject) => {
    const urlObj = new URL(url);
    
    const options = {
      hostname: urlObj.hostname,
      port: urlObj.port,
      path: urlObj.pathname,
      method: 'POST',
      headers: {
        'Content-Type': 'text/plain',
        'X-Wix-Signature': 'test-signature'
      }
    };

    const req = http.request(options, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        console.log('Response status:', res.statusCode);
        console.log('Response headers:', res.headers);
        console.log('Response body:', data);
        resolve({ statusCode: res.statusCode, body: data });
      });
    });

    req.on('error', (error) => {
      console.error('Request error:', error);
      reject(error);
    });

    req.write(body);
    req.end();
  });
}

// Run the test
runTest();
