/**
 * Test script for the Wix SDK Adapter getMemberPricingPlans method
 * This script tests the updated implementation of the getMemberPricingPlans method
 * following the official Wix JavaScript SDK documentation
 */

// Import the Wix SDK Adapter
const WixSdkAdapter = require('./src/services/WixSdkAdapter');

// Test member ID (replace with a valid member ID for testing)
const TEST_MEMBER_ID = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

/**
 * Main test function
 */
async function testGetMemberPricingPlans() {
  console.log('=== Testing Updated getMemberPricingPlans method ===');
  console.log(`Testing with member ID: ${TEST_MEMBER_ID}`);
  
  try {
    // Call the getMemberPricingPlans method
    const result = await WixSdkAdapter.getMemberPricingPlans(TEST_MEMBER_ID);
    
    // Log the result
    console.log('\nResult:');
    console.log('Success:', result.success);
    console.log('Total plans:', result.total);
    
    if (result.success && result.plans && result.plans.length > 0) {
      console.log('\nActive Plans:', result.activePlans.length);
      console.log('Inactive Plans:', result.inactivePlans.length);
      
      // Display the first plan details
      const firstPlan = result.plans[0];
      console.log('\nFirst Plan Details:');
      console.log('- Plan Name:', firstPlan.planName);
      console.log('- Plan ID:', firstPlan.planId);
      console.log('- Buyer ID:', firstPlan.buyerId);
      console.log('- Contact ID:', firstPlan.contactId);
      console.log('- Status:', firstPlan.status);
      console.log('- Is Active:', firstPlan.isActive);
      console.log('- Created Date:', firstPlan.formattedCreatedDate);
      console.log('- Start Date:', firstPlan.formattedStartDate);
      console.log('- End Date:', firstPlan.formattedEndDate);
      console.log('- Payment Amount:', firstPlan.paymentAmount);
      console.log('- Payment Method:', firstPlan.paymentMethod);
      
      // Display all plans
      console.log('\nAll Plans:');
      result.plans.forEach((plan, index) => {
        console.log(`\n[Plan ${index + 1}]`);
        console.log('- Plan Name:', plan.planName);
        console.log('- Buyer ID:', plan.buyerId);
        console.log('- Status:', plan.status);
        console.log('- Is Active:', plan.isActive);
        console.log('- Created:', plan.formattedCreatedDate);
        console.log('- Payment:', plan.paymentAmount);
      });
    } else {
      console.log('\nNo plans found or error occurred:');
      console.log('Error:', result.error || 'No error message provided');
      console.log('Details:', result.details || 'No details provided');
    }
  } catch (err) {
    console.error('Error running test:', err);
  }
}

// Run the test
testGetMemberPricingPlans()
  .then(() => console.log('\nTest completed'))
  .catch(err => console.error('Test failed:', err));
