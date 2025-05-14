// listRecentOrders.js
// Standalone script to list the 5 most recent Wix Pricing Plan orders using the Wix JavaScript SDK

const fs = require('fs');
const path = require('path');
const { createClient, ApiKeyStrategy } = require('@wix/sdk');
const { orders } = require('@wix/pricing-plans');
const { contacts } = require('@wix/crm');

// Load Wix configuration
const CONFIG_PATH = path.join(__dirname, '../../wix.config.json');
let WIX_CONFIG = {};
if (fs.existsSync(CONFIG_PATH)) {
  WIX_CONFIG = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
} else {
  console.error('Missing wix.config.json');
  process.exit(1);
}

async function main() {
  // Initialize Wix SDK client
  const client = createClient({
    modules: { orders, contacts },
    auth: ApiKeyStrategy({
      apiKey: WIX_CONFIG.apiKey,
      siteId: WIX_CONFIG.siteId,
    }),
  });

  // List the 5 most recent orders
  try {
    // Inspect available methods
    const orderMethods = Object.getOwnPropertyNames(client.orders).filter(
      (m) => typeof client.orders[m] === 'function'
    );
    console.log('Available methods on client.orders:', orderMethods);

    let response;
    if (typeof client.orders.listOrders === 'function') {
      response = await client.orders.listOrders({
        limit: 5,
        sort: { fieldName: 'createdDate', order: 'DESC' },
      });
    } else if (typeof client.orders.list === 'function') {
      response = await client.orders.list({
        limit: 5,
        sort: { fieldName: 'createdDate', order: 'DESC' },
      });
    } else if (typeof client.orders.managementListOrders === 'function') {
      response = await client.orders.managementListOrders({
        limit: 5,
        sort: { fieldName: 'createdDate', order: 'DESC' },
      });
    } else {
      throw new Error('No suitable method found to list orders. Methods: ' + orderMethods.join(', '));
    }

    // Try to print the orders
    const items = response.orders || response.items;
    const summary = [];
    if (items && items.length) {
      for (let idx = 0; idx < items.length; idx++) {
        const order = items[idx];
        let contactName = 'Unknown';
        let contactId = order.contactId || (order.buyer && order.buyer.contactId);
        let contactError = null;
        let contactErrorExplanation = null;
        let contactDiagnostics = null;
        if (contactId && client.contacts && typeof client.contacts.getContact === 'function') {
          try {
            const contactResp = await client.contacts.getContact({ contactId });
            if (contactResp && contactResp.contact && contactResp.contact.info && contactResp.contact.info.name) {
              const nameObj = contactResp.contact.info.name;
              contactName = [nameObj.first, nameObj.last].filter(Boolean).join(' ');
            } else {
              contactName = 'No name found';
            }
          } catch (e) {
            contactName = 'Lookup failed';
            contactError = e.message;
            contactDiagnostics = e.stack;
            // Error explanations
            if (contactError.includes('403')) {
              contactErrorExplanation = '403 Forbidden: This usually means your API key or app does not have permission to access contacts. Make sure your Wix app permissions include CRM/Contacts Read access.';
            } else if (contactError.includes('404')) {
              contactErrorExplanation = '404 Not Found: The contactId does not exist or is not accessible. This can happen if the order is for a deleted or external contact.';
            } else if (contactError.includes('Invalid') || contactError.includes('not valid')) {
              contactErrorExplanation = 'Invalid contactId: The ID may be missing, malformed, or not a real Wix contact ID.';
            } else {
              contactErrorExplanation = 'General error: Check your API credentials, permissions, and that the contactId is valid.';
            }
          }
        } else if (!contactId) {
          contactName = 'No contactId';
          contactErrorExplanation = 'No contactId found on the order. This may indicate the order was created without a linked contact or the field is named differently.';
        }
        summary.push({
          idx: idx + 1,
          id: order._id || order.id,
          plan: order.planName,
          buyer: order.buyer,
          contactId,
          contactName,
          contactError,
          contactErrorExplanation,
          contactDiagnostics,
          status: order.status,
          createdDate: order.createdDate,
          fullOrder: order
        });
      }
      fs.writeFileSync(path.join(__dirname, 'order_summary.json'), JSON.stringify(summary, null, 2));
      console.log('Order summary written to order_summary.json');
    } else {
      console.log('No orders found.');
      fs.writeFileSync(path.join(__dirname, 'order_summary.json'), JSON.stringify([], null, 2));
    }
  } catch (err) {
    console.error('Error listing orders:', err.message, err);
  }
}

main();
