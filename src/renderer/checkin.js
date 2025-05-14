/**
 * Check-in functionality for the mini check-in app
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for APIs
 * - Fail Fast and Learn: Using fallback mechanisms and detailed error reporting
 * - Separation of Concerns: Maintaining clear boundaries between components
 */

// Add a Watch button next to the Scan button
const scanBtn = document.getElementById('scan-btn');
const watchBtnContainer = document.createElement('div');
watchBtnContainer.className = 'watch-btn-container';
watchBtnContainer.innerHTML = `
  <button id="watch-btn" class="secondary-btn">Watch Scan-ID</button>
  <span id="watch-status" class="watch-status">Not watching</span>
`;
scanBtn.parentNode.insertBefore(watchBtnContainer, scanBtn.nextSibling);

// Get references to the new elements
const watchBtn = document.getElementById('watch-btn');
const watchStatus = document.getElementById('watch-status');

// Set up watch button hover effects and click handler
watchBtn.addEventListener('mouseenter', () => {
  if (watchBtn.getAttribute('data-watching') === 'true') {
    watchBtn.textContent = 'End Watch';
    watchBtn.classList.add('end-watch-hover');
  }
  // No hover effect when not watching
});

watchBtn.addEventListener('mouseleave', () => {
  if (watchBtn.getAttribute('data-watching') === 'true') {
    watchBtn.textContent = 'Stop Watching';
    watchBtn.classList.remove('end-watch-hover');
  }
  // No hover effect to remove when not watching
});

// Set up watch button click handler
watchBtn.addEventListener('click', async () => {
  // Toggle watching state
  const status = await window.scanidAPI.getWatchStatus();
  
  if (status.success && status.watching) {
    // Currently watching, so stop
    const result = await window.scanidAPI.stopWatching();
    if (result.success) {
      watchBtn.textContent = 'Watch Scan-ID';
      watchBtn.setAttribute('data-watching', 'false');
      watchBtn.classList.remove('end-watch-hover');
      watchStatus.textContent = 'Not watching';
      watchStatus.className = 'watch-status';
    } else {
      console.error('Failed to stop watching:', result.error);
    }
  } else {
    // Not watching, so start
    const result = await window.scanidAPI.startWatching();
    if (result.success) {
      watchBtn.textContent = 'Stop Watching';
      watchBtn.setAttribute('data-watching', 'true');
      watchStatus.textContent = 'Watching for scans...';
      watchStatus.className = 'watch-status watching';
    } else {
      console.error('Failed to start watching:', result.error);
    }
  }
});

// Register for scan watch events
let unregisterCallback;
document.addEventListener('DOMContentLoaded', () => {
  // Set up the scan watch callback
  unregisterCallback = window.scanidAPI.onScanWatchEvent((eventType, data) => {
    if (eventType === 'status') {
      // Update the watch status UI
      if (data.watching) {
        watchBtn.textContent = 'Stop Watching';
        watchBtn.setAttribute('data-watching', 'true');
        watchStatus.textContent = 'Watching for scans...';
        watchStatus.className = 'watch-status watching';
      } else {
        watchBtn.textContent = 'Watch Scan-ID';
        watchBtn.setAttribute('data-watching', 'false');
        watchStatus.textContent = 'Not watching';
        watchStatus.className = 'watch-status';
      }
    } else if (eventType === 'newscan') {
      // New scan detected, process it
      watchStatus.textContent = 'New scan detected!';
      watchStatus.className = 'watch-status new-scan';
      
      // Process the scan automatically
      processScan(data);
      
      // Reset the status after a delay
      setTimeout(() => {
        watchStatus.textContent = 'Watching for scans...';
        watchStatus.className = 'watch-status watching';
      }, 3000);
    } else if (eventType === 'error') {
      // Error occurred
      watchStatus.textContent = `Error: ${data.error}`;
      watchStatus.className = 'watch-status error';
    }
  });
});

// Clean up on page unload
window.addEventListener('beforeunload', () => {
  if (unregisterCallback) {
    unregisterCallback();
  }
});

// Main function to process a scan (either from button click or watch feature)
async function processScan(scan) {
  const resultDiv = document.getElementById('scan-result');
  const accountDiv = document.getElementById('account-info');
  
  // Diagnostics panel
  let diagPanel = document.getElementById('diagnostics-panel');
  if (!diagPanel) {
    diagPanel = document.createElement('div');
    diagPanel.id = 'diagnostics-panel';
    diagPanel.style = 'background:#f4f4f4;border:1px solid #ccc;padding:8px;margin:8px 0;overflow:auto;max-height:200px;font-size:12px;';
    resultDiv.parentNode.insertBefore(diagPanel, resultDiv.nextSibling);
  }
  
  function showDiagnostics(title, data) {
    diagPanel.innerHTML = `<strong>${title}</strong><pre>${JSON.stringify(data, null, 2)}</pre>`;
  }
  
  resultDiv.textContent = 'Processing scan...';
  accountDiv.textContent = '';
  
  try {
    if (!scan) {
      // If no scan provided, fetch the latest
      scan = await window.scanidAPI.getLatestScan();
    }
    
    if (!scan) {
      resultDiv.textContent = JSON.stringify(scan, null, 2);
      showDiagnostics('Scan API Response', scan);
      return;
    }
    
    if (scan.error) {
      resultDiv.textContent = 'Error: ' + scan.error;
      showDiagnostics('Scan API Error', scan);
      return;
    }
    
    // Format the date in a more readable format
    const dob = scan.DateOfBirth ? new Date(scan.DateOfBirth.replace(/-/g, '/')).toLocaleDateString() : 'N/A';
    const scanTime = scan.ScanTime ? new Date(scan.ScanTime.split(' ')[0].replace(/\//g, '-')).toLocaleString() : 'N/A';
    const expires = scan.IDExpiration ? new Date(scan.IDExpiration.replace(/-/g, '/')).toLocaleDateString() : 'N/A';
    
    // Check if photo path exists and create appropriate HTML
    let photoHtml = '';
    if (scan.PhotoPath && scan.PhotoPath.trim() !== '') {
      // Use actual photo from the file path with click-to-expand functionality
      photoHtml = `<img src="file://${scan.PhotoPath}" alt="ID Photo" class="id-photo" data-photo-path="file://${scan.PhotoPath}" onerror="this.onerror=null; this.src=''; this.parentNode.innerHTML='<div class=\'photo-placeholder\'>Image not found</div>';">`;
    } else {
      // Fallback to placeholder
      photoHtml = `<div class="photo-placeholder">No Photo</div>`;
    }
    
    resultDiv.innerHTML = `
      <div class="scan-card">
        <h3>ID Scan Result</h3>
        <div class="scan-details">
          <div class="scan-photo">
            ${photoHtml}
          </div>
          <div class="scan-info">
            <p><strong>Full Name:</strong> ${scan.FullName || 'N/A'}</p>
            <p><strong>DOB:</strong> ${dob} (Age: ${scan.Age || 'N/A'})</p>
            <p><strong>ID Number:</strong> ${scan.IDNumber || 'N/A'}</p>
            <p><strong>Expires:</strong> ${expires}</p>
            <p><strong>Scan Time:</strong> ${scanTime}</p>
          </div>
        </div>
      </div>
    `;
    
    showDiagnostics('Scan API Response', scan);
    
    // Step 2: Search for the member in Wix
    try {
      accountDiv.innerHTML = '<div class="loading">Looking up member in Wix...</div>';
      
      // Get the first name, last name, and DOB from the scan
      const firstName = scan.FirstName || '';
      const lastName = scan.LastName || '';
      const dateOfBirth = scan.DateOfBirth || '';
      
      // Search by name and DOB using either SDK or Direct API based on selected mode
      showDiagnostics('Member Search API Request', { firstName, lastName, dateOfBirth });
    
      // Use the Wix JavaScript SDK for member search as per Wix documentation
      console.log('Using Wix SDK for member search');
      // Make sure we have valid search parameters to avoid undefined values
      const searchParams = {
        firstName: scan.FirstName || '',
        lastName: scan.LastName || '',
        dateOfBirth: scan.DateOfBirth || ''
      };
      
      console.log('Search parameters:', searchParams);
      
      // Pass parameters as an object to match the expected format in WixSdkAdapter
      const memberResult = await window.wixSdk.searchMember(searchParams);
    
      // Display the query details used for the search
      if (memberResult.queryDetails) {
        const queryDetailsHtml = `
          <div class="query-details-panel">
            <h4>Wix SDK Query Details</h4>
            <div class="query-details">
              <p><strong>Method Used:</strong> ${memberResult.queryDetails.methodUsed}</p>
              <p><strong>First Name:</strong> "${memberResult.queryDetails.firstName}"</p>
              <p><strong>Last Name:</strong> "${memberResult.queryDetails.lastName}"</p>
              <p><strong>Date of Birth:</strong> "${memberResult.queryDetails.dateOfBirth}"</p>
            </div>
          </div>
        `;
        accountDiv.innerHTML += queryDetailsHtml;
      }
      
      // Show the diagnostics panel with the raw API response
      showDiagnostics('Wix SDK Response', memberResult);
      
      if (!memberResult.success) {
        accountDiv.innerHTML = `<div class="error">Wix API Error: ${memberResult.error}</div>`;
        return;
      }
      
      if (!memberResult.items || memberResult.items.length === 0) {
        accountDiv.innerHTML = '<div class="error">No matching Wix member found.</div>';
        return;
      }
    
      // Format the member data for display with confidence scores
      let memberHtml = `
        <div class="member-info">
          <h3>Contacts Found (${memberResult.items.length})</h3>
          <p class="member-source">Source: ${memberResult.source}</p>
          <div class="confidence-legend">
            <div class="confidence-info">Confidence scores: 
              <span class="high-confidence">High (60-100)</span> | 
              <span class="medium-confidence">Medium (35-59)</span> | 
              <span class="low-confidence">Low (0-34)</span>
            </div>
          </div>
          <div class="contacts-list">
      `;
      
      // Add each contact with confidence score
      memberResult.items.forEach((contact, index) => {
        const confidenceScore = contact._confidence?.score || 0;
        let confidenceClass = 'low-confidence';
        
        if (confidenceScore >= 60) {
          confidenceClass = 'high-confidence';
        } else if (confidenceScore >= 35) {
          confidenceClass = 'medium-confidence';
        }
        
        // Get contact details
        const contactName = `${contact.info?.name?.first || ''} ${contact.info?.name?.last || ''}`;
        const contactId = contact._id || contact.id || 'N/A';
        
        // Access the email field based on Wix CRM Contacts API structure
        let contactEmail = 'N/A';
        
        // Primary email address is stored in the primaryInfo.email field
        if (contact.primaryInfo?.email) {
          contactEmail = contact.primaryInfo.email;
        }
        // Check for email in the info.emails array
        else if (contact.info?.emails && contact.info.emails.length > 0) {
          // Find the primary email first
          const primaryEmail = contact.info.emails.find(e => e.primary === true);
          if (primaryEmail?.email) {
            contactEmail = primaryEmail.email;
          } 
          // If no primary email, use the first one
          else if (contact.info.emails[0]?.email) {
            contactEmail = contact.info.emails[0].email;
          }
        }
        // Check for loginEmail field (used for members)
        else if (contact.loginEmail) {
          contactEmail = contact.loginEmail;
        }
        
        const contactCreated = new Date(contact._createdDate || contact.createdDate).toLocaleString();
        
        // Format confidence details
        const confidenceDetails = contact._confidence?.details || [];
        const confidenceDetailsHtml = confidenceDetails.length > 0 ?
          `<div class="confidence-details">${confidenceDetails.join('<br>')}</div>` : '';
        
        // Add this contact to the HTML
        memberHtml += `
          <div class="contact-item ${confidenceClass}">
            <div class="contact-header">
              <span class="contact-name">${contactName}</span>
              <span class="confidence-score">Match: ${confidenceScore}%</span>
            </div>
            <div class="contact-details">
              <div class="contact-id">ID: ${contactId}</div>
              <div class="contact-email">Email: ${contactEmail}</div>
              <div class="contact-created">Created: ${contactCreated}</div>
              ${confidenceDetailsHtml}
            </div>
            <div class="contact-actions">
              <button class="view-plans-btn" data-member-id="${contactId}">View Plans</button>
            </div>
            <div id="plans-${contactId}" class="contact-plans"></div>
          </div>
        `;
      });
      
      // Close the contacts list div
      memberHtml += `
          </div>
        </div>
      `;
      
      accountDiv.innerHTML = memberHtml;
      
      // Add event listeners for the View Plans buttons
      setTimeout(() => {
        // Set up the view plans buttons
        document.querySelectorAll('.view-plans-btn').forEach(button => {
          button.addEventListener('click', async function() {
            const memberId = this.getAttribute('data-member-id');
            const plansContainer = document.getElementById(`plans-${memberId}`);
            
            if (plansContainer) {
              plansContainer.innerHTML = '<div class="loading">Loading plans and orders...</div>';
              
              // Use the Wix JavaScript SDK to get orders for this contact
              // The getMemberPricingPlans method now uses the SDK implementation
              const plansResult = await window.scanidAPI.getMemberPricingPlans(memberId);
              
              // Store the response in the lastWixResponse for debugging
              window.lastWixResponse = { plansResult };
              
              // Set ordersResult to plansResult since they're now the same API call
              const ordersResult = plansResult;
              
              // Show the raw response in the diagnostics panel for debugging
              if (typeof showDiagnostics === 'function') {
                showDiagnostics('Wix Pricing Plans Response', plansResult);
              }
              
              // Log the results for debugging
              console.log('Plans result:', plansResult);
              console.log('Orders result:', ordersResult);
              
              // Build the HTML for plans
              let plansHtml = '';
              if (plansResult.success && plansResult.plans && plansResult.plans.length > 0) {
              // Filter for active plans to show at the top
              const activePlans = plansResult.plans.filter(plan => plan.status === 'ACTIVE');
              const inactivePlans = plansResult.plans.filter(plan => plan.status !== 'ACTIVE');
              const sortedPlans = [...activePlans, ...inactivePlans];
              
              plansHtml = `
                <div class="pricing-plans">
                  <h4>Membership Plans (${plansResult.plans.length})</h4>
                  <ul class="plans-list">
                    ${sortedPlans.map(plan => {
                      // Format price if available
                      const priceDisplay = plan.price ? 
                        `${plan.price} ${plan.currency || ''}` : 'Free';
                      
                      // Determine plan status class and label
                      let statusClass = 'inactive-plan';
                      let statusLabel = plan.status || 'Unknown';
                      
                      if (plan.status === 'ACTIVE') {
                        statusClass = 'active-plan';
                        statusLabel = 'ACTIVE';
                      } else if (plan.status === 'CANCELED') {
                        statusLabel = 'CANCELED';
                      } else if (plan.status === 'PENDING') {
                        statusClass = 'pending-plan';
                        statusLabel = 'PENDING';
                      } else if (plan.status === 'PAUSED') {
                        statusClass = 'paused-plan';
                        statusLabel = 'PAUSED';
                      }
                      
                      // Format dates
                      const startDate = plan.validFrom ? 
                        new Date(plan.validFrom).toLocaleDateString() : 'N/A';
                      const endDate = plan.expiresAt ? 
                        new Date(plan.expiresAt).toLocaleDateString() : 'No expiration';
                      
                      // Calculate if plan is expired
                      const isExpired = plan.expiresAt && new Date(plan.expiresAt) < new Date();
                      if (isExpired && statusClass !== 'active-plan') {
                        statusClass = 'expired-plan';
                        statusLabel = 'EXPIRED';
                      }
                      
                      // Format recurring information
                      const recurringInfo = plan.isRecurring ? 
                        `<div class="plan-recurring">Recurring: Yes</div>` : '';
                      
                      return `
                        <li class="plan-item ${statusClass}">
                          <div class="plan-header">
                            <div class="plan-name">${plan.planName || 'Unnamed Plan'}</div>
                            <div class="plan-status">${statusLabel}</div>
                          </div>
                          <div class="plan-details">
                            <div class="plan-price">Price: ${priceDisplay}</div>
                            <div class="plan-dates">Start: ${startDate}</div>
                            <div class="plan-dates">End: ${endDate}</div>
                            ${recurringInfo}
                            ${plan.orderType ? `<div class="plan-type">Type: ${plan.orderType}</div>` : ''}
                            ${plan.paymentStatus ? `<div class="payment-status">Payment: ${plan.paymentStatus}</div>` : ''}
                          </div>
                        </li>
                      `;
                    }).join('')}
                  </ul>
                </div>
              `;
            } else {
              plansHtml = `<div class="no-plans">No membership plans found</div>`;
            }
            
              // Since we're already showing plans in a detailed way, we can simplify the orders section
              // or focus on different aspects like payment history
              let ordersHtml = '';
              if (ordersResult.success && ordersResult.orders && ordersResult.orders.length > 0) {
              // Sort orders by creation date (newest first)
              const sortedOrders = [...ordersResult.orders].sort((a, b) => {
                return new Date(b.createdDate || 0) - new Date(a.createdDate || 0);
              });
              
              ordersHtml = `
                <div class="pricing-orders">
                  <h4>Payment History</h4>
                  <ul class="orders-list">
                    ${sortedOrders.map(order => {
                      // Format payment details
                      let paymentInfo = 'No payment details';
                      if (order.paymentDetails) {
                        const paymentMethod = order.paymentDetails.paymentMethod || 'Unknown method';
                        const paymentAmount = order.pricing?.price ? 
                          `${order.pricing.price} ${order.pricing.currency || ''}` : 'Free';
                        paymentInfo = `${paymentMethod} - ${paymentAmount}`;
                      }
                      
                      // Format dates in a more readable way
                      const createdDate = order.createdDate ? 
                        new Date(order.createdDate).toLocaleString() : 'Unknown date';
                      
                      // Get payment status with appropriate styling
                      let paymentStatusClass = 'payment-unknown';
                      if (order.paymentStatus === 'PAID') {
                        paymentStatusClass = 'payment-paid';
                      } else if (order.paymentStatus === 'PENDING') {
                        paymentStatusClass = 'payment-pending';
                      } else if (order.paymentStatus === 'REFUNDED') {
                        paymentStatusClass = 'payment-refunded';
                      } else if (order.paymentStatus === 'FAILED') {
                        paymentStatusClass = 'payment-failed';
                      }
                      
                      return `
                        <li class="order-item">
                          <div class="order-header">
                            <div class="order-name">${order.planName || 'Unnamed Order'}</div>
                            <div class="order-date">${createdDate}</div>
                          </div>
                          <div class="order-details">
                            <div class="order-payment ${paymentStatusClass}">
                              <span class="payment-status">${order.paymentStatus || 'Unknown'}</span>
                              <span class="payment-info">${paymentInfo}</span>
                            </div>
                            <div class="order-meta">
                              <span class="order-id">ID: ${order._id || order.id || 'N/A'}</span>
                              ${order.orderType ? `<span class="order-type">Type: ${order.orderType}</span>` : ''}
                            </div>
                          </div>
                        </li>
                      `;
                    }).join('')}
                  </ul>
                </div>
              `;
            } else {
              ordersHtml = `<div class="no-orders">No payment history found</div>`;
            }
            
              // Combine plans and orders HTML
              plansContainer.innerHTML = `
                <div class="member-subscription-info">
                  ${plansHtml}
                  ${ordersHtml}
                </div>
              `;
          }
        });
      });
    }, 100);
    } catch (err) {
      console.error('Error processing member lookup:', err);
      accountDiv.innerHTML = `<div class="error">Error: ${err.message}</div>`;
    }
  } catch (err) {
    resultDiv.textContent = 'Error: ' + err.message;
  }
}

// Original scan button click handler
document.getElementById('scan-btn').addEventListener('click', async () => {
  // Simply call the processScan function with no arguments
  // This will fetch the latest scan and process it
  await processScan();
});

// Debug and Help panel functionality
let lastWixResponse = null;
let restartCount = 0;
const sessionKey = 'mini_checkin_restart_count';

// Check if we have a restart count stored in sessionStorage
if (sessionStorage.getItem(sessionKey)) {
  restartCount = parseInt(sessionStorage.getItem(sessionKey), 10);
}

// Debug panel functionality
const debugBtn = document.getElementById('debug-btn');
const debugPanel = document.getElementById('debug-panel');
const closeDebugBtn = document.getElementById('close-debug-btn');
const debugContent = document.getElementById('debug-content');

debugBtn.addEventListener('click', () => {
  // Toggle debug panel visibility
  debugPanel.classList.toggle('visible');
  
  // If we have a Wix response, display it
  if (lastWixResponse) {
    debugContent.textContent = JSON.stringify(lastWixResponse, null, 2);
  } else {
    debugContent.textContent = 'No Wix SDK Response data available yet. Perform a scan to see data.';
  }
});

closeDebugBtn.addEventListener('click', () => {
  debugPanel.classList.remove('visible');
});

// Help panel functionality
const helpBtn = document.getElementById('help-btn');
const helpPanel = document.getElementById('help-panel');
const closeHelpBtn = document.getElementById('close-help-btn');
const restartBtn = document.getElementById('restart-btn');
const supportInfo = document.getElementById('support-info');

helpBtn.addEventListener('click', () => {
  // Toggle help panel visibility
  helpPanel.classList.toggle('visible');
  
  // Show support info if restart count is at least 1
  if (restartCount > 0) {
    supportInfo.classList.remove('hidden');
  } else {
    supportInfo.classList.add('hidden');
  }
});

closeHelpBtn.addEventListener('click', () => {
  helpPanel.classList.remove('visible');
});

restartBtn.addEventListener('click', () => {
  // Increment restart count and save to sessionStorage
  restartCount++;
  sessionStorage.setItem(sessionKey, restartCount.toString());
  
  // Show support info if this is at least the first restart
  if (restartCount > 0) {
    supportInfo.classList.remove('hidden');
  }
  
  // Request app restart via IPC if in Electron
  if (window.electronAPI) {
    window.electronAPI.restartApp();
  } else {
    // In web mode, just reload the page
    window.location.reload();
  }
});

// Modify the processScan function to store the Wix response for debugging
const originalProcessScan = processScan;
processScan = async function(scan) {
  const result = await originalProcessScan.apply(this, arguments);
  
  // Store the last Wix response for debugging
  if (window.lastWixResponse) {
    lastWixResponse = window.lastWixResponse;
  }
  
  return result;
};

// Modify the showDiagnostics function to capture Wix SDK responses
const originalShowDiagnostics = processScan.showDiagnostics;
processScan.showDiagnostics = function(title, data) {
  // Call the original function
  originalShowDiagnostics.apply(this, arguments);
  
  // If this is a Wix SDK response, store it for debugging
  if (title.includes('Wix') || title.includes('Member') || title.includes('Contact')) {
    window.lastWixResponse = data;
    lastWixResponse = data;
  }
};

// Function to set up photo expansion functionality
function setupPhotoExpansion() {
  // Remove any existing expanded photo container
  const existingExpanded = document.querySelector('.id-photo-expanded');
  if (existingExpanded) {
    document.body.removeChild(existingExpanded);
  }
  
  // Add click event listeners to all ID photos
  document.querySelectorAll('.id-photo').forEach(photo => {
    // Remove existing listeners to prevent duplicates
    photo.removeEventListener('click', expandPhoto);
    // Add click event listener
    photo.addEventListener('click', expandPhoto);
  });
}

// Function to handle photo expansion
function expandPhoto(event) {
  const photoPath = event.target.getAttribute('data-photo-path');
  
  // Create expanded view container
  const expandedContainer = document.createElement('div');
  expandedContainer.className = 'id-photo-expanded';
  
  // Create image element
  const expandedImg = document.createElement('img');
  expandedImg.src = photoPath;
  expandedImg.alt = 'Expanded ID Photo';
  
  // Add image to container
  expandedContainer.appendChild(expandedImg);
  
  // Add click event to close on click
  expandedContainer.addEventListener('click', () => {
    document.body.removeChild(expandedContainer);
  });
  
  // Add to body
  document.body.appendChild(expandedContainer);
}

// Call setupPhotoExpansion after processing a scan
document.addEventListener('DOMContentLoaded', () => {
  // Patch the processScan function to add photo expansion functionality
  const originalProcessScan = window.processScan || processScan;
  window.processScan = async function() {
    const result = await originalProcessScan.apply(this, arguments);
    // Setup photo expansion after scan is processed
    setTimeout(setupPhotoExpansion, 100);
    return result;
  };
});
