#!/bin/bash

# This script tests the webhook server by sending a JWT-like token
# It will try multiple ports to find the webhook server

# Sample JWT-like token (this is not a real JWT, just a similar format for testing)
JWT_TOKEN="eyJraWQiOiJQb3pIV1ZJaFwvdlVNRWxFUlpXdGdUbWdQbz0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI3M2RiZDBkMy1kNDRlLTRkNmMtODg4Zi0xNmEyMzYwZDQ4MzgiLCJpc3MiOiJodHRwczpcL1wvd3d3Lndpei5jb20iLCJleHAiOjE3MTU4NzY2NTUsImlhdCI6MTcxNTg3MzA1NX0.example"

# Try different ports
PORTS=(3000 3001 3002 3003 3004 3005)

for PORT in "${PORTS[@]}"; do
  WEBHOOK_URL="http://localhost:$PORT/webhook"
  
  echo "\nTrying webhook URL: $WEBHOOK_URL"
  echo "Using JWT-like token format"
  
  # Check if the server is running on this port
  if curl -s --head "http://localhost:$PORT/webhook/health" | grep "200 OK" > /dev/null; then
    echo "Found webhook server at $WEBHOOK_URL"
    
    # Send the webhook test
    echo "Sending webhook test..."
    curl -X POST \
      -H "Content-Type: text/plain" \
      -H "X-Wix-Signature: test-signature" \
      -d "$JWT_TOKEN" \
      "$WEBHOOK_URL"
    
    echo -e "\nTest completed for $WEBHOOK_URL"
    exit 0
  else
    echo "No webhook server found at $PORT"
  fi
done

echo -e "\nCould not find webhook server on any of the tested ports."
echo "Please make sure the webhook server is running and try again."
exit 1
