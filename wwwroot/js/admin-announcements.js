// admin-announcements.js - Announcements page functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Search functionality
    const searchInput = document.getElementById('announcementSearch');
    const announcementsList = document.querySelector('.announcements-list');
    const announcementItems = announcementsList.querySelectorAll('.announcement-item');

    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();

        announcementItems.forEach(item => {
            const title = item.querySelector('h6').textContent.toLowerCase();
            const content = item.querySelector('p:not(.text-muted)').textContent.toLowerCase();

            item.style.display = (title.includes(searchTerm) || content.includes(searchTerm)) ? '' : 'none';
        });
    });

    // New Announcement Modal functionality
    const newAnnouncementModal = document.getElementById('newAnnouncementModal');
    const newAnnouncementForm = newAnnouncementModal.querySelector('form');

    newAnnouncementModal.addEventListener('shown.bs.modal', function() {
        // Focus on title input
        newAnnouncementForm.querySelector('input[placeholder*="title"]').focus();
    });

    // Form validation and submission
    newAnnouncementForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const title = this.querySelector('input[placeholder*="title"]').value.trim();
        const content = this.querySelector('textarea').value.trim();

        if (!title || !content) {
            alert('Please fill in title and content.');
            return;
        }

        // Simulate posting announcement
        alert('Announcement posted successfully!');

        // Reset form and close modal
        this.reset();
        bootstrap.Modal.getInstance(newAnnouncementModal).hide();

        // Optionally refresh the list (in a real app, this would reload data)
        location.reload();
    });

    // Action buttons (edit, delete)
    announcementsList.addEventListener('click', function(e) {
        const button = e.target.closest('button');
        if (!button) return;

        const item = button.closest('.announcement-item');
        const title = item.querySelector('h6').textContent;

        if (button.querySelector('.bi-pencil')) {
            // Edit announcement
            alert(`Editing announcement: ${title}`);
        } else if (button.querySelector('.bi-trash')) {
            // Delete announcement
            if (confirm(`Are you sure you want to delete "${title}"?`)) {
                item.remove();
                alert('Announcement deleted successfully!');
            }
        }
    });

    // Auto-hide expired announcements after some time (simulation)
    setTimeout(() => {
        const expiredItems = announcementsList.querySelectorAll('.announcement-item');
        expiredItems.forEach(item => {
            if (item.querySelector('.badge.bg-secondary')) {
                item.style.opacity = '0.6';
            }
        });
    }, 1000);
});