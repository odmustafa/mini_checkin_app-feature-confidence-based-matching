/**
 * Anviz Integration with Member Check-In
 * 
 * This file handles the integration between the Member Check-In functionality
 * and the Anviz fingerprint enrollment system.
 */

// Global state to store the selected member data
let selectedMemberData = null;

// Function to set the selected member data
function setSelectedMember(memberData) {
  selectedMemberData = memberData;
  
  // Store the data in localStorage for persistence between pages
  if (memberData) {
    localStorage.setItem('selectedMemberData', JSON.stringify(memberData));
  } else {
    localStorage.removeItem('selectedMemberData');
  }
  
  // Update UI if we're on the Anviz page
  updateAnvizUI();
}

// Function to get the selected member data
function getSelectedMember() {
  // If we don't have data in memory, try to get it from localStorage
  if (!selectedMemberData) {
    const storedData = localStorage.getItem('selectedMemberData');
    if (storedData) {
      try {
        selectedMemberData = JSON.parse(storedData);
      } catch (e) {
        console.error('Error parsing stored member data:', e);
      }
    }
  }
  
  return selectedMemberData;
}

// Function to update the Anviz UI with the selected member data
function updateAnvizUI() {
  // Only proceed if we're on the Anviz page
  if (!window.location.pathname.includes('anviz')) return;
  
  // Get references to all possible input fields on the Anviz page
  const firstNameInput = document.getElementById('first-name');
  const lastNameInput = document.getElementById('last-name');
  const employeeIdInput = document.getElementById('employee-id');
  const enrollEmployeeIdInput = document.getElementById('enroll-employee-id');
  
  // If none of these fields exist, we're not on the right page
  if (!firstNameInput && !lastNameInput && !employeeIdInput) return;
  
  const memberData = getSelectedMember();
  if (!memberData) return;
  
  console.log('Updating Anviz UI with member data:', memberData);
  
  // Extract first and last name based on the available data
  let firstName = '';
  let lastName = '';
  let memberId = memberData.memberId || '';
  
  // Extract from contact data (from Wix Contacts API)
  if (memberData.contact) {
    const contact = memberData.contact;
    if (contact.info && contact.info.name) {
      firstName = contact.info.name.first || '';
      lastName = contact.info.name.last || '';
    }
  }
  // Extract from member data (from Wix Members API)
  else if (memberData.member) {
    const member = memberData.member;
    if (member.contactDetails) {
      firstName = member.contactDetails.firstName || '';
      lastName = member.contactDetails.lastName || '';
    } else if (member.profile && member.profile.nickname) {
      // If only nickname is available, use it as first name
      firstName = member.profile.nickname || '';
    }
  }
  // Fall back to scan data if available
  else if (memberData.scan) {
    const scan = memberData.scan;
    firstName = scan.FirstName || '';
    lastName = scan.LastName || '';
  }
  
  // Update the input fields if they exist
  if (firstNameInput) firstNameInput.value = firstName;
  if (lastNameInput) lastNameInput.value = lastName;
  
  // Update ID fields
  if (employeeIdInput && memberId) employeeIdInput.value = memberId;
  if (enrollEmployeeIdInput && memberId) enrollEmployeeIdInput.value = memberId;
  
  // Log the update for debugging
  console.log(`Updated Anviz UI with: ${firstName} ${lastName} (ID: ${memberId || 'Not set'})`);
}

// Function to create a button to enroll the selected member
function addEnrollFingerprintButton(memberElement, memberData) {
  // Check if button already exists
  if (memberElement.querySelector('.enroll-fingerprint-btn')) {
    return;
  }
  
  // Create the button
  const enrollButton = document.createElement('button');
  enrollButton.className = 'enroll-fingerprint-btn';
  enrollButton.textContent = 'Enroll Fingerprint';
  enrollButton.addEventListener('click', (event) => {
    event.preventDefault();
    event.stopPropagation();
    
    // Store the member data
    setSelectedMember(memberData);
    
    // Navigate to the Anviz page
    window.location.href = 'anviz.html';
  });
  
  // Add the button to the member element
  memberElement.appendChild(enrollButton);
}

// Initialize the integration
document.addEventListener('DOMContentLoaded', () => {
  // If we're on the Anviz page, update the UI with any stored member data
  if (window.location.pathname.includes('anviz')) {
    updateAnvizUI();
  }
  
  // If we're on the check-in page, add event listeners for member selection
  if (window.location.pathname.includes('checkin.html') || window.location.pathname.endsWith('/')) {
    // We'll hook into the account-info div which contains member results
    const accountInfo = document.getElementById('account-info');
    if (accountInfo) {
      // Use a MutationObserver to detect when member results are added
      const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
          if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
            // Add event listeners to the Enroll Fingerprint buttons that already exist in the HTML
            const enrollButtons = accountInfo.querySelectorAll('.enroll-fingerprint-btn');
            
            enrollButtons.forEach((button) => {
              // Skip if we've already added a listener
              if (button.getAttribute('data-listener-added')) return;
              
              // Mark that we've added a listener to this button
              button.setAttribute('data-listener-added', 'true');
              
              button.addEventListener('click', function() {
                // Get the member card element
                const memberCard = this.closest('.member-card');
                if (!memberCard) return;
                
                // Get data attributes
                const memberId = memberCard.getAttribute('data-member-id');
                const source = memberCard.getAttribute('data-source');
                const index = parseInt(memberCard.getAttribute('data-index'), 10);
                
                // Get the name from the header
                const nameElement = memberCard.querySelector('.member-card-header h4');
                const name = nameElement ? nameElement.textContent : '';
                
                // Get the email from the details
                const emailElement = memberCard.querySelector('.member-card-details p:first-child');
                const email = emailElement ? emailElement.textContent.replace('Email:', '').trim() : '';
                
                // Create member data object based on the source
                let memberData = {
                  memberId: memberId,
                  source: source,
                  index: index
                };
                
                // Add scan data if available (from global scope)
                if (window.currentScan) {
                  memberData.scan = window.currentScan;
                }
                
                // Add contact or member data based on the source
                if (source === 'contacts') {
                  memberData.contact = {
                    info: {
                      name: {
                        full: name,
                        first: name.split(' ')[0],
                        last: name.split(' ').slice(1).join(' ')
                      },
                      email: email,
                      memberId: memberId
                    }
                  };
                } else {
                  memberData.member = {
                    _id: memberId,
                    contactDetails: {
                      firstName: name.split(' ')[0],
                      lastName: name.split(' ').slice(1).join(' '),
                      email: email
                    }
                  };
                }
                
                // Store the member data
                setSelectedMember(memberData);
                
                // Navigate to the Anviz page
                window.location.href = 'anviz.html';
              });
            });
          }
        });
      });
      
      // Start observing the account-info div
      observer.observe(accountInfo, { childList: true, subtree: true });
    }
  }
});

// Export functions for use in other modules
window.anvizIntegration = {
  setSelectedMember,
  getSelectedMember,
  updateAnvizUI,
  addEnrollFingerprintButton
};
