# New Account Activation Flow

## Overview

The New Account Activation process is the final step for new members joining Tribute Music Gallery. This process occurs during the member's initial visit to the venue and serves two critical purposes:
1. Verification of identity and age (21+)
2. Creation of TouchID (fingerprint) for express check-in during future visits

## Prerequisites

Before beginning the New Account Activation process, members must have:
- Created an account on [tribute.gallery](https://tribute.gallery)
- Purchased one of the available membership plans (week, month, or year)

## Hardware/Software Requirements

**Hardware:**
- Anviz C2 Pro fingerprint scanner
- Duplex Driver License Scanner (USB)
- Front Desk Computer (Windows)

**Software:**
- Wix Website (Member accounts and membership information)
- Scan-ID (connects to license scanner)
- TimeXpress (connects to Anviz C2 Pro)
- FiDO (Front Desk Operations application)

## Process Flow

The diagram below illustrates the complete New Account Activation process from start to finish:

![New Account Activation Flow](./images/new-account-activation-flow.svg)

## Process Description

1. **Initial Steps:**
   - Member visits the Front Desk during their initial visit
   - Member presents government-issued photo ID for verification
   - Staff confirms the member visually matches their ID

2. **ID Verification:**
   - Staff initiates the process in FiDO
   - ID is scanned through the Duplex Scanner
   - System verifies ID information and age requirement (21+)

3. **Account Matching:**
   - FiDO searches for a matching account using scanned information
   - Staff confirms the account belongs to the member
   - System pulls membership plan details

4. **TouchID Registration:**
   - Staff initiates TouchID registration
   - Member scans their fingerprint three times
   - System verifies successful registration with a final scan

5. **Completion:**
   - Member is granted entry for their first visit
   - Account is now fully activated for future visits

## Implementation Notes

- If the ID scan fails or member is underage, the process cannot proceed
- If account matching fails, staff should request the member log in to their account on their phone
- Failed TouchID registrations can be restarted from the "Register TouchID" step

## Troubleshooting

Common issues and their solutions:
- **ID Scan Failure**: Ensure ID is properly inserted and try again
- **Account Not Found**: Verify spelling of name or check with alternative information
- **TouchID Registration Error**: Clean fingerprint scanner and retry registration

For detailed technical documentation on the process, refer to the [Technical Documentation](./docs/technical/new-account-activation.md).
