/**
 * app.js - Main application logic for tab navigation
 */
document.addEventListener('DOMContentLoaded', () => {
  // Tab navigation
  const tabButtons = document.querySelectorAll('.tab-btn');
  const tabContents = document.querySelectorAll('.tab-content');
  
  tabButtons.forEach(button => {
    button.addEventListener('click', () => {
      const tabId = button.getAttribute('data-tab');
      
      // Update active tab button
      tabButtons.forEach(btn => btn.classList.remove('active'));
      button.classList.add('active');
      
      // Update active tab content
      tabContents.forEach(content => {
        content.classList.remove('active');
        if (content.id === `${tabId}-tab`) {
          content.classList.add('active');
        }
      });
    });
  });
  
  // Result tabs in API Explorer
  const resultTabButtons = document.querySelectorAll('.result-tab-btn');
  const resultTabContents = document.querySelectorAll('.result-tab-content');
  
  resultTabButtons.forEach(button => {
    button.addEventListener('click', () => {
      const tabId = button.getAttribute('data-result-tab');
      
      // Update active result tab button
      resultTabButtons.forEach(btn => btn.classList.remove('active'));
      button.classList.add('active');
      
      // Update active result tab content
      resultTabContents.forEach(content => {
        content.classList.remove('active');
        if (content.getAttribute('data-tab') === tabId) {
          content.classList.add('active');
        }
      });
    });
  });
});
