/**
 * Test script for the Wix SDK Adapter searchMember method
 * This script tests the implementation of the searchMember method
 * following the Wix JavaScript SDK documentation
 */

// Import the Wix SDK Adapter
const WixSdkAdapter = require('./src/services/WixSdkAdapter');

// Test search criteria
const TEST_FIRST_NAME = 'TEST';
const TEST_LAST_NAME = 'PRICINGPLAN';
const TEST_DOB = '01-01-1990';

/**
 * Main test function
 */
async function testSearchMember() {
  console.log('=== Testing searchMember method ===');
  console.log(`Testing with name: ${TEST_FIRST_NAME} ${TEST_LAST_NAME}, DOB: ${TEST_DOB}`);
  
  try {
    // Call the searchMember method
    const result = await WixSdkAdapter.searchMember({
      firstName: TEST_FIRST_NAME,
      lastName: TEST_LAST_NAME,
      dateOfBirth: TEST_DOB
    });
    
    // Log the result
    console.log('\nResult:');
    console.log('Success:', result.success);
    console.log('Total contacts:', result.total);
    
    if (result.success && result.contacts && result.contacts.length > 0) {
      console.log('\nFound contacts:');
      
      // Display all contacts
      result.contacts.forEach((contact, index) => {
        console.log(`\n[Contact ${index + 1}]`);
        console.log('- Name:', contact.info?.name?.first, contact.info?.name?.last);
        console.log('- Email:', contact.info?.emails?.[0]?.email || 'N/A');
        console.log('- Phone:', contact.info?.phones?.[0]?.phone || 'N/A');
        console.log('- Confidence Score:', contact.confidenceScore);
        console.log('- Confidence Level:', contact.confidenceLevel);
        console.log('- Match Details:', contact.matchDetails?.join(', ') || 'None');
      });
    } else {
      console.log('\nNo contacts found or error occurred:');
      console.log('Error:', result.error || 'No error message provided');
      console.log('Details:', result.details || 'No details provided');
    }
  } catch (err) {
    console.error('Error running test:', err);
  }
}

// Run the test
testSearchMember()
  .then(() => console.log('\nTest completed'))
  .catch(err => console.error('Test failed:', err));
