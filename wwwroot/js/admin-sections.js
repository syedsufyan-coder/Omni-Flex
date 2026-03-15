// admin-sections.js - Sections page functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Search functionality
    const searchInput = document.getElementById('sectionSearch');
    const table = document.getElementById('sectionsTable');
    const tbody = table.querySelector('tbody');

    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        const rows = tbody.querySelectorAll('tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(searchTerm) ? '' : 'none';
        });
    });

    // Sort functionality
    const sortableHeaders = document.querySelectorAll('.sortable');

    sortableHeaders.forEach(header => {
        header.addEventListener('click', function() {
            const columnIndex = Array.from(header.parentElement.children).indexOf(header);
            const rows = Array.from(tbody.querySelectorAll('tr'));
            const isAscending = header.classList.contains('asc');

            // Remove sort classes from all headers
            sortableHeaders.forEach(h => {
                h.classList.remove('asc', 'desc');
                h.querySelector('.sort-icon').className = 'bi bi-sort sort-icon ms-1';
            });

            // Sort rows
            rows.sort((a, b) => {
                const aText = a.children[columnIndex].textContent.trim();
                const bText = b.children[columnIndex].textContent.trim();

                if (isAscending) {
                    header.classList.add('desc');
                    header.querySelector('.sort-icon').className = 'bi bi-sort-down sort-icon ms-1';
                    return bText.localeCompare(aText);
                } else {
                    header.classList.add('asc');
                    header.querySelector('.sort-icon').className = 'bi bi-sort-up sort-icon ms-1';
                    return aText.localeCompare(bText);
                }
            });

            // Re-append sorted rows
            rows.forEach(row => tbody.appendChild(row));
        });
    });

    // Add Section Modal functionality
    const addSectionModal = document.getElementById('addSectionModal');
    const addSectionForm = addSectionModal.querySelector('form');

    addSectionModal.addEventListener('shown.bs.modal', function() {
        // Focus on first input
        addSectionForm.querySelector('select').focus();
    });

    // Form validation and submission
    addSectionForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const course = this.querySelector('select').value;
        const sectionCode = this.querySelector('input[placeholder*="CS-101-A"]').value.trim();
        const schedule = this.querySelector('input[placeholder*="MWF 9:00-10:30"]').value.trim();
        const room = this.querySelector('input[placeholder*="Lab-101"]').value.trim();

        if (!course || !sectionCode || !schedule || !room) {
            alert('Please fill in all required fields.');
            return;
        }

        // Simulate adding section
        alert('Section added successfully!');

        // Reset form and close modal
        this.reset();
        bootstrap.Modal.getInstance(addSectionModal).hide();
    });

    // Action buttons (view, transfer, edit, delete)
    tbody.addEventListener('click', function(e) {
        const button = e.target.closest('button');
        if (!button) return;

        const row = button.closest('tr');
        const sectionCode = row.children[1].textContent;
        const course = row.children[2].textContent;

        if (button.querySelector('.bi-eye')) {
            // View section details
            alert(`Viewing details for ${sectionCode}: ${course}`);
        } else if (button.querySelector('.bi-arrow-left-right')) {
            // Transfer students
            const targetSection = prompt('Enter target section code for transfer:');
            if (targetSection) {
                alert(`Students transferred from ${sectionCode} to ${targetSection}`);
            }
        } else if (button.querySelector('.bi-pencil')) {
            // Edit section
            alert(`Editing ${sectionCode}: ${course}`);
        } else if (button.querySelector('.bi-trash')) {
            // Delete section
            if (confirm(`Are you sure you want to delete ${sectionCode}: ${course}?`)) {
                row.remove();
                alert('Section deleted successfully!');
            }
        }
    });
});