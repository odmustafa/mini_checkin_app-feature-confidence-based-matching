/**
 * Web compatibility layer for mini check-in app
 * Provides compatibility between Electron and web browser environments
 * 
 * Following the Ethereal Engineering Technical Codex principles:
 * - Boundary Protection: Implementing strict interface contracts for APIs
 * - Separation of Concerns: Maintaining clear boundaries between components
 * - Fail Fast and Learn: Using fallback mechanisms and detailed error reporting
 */

// Initialize wixSdk immediately to avoid "Cannot read properties of undefined (reading 'getConfig')" error
if (typeof window !== 'undefined' && !window.wixSdk) {
  console.log('Creating wixSdk compatibility layer for web (immediate initialization)');
  
  // Add a configuration object
  const wixConfig = {
    apiKey: 'web-mode-api-key',
    siteId: 'web-mode-site-id',
    environment: 'web'
  };
  
  window.wixSdk = {
    // Add getConfig method
    getConfig: () => {
      console.log('Web compatibility: Returning mock Wix configuration');
      return wixConfig;
    },
    // Map searchMember to scanidAPI.findWixMember (will be properly initialized later)
    searchMember: async (params) => {
      console.log('Web compatibility: wixSdk.searchMember called before full initialization');
      if (window.scanidAPI && window.scanidAPI.findWixMember) {
        const { firstName, lastName, dateOfBirth } = params;
        return await window.scanidAPI.findWixMember(firstName, lastName, dateOfBirth);
      }
      return { success: false, error: 'scanidAPI not yet initialized' };
    },
    // Stub methods that will be properly initialized later
    getMemberPricingPlans: async () => ({ success: false, error: 'Not fully initialized yet' }),
    listPricingPlanOrders: async () => ({ success: false, error: 'Not fully initialized yet' })
  };
}

// Helper function to convert file:// URLs to web server URLs for photos
function convertPhotoPath(photoPath) {
  if (!photoPath) return '';
  
  // Check if it's a file:// URL
  if (photoPath.startsWith('file://')) {
    // Extract the filename from the path
    let filename = photoPath.split('/').pop().split('\\').pop();
    
    // Handle the file extension - we need to convert to .jpeg.jpeg for the new location
    if (filename.toLowerCase().endsWith('.jpg')) {
      filename = filename.replace(/\.jpg$/i, '.jpeg.jpeg');
    }
    
    // Return a URL to our photo endpoint that serves from the new location
    return `/api/photos/${filename}`;
  } else if (photoPath.includes('scan-id-export-scan-demo')) {
    // If it's already a path to the new location, extract just the filename
    const filename = photoPath.split('/').pop().split('\\').pop();
    return `/api/photos/${filename}`;
  }
  
  return photoPath;
}

// Function to set up the photo path converter
function setupPhotoPathConverter() {
  console.log('Setting up photo path converter');
  
  // Fix any existing images
  fixExistingImages();
  
  // Set up a MutationObserver to watch for new images
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.type === 'childList') {
        mutation.addedNodes.forEach((node) => {
          // Check if it's an element node
          if (node.nodeType === Node.ELEMENT_NODE) {
            // If it's an image, fix its src
            if (node.tagName === 'IMG') {
              fixImageSrc(node);
            }
            
            // Check for images within the added node
            const images = node.querySelectorAll('img');
            images.forEach(fixImageSrc);
          }
        });
      } else if (mutation.type === 'attributes' && 
                mutation.attributeName === 'src' && 
                mutation.target.tagName === 'IMG') {
        // If an image's src attribute changed
        fixImageSrc(mutation.target);
      }
    });
  });
  
  // Start observing the entire document
  observer.observe(document.body, {
    childList: true,
    subtree: true,
    attributes: true,
    attributeFilter: ['src', 'data-photo-path']
  });
}

// Function to fix existing images on the page
function fixExistingImages() {
  const images = document.querySelectorAll('img');
  images.forEach(fixImageSrc);
}

// Function to fix an image's src attribute
function fixImageSrc(img) {
  // Check if the image has a src that needs conversion
  if (img.src && img.src.startsWith('file://')) {
    console.log('Converting image src from:', img.src);
    img.src = convertPhotoPath(img.src);
    console.log('Converted to:', img.src);
  }
  
  // Also check data-photo-path attribute if it exists
  if (img.hasAttribute('data-photo-path') && img.getAttribute('data-photo-path').startsWith('file://')) {
    console.log('Converting data-photo-path from:', img.getAttribute('data-photo-path'));
    img.setAttribute('data-photo-path', convertPhotoPath(img.getAttribute('data-photo-path')));
    console.log('Converted to:', img.getAttribute('data-photo-path'));
  }
}

// Initialize on document load
document.addEventListener('DOMContentLoaded', function() {
  // Force desktop mode for the front desk Windows computer
  const isWebEnvironment = false; // Disabled web development mode
  
  if (isWebEnvironment) {
    console.log('Initializing web compatibility layer for browser environment');
    
    // Add a MutationObserver to automatically convert file:// URLs in images
    setupPhotoPathConverter();
    
    // Update the compatibility layer for Electron's window.wixSdk in web environment
    // The basic wixSdk object was already created at the top of the file
    if (window.wixSdk) {
      console.log('Updating wixSdk compatibility layer with full functionality');
      
      // Update the methods to use scanidAPI now that it's available
      window.wixSdk.searchMember = async (params) => {
        console.log('Web compatibility: Redirecting wixSdk.searchMember to scanidAPI.findWixMember', params);
        const { firstName, lastName, dateOfBirth } = params;
        return await window.scanidAPI.findWixMember(firstName, lastName, dateOfBirth);
      };
      
      // Map getMemberPricingPlans to scanidAPI.getMemberPricingPlans
      window.wixSdk.getMemberPricingPlans = async (memberId) => {
        console.log('Web compatibility: Redirecting wixSdk.getMemberPricingPlans to scanidAPI.getMemberPricingPlans', memberId);
        return await window.scanidAPI.getMemberPricingPlans(memberId);
      };
      
      // Add other methods as needed
      window.wixSdk.listPricingPlanOrders = async (options) => {
        console.log('Web compatibility: Redirecting wixSdk.listPricingPlanOrders to scanidAPI.listPricingPlanOrders', options);
        return await window.scanidAPI.listPricingPlanOrders(options);
      };
    }
  }
  
  // Create a compatibility layer for Electron's window.electronAPI in web environment
  if (!window.electronAPI) {
    console.log('Creating electronAPI compatibility layer for web');
    
    window.electronAPI = {
      // Stub for restart app - just reloads the page in web mode
      restartApp: () => {
        console.log('Web compatibility: Reloading page instead of restarting app');
        window.location.reload();
      },
      
      // Add other methods as needed
      showNotification: (options) => {
        console.log('Web compatibility: Showing browser notification instead of Electron notification', options);
        // Use browser notifications API if available
        if ('Notification' in window) {
          Notification.requestPermission().then(permission => {
            if (permission === 'granted') {
              new Notification(options.title, { 
                body: options.body,
                icon: options.icon
              });
            }
          });
        }
      }
    };
  }
  
  if (isWebEnvironment) {
    console.log('Web compatibility layer initialized');
  } else {
    console.log('Running in Electron mode - web compatibility layer not needed');
  }
});

