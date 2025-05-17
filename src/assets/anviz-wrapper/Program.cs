using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace AnvizWrapper
{
    /// <summary>
    /// AnvizWrapper - A command-line application that acts as a bridge between Node.js and the Anviz SDK
    /// </summary>
    class Program
    {
        // Constants for file paths
        private static readonly string TempDir = Path.GetTempPath();
        private static readonly string RequestFile = Path.Combine(TempDir, "anviz_request.json");
        private static readonly string ResponseFile = Path.Combine(TempDir, "anviz_response.json");

        // Anviz SDK handle
        private static IntPtr anvizHandle = IntPtr.Zero;
        private static bool isListening = false;
        private static Dictionary<int, int> deviceTypeFlags = new Dictionary<int, int>();

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Usage: AnvizWrapper.exe [--execute|--listen]");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "--execute":
                        ExecuteCommand();
                        break;
                    case "--listen":
                        StartListening();
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {args[0]}");
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteResponse(new { success = false, error = ex.Message });
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Execute a command from the request file and write the result to the response file
        /// </summary>
        private static void ExecuteCommand()
        {
            if (!File.Exists(RequestFile))
            {
                throw new FileNotFoundException("Request file not found", RequestFile);
            }

            string requestJson = File.ReadAllText(RequestFile);
            var request = JsonSerializer.Deserialize<CommandRequest>(requestJson);

            if (request == null)
            {
                throw new InvalidOperationException("Invalid request format");
            }

            object result;

            switch (request.Command.ToLower())
            {
                case "initialize":
                    result = Initialize();
                    break;
                case "connectdevice":
                    result = ConnectDevice(request.Params);
                    break;
                case "disconnectdevice":
                    result = DisconnectDevice();
                    break;
                case "adduser":
                    result = AddUser(request.Params);
                    break;
                case "startfingerenrollment":
                    result = StartFingerEnrollment(request.Params);
                    break;
                default:
                    result = new { success = false, error = $"Unknown command: {request.Command}" };
                    break;
            }

            WriteResponse(result);
        }

        /// <summary>
        /// Start listening for events from the Anviz device
        /// </summary>
        private static void StartListening()
        {
            // Initialize the SDK if not already initialized
            if (anvizHandle == IntPtr.Zero)
            {
                var initResult = Initialize();
                if (!(bool)initResult.GetType().GetProperty("success").GetValue(initResult))
                {
                    Console.Error.WriteLine("Failed to initialize Anviz SDK");
                    Environment.Exit(1);
                }
            }

            isListening = true;

            // Main event loop
            while (isListening)
            {
                try
                {
                    // Call CChex_Update to process events
                    // In a real implementation, this would handle events from the Anviz device
                    // For now, we'll just simulate events for demonstration purposes
                    
                    // Simulate a fingerprint scan event every 5 seconds
                    Thread.Sleep(5000);
                    
                    // Create a simulated event
                    var eventData = new
                    {
                        type = "fingerprint_scan",
                        data = new
                        {
                            employeeId = "12345",
                            fingerprintIndex = 0,
                            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    };
                    
                    // Output the event as JSON to stdout
                    Console.WriteLine(JsonSerializer.Serialize(eventData));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error in event loop: {ex.Message}");
                }
            }
        }

        #region Anviz SDK Commands

        /// <summary>
        /// Initialize the Anviz SDK
        /// </summary>
        private static object Initialize()
        {
            try
            {
                // In a real implementation, this would call AnvizNew.CChex_Init() and AnvizNew.CChex_Start()
                // For now, we'll just simulate the initialization
                
                // Simulate successful initialization
                anvizHandle = new IntPtr(1); // Dummy handle
                
                return new { success = true };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Connect to an Anviz device
        /// </summary>
        private static object ConnectDevice(JsonElement? paramsObj)
        {
            try
            {
                if (anvizHandle == IntPtr.Zero)
                {
                    return new { success = false, error = "Anviz SDK not initialized" };
                }

                // Extract connection parameters
                string ip = "192.168.1.100"; // Default IP
                int port = 5010; // Default port
                
                if (paramsObj.HasValue)
                {
                    if (paramsObj.Value.TryGetProperty("ip", out JsonElement ipElement))
                    {
                        ip = ipElement.GetString();
                    }
                    
                    if (paramsObj.Value.TryGetProperty("port", out JsonElement portElement))
                    {
                        port = portElement.GetInt32();
                    }
                }

                // In a real implementation, this would call AnvizNew.CChex_Connect to connect to the device
                // For now, we'll just simulate a successful connection
                
                // Simulate device info
                var deviceInfo = new
                {
                    deviceId = "C2Pro123456",
                    firmwareVersion = "2.5.2",
                    model = "C2 Pro"
                };
                
                return new { success = true, deviceInfo };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Disconnect from the Anviz device
        /// </summary>
        private static object DisconnectDevice()
        {
            try
            {
                // In a real implementation, this would call AnvizNew.CChex_Disconnect
                // For now, we'll just simulate a successful disconnection
                
                return new { success = true };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Add a new user to the Anviz device
        /// </summary>
        private static object AddUser(JsonElement? paramsObj)
        {
            try
            {
                if (anvizHandle == IntPtr.Zero)
                {
                    return new { success = false, error = "Anviz SDK not initialized" };
                }

                // Extract user data
                string firstName = "";
                string lastName = "";
                string employeeId = "";
                
                if (paramsObj.HasValue)
                {
                    if (paramsObj.Value.TryGetProperty("firstName", out JsonElement firstNameElement))
                    {
                        firstName = firstNameElement.GetString();
                    }
                    
                    if (paramsObj.Value.TryGetProperty("lastName", out JsonElement lastNameElement))
                    {
                        lastName = lastNameElement.GetString();
                    }
                    
                    if (paramsObj.Value.TryGetProperty("employeeId", out JsonElement employeeIdElement))
                    {
                        employeeId = employeeIdElement.GetString();
                    }
                }

                // In a real implementation, this would call AnvizNew.CChex_ModifyPersonInfo to add the user
                // For now, we'll just simulate a successful user addition
                
                return new { success = true, employeeId = employeeId ?? "12345" };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Start fingerprint enrollment for a user
        /// </summary>
        private static object StartFingerEnrollment(JsonElement? paramsObj)
        {
            try
            {
                if (anvizHandle == IntPtr.Zero)
                {
                    return new { success = false, error = "Anviz SDK not initialized" };
                }

                // Extract enrollment data
                string employeeId = "";
                int fingerIndex = 0;
                
                if (paramsObj.HasValue)
                {
                    if (paramsObj.Value.TryGetProperty("employeeId", out JsonElement employeeIdElement))
                    {
                        employeeId = employeeIdElement.GetString();
                    }
                    
                    if (paramsObj.Value.TryGetProperty("fingerIndex", out JsonElement fingerIndexElement))
                    {
                        fingerIndex = fingerIndexElement.GetInt32();
                    }
                }

                // In a real implementation, this would call AnvizNew.CCHex_AddFingerprintOnline
                // For now, we'll just simulate a successful enrollment start
                
                return new { success = true, message = "Enrollment started. Please place finger on the sensor." };
            }
            catch (Exception ex)
            {
                return new { success = false, error = ex.Message };
            }
        }

        #endregion

        /// <summary>
        /// Write a response object to the response file as JSON
        /// </summary>
        private static void WriteResponse(object response)
        {
            string responseJson = JsonSerializer.Serialize(response);
            File.WriteAllText(ResponseFile, responseJson);
        }
    }

    /// <summary>
    /// Represents a command request from Node.js
    /// </summary>
    class CommandRequest
    {
        public string Command { get; set; }
        public JsonElement? Params { get; set; }
    }
}
