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

// Set up watch button event handler
watchBtn.addEventListener('click', async () => {
  // Toggle watching state
  const status = await window.scanidAPI.getWatchStatus();
  
  if (status.success && status.watching) {
    // Currently watching, so stop
    const result = await window.scanidAPI.stopWatching();
    if (result.success) {
      watchBtn.textContent = 'Watch Scan-ID';
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
        watchStatus.textContent = 'Watching for scans...';
        watchStatus.className = 'watch-status watching';
      } else {
        watchBtn.textContent = 'Watch Scan-ID';
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
    
    // Prepare the photo HTML
    let photoHtml = '';
    if (scan.PhotoPath) {
      // Convert Windows path to file URL and normalize slashes
      const photoPath = 'file:///' + scan.PhotoPath.replace(/\\/g, '/');
      photoHtml = `
        <img src="${photoPath}" 
             alt="ID Photo" 
             class="id-photo" 
             onerror="this.style.display='none'; this.parentElement.querySelector('.photo-placeholder').style.display='block';">
        <div class="photo-placeholder" style="display: none;">ID Photo</div>
      `;
    } else {
      photoHtml = '<div class="photo-placeholder">ID Photo</div>';
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
    
    // Call Direct Wix API for member lookup
    accountDiv.innerHTML = '<div class="loading">Looking up member in Wix...</div>';
    
    // Get the first name, last name, and DOB from the scan
    const firstName = scan.FirstName || '';
    const lastName = scan.LastName || '';
    const dateOfBirth = scan.DateOfBirth || '';
    
    // Search by name and DOB using our new Direct API
    showDiagnostics('Member Search API Request', { firstName, lastName, dateOfBirth });
    const searchResult = await window.scanidAPI.findWixMember(firstName, lastName, dateOfBirth);
    showDiagnostics('Member Search API Response', searchResult);
    
    if (!searchResult.success) {
      accountDiv.innerHTML = `<div class="error">Wix API Error: ${searchResult.error}</div>`;
      return;
    }
    
    if (!searchResult.results || searchResult.results.length === 0) {
      accountDiv.innerHTML = '<div class="error">No matching Wix member found.</div>';
      return;
    }
    
    // Show multiple matches if found
    if (searchResult.results.length > 1) {
      accountDiv.innerHTML = `
        <div class="member-info">
          <h3>Multiple Matching Members Found</h3>
          <div class="matches-list">
            ${searchResult.results.map((member, index) => {
              // Handle different data structures based on source (members vs contacts)
              const name = searchResult.source === 'members' 
                ? `${member.profile?.firstName || ''} ${member.profile?.lastName || ''}` 
                : `${member.info?.name?.first || ''} ${member.info?.name?.last || ''}`;
                
              const email = searchResult.source === 'members'
                ? member.profile?.email || 'No Email'
                : member.info?.emails?.[0] || 'No Email';
                
              const memberId = searchResult.source === 'members'
                ? member._id
                : member.info?.memberId;
                
              return `
                <div class="member-match">
                  <div class="member-details">
                    <p><strong>Name:</strong> ${name}</p>
                    <p><strong>Email:</strong> ${email}</p>
                    <p><strong>Member ID:</strong> ${memberId || 'N/A'}</p>
                  </div>
                  <div class="plans-container" id="plans-${memberId}"></div>
                </div>
              `;
            }).join('')}
          </div>
        </div>
      `;
      
      // Add event listeners for the view plans buttons
      setTimeout(() => {
        const viewPlanButtons = document.querySelectorAll('.view-plans-btn');
        viewPlanButtons.forEach(button => {
          button.addEventListener('click', async function() {
            const memberId = this.getAttribute('data-member-id');
            const plansContainer = document.getElementById(`plans-${memberId}`);
            
            // Toggle the plans container
            if (plansContainer.style.display === 'none' || !plansContainer.style.display) {
              // Show loading state
              plansContainer.innerHTML = '<div class="loading">Loading plans...</div>';
              plansContainer.style.display = 'block';
              
              // Fetch the member's plans
              const plansResult = await window.scanidAPI.getMemberPlans(memberId);
              
              if (plansResult.success && plansResult.plans && plansResult.plans.length > 0) {
                // Display the plans
                plansContainer.innerHTML = `
                  <div class="plans-list">
                    ${plansResult.plans.map(plan => `
                      <div class="plan-item">
                        <p><strong>Plan:</strong> ${plan.name || 'Unnamed Plan'}</p>
                        <p><strong>Status:</strong> ${plan.status || 'Unknown'}</p>
                        <p><strong>Start Date:</strong> ${plan.startDate || 'N/A'}</p>
                        <p><strong>End Date:</strong> ${plan.endDate || 'N/A'}</p>
                      </div>
                    `).join('')}
                  </div>
                `;
              } else {
                plansContainer.innerHTML = '<div class="no-plans">No active plans found.</div>';
              }
            } else {
              plansContainer.style.display = 'none';
            }
          });
        });
      }, 100);
    } else {
      // Single match found
      const member = searchResult.results[0];
      const memberId = searchResult.source === 'members' ? member._id : member.info?.memberId;
      
      // Display member details
      accountDiv.innerHTML = `
        <div class="member-info">
          <h3>Member Found</h3>
          <div class="member-details">
            <p><strong>Name:</strong> ${searchResult.source === 'members' ? 
              `${member.profile?.firstName || ''} ${member.profile?.lastName || ''}` : 
              `${member.info?.name?.first || ''} ${member.info?.name?.last || ''}`}</p>
            <p><strong>Email:</strong> ${searchResult.source === 'members' ? 
              member.profile?.email || 'No Email' : 
              member.info?.emails?.[0] || 'No Email'}</p>
            <p><strong>Member ID:</strong> ${memberId || 'N/A'}</p>
          </div>
          <div class="member-actions">
            <button class="enroll-fingerprint-btn" data-member-id="${memberId}">Enroll Fingerprint</button>
          </div>
          <div class="plans-container" id="plans-${memberId}">
            <div class="loading">Loading plans...</div>
          </div>
        </div>
      `;
      
      // Fetch and display the member's plans
      const plansResult = await window.scanidAPI.getMemberPlans(memberId);
      const plansContainer = document.getElementById(`plans-${memberId}`);
      
      if (plansResult.success && plansResult.plans && plansResult.plans.length > 0) {
        plansContainer.innerHTML = `
          <h4>Active Plans</h4>
          <div class="plans-list">
            ${plansResult.plans.map(plan => `
              <div class="plan-item">
                <p><strong>Plan:</strong> ${plan.name || 'Unnamed Plan'}</p>
                <p><strong>Status:</strong> ${plan.status || 'Unknown'}</p>
                <p><strong>Start Date:</strong> ${plan.startDate || 'N/A'}</p>
                <p><strong>End Date:</strong> ${plan.endDate || 'N/A'}</p>
              </div>
            `).join('')}
          </div>
        `;
      } else {
        plansContainer.innerHTML = '<div class="no-plans">No active plans found.</div>';
      }
    }
  } catch (error) {
    console.error('Error processing scan:', error);
    resultDiv.innerHTML = `
      <div class="error">
        <h3>Error Processing Scan</h3>
        <p>${error.message || 'An unknown error occurred'}</p>
      </div>
    `;
  }
}

// Original scan button click handler
document.getElementById('scan-btn').addEventListener('click', async () => {
  // Simply call the processScan function with no arguments
  // This will fetch the latest scan and process it
  processScan();
});

// Helper function to handle fingerprint enrollment
async function handleFingerprintEnrollment(memberData) {
  try {
    // Show loading state
    const enrollBtn = document.querySelector(`.enroll-fingerprint-btn[data-member-id="${memberData.id}"]`);
    if (!enrollBtn) return;
    
    const originalText = enrollBtn.textContent;
    enrollBtn.disabled = true;
    enrollBtn.textContent = 'Initializing...';
    
    // Initialize the Anviz SDK if not already initialized
    const initResult = await window.anvizAPI.initialize();
    if (!initResult.success) {
      throw new Error(initResult.error || 'Failed to initialize Anviz SDK');
    }
    
    // Connect to the Anviz device
    enrollBtn.textContent = 'Connecting to device...';
    const connectResult = await window.anvizAPI.connectDevice();
    if (!connectResult.success) {
      throw new Error(connectResult.error || 'Failed to connect to Anviz device');
    }
    
    // Start fingerprint enrollment
    enrollBtn.textContent = 'Ready for fingerprint...';
    const enrollmentResult = await window.anvizAPI.startFingerEnrollment({
      userId: memberData.id,
      userName: memberData.name || 'Unknown User'
    });
    
    if (!enrollmentResult.success) {
      throw new Error(enrollmentResult.error || 'Fingerprint enrollment failed');
    }
    
    // Show success message
    enrollBtn.textContent = 'Enrollment Successful!';
    setTimeout(() => {
      enrollBtn.textContent = originalText;
      enrollBtn.disabled = false;
    }, 3000);
    
  } catch (error) {
    console.error('Fingerprint enrollment error:', error);
    alert(`Fingerprint enrollment failed: ${error.message}`);
    
    // Reset the button
    const enrollBtn = document.querySelector(`.enroll-fingerprint-btn[data-member-id="${memberData.id}"]`);
    if (enrollBtn) {
      enrollBtn.textContent = 'Enroll Fingerprint';
      enrollBtn.disabled = false;
    }
  }
}

// Add event listener for the Enroll Fingerprint buttons
document.addEventListener('click', function(event) {
  if (event.target && event.target.classList.contains('enroll-fingerprint-btn')) {
    const memberId = event.target.getAttribute('data-member-id');
    const memberName = event.target.getAttribute('data-member-name') || 'Unknown User';
    
    handleFingerprintEnrollment({
      id: memberId,
      name: memberName
    });
  }
});

// Add CSS for the ID photo
const style = document.createElement('style');
style.textContent = `
  .scan-photo {
    width: 200px;
    height: 250px;
    border: 1px solid #ddd;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #f5f5f5;
    margin-right: 20px;
    overflow: hidden;
  }
  
  .scan-photo .id-photo {
    max-width: 100%;
    max-height: 100%;
    object-fit: contain;
  }
  
  .photo-placeholder {
    color: #999;
    text-align: center;
    padding: 10px;
  }
`;
document.head.appendChild(style);
