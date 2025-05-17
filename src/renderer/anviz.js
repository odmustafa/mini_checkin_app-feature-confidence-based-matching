/**
 * Anviz Fingerprint Enrollment and Check-in functionality
 * 
 * This file handles the UI interactions for the Anviz C2 Pro device integration.
 * It uses the anvizAPI exposed by the preload script to communicate with the main process.
 */

document.addEventListener('DOMContentLoaded', () => {
  // UI Elements
  const initializeBtn = document.getElementById('initialize-btn');
  const connectBtn = document.getElementById('connect-btn');
  const disconnectBtn = document.getElementById('disconnect-btn');
  const connectionStatus = document.getElementById('connection-status');
  
  const deviceIpInput = document.getElementById('device-ip');
  const devicePortInput = document.getElementById('device-port');
  
  const addUserBtn = document.getElementById('add-user-btn');
  const employeeIdInput = document.getElementById('employee-id');
  const firstNameInput = document.getElementById('first-name');
  const lastNameInput = document.getElementById('last-name');
  
  const startEnrollmentBtn = document.getElementById('start-enrollment-btn');
  const enrollEmployeeIdInput = document.getElementById('enroll-employee-id');
  const fingerIndexSelect = document.getElementById('finger-index');
  const enrollmentStatus = document.getElementById('enrollment-status');
  
  const startListenerBtn = document.getElementById('start-listener-btn');
  const stopListenerBtn = document.getElementById('stop-listener-btn');
  const eventLogContent = document.getElementById('event-log-content');
  
  // State variables
  let sdkInitialized = false;
  let deviceConnected = false;
  let listenerActive = false;
  let eventListenerCleanup = null;
  
  // Initialize the Anviz SDK
  initializeBtn.addEventListener('click', async () => {
    try {
      updateStatus(connectionStatus, 'Initializing Anviz SDK...');
      
      const result = await window.anvizAPI.initialize();
      
      if (result.success) {
        sdkInitialized = true;
        updateStatus(connectionStatus, 'SDK initialized successfully');
        connectBtn.disabled = false;
      } else {
        updateStatus(connectionStatus, `Failed to initialize SDK: ${result.error}`, true);
      }
    } catch (error) {
      updateStatus(connectionStatus, `Error initializing SDK: ${error.message}`, true);
    }
  });
  
  // Connect to the Anviz device
  connectBtn.addEventListener('click', async () => {
    try {
      if (!sdkInitialized) {
        updateStatus(connectionStatus, 'Please initialize the SDK first', true);
        return;
      }
      
      const ip = deviceIpInput.value.trim();
      const port = parseInt(devicePortInput.value.trim(), 10);
      
      if (!ip) {
        updateStatus(connectionStatus, 'Please enter a valid IP address', true);
        return;
      }
      
      updateStatus(connectionStatus, `Connecting to device at ${ip}:${port}...`);
      
      const result = await window.anvizAPI.connectDevice({ ip, port });
      
      if (result.success) {
        deviceConnected = true;
        updateStatus(connectionStatus, `Connected to ${result.deviceInfo.model} (${result.deviceInfo.deviceId})`);
        
        // Enable device-specific buttons
        disconnectBtn.disabled = false;
        addUserBtn.disabled = false;
        startEnrollmentBtn.disabled = false;
        startListenerBtn.disabled = false;
        
        // Disable connect button
        connectBtn.disabled = true;
      } else {
        updateStatus(connectionStatus, `Failed to connect: ${result.error}`, true);
      }
    } catch (error) {
      updateStatus(connectionStatus, `Error connecting to device: ${error.message}`, true);
    }
  });
  
  // Disconnect from the Anviz device
  disconnectBtn.addEventListener('click', async () => {
    try {
      updateStatus(connectionStatus, 'Disconnecting from device...');
      
      const result = await window.anvizAPI.disconnectDevice();
      
      if (result.success) {
        deviceConnected = false;
        updateStatus(connectionStatus, 'Disconnected from device');
        
        // Disable device-specific buttons
        disconnectBtn.disabled = true;
        addUserBtn.disabled = true;
        startEnrollmentBtn.disabled = true;
        startListenerBtn.disabled = true;
        
        // Stop the event listener if active
        if (listenerActive) {
          await stopEventListener();
        }
        
        // Enable connect button
        connectBtn.disabled = false;
      } else {
        updateStatus(connectionStatus, `Failed to disconnect: ${result.error}`, true);
      }
    } catch (error) {
      updateStatus(connectionStatus, `Error disconnecting from device: ${error.message}`, true);
    }
  });
  
  // Add a user to the Anviz device
  addUserBtn.addEventListener('click', async () => {
    try {
      if (!deviceConnected) {
        updateStatus(connectionStatus, 'Please connect to the device first', true);
        return;
      }
      
      const employeeId = employeeIdInput.value.trim();
      const firstName = firstNameInput.value.trim();
      const lastName = lastNameInput.value.trim();
      
      if (!employeeId) {
        updateStatus(connectionStatus, 'Please enter an employee ID', true);
        return;
      }
      
      updateStatus(connectionStatus, `Adding user ${firstName} ${lastName} (ID: ${employeeId})...`);
      
      const result = await window.anvizAPI.addUser({
        employeeId,
        firstName,
        lastName
      });
      
      if (result.success) {
        updateStatus(connectionStatus, `User added successfully with ID: ${result.employeeId}`);
        
        // Auto-fill the enrollment employee ID
        enrollEmployeeIdInput.value = result.employeeId;
      } else {
        updateStatus(connectionStatus, `Failed to add user: ${result.error}`, true);
      }
    } catch (error) {
      updateStatus(connectionStatus, `Error adding user: ${error.message}`, true);
    }
  });
  
  // Start fingerprint enrollment
  startEnrollmentBtn.addEventListener('click', async () => {
    try {
      if (!deviceConnected) {
        updateStatus(enrollmentStatus, 'Please connect to the device first', true);
        return;
      }
      
      const employeeId = enrollEmployeeIdInput.value.trim();
      const fingerIndex = parseInt(fingerIndexSelect.value, 10);
      
      if (!employeeId) {
        updateStatus(enrollmentStatus, 'Please enter an employee ID', true);
        return;
      }
      
      updateStatus(enrollmentStatus, `Starting fingerprint enrollment for user ID: ${employeeId}, finger index: ${fingerIndex}...`);
      
      const result = await window.anvizAPI.startFingerEnrollment({
        employeeId,
        fingerIndex
      });
      
      if (result.success) {
        updateStatus(enrollmentStatus, result.message);
      } else {
        updateStatus(enrollmentStatus, `Failed to start enrollment: ${result.error}`, true);
      }
    } catch (error) {
      updateStatus(enrollmentStatus, `Error starting enrollment: ${error.message}`, true);
    }
  });
  
  // Start the event listener
  startListenerBtn.addEventListener('click', async () => {
    try {
      if (!deviceConnected) {
        addEventLog('Please connect to the device first', true);
        return;
      }
      
      if (listenerActive) {
        addEventLog('Event listener is already active');
        return;
      }
      
      addEventLog('Starting event listener...');
      
      const result = await window.anvizAPI.startEventListener();
      
      if (result.success) {
        listenerActive = true;
        addEventLog('Event listener started successfully');
        
        // Register event listener
        eventListenerCleanup = window.anvizAPI.onAnvizEvent((event) => {
          addEventLog(`Event received: ${event.type}`);
          addEventLog(`Data: ${JSON.stringify(event.data)}`);
        });
        
        // Update UI
        startListenerBtn.disabled = true;
        stopListenerBtn.disabled = false;
      } else {
        addEventLog(`Failed to start event listener: ${result.error}`, true);
      }
    } catch (error) {
      addEventLog(`Error starting event listener: ${error.message}`, true);
    }
  });
  
  // Stop the event listener
  stopListenerBtn.addEventListener('click', async () => {
    await stopEventListener();
  });
  
  // Helper function to stop the event listener
  async function stopEventListener() {
    try {
      if (!listenerActive) {
        addEventLog('Event listener is not active');
        return;
      }
      
      addEventLog('Stopping event listener...');
      
      const result = await window.anvizAPI.stopEventListener();
      
      if (result.success) {
        listenerActive = false;
        addEventLog('Event listener stopped successfully');
        
        // Remove event listener
        if (eventListenerCleanup) {
          eventListenerCleanup();
          eventListenerCleanup = null;
        }
        
        // Update UI
        startListenerBtn.disabled = false;
        stopListenerBtn.disabled = true;
      } else {
        addEventLog(`Failed to stop event listener: ${result.error}`, true);
      }
    } catch (error) {
      addEventLog(`Error stopping event listener: ${error.message}`, true);
    }
  }
  
  // Helper function to update status elements
  function updateStatus(element, message, isError = false) {
    if (element) {
      element.innerHTML = `<p class="${isError ? 'error' : 'success'}">${message}</p>`;
    }
    
    // Also log to event log if it's important
    if (isError || message.includes('success')) {
      addEventLog(message, isError);
    }
  }
  
  // Helper function to add entries to the event log
  function addEventLog(message, isError = false) {
    const timestamp = new Date().toLocaleTimeString();
    const logEntry = document.createElement('div');
    logEntry.className = `log-entry ${isError ? 'error' : ''}`;
    logEntry.innerHTML = `<span class="timestamp">${timestamp}</span> ${message}`;
    
    eventLogContent.appendChild(logEntry);
    eventLogContent.scrollTop = eventLogContent.scrollHeight;
  }
  
  // Add a link to the Anviz tab in the main index.html
  function addAnvizTabToMainPage() {
    // This function would ideally be in app.js, but we're adding it here for simplicity
    // In a real implementation, you would modify index.html directly
    console.log('Adding Anviz tab to main page');
  }
  
  // Initialize the page
  function init() {
    // Disable buttons initially
    connectBtn.disabled = false;
    disconnectBtn.disabled = true;
    addUserBtn.disabled = true;
    startEnrollmentBtn.disabled = true;
    startListenerBtn.disabled = true;
    stopListenerBtn.disabled = true;
    
    // Add initial event log entry
    addEventLog('Anviz Fingerprint Enrollment page loaded');
    
    // Check if we have member data from the integration
    if (window.anvizIntegration) {
      const memberData = window.anvizIntegration.getSelectedMember();
      if (memberData) {
        addEventLog('Member data found from Check-In page');
        
        // Populate the user form with the member data
        let fullName = '';
        let memberId = '';
        
        // Extract data based on what's available
        if (memberData.contact) {
          const contact = memberData.contact;
          if (contact.info && contact.info.name) {
            if (contact.info.name.first) {
              firstNameInput.value = contact.info.name.first;
            }
            if (contact.info.name.last) {
              lastNameInput.value = contact.info.name.last;
            }
            fullName = contact.info.name.full || `${firstNameInput.value} ${lastNameInput.value}`.trim();
          }
          
          // Use the member ID if available
          if (memberData.memberId) {
            memberId = memberData.memberId;
            employeeIdInput.value = memberId;
            enrollEmployeeIdInput.value = memberId;
          }
          
          addEventLog(`Populated form with member data: ${fullName} (ID: ${memberId || 'Not set'})`);
        } 
        // If we have scan data but no contact data
        else if (memberData.scan) {
          const scan = memberData.scan;
          if (scan.FirstName) {
            firstNameInput.value = scan.FirstName;
          }
          if (scan.LastName) {
            lastNameInput.value = scan.LastName;
          }
          
          // Generate a member ID if none is available
          if (memberData.memberId) {
            memberId = memberData.memberId;
          } else {
            // Create a simple ID based on the name
            memberId = `${Date.now().toString().slice(-5)}`;
          }
          
          employeeIdInput.value = memberId;
          enrollEmployeeIdInput.value = memberId;
          
          fullName = scan.FullName || `${firstNameInput.value} ${lastNameInput.value}`.trim();
          addEventLog(`Populated form with scan data: ${fullName} (ID: ${memberId})`);
        }
      }
    }
  }
  
  // Call init function
  init();
});
