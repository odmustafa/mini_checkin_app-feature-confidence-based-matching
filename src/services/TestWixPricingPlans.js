/**
 * TestWixPricingPlans.js
 * A simple test script to demonstrate the Wix Pricing Plans functionality
 */
const WixPricingPlansService = require('./WixPricingPlansService');

// Main function to run all tests
async function runTests() {
  try {
    console.log('Starting Wix Pricing Plans Test...');
    
    // Test 1: Get all pricing plans
    console.log('\n=== Test 1: Get All Pricing Plans ===');
    const plansResult = await WixPricingPlansService.getAllPricingPlans();
    console.log('Plans result:', JSON.stringify(plansResult, null, 2));
    
    // Test 2: Get all orders
    console.log('\n=== Test 2: Get All Orders ===');
    const ordersResult = await WixPricingPlansService.listOrders();
    console.log('Orders result:', JSON.stringify(ordersResult, null, 2));
    
    // If we have orders, run more specific tests
    if (ordersResult.success && ordersResult.orders && ordersResult.orders.length > 0) {
      const sampleOrder = ordersResult.orders[0];
      
      // Test 3: Get order by ID
      console.log(`\n=== Test 3: Get Order by ID (${sampleOrder._id}) ===`);
      const orderResult = await WixPricingPlansService.getOrderById(sampleOrder._id);
      console.log('Order result:', JSON.stringify(orderResult, null, 2));
      
      // Test 4: Get orders by buyer ID
      if (sampleOrder.buyerId) {
        console.log(`\n=== Test 4: Get Orders by Buyer ID (${sampleOrder.buyerId}) ===`);
        const buyerOrdersResult = await WixPricingPlansService.getOrdersByBuyerId(sampleOrder.buyerId);
        console.log('Buyer orders result:', JSON.stringify(buyerOrdersResult, null, 2));
      }
      
      // Test 5: Get orders by plan ID
      if (sampleOrder.planId) {
        console.log(`\n=== Test 5: Get Orders by Plan ID (${sampleOrder.planId}) ===`);
        const planOrdersResult = await WixPricingPlansService.getOrdersByPlanId(sampleOrder.planId);
        console.log('Plan orders result:', JSON.stringify(planOrdersResult, null, 2));
      }
      
      // Test 6: Get pricing plan details
      if (sampleOrder.planId) {
        console.log(`\n=== Test 6: Get Pricing Plan Details (${sampleOrder.planId}) ===`);
        const planDetailsResult = await WixPricingPlansService.getPricingPlanById(sampleOrder.planId);
        console.log('Plan details result:', JSON.stringify(planDetailsResult, null, 2));
      }
    } else {
      console.log('No orders found. Skipping order-specific tests.');
    }
    
    // Test 7: Get orders by date range (last 30 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    
    console.log(`\n=== Test 7: Get Orders by Date Range (${startDate.toISOString()} to ${endDate.toISOString()}) ===`);
    const dateRangeOrdersResult = await WixPricingPlansService.getOrdersByDateRange(startDate, endDate);
    console.log('Date range orders result:', JSON.stringify(dateRangeOrdersResult, null, 2));
    
    // Test 8: Get active orders
    console.log(`\n=== Test 8: Get Orders by Status (ACTIVE) ===`);
    const activeOrdersResult = await WixPricingPlansService.getOrdersByStatus('ACTIVE');
    console.log('Active orders result:', JSON.stringify(activeOrdersResult, null, 2));
    
    console.log('\nAll tests completed!');
  } catch (err) {
    console.error('Error running tests:', err);
  }
}

// Run the tests
runTests().catch(console.error);
