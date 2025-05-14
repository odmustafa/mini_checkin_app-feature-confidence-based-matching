/**
 * api-explorer.js - Wix API Explorer functionality
 * Allows testing different Wix API endpoints and examining data structure
 */
document.addEventListener('DOMContentLoaded', () => {
  // Elements
  const configInfoEl = document.getElementById('api-config-info');
  const endpointSelectEl = document.getElementById('endpoint-select');
  const searchNameEl = document.getElementById('search-name');
  const testApiBtn = document.getElementById('test-api-btn');
  const apiResultsEl = document.getElementById('api-results');
  const fieldMappingsEl = document.getElementById('field-mappings');
  const collectionIdEl = document.getElementById('collection-id');
  const testSdkBtn = document.getElementById('test-sdk-btn');
  const testSdkSimpleBtn = document.getElementById('test-sdk-simple-btn');
  const inspectSdkBtn = document.getElementById('inspect-sdk-btn');
  const testAdapterBtn = document.getElementById('test-adapter-btn');
  const testCompatAdapterBtn = document.getElementById('test-compat-adapter-btn');
  const endpointTypeEl = document.getElementById('endpoint-type');
  const testDirectApiBtn = document.getElementById('test-direct-api-btn');
  
  // Load API configuration
  async function loadApiConfig() {
    try {
      const config = await window.wixExplorer.getConfig();
      configInfoEl.innerHTML = `
        <div class="config-item">
          <span class="config-label">API Key:</span>
          <span class="config-value">${config.apiKey}</span>
        </div>
        <div class="config-item">
          <span class="config-label">Site ID:</span>
          <span class="config-value">${config.siteId}</span>
        </div>
        <div class="config-item">
          <span class="config-label">Client ID:</span>
          <span class="config-value">${config.clientId}</span>
        </div>
      `;
    } catch (err) {
      configInfoEl.innerHTML = `<div class="error">Error loading configuration: ${err.message}</div>`;
    }
  }
  
  // Load available endpoints
  async function loadEndpoints() {
    try {
      const endpoints = await window.wixExplorer.getEndpoints();
      endpointSelectEl.innerHTML = endpoints.map(endpoint => 
        `<option value="${endpoint.id}">${endpoint.name}</option>`
      ).join('');
    } catch (err) {
      endpointSelectEl.innerHTML = `<option value="">Error loading endpoints</option>`;
    }
  }
  
  // Format JSON for display
  function formatJson(obj) {
    return JSON.stringify(obj, null, 2)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, match => {
        let cls = 'json-number';
        if (/^"/.test(match)) {
          if (/:$/.test(match)) {
            cls = 'json-key';
          } else {
            cls = 'json-string';
          }
        } else if (/true|false/.test(match)) {
          cls = 'json-boolean';
        } else if (/null/.test(match)) {
          cls = 'json-null';
        }
        return `<span class="${cls}">${match}</span>`;
      });
  }
  
  // Display field mappings
  function displayFieldMappings(mappings) {
    if (!mappings || mappings.length === 0) {
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available</div>';
      return;
    }
    
    let html = '<table class="mapping-table">';
    html += `
      <thead>
        <tr>
          <th>Scan-ID Field</th>
          <th>Wix Field Path</th>
          <th>Example Value</th>
        </tr>
      </thead>
      <tbody>
    `;
    
    mappings.forEach(mapping => {
      if (mapping.note) {
        html += `<tr><td colspan="3" class="mapping-note">${mapping.note}</td></tr>`;
      } else {
        html += `
          <tr>
            <td>${mapping.scanIdField}</td>
            <td><code>${mapping.wixField}</code></td>
            <td>${mapping.example}</td>
          </tr>
        `;
      }
    });
    
    html += '</tbody></table>';
    fieldMappingsEl.innerHTML = html;
  }
  
  // Test API call
  async function testApiCall() {
    const endpointId = endpointSelectEl.value;
    if (!endpointId) {
      apiResultsEl.innerHTML = '<div class="error">Please select an endpoint</div>';
      return;
    }
    
    apiResultsEl.innerHTML = '<div class="loading">Loading API results...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing data structure...</div>';
    
    try {
      // Prepare search parameters
      const searchParams = {
        name: searchNameEl.value
      };
      
      // Make the API call
      const result = await window.wixExplorer.testApiCall(endpointId, searchParams);
      
      if (result.error) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>API Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details ? formatJson(result.details) : 'No additional details'}</pre>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display API results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>Endpoint:</strong> ${result.endpoint}
          </div>
          <div class="request-info">
            <strong>Request:</strong>
            <pre class="json">${formatJson(result.requestData)}</pre>
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.data)}</pre>
          </div>
        </div>
      `;
      
      // Display field mappings
      displayFieldMappings(result.suggestedMappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Test Wix JavaScript SDK
  async function testWixSdk() {
    const collectionId = collectionIdEl.value.trim();
    if (!collectionId) {
      apiResultsEl.innerHTML = '<div class="error">Please enter a collection ID</div>';
      return;
    }
    
    apiResultsEl.innerHTML = '<div class="loading">Testing Wix JavaScript SDK...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing SDK data structure...</div>';
    
    try {
      const result = await window.wixSdk.testSdk(collectionId);
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>SDK Test Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details || 'No additional details'}</pre>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display SDK test results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>SDK Test:</strong> ${result.message}
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.result)}</pre>
          </div>
        </div>
      `;
      
      // Generate field mappings from SDK data
      const mappings = generateSdkFieldMappings(result.result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Generate field mappings from SDK data
  function generateSdkFieldMappings(data) {
    const mappings = [];
    
    if (!data || !data.items || data.items.length === 0) {
      return [{ note: 'No items found to analyze' }];
    }
    
    // Sample the first item
    const sample = data.items[0];
    
    mappings.push({ note: 'Wix SDK Data Collection Fields' });
    
    // Add mappings for each field in the data
    if (sample.data) {
      Object.keys(sample.data).forEach(key => {
        const value = sample.data[key];
        let scanIdField = '';
        
        // Try to match to Scan-ID fields based on key name
        if (key.toLowerCase().includes('first') || key === 'firstName') {
          scanIdField = 'FIRST NAME';
        } else if (key.toLowerCase().includes('last') || key === 'lastName') {
          scanIdField = 'LAST NAME';
        } else if (key.toLowerCase().includes('full') || key === 'fullName') {
          scanIdField = 'FULL NAME';
        } else if (key.toLowerCase().includes('birth') || key.toLowerCase().includes('dob')) {
          scanIdField = 'BIRTHDATE';
        } else if (key.toLowerCase().includes('id') || key.toLowerCase().includes('license')) {
          scanIdField = 'DRV LC NO';
        }
        
        mappings.push({
          scanIdField: scanIdField || 'N/A',
          wixField: `data.${key}`,
          example: typeof value === 'object' ? JSON.stringify(value) : String(value)
        });
      });
    }
    
    return mappings;
  }
  
  // Test Wix JavaScript SDK (Simple version)
  async function testWixSdkSimple() {
    const collectionId = collectionIdEl.value.trim();
    if (!collectionId) {
      apiResultsEl.innerHTML = '<div class="error">Please enter a collection ID</div>';
      return;
    }
    
    apiResultsEl.innerHTML = '<div class="loading">Testing Wix JavaScript SDK (Simple)...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing SDK data structure...</div>';
    
    try {
      const result = await window.wixSdk.testSdkSimple(collectionId);
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>Simple SDK Test Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details || 'No additional details'}</pre>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display SDK test results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>Simple SDK Test:</strong> ${result.message}
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.result)}</pre>
          </div>
        </div>
      `;
      
      // Generate field mappings from SDK data
      const mappings = generateSdkFieldMappings(result.result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Inspect Wix SDK structure
  async function inspectWixSdk() {
    apiResultsEl.innerHTML = '<div class="loading">Inspecting Wix SDK structure...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing SDK structure...</div>';
    
    try {
      const result = await window.wixSdk.inspectSdk();
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>SDK Inspection Error:</h4>
            <p>${result.error}</p>
            <pre>${result.stack || 'No stack trace available'}</pre>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No SDK structure available due to error</div>';
        return;
      }
      
      // Display SDK inspection results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>SDK Inspection:</strong> ${result.message}
          </div>
          <div class="response-info">
            <strong>SDK Structure:</strong>
            <pre class="json">${formatJson(result)}</pre>
          </div>
        </div>
      `;
      
      // Generate SDK method mappings
      const mappings = generateSdkMethodMappings(result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No SDK structure available due to error</div>';
    }
  }
  
  // Generate SDK method mappings
  function generateSdkMethodMappings(data) {
    const mappings = [];
    
    mappings.push({ note: 'Wix SDK Structure Analysis' });
    
    // SDK Version Information
    mappings.push({
      scanIdField: 'SDK Versions',
      wixField: '@wix/sdk',
      example: data.sdkInfo.sdkVersion
    });
    
    mappings.push({
      scanIdField: '',
      wixField: '@wix/data',
      example: data.sdkInfo.dataVersion
    });
    
    // Client Methods
    mappings.push({ note: 'Available Client Methods' });
    
    if (data.clientStructure && data.clientStructure.itemsMethods) {
      data.clientStructure.itemsMethods.forEach(method => {
        mappings.push({
          scanIdField: 'Client Method',
          wixField: `myWixClient.items.${method}()`,
          example: 'Available'
        });
      });
    }
    
    // Module Methods
    mappings.push({ note: 'Available Module Methods' });
    
    if (data.itemsModuleStructure && data.itemsModuleStructure.exportedMethods) {
      data.itemsModuleStructure.exportedMethods.forEach(method => {
        mappings.push({
          scanIdField: 'Module Method',
          wixField: `items.${method}()`,
          example: 'Available'
        });
      });
    }
    
    return mappings;
  }
  
  // Test Wix SDK Adapter
  async function testWixSdkAdapter() {
    const collectionId = collectionIdEl.value.trim();
    if (!collectionId) {
      apiResultsEl.innerHTML = '<div class="error">Please enter a collection ID</div>';
      return;
    }
    
    apiResultsEl.innerHTML = '<div class="loading">Testing Wix SDK Adapter...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing adapter results...</div>';
    
    try {
      const result = await window.wixSdk.testAdapter(collectionId);
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>SDK Adapter Test Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details || 'No additional details'}</pre>
            <div class="sdk-info">
              <p>SDK Version: ${result.sdkVersion}</p>
              <p>Data Version: ${result.dataVersion}</p>
              <p>Available Methods: ${result.availableMethods.join(', ') || 'None detected'}</p>
            </div>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display SDK adapter test results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>SDK Adapter Test:</strong> ${result.message}
            <div class="sdk-info">
              <p>SDK Version: ${result.result.sdkVersion}</p>
              <p>Data Version: ${result.result.dataVersion}</p>
              <p>Available Methods: ${result.result.availableMethods.join(', ')}</p>
            </div>
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.result)}</pre>
          </div>
        </div>
      `;
      
      // Generate field mappings from adapter data
      const mappings = generateAdapterFieldMappings(result.result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Generate field mappings from adapter data
  function generateAdapterFieldMappings(data) {
    const mappings = [];
    
    mappings.push({ note: 'Wix SDK Adapter Results' });
    
    // SDK Version Information
    mappings.push({
      scanIdField: 'SDK Information',
      wixField: 'SDK Version',
      example: data.sdkVersion
    });
    
    mappings.push({
      scanIdField: '',
      wixField: 'Data Version',
      example: data.dataVersion
    });
    
    mappings.push({
      scanIdField: '',
      wixField: 'Available Methods',
      example: data.availableMethods.join(', ')
    });
    
    // Add mappings for each field in the data
    if (data.items && data.items.length > 0) {
      mappings.push({ note: 'Data Collection Fields' });
      
      const sample = data.items[0];
      let fields = [];
      
      // Extract fields based on the data structure
      if (sample.data) {
        // Standard format
        fields = Object.keys(sample.data).map(key => ({ 
          key: `data.${key}`, 
          value: sample.data[key] 
        }));
      } else if (typeof sample === 'object') {
        // Direct object format
        fields = Object.keys(sample)
          .filter(key => !key.startsWith('_') && key !== 'id' && key !== '_id')
          .map(key => ({ 
            key, 
            value: sample[key] 
          }));
      }
      
      // Map fields to Scan-ID fields
      fields.forEach(({ key, value }) => {
        let scanIdField = '';
        const keyLower = key.toLowerCase();
        
        // Try to match to Scan-ID fields based on key name
        if (keyLower.includes('first') || key === 'firstName') {
          scanIdField = 'FIRST NAME';
        } else if (keyLower.includes('last') || key === 'lastName') {
          scanIdField = 'LAST NAME';
        } else if (keyLower.includes('full') || key === 'fullName') {
          scanIdField = 'FULL NAME';
        } else if (keyLower.includes('birth') || keyLower.includes('dob')) {
          scanIdField = 'BIRTHDATE';
        } else if (keyLower.includes('id') || keyLower.includes('license')) {
          scanIdField = 'DRV LC NO';
        } else if (keyLower.includes('email')) {
          scanIdField = 'EMAIL';
        } else if (keyLower.includes('phone')) {
          scanIdField = 'PHONE';
        } else if (keyLower.includes('address')) {
          scanIdField = 'ADDRESS';
        }
        
        mappings.push({
          scanIdField: scanIdField || 'N/A',
          wixField: key,
          example: typeof value === 'object' ? JSON.stringify(value) : String(value || '')
        });
      });
    } else {
      mappings.push({ note: 'No items found to analyze' });
    }
    
    return mappings;
  }
  
  // Test Wix Direct API
  async function testWixDirectApi() {
    const endpoint = endpointTypeEl.value;
    
    apiResultsEl.innerHTML = '<div class="loading">Testing Wix Direct API...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing API results...</div>';
    
    try {
      const result = await window.wixDirect.testApi(endpoint);
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>Direct API Test Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details || 'No additional details'}</pre>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display Direct API test results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>Direct API Test:</strong> ${result.message}
            <div class="endpoint-details">
              <p>Endpoint: ${result.endpoint}</p>
            </div>
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.result)}</pre>
          </div>
        </div>
      `;
      
      // Generate field mappings from Direct API data
      const mappings = generateDirectApiMappings(result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Generate field mappings from Direct API data
  function generateDirectApiMappings(result) {
    const mappings = [];
    const endpoint = result.endpoint;
    
    mappings.push({ note: `Wix Direct API Results (${endpoint})` });
    
    let items = [];
    let sampleItem = null;
    
    // Extract items based on the endpoint
    if (endpoint === 'members' && result.result.members) {
      items = result.result.members;
      if (items.length > 0) sampleItem = items[0];
    } else if (endpoint === 'contacts' && result.result.contacts) {
      items = result.result.contacts;
      if (items.length > 0) sampleItem = items[0];
    } else if (endpoint === 'collection' && result.result.items) {
      items = result.result.items;
      if (items.length > 0) sampleItem = items[0];
    }
    
    // Add count information
    mappings.push({
      scanIdField: 'Result Count',
      wixField: 'Total Items',
      example: items.length.toString()
    });
    
    if (!sampleItem) {
      mappings.push({ note: 'No items found to analyze' });
      return mappings;
    }
    
    // Process member data
    if (endpoint === 'members' && sampleItem.profile) {
      mappings.push({ note: 'Member Profile Fields' });
      
      // Map common member fields
      if (sampleItem.profile.firstName) {
        mappings.push({
          scanIdField: 'FIRST NAME',
          wixField: 'profile.firstName',
          example: sampleItem.profile.firstName
        });
      }
      
      if (sampleItem.profile.lastName) {
        mappings.push({
          scanIdField: 'LAST NAME',
          wixField: 'profile.lastName',
          example: sampleItem.profile.lastName
        });
      }
      
      if (sampleItem.profile.email) {
        mappings.push({
          scanIdField: 'EMAIL',
          wixField: 'profile.email',
          example: sampleItem.profile.email
        });
      }
      
      if (sampleItem.profile.phone) {
        mappings.push({
          scanIdField: 'PHONE',
          wixField: 'profile.phone',
          example: sampleItem.profile.phone
        });
      }
      
      // Extended fields
      if (sampleItem.extendedFields) {
        mappings.push({ note: 'Member Extended Fields' });
        
        Object.keys(sampleItem.extendedFields).forEach(key => {
          const value = sampleItem.extendedFields[key];
          let scanIdField = '';
          
          // Try to match to Scan-ID fields
          if (key.toLowerCase().includes('birth') || key.toLowerCase().includes('dob')) {
            scanIdField = 'BIRTHDATE';
          } else if (key.toLowerCase().includes('license') || key.toLowerCase().includes('id')) {
            scanIdField = 'DRV LC NO';
          } else if (key.toLowerCase().includes('address')) {
            scanIdField = 'ADDRESS';
          }
          
          mappings.push({
            scanIdField: scanIdField || 'N/A',
            wixField: `extendedFields.${key}`,
            example: typeof value === 'object' ? JSON.stringify(value) : String(value || '')
          });
        });
      }
    }
    
    // Process contact data
    else if (endpoint === 'contacts' && sampleItem.info) {
      mappings.push({ note: 'Contact Info Fields' });
      
      // Map common contact fields
      if (sampleItem.info.name && sampleItem.info.name.first) {
        mappings.push({
          scanIdField: 'FIRST NAME',
          wixField: 'info.name.first',
          example: sampleItem.info.name.first
        });
      }
      
      if (sampleItem.info.name && sampleItem.info.name.last) {
        mappings.push({
          scanIdField: 'LAST NAME',
          wixField: 'info.name.last',
          example: sampleItem.info.name.last
        });
      }
      
      if (sampleItem.info.emails && sampleItem.info.emails.length > 0) {
        mappings.push({
          scanIdField: 'EMAIL',
          wixField: 'info.emails[0]',
          example: sampleItem.info.emails[0]
        });
      }
      
      if (sampleItem.info.phones && sampleItem.info.phones.length > 0) {
        mappings.push({
          scanIdField: 'PHONE',
          wixField: 'info.phones[0]',
          example: sampleItem.info.phones[0]
        });
      }
      
      // Extended fields
      if (sampleItem.extendedFields) {
        mappings.push({ note: 'Contact Extended Fields' });
        
        Object.keys(sampleItem.extendedFields).forEach(key => {
          const value = sampleItem.extendedFields[key];
          let scanIdField = '';
          
          // Try to match to Scan-ID fields
          if (key.toLowerCase().includes('birth') || key.toLowerCase().includes('dob')) {
            scanIdField = 'BIRTHDATE';
          } else if (key.toLowerCase().includes('license') || key.toLowerCase().includes('id')) {
            scanIdField = 'DRV LC NO';
          } else if (key.toLowerCase().includes('address')) {
            scanIdField = 'ADDRESS';
          }
          
          mappings.push({
            scanIdField: scanIdField || 'N/A',
            wixField: `extendedFields.${key}`,
            example: typeof value === 'object' ? JSON.stringify(value) : String(value || '')
          });
        });
      }
    }
    
    // Process collection data
    else if (endpoint === 'collection') {
      mappings.push({ note: 'Data Collection Fields' });
      
      // Map fields from the collection item
      if (typeof sampleItem === 'object') {
        Object.keys(sampleItem).forEach(key => {
          if (key !== '_id' && key !== 'id' && !key.startsWith('_')) {
            const value = sampleItem[key];
            let scanIdField = '';
            
            // Try to match to Scan-ID fields
            const keyLower = key.toLowerCase();
            if (keyLower.includes('first')) {
              scanIdField = 'FIRST NAME';
            } else if (keyLower.includes('last')) {
              scanIdField = 'LAST NAME';
            } else if (keyLower.includes('birth') || keyLower.includes('dob')) {
              scanIdField = 'BIRTHDATE';
            } else if (keyLower.includes('license') || keyLower.includes('id')) {
              scanIdField = 'DRV LC NO';
            } else if (keyLower.includes('email')) {
              scanIdField = 'EMAIL';
            } else if (keyLower.includes('phone')) {
              scanIdField = 'PHONE';
            } else if (keyLower.includes('address')) {
              scanIdField = 'ADDRESS';
            }
            
            mappings.push({
              scanIdField: scanIdField || 'N/A',
              wixField: key,
              example: typeof value === 'object' ? JSON.stringify(value) : String(value || '')
            });
          }
        });
      }
    }
    
    return mappings;
  }
  
  // Test Wix SDK Compatibility Adapter
  async function testWixSdkCompatAdapter() {
    const collectionId = collectionIdEl.value.trim();
    if (!collectionId) {
      apiResultsEl.innerHTML = '<div class="error">Please enter a collection ID</div>';
      return;
    }
    
    apiResultsEl.innerHTML = '<div class="loading">Testing Wix SDK Compatibility Adapter...</div>';
    fieldMappingsEl.innerHTML = '<div class="loading">Analyzing module structure...</div>';
    
    try {
      const result = await window.wixSdk.testCompatAdapter(collectionId);
      
      if (!result.success) {
        apiResultsEl.innerHTML = `
          <div class="error">
            <h4>SDK Compatibility Adapter Test Error:</h4>
            <p>${result.error}</p>
            <pre>${result.details || 'No additional details'}</pre>
            <div class="sdk-info">
              <h5>Module Structure:</h5>
              <pre>${formatJson(result.moduleStructure)}</pre>
            </div>
          </div>
        `;
        fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
        return;
      }
      
      // Display SDK compatibility adapter test results
      apiResultsEl.innerHTML = `
        <div class="api-result-info">
          <div class="endpoint-info">
            <strong>SDK Compatibility Adapter Test:</strong> ${result.message}
            <div class="sdk-info">
              <h5>Module Structure:</h5>
              <pre>${formatJson(result.moduleStructure)}</pre>
            </div>
          </div>
          <div class="response-info">
            <strong>Response:</strong>
            <pre class="json">${formatJson(result.result)}</pre>
          </div>
        </div>
      `;
      
      // Generate field mappings from compatibility adapter data
      const mappings = generateCompatAdapterMappings(result);
      displayFieldMappings(mappings);
      
    } catch (err) {
      apiResultsEl.innerHTML = `<div class="error">Error: ${err.message}</div>`;
      fieldMappingsEl.innerHTML = '<div class="placeholder">No field mappings available due to error</div>';
    }
  }
  
  // Generate field mappings from compatibility adapter data
  function generateCompatAdapterMappings(result) {
    const mappings = [];
    
    mappings.push({ note: 'Wix SDK Compatibility Adapter Results' });
    
    // SDK Version Information
    if (result.moduleStructure) {
      mappings.push({
        scanIdField: 'SDK Information',
        wixField: 'SDK Version',
        example: result.moduleStructure.sdkVersion || 'Unknown'
      });
      
      mappings.push({
        scanIdField: '',
        wixField: 'Data Version',
        example: result.moduleStructure.dataVersion || 'Unknown'
      });
      
      mappings.push({
        scanIdField: '',
        wixField: 'Module Type',
        example: result.moduleStructure.type || 'Unknown'
      });
      
      mappings.push({
        scanIdField: '',
        wixField: 'Is Function',
        example: String(result.moduleStructure.isFunction || false)
      });
      
      if (result.moduleStructure.methods && result.moduleStructure.methods.length > 0) {
        mappings.push({
          scanIdField: '',
          wixField: 'Available Methods',
          example: result.moduleStructure.methods.join(', ')
        });
      }
    }
    
    // Add data fields if available
    if (result.result && typeof result.result === 'object') {
      let items = [];
      
      // Try to extract items based on common patterns
      if (result.result.items && Array.isArray(result.result.items)) {
        items = result.result.items;
      } else if (Array.isArray(result.result)) {
        items = result.result;
      }
      
      if (items.length > 0) {
        mappings.push({ note: 'Data Fields' });
        
        const sample = items[0];
        let fields = [];
        
        // Extract fields based on the data structure
        if (sample.data) {
          // Standard format
          fields = Object.keys(sample.data).map(key => ({ 
            key: `data.${key}`, 
            value: sample.data[key] 
          }));
        } else if (typeof sample === 'object') {
          // Direct object format
          fields = Object.keys(sample)
            .filter(key => !key.startsWith('_') && key !== 'id' && key !== '_id')
            .map(key => ({ 
              key, 
              value: sample[key] 
            }));
        }
        
        // Map fields to Scan-ID fields
        fields.forEach(({ key, value }) => {
          let scanIdField = '';
          const keyLower = key.toLowerCase();
          
          // Try to match to Scan-ID fields based on key name
          if (keyLower.includes('first') || key === 'firstName') {
            scanIdField = 'FIRST NAME';
          } else if (keyLower.includes('last') || key === 'lastName') {
            scanIdField = 'LAST NAME';
          } else if (keyLower.includes('full') || key === 'fullName') {
            scanIdField = 'FULL NAME';
          } else if (keyLower.includes('birth') || keyLower.includes('dob')) {
            scanIdField = 'BIRTHDATE';
          } else if (keyLower.includes('id') || keyLower.includes('license')) {
            scanIdField = 'DRV LC NO';
          } else if (keyLower.includes('email')) {
            scanIdField = 'EMAIL';
          } else if (keyLower.includes('phone')) {
            scanIdField = 'PHONE';
          } else if (keyLower.includes('address')) {
            scanIdField = 'ADDRESS';
          }
          
          mappings.push({
            scanIdField: scanIdField || 'N/A',
            wixField: key,
            example: typeof value === 'object' ? JSON.stringify(value) : String(value || '')
          });
        });
      } else {
        mappings.push({ note: 'No items found to analyze' });
      }
    }
    
    return mappings;
  }
  
  // Event listeners
  testApiBtn.addEventListener('click', testApiCall);
  testSdkBtn.addEventListener('click', testWixSdk);
  testSdkSimpleBtn.addEventListener('click', testWixSdkSimple);
  inspectSdkBtn.addEventListener('click', inspectWixSdk);
  testAdapterBtn.addEventListener('click', testWixSdkAdapter);
  testCompatAdapterBtn.addEventListener('click', testWixSdkCompatAdapter);
  testDirectApiBtn.addEventListener('click', testWixDirectApi);
  
  // Initialize
  loadApiConfig();
  loadEndpoints();
});
