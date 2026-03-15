// admin-settings.js - Settings page functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Color picker synchronization
    const colorInputs = document.querySelectorAll('input[type="color"]');
    colorInputs.forEach(colorInput => {
        const textInput = colorInput.nextElementSibling;
        colorInput.addEventListener('input', function() {
            textInput.value = this.value;
        });
    });

    // Save All Settings Function
    window.saveAllSettings = function() {
        // Collect all form data
        const settings = {
            general: collectTabData('general'),
            academic: collectTabData('academic'),
            notifications: collectTabData('notifications'),
            security: collectTabData('security'),
            appearance: collectTabData('appearance')
        };

        // Simulate saving (in a real app, this would be an AJAX call)
        console.log('Saving settings:', settings);

        // Show success message
        showNotification('All settings saved successfully!', 'success');
    };

    function collectTabData(tabId) {
        const tab = document.getElementById(tabId);
        const data = {};

        // Collect input values
        const inputs = tab.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            if (input.type === 'checkbox') {
                data[input.id] = input.checked;
            } else if (input.type === 'file') {
                // Handle file inputs differently
                data[input.name || input.id] = input.files[0] ? input.files[0].name : null;
            } else {
                data[input.id || input.name] = input.value;
            }
        });

        return data;
    }

    // Tab change handler
    const tabs = document.querySelectorAll('#settingsTabs button');
    tabs.forEach(tab => {
        tab.addEventListener('shown.bs.tab', function(e) {
            const targetTab = e.target.getAttribute('data-bs-target').substring(1);
            console.log('Switched to tab:', targetTab);
        });
    });

    // Real-time validation
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value && !this.value.includes('@')) {
                this.classList.add('is-invalid');
                showFieldError(this, 'Please enter a valid email address');
            } else {
                this.classList.remove('is-invalid');
                hideFieldError(this);
            }
        });
    });

    const numberInputs = document.querySelectorAll('input[type="number"]');
    numberInputs.forEach(input => {
        input.addEventListener('blur', function() {
            const value = parseInt(this.value);
            const min = this.min ? parseInt(this.min) : 0;
            const max = this.max ? parseInt(this.max) : Infinity;

            if (this.value && (value < min || value > max)) {
                this.classList.add('is-invalid');
                showFieldError(this, `Value must be between ${min} and ${max}`);
            } else {
                this.classList.remove('is-invalid');
                hideFieldError(this);
            }
        });
    });

    function showFieldError(input, message) {
        // Remove existing error
        hideFieldError(input);

        // Add error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback d-block';
        errorDiv.textContent = message;
        input.parentNode.appendChild(errorDiv);
    }

    function hideFieldError(input) {
        const error = input.parentNode.querySelector('.invalid-feedback');
        if (error) {
            error.remove();
        }
    }

    // Theme preview updates
    const themeSelect = document.querySelector('#appearance select');
    const previewDiv = document.querySelector('#appearance .border');

    themeSelect.addEventListener('change', function() {
        const theme = this.value;
        if (theme === 'Dark') {
            previewDiv.style.backgroundColor = '#1a1a1a';
            previewDiv.style.color = '#ffffff';
        } else {
            previewDiv.style.backgroundColor = '#ffffff';
            previewDiv.style.color = '#000000';
        }
    });

    // Font preview updates
    const fontSelect = document.querySelector('#appearance select');
    fontSelect.addEventListener('change', function() {
        const font = this.value;
        const previewText = previewDiv.querySelector('p');
        previewText.style.fontFamily = `'${font}', sans-serif`;
    });

    // Color preview updates
    const primaryColorInput = document.querySelector('#appearance input[type="color"]');
    primaryColorInput.addEventListener('input', function() {
        const primaryBtn = previewDiv.querySelector('.bg-primary');
        primaryBtn.style.backgroundColor = this.value;
    });

    // File upload preview
    const logoInput = document.querySelector('#appearance input[type="file"]');
    logoInput.addEventListener('change', function() {
        const file = this.files[0];
        if (file) {
            // In a real app, you'd upload and show preview
            showNotification(`Logo "${file.name}" selected for upload.`, 'info');
        }
    });

    // Notification function
    function showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        // Add to page
        document.body.appendChild(notification);

        // Auto remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }

    // Settings change tracking
    let hasUnsavedChanges = false;
    const allInputs = document.querySelectorAll('input, select, textarea');
    allInputs.forEach(input => {
        input.addEventListener('input', function() {
            hasUnsavedChanges = true;
            updateSaveButton();
        });
    });

    function updateSaveButton() {
        const saveBtn = document.querySelector('.btn-success');
        if (hasUnsavedChanges) {
            saveBtn.innerHTML = '<i class="bi bi-exclamation-circle me-2"></i>Save Changes';
            saveBtn.classList.add('btn-warning');
            saveBtn.classList.remove('btn-success');
        } else {
            saveBtn.innerHTML = '<i class="bi bi-check-circle me-2"></i>Save All Changes';
            saveBtn.classList.add('btn-success');
            saveBtn.classList.remove('btn-warning');
        }
    }

    // Warn about unsaved changes
    window.addEventListener('beforeunload', function(e) {
        if (hasUnsavedChanges) {
            e.preventDefault();
            e.returnValue = 'You have unsaved changes. Are you sure you want to leave?';
        }
    });

    // Reset unsaved changes flag after save
    const originalSaveFunction = window.saveAllSettings;
    window.saveAllSettings = function() {
        originalSaveFunction();
        hasUnsavedChanges = false;
        updateSaveButton();
    };
});