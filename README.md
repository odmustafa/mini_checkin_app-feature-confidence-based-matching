# Mini Check-In App

A check-in application that reads Scan-ID export data from a CSV file and integrates with Wix APIs for member verification and pricing plan retrieval. The app can run either as an Electron desktop application or as a web-based application in your browser.

> UPDATE: Wix REST API works a lot better than Wix Javascript API for getting the data from Wix and displaying on a standalone desktop application. Some of this `README.md` file is outdated and has not been updated yet.

>SEE THIS OTHER REPO: https://github.com/odmustafa/wix-rest-api-test/


> NOTE: if you see any reference to "Wix JavaScript SDK" or "Anviz C2 Pro" (fingerprint scanner) or "Anviz SDK",  we decided to instead use "Wix REST API" and "Secugen Hamster Pro 20 Usb Fingerprint Reader" and "Secugen SDK"

## New Account Activation Flow - Explained
- Check [New Account Activation Flow](./New-Account-Activation-flow.md) for a detailed explanation of the New Account Activation flow (now with a neat flowchart!)

## Features
- Scan ID data parsing from CSV exports
- Contact verification using Wix REST API
- Confidence-based contact matching with visual indicators
- Flexible name matching for better search results
- Proper case conversion for improved contact matching
- Pricing plan retrieval for verified contacts
- Local database used to store data associated with each member, their membership, and various membership-related counters (i.e. information from ID scan, Wix account information, list of orders (for latest membership plan information along with membership history)

## Recent Improvements
- Added confidence-based contact matching system with color-coded indicators
- Implemented intelligent scoring algorithm for name and DOB matching
- Prioritized exact last name matches with partial first name matches
- Added detailed match explanations showing why contacts were matched
- Migrated from Wix Members API to CRM Contacts API for all lookups
- Implemented query builder pattern following Wix documentation
- Enhanced contact search with first/last name filtering
- Added test scripts to verify contact search functionality
- Improved authentication with ApiKeyStrategy and account-level headers
- Integrated Wix Pricing Plans Orders API using the official Wix JavaScript SDK
- Added Debug panel for viewing raw API responses during troubleshooting
- Added Help panel with restart option and support contact information
- Fixed email display in contact list to properly show primary email addresses
- Deprecated all direct API calls in favor of the official Wix JavaScript SDK

 

## How it works
- Click "Scan ID" to read the latest scan from the Scan-ID CSV export (Staff will insert members ID into the Duplex ID Scanner, which will automatically update csv file and save a combined front/back scan of ID in a single image file)
- The app displays the person's name, DOB, ID number, and other details as well as the image of the scanned Id
- It then searches for matching contacts in Wix using the Wix REST API
- Matches are displayed with confidence scores and color-coding:
  - High confidence (60-100 points): Green
  - Medium confidence (35-59 points): Yellow
  - Low confidence (0-34 points): Red
  - (Each match shows detailed information about why it was matched)
- Contacts are sorted by confidence score with the best matches first
- You can select any match to view their pricing plans
- **SLIGHT CHANGE OF PLAN FOR THIS PART OF THE PROCESS** Need to integrate fingerprint scanner functionality with SecuGen SDK 

## Running the App

### As a Desktop App (Electron)
```
npm start
```

### As a Web App
```
npm run web
```
Then open http://localhost:3000 (or the port shown in the console) in your browser.

## Configuration
- The Scan-ID CSV export path is located at: `src/assets/scan-id-export.csv`
- Wix API credentials are stored in `wix.config.json` in the root directory
- Sample format for `wix.config.json`:
- * NOTE: `wix.config.json` FILE WILL BE INCLUDED IN EMAIL *
```json
{
  "apiKey": "YOUR_WIX_API_KEY",
  "siteId": "YOUR_WIX_SITE_ID",
  "clientId": "YOUR_WIX_CLIENT_ID",
  "appSecret": "YOUR_WIX_APP_SECRET",
  "publicKey": "YOUR_WIX_PUBLIC_KEY"
}
```

## Architecture
- **Frontend**: HTML, CSS, JavaScript in `src/renderer/`
- **Backend**: Node.js with Express server in `src/web-server.js`
- **Electron**: Main process in `src/main.js`
- **Services**: Wix API integration in `src/services/`


## Setup
```bash
npm install
```

## Running the App

### As Desktop App (Electron)
```bash
npm start
```

### As Web App (Browser)
```bash
npm run web
```
This will start a web server and automatically open your browser to the app. The server will try port 3000 first, and if it's in use, it will try 3001, 3002, etc.

---

## Development
The purpose of the program in this repository was mainly to explore best strategy for pulling data from Wix into a standalone desktop application. The UI was not the main focus in this, so the final product will look nothing like what you see with this app. Also, been using this to explain what we're needing overall with this Front Desk Ops application. 

- UPDATE: Wix REST API works a lot better than Wix Javascript API for getting the data from Wix and displaying on a standalone desktop application.



## AI Usage
This project relies heavily on AI technologies including:
- Windsurf (Pro)
- Claude 3.7 Sonnet (within Windsurf via Cascade)
- GPT-4.1 (free limited time) (within Windsurf via Cascade)
- ChatGPT (Pro)

## Useage
This project is for developing an application that will be used by staff at Tribute Music Gallery primarily to 
- Check-in members upon arrival
- New account activation

## TO-DO
- (MISSION CRITICAL) Get all of the parts of the New Account Activation flow functioning.
- (MISSION CRITICAL) Get Member Check-in process functioning
- (Not as critical, but still important) DJ/Artist list and check-in
- (Not as critical, but still important) DJ/Artist guestlist check-in
- (Not as critical, but still important) Implimentation of Guest Pass feature (this will enable new Membership Perk for month/year membership renewals)
- Not cry (...I am but a simple man .__.;; )
- (Future components) Staff Task List (for internal use)
- (Future components) Staff Schedule Management (for internal use)
- (Future components) Incident Report Module (for staff to create Incident Reports - hopefully implimented before incident occures which requires reporting (i.e. Breach of the Peace Report (Tx Tabc Enf 5122))

