// admin-courses.js - Courses page functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Search functionality
    const searchInput = document.getElementById('courseSearch');
    const table = document.getElementById('coursesTable');
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

    // Add Course Modal functionality
    const addCourseModal = document.getElementById('addCourseModal');
    const addCourseForm = addCourseModal.querySelector('form');

    addCourseModal.addEventListener('shown.bs.modal', function() {
        // Focus on first input
        addCourseForm.querySelector('input').focus();
    });

    // Form validation and submission
    addCourseForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const courseCode = this.querySelector('input[placeholder*="CS-101"]').value.trim();
        const courseName = this.querySelector('input[placeholder*="Programming Fundamentals"]').value.trim();

        if (!courseCode || !courseName) {
            alert('Please fill in all required fields.');
            return;
        }

        // Simulate adding course
        alert('Course added successfully!');

        // Reset form and close modal
        this.reset();
        bootstrap.Modal.getInstance(addCourseModal).hide();
    });

    // Action buttons (view, assign, edit, delete)
    tbody.addEventListener('click', function(e) {
        const button = e.target.closest('button');
        if (!button) return;

        const row = button.closest('tr');
        const courseCode = row.children[1].textContent;
        const courseName = row.children[2].textContent;

        if (button.querySelector('.bi-eye')) {
            // View course details
            alert(`Viewing details for ${courseCode}: ${courseName}`);
        } else if (button.querySelector('.bi-person-plus')) {
            // Assign instructor
            const instructor = prompt('Enter instructor name:');
            if (instructor) {
                row.children[6].textContent = instructor;
                alert(`Instructor ${instructor} assigned to ${courseCode}`);
            }
        } else if (button.querySelector('.bi-pencil')) {
            // Edit course
            alert(`Editing ${courseCode}: ${courseName}`);
        } else if (button.querySelector('.bi-trash')) {
            // Delete course
            if (confirm(`Are you sure you want to delete ${courseCode}: ${courseName}?`)) {
                row.remove();
                alert('Course deleted successfully!');
            }
        }
    });
});