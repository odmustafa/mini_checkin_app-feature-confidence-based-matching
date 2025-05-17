/**
 * AnvizService.js
 * 
 * Service for interacting with the Anviz C2 Pro device through a C# wrapper application.
 * This service follows the same pattern as other services in the application and
 * maintains compatibility with the Wix JavaScript SDK integration.
 * 
 * In development mode (on Mac), it simulates the Anviz SDK functionality.
 * In production mode (on Windows), it uses the actual C# wrapper to communicate with the Anviz SDK.
 */

const { spawn } = require('child_process');
const path = require('path');
const fs = require('fs');
const os = require('os');

// Detect platform
const isWindows = process.platform === 'win32';
const isDevelopment = !isWindows;

// Path to the C# wrapper executable (Windows only)
const ANVIZ_WRAPPER_PATH = isWindows ? path.join(__dirname, '../assets/anviz-wrapper/AnvizWrapper.exe') : null;

// Communication channels (Windows only)
const TEMP_DIR = os.tmpdir();
const REQUEST_FILE = path.join(TEMP_DIR, 'anviz_request.json');
const RESPONSE_FILE = path.join(TEMP_DIR, 'anviz_response.json');

// Simulation mode settings
let simulationMode = isDevelopment;
let simulatedDeviceConnected = false;
let simulatedDeviceInfo = null;
let simulatedEmployeeId = 10000; // Starting ID for simulated employees

class AnvizService {
  constructor() {
    this.initialized = false;
    this.deviceConnected = false;
    this.deviceInfo = null;
    this.wrapperProcess = null;
    this.callbackHandlers = {};
  }

  /**
   * Initialize the Anviz SDK
   * @returns {Promise<Object>} Result of the initialization
   */
  async initialize() {
    try {
      console.log('Initializing Anviz SDK...');
      
      if (simulationMode) {
        console.log('Running in simulation mode');
        this.initialized = true;
        return { success: true, message: 'Simulation mode initialized' };
      }
      
      const result = await this._executeCommand({
        command: 'Initialize'
      });
      
      this.initialized = result.success;
      return result;
    } catch (error) {
      console.error('Error initializing Anviz SDK:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Connect to an Anviz device
   * @param {Object} options Connection options (IP, port, etc.)
   * @returns {Promise<Object>} Connection result
   */
  async connectDevice(options = {}) {
    try {
      if (!this.initialized) {
        await this.initialize();
      }

      console.log('Connecting to Anviz device:', options);
      
      if (simulationMode) {
        // Simulate device connection in development mode
        simulatedDeviceConnected = true;
        simulatedDeviceInfo = {
          deviceId: 'C2Pro-SIM-123456',
          firmwareVersion: '2.5.2',
          model: 'C2 Pro (Simulated)'
        };
        
        this.deviceConnected = true;
        this.deviceInfo = simulatedDeviceInfo;
        
        return { 
          success: true, 
          deviceInfo: simulatedDeviceInfo,
          message: 'Connected to simulated device'
        };
      }
      
      const result = await this._executeCommand({
        command: 'ConnectDevice',
        params: options
      });
      
      if (result.success) {
        this.deviceConnected = true;
        this.deviceInfo = result.deviceInfo;
      }
      
      return result;
    } catch (error) {
      console.error('Error connecting to Anviz device:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Disconnect from the Anviz device
   * @returns {Promise<Object>} Disconnection result
   */
  async disconnectDevice() {
    try {
      if (!this.deviceConnected) {
        return { success: true, message: 'Device not connected' };
      }

      console.log('Disconnecting from Anviz device');
      
      if (simulationMode) {
        // Simulate device disconnection in development mode
        simulatedDeviceConnected = false;
        simulatedDeviceInfo = null;
        
        this.deviceConnected = false;
        this.deviceInfo = null;
        
        return { 
          success: true, 
          message: 'Disconnected from simulated device'
        };
      }
      
      const result = await this._executeCommand({
        command: 'DisconnectDevice'
      });
      
      if (result.success) {
        this.deviceConnected = false;
        this.deviceInfo = null;
      }
      
      return result;
    } catch (error) {
      console.error('Error disconnecting from Anviz device:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Add a new user to the Anviz device
   * @param {Object} userData User data to add
   * @returns {Promise<Object>} Result of the operation
   */
  async addUser(userData) {
    try {
      if (!this.deviceConnected) {
        return { success: false, error: 'Device not connected' };
      }

      console.log('Adding user to Anviz device:', userData);
      
      if (simulationMode) {
        // Simulate adding a user in development mode
        const userId = userData.userId || simulatedEmployeeId++;
        
        return { 
          success: true, 
          userId: userId,
          message: `User ${userData.name || 'Unknown'} added to simulated device with ID ${userId}`
        };
      }
      
      const result = await this._executeCommand({
        command: 'AddUser',
        params: userData
      });
      
      return result;
    } catch (error) {
      console.error('Error adding user to Anviz device:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Start fingerprint enrollment for a user
   * @param {Object} enrollmentData Enrollment data
   * @returns {Promise<Object>} Result of the operation
   */
  async startFingerEnrollment(enrollmentData) {
    try {
      if (!this.deviceConnected) {
        return { success: false, error: 'Device not connected' };
      }

      console.log('Starting fingerprint enrollment:', enrollmentData);
      
      if (simulationMode) {
        // Simulate fingerprint enrollment in development mode
        const userId = enrollmentData.userId;
        const fingerIndex = enrollmentData.fingerIndex || 0;
        
        // Simulate the enrollment process
        setTimeout(() => {
          // Simulate first scan event
          this._handleEvent({
            type: 'enrollmentProgress',
            data: { userId, fingerIndex, step: 1, message: 'Place finger on scanner' }
          });
          
          // Simulate second scan after 2 seconds
          setTimeout(() => {
            this._handleEvent({
              type: 'enrollmentProgress',
              data: { userId, fingerIndex, step: 2, message: 'Place finger on scanner again' }
            });
            
            // Simulate completion after another 2 seconds
            setTimeout(() => {
              this._handleEvent({
                type: 'enrollmentComplete',
                data: { userId, fingerIndex, success: true, message: 'Enrollment completed successfully' }
              });
            }, 2000);
          }, 2000);
        }, 1000);
        
        return { 
          success: true, 
          message: `Started fingerprint enrollment for user ${userId}, finger ${fingerIndex}`
        };
      }
      
      const result = await this._executeCommand({
        command: 'StartFingerEnrollment',
        params: enrollmentData
      });
      
      return result;
    } catch (error) {
      console.error('Error starting fingerprint enrollment:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Register a callback for device events
   * @param {string} eventType Event type to listen for
   * @param {Function} callback Callback function
   * @returns {Function} Function to unregister the callback
   */
  registerCallback(eventType, callback) {
    if (!this.callbackHandlers[eventType]) {
      this.callbackHandlers[eventType] = [];
    }
    
    this.callbackHandlers[eventType].push(callback);
    
    return () => {
      const index = this.callbackHandlers[eventType].indexOf(callback);
      if (index !== -1) {
        this.callbackHandlers[eventType].splice(index, 1);
      }
    };
  }

  /**
   * Start listening for events from the device
   * @returns {Promise<Object>} Result of the operation
   */
  async startEventListener() {
    try {
      if (!this.deviceConnected) {
        return { success: false, error: 'Device not connected' };
      }

      console.log('Starting event listener for Anviz device');
      
      if (simulationMode) {
        console.log('Event listener started in simulation mode');
        
        // Simulate a device ready event
        setTimeout(() => {
          this._handleEvent({
            type: 'deviceReady',
            data: { deviceId: simulatedDeviceInfo.deviceId }
          });
        }, 500);
        
        return { success: true, message: 'Event listener started in simulation mode' };
      }
      
      // Start the wrapper process in event listening mode
      this.wrapperProcess = spawn(ANVIZ_WRAPPER_PATH, ['--listen']);
      
      // Handle stdout for events
      this.wrapperProcess.stdout.on('data', (data) => {
        try {
          const event = JSON.parse(data.toString());
          this._handleEvent(event);
        } catch (error) {
          console.error('Error parsing event data:', error);
        }
      });
      
      // Handle errors
      this.wrapperProcess.on('error', (error) => {
        console.error('Error in wrapper process:', error);
      });
      
      // Handle process exit
      this.wrapperProcess.on('exit', (code) => {
        console.log(`Wrapper process exited with code ${code}`);
        this.wrapperProcess = null;
      });
      
      return { success: true };
    } catch (error) {
      console.error('Error starting event listener:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Stop the event listener
   * @returns {Promise<Object>} Result of the operation
   */
  async stopEventListener() {
    try {
      if (this.wrapperProcess) {
        this.wrapperProcess.kill();
        this.wrapperProcess = null;
      }
      
      return { success: true };
    } catch (error) {
      console.error('Error stopping event listener:', error);
      return { success: false, error: error.message };
    }
  }

  /**
   * Execute a command through the wrapper application
   * @private
   * @param {Object} commandData Command data
   * @returns {Promise<Object>} Command result
   */
  async _executeCommand(commandData) {
    // If in simulation mode, don't actually execute the command
    if (simulationMode) {
      console.log('Simulating command execution:', commandData.command);
      return Promise.resolve({ success: true, message: `Simulated ${commandData.command} command` });
    }
    
    return new Promise((resolve, reject) => {
      try {
        if (!isWindows) {
          reject(new Error('Cannot execute Anviz wrapper on non-Windows platform'));
          return;
        }
        
        // Write the command to the request file
        fs.writeFileSync(REQUEST_FILE, JSON.stringify(commandData));
        
        // Spawn the wrapper process
        const process = spawn(ANVIZ_WRAPPER_PATH, ['--execute']);
        
        // Wait for the process to exit
        process.on('exit', (code) => {
          if (code !== 0) {
            reject(new Error(`Wrapper process exited with code ${code}`));
            return;
          }
          
          try {
            // Read the response from the response file
            const responseData = JSON.parse(fs.readFileSync(RESPONSE_FILE, 'utf8'));
            resolve(responseData);
          } catch (error) {
            reject(error);
          }
        });
        
        // Handle errors
        process.on('error', (error) => {
          reject(error);
        });
      } catch (error) {
        reject(error);
      }
    });
  }

  /**
   * Handle an event from the device
   * @private
   * @param {Object} event Event data
   */
  _handleEvent(event) {
    const { type, data } = event;
    
    // Call the registered callbacks for this event type
    if (this.callbackHandlers[type]) {
      this.callbackHandlers[type].forEach(callback => {
        try {
          callback(data);
        } catch (error) {
          console.error('Error in event callback:', error);
        }
      });
    }
    
    // Call the registered callbacks for 'all' events
    if (this.callbackHandlers['all']) {
      this.callbackHandlers['all'].forEach(callback => {
        try {
          callback(event);
        } catch (error) {
          console.error('Error in event callback:', error);
        }
      });
    }
  }

  /**
   * Clean up resources
   */
  async cleanup() {
    await this.stopEventListener();
    await this.disconnectDevice();
  }
}

// Create a singleton instance
const anvizService = new AnvizService();

// Export the singleton
module.exports = anvizService;
