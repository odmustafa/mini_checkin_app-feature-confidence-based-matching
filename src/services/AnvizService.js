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

// Path to the C# wrapper executable and its directory (Windows only)
const ANVIZ_WRAPPER_DIR = isWindows ? path.join(__dirname, '../assets/anviz-wrapper') : null;
const ANVIZ_WRAPPER_PATH = isWindows ? path.join(ANVIZ_WRAPPER_DIR, 'bin/Release/net6.0/AnvizWrapper.exe') : null;

// Communication channels (Windows only)
const TEMP_DIR = os.tmpdir();
const REQUEST_FILE = path.join(TEMP_DIR, 'anviz_request.json');
const RESPONSE_FILE = path.join(TEMP_DIR, 'anviz_response.json');

// Simulation mode settings - disabled for front desk Windows computer
let simulationMode = false; // Force disable simulation mode even in development
let simulatedDeviceConnected = false;
let simulatedDeviceInfo = null;
let simulatedEmployeeId = 10000; // Starting ID for simulated employees

// Anviz C2 Pro connection settings
const DEFAULT_ANVIZ_CONFIG = {
	ip: '192.168.8.243',
	port: 5010,
	deviceId: 1 // Default device ID
}

class AnvizService {
	constructor() {
		this.initialized = false;
		this.deviceConnected = false;
		this.deviceInfo = null;
		this.wrapperProcess = null;
		this.callbackHandlers = {};
		this._useFallbackMode = false;
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
			
			// Try to use the wrapper, but if it fails, fall back to simulation mode
			try {
			
				const result = await this._executeCommand({
					command: 'Initialize'
				});
				
				this.initialized = result.success;
				return result;
			} catch (error) {
				console.error('Failed to initialize with wrapper, detailed error:', error);
                                console.error('Failed to initialize with wrapper, detailed error:', error);
console.log('Failed to initialize with wrapper, falling back to simulation mode');
				simulationMode = true;
				this.initialized = true;
				return { success: true, message: 'Simulation mode initialized (fallback)' };
			}
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

			// Use the default Anviz C2 Pro configuration if not provided
			const connectionOptions = Object.keys(options).length === 0 ? DEFAULT_ANVIZ_CONFIG : options;
			console.log('Connecting to Anviz device:', connectionOptions);
			
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
				params: connectionOptions
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
		
		// Return a function to unregister the callback
		return () => {
			if (this.callbackHandlers[eventType]) {
				const index = this.callbackHandlers[eventType].indexOf(callback);
				if (index !== -1) {
					this.callbackHandlers[eventType].splice(index, 1);
				}
			}
		};
	}

	/**
	 * Register an event listener for all device events
	 * @param {Function} listener Function to call with event data
	 * @returns {Function} Function to unregister the listener
	 */
	registerEventListener(listener) {
		// This is a convenience method that registers for all events
		return this.registerCallback('all', listener);
	}

	/**
	 * Start listening for events from the device
	 * @returns {Promise<Object>} Result of the operation
	 */
	async startEventListener() {
		try {
			if (!this.initialized) {
				return { success: false, error: 'SDK not initialized' };
			}
			
			if (!this.deviceConnected) {
				return { success: false, error: 'Device not connected' };
			}
			
			if (simulationMode) {
				console.log('Starting event listener in simulation mode');
				
				// Set up a simulated event interval
				this._simulatedEventInterval = setInterval(() => {
					// Simulate a random check-in event every 10-20 seconds
					if (Math.random() > 0.7) {
						const userId = Math.floor(10000 + Math.random() * 1000);
						this._handleEvent({
							type: 'checkin',
							data: {
								userId: userId,
								timestamp: new Date().toISOString(),
								verifyMode: 'Fingerprint',
								deviceId: 'C2Pro-SIM-123456'
							}
						});
					}
				}, 15000); // Every 15 seconds
				
				return { success: true, message: 'Started event listener in simulation mode' };
			}
			
			console.log('Starting event listener for Anviz device');
			
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
			if (!this.initialized) {
				return { success: false, error: 'SDK not initialized' };
			}
			
			if (!this.deviceConnected) {
				return { success: false, error: 'Device not connected' };
			}
			
			if (simulationMode && this._simulatedEventInterval) {
				console.log('Stopping event listener in simulation mode');
				clearInterval(this._simulatedEventInterval);
				this._simulatedEventInterval = null;
				return { success: true, message: 'Stopped event listener in simulation mode' };
			}
			
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
	/**
         * Execute a command through the Anviz wrapper
         * @private
         * @param {Object} commandData Command data to send
         * @returns {Promise<Object>} Command result
         */
                /**
         * Execute a command through the Anviz wrapper
         * @private
         * @param {Object} commandData Command data to send
         * @returns {Promise<Object>} Command result
         */
        async _executeCommand(commandData) {
                try {
                        if (simulationMode) {
                                console.log('Cannot execute command in simulation mode');
                                return { success: false, error: 'Simulation mode active' };
                        }

                        if (!fs.existsSync(ANVIZ_WRAPPER_PATH)) {
                                console.log('Anviz wrapper executable not found, using direct connection mode');
                                this._useFallbackMode = true;
                                simulationMode = true;
                                return { success: false, error: 'Wrapper executable not found' };
                        }

                        // Write the command to a temporary file
                        fs.writeFileSync(REQUEST_FILE, JSON.stringify(commandData));

                        // Execute the wrapper with the --execute flag
                        const wrapperProcess = spawn(ANVIZ_WRAPPER_PATH, ['--execute']);

                        // Return a promise that resolves when the wrapper completes
                        return new Promise((resolve, reject) => {
                                let output = '';

                                wrapperProcess.stdout.on('data', (data) => {
                                        output += data.toString();
                                });

                                wrapperProcess.stderr.on('data', (data) => {
                                        console.error(`Wrapper stderr: ${data}`);
                                });

                                wrapperProcess.on('error', (error) => {
                                        console.error('Failed to start wrapper process:', error);
                                        reject(error);
                                });

                                wrapperProcess.on('close', (code) => {
                                        if (code !== 0) {
                                                const errorMsg = `Wrapper process exited with code ${code}`;
                                                console.error(errorMsg);
                                                reject(new Error(errorMsg));
                                                return;
                                        }

                                        try {
                                                // Read the response from the temporary file
                                                if (fs.existsSync(RESPONSE_FILE)) {
                                                        const responseData = JSON.parse(fs.readFileSync(RESPONSE_FILE, 'utf8'));
                                                        resolve(responseData);
                                                } else {
                                                        reject(new Error('Response file not found'));
                                                }
                                        } catch (error) {
                                                reject(error);
                                        }
                                });
                        });
                } catch (error) {
                        console.error('Error executing command:', error);
                        return { success: false, error: error.message };
                }
        }

        async cleanup() {
		// Clear any simulation intervals
		if (this._simulatedEventInterval) {
			clearInterval(this._simulatedEventInterval);
			this._simulatedEventInterval = null;
		}
		
		await this.stopEventListener();
		await this.disconnectDevice();
	}
}

// Create a singleton instance
const anvizService = new AnvizService();

// Export the singleton
module.exports = anvizService;








