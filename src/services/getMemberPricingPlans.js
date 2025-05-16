/**
 * Get pricing plans for a member
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for the Wix API
 * - Separation of Concerns: Maintaining clear boundaries between components
 * 
 * This implementation strictly follows the Wix JavaScript SDK documentation for pricing plans
 */
async function getMemberPricingPlans(memberId) {
  try {
    await this.initialize();
    
    console.log(`Getting pricing plans for member: ${memberId} with SDK Adapter`);
    
    if (!memberId) {
      throw new Error('Member ID is required');
    }
    
    // Import the pricing plans module
    const { orders } = require('@wix/pricing-plans');
    
    // Add the pricing plans module to the client
    if (!this.client.orders) {
      console.log('Adding pricing-plans orders module to the client');
      this.client.orders = orders;
    }
    
    // Log the request details for debugging
    console.log(`Requesting pricing plan orders for buyerId: ${memberId}`);
    
    // Use the managementListOrders method according to Wix JavaScript SDK documentation
    // https://dev.wix.com/docs/sdk/backend-modules/pricing-plans/orders/management-list-orders
    const response = await this.client.orders.managementListOrders({
      // Filter by buyerId using the proper parameter structure
      buyerIds: [memberId],
      // Sort by created date in descending order (newest first)
      sort: {
        fieldName: 'createdDate',
        order: 'DESC'
      },
      // Get maximum allowed orders
      limit: 50
    });
    
    // Log the response for debugging
    console.log(`Found ${response.orders?.length || 0} orders for member ${memberId}`);
    
    // Process orders to extract plan details according to the SDK documentation structure
    const processedOrders = (response.orders || []).map(order => {
      // Extract buyer information from the correct location in the response
      const buyerId = order.buyer?.memberId || 'N/A';
      const contactId = order.buyer?.contactId || 'N/A';
      
      // Extract pricing information from the correct location in the response
      const priceAmount = order.planPrice?.amount;
      const priceCurrency = order.planPrice?.currency;
      
      // Get creation date from the correct field (_createdDate)
      const createdDate = order._createdDate || null;
      
      // Get start date from the appropriate fields based on the SDK structure
      const startDate = order.startDate || 
                       (order.currentCycle && order.currentCycle.startedDate) || 
                       createdDate;
      
      // Format payment amount based on available data
      const paymentAmount = priceAmount !== undefined ? 
        `${priceAmount} ${priceCurrency || ''}` : 
        'Free';
      
      // Calculate if plan is active based on status and dates
      const isActive = order.status === 'ACTIVE' && 
                      (!order.endDate || new Date(order.endDate) > new Date());
      
      // Return a comprehensive processed order object with all available information
      return {
        // Original order data
        _id: order._id,
        planId: order.planId,
        
        // Plan information
        planName: order.planName || 'Unnamed Plan',
        planDescription: order.planDescription || '',
        status: order.status || 'UNKNOWN',
        
        // Buyer information
        buyerId: buyerId,
        contactId: contactId,
        
        // Dates
        createdDate: createdDate,
        startDate: startDate,
        endDate: order.endDate,
        
        // Pricing information
        price: priceAmount,
        currency: priceCurrency,
        paymentStatus: order.lastPaymentStatus || 'UNKNOWN',
        orderMethod: order.orderMethod || 'UNKNOWN',
        
        // Recurring information
        isRecurring: !!order.recurring,
        recurringCycle: order.recurring?.cycle,
        recurringPeriod: order.recurring?.period,
        
        // Payment details
        paymentMethod: order.paymentDetails?.paymentMethod || 'Unknown',
        paymentAmount: paymentAmount,
        paymentDate: order.paymentDetails?.paymentDate,
        
        // Formatted dates for display
        formattedStartDate: startDate ? new Date(startDate).toLocaleDateString() : 'N/A',
        formattedEndDate: order.endDate ? new Date(order.endDate).toLocaleDateString() : 'No expiration',
        formattedCreatedDate: createdDate ? new Date(createdDate).toLocaleString() : 'Unknown',
        
        // Status indicators
        isActive: isActive,
        isExpired: order.status === 'ENDED' || (order.endDate && new Date(order.endDate) < new Date()),
        isPaused: order.pausePeriods && order.pausePeriods.length > 0
      };
    });
    
    // Separate active and inactive plans for better organization
    const activePlans = processedOrders.filter(plan => plan.isActive);
    const inactivePlans = processedOrders.filter(plan => !plan.isActive);
    
    // Sort plans by creation date (newest first within each category)
    const sortedActivePlans = activePlans.sort((a, b) => {
      const dateA = a.createdDate ? new Date(a.createdDate) : new Date(0);
      const dateB = b.createdDate ? new Date(b.createdDate) : new Date(0);
      return dateB - dateA;
    });
    
    const sortedInactivePlans = inactivePlans.sort((a, b) => {
      const dateA = a.createdDate ? new Date(a.createdDate) : new Date(0);
      const dateB = b.createdDate ? new Date(b.createdDate) : new Date(0);
      return dateB - dateA;
    });
    
    // Combine sorted plans with active plans first
    const sortedPlans = [...sortedActivePlans, ...sortedInactivePlans];
    
    return {
      success: true,
      plans: sortedPlans,
      orders: processedOrders, // For backward compatibility
      activePlans: sortedActivePlans,
      inactivePlans: sortedInactivePlans,
      total: processedOrders.length,
      source: 'wix-pricing-plans'
    };
  } catch (err) {
    console.error('Error getting pricing plans with SDK:', err);
    
    // Ensure we have a valid error message even if err is empty or undefined
    const errorMessage = err && err.message ? err.message : 'Unknown error occurred while retrieving pricing plans';
    
    // Create a detailed error response
    return {
      success: false,
      error: `Error getting pricing plans: ${errorMessage}`,
      details: err && err.stack ? err.stack : 'No stack trace available',
      errorObject: err || {},
      source: 'wix-sdk',
      timestamp: new Date().toISOString()
    };
  }
}
