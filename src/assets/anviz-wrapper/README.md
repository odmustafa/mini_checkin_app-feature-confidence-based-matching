# Anviz C2 Pro SDK Wrapper

This is a C# wrapper application that bridges the Node.js/Electron application with the Anviz C2 Pro SDK. It provides a simple command-line interface that can be called from the Node.js application to interact with the Anviz C2 Pro device.

## How It Works

The wrapper uses a file-based communication mechanism:

1. The Node.js application writes a command and parameters to a JSON file in the system's temp directory.
2. The wrapper reads the command file, executes the corresponding Anviz SDK function, and writes the result to a response file.
3. The Node.js application reads the response file to get the result.

For event listening (e.g., fingerprint scans), the wrapper runs in a continuous loop and outputs events as JSON to stdout, which the Node.js application can read.

## Building the Wrapper

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or Visual Studio Code with C# extension

### Build Steps

1. Copy the Anviz SDK DLL (`tc-b_new_sdk.dll`) to the wrapper directory.
2. Build the wrapper:

```bash
cd src/assets/anviz-wrapper
dotnet build -c Release
```

3. The compiled executable will be in the `bin/Release/net6.0` directory.

## Usage

The wrapper supports two modes:

### Execute Mode

Executes a single command and exits:

```bash
AnvizWrapper.exe --execute
```

It reads the command from `%TEMP%/anviz_request.json` and writes the result to `%TEMP%/anviz_response.json`.

### Listen Mode

Starts a continuous loop to listen for events from the Anviz device:

```bash
AnvizWrapper.exe --listen
```

It outputs events as JSON to stdout, which can be read by the Node.js application.

## Supported Commands

- `Initialize`: Initialize the Anviz SDK
- `ConnectDevice`: Connect to an Anviz device
- `DisconnectDevice`: Disconnect from the Anviz device
- `AddUser`: Add a new user to the Anviz device
- `StartFingerEnrollment`: Start fingerprint enrollment for a user

## Integration with Node.js

The wrapper is designed to be called from the `AnvizService.js` in the Node.js application. The service handles the communication with the wrapper and provides a JavaScript API for the rest of the application.

## Development vs. Production Mode

### Simulation Mode for Development

The application now includes a simulation mode for development on non-Windows platforms (like macOS). This allows developers to work on the application without needing the actual Anviz hardware or Windows environment.

In simulation mode:
- All Anviz SDK functions are simulated with realistic responses
- Fingerprint enrollment process is simulated with appropriate events
- No actual calls to the C# wrapper are made

Simulation mode is automatically enabled when the application is running on a non-Windows platform.

### Production Mode

In production mode (on Windows):
- The application uses the actual C# wrapper to communicate with the Anviz SDK
- All functions interact with the real Anviz hardware
- The C# wrapper must be properly built and the Anviz SDK DLL must be present

## Notes for Production

- In a production environment, you would need to implement proper error handling and logging.
- You would need to implement the actual Anviz SDK function calls in the wrapper.
- Make sure the Anviz SDK DLL (`tc-b_new_sdk.dll`) is properly installed in the wrapper directory.
