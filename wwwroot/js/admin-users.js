// admin-users.js - Users page functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Search functionality
    const searchInput = document.getElementById('userSearch');
    const roleFilter = document.getElementById('roleFilter');
    const statusFilter = document.getElementById('statusFilter');
    const clearFiltersBtn = document.getElementById('clearFilters');
    const table = document.getElementById('usersTable');
    const tbody = table.querySelector('tbody');

    function filterUsers() {
        const searchTerm = searchInput.value.toLowerCase();
        const roleValue = roleFilter.value.toLowerCase();
        const statusValue = statusFilter.value.toLowerCase();
        const rows = tbody.querySelectorAll('tr');

        let visibleCount = 0; // counter for visible rows

        rows.forEach(row => {
            const role = row.getAttribute('data-role') || '';
            const name = row.children[1].textContent.toLowerCase();
            const email = row.children[3].textContent.toLowerCase();
            const statusBadge = row.children[5].querySelector('[data-status]');
            const status = statusBadge ? statusBadge.getAttribute('data-status') : '';

            const matchesSearch = name.includes(searchTerm) || email.includes(searchTerm);
            const matchesRole = !roleValue || role === roleValue;  
            const matchesStatus = !statusValue || status.includes(statusValue);

            if (matchesSearch && matchesRole && matchesStatus) {
                row.style.display = '';
                visibleCount++;
                // Update sr no. dynamically based on visible rows only
                row.children[0].textContent = visibleCount;
            } else {
                row.style.display = 'none';
            }
        });
    }

    searchInput.addEventListener('input', filterUsers);
    roleFilter.addEventListener('change', filterUsers);
    statusFilter.addEventListener('change', filterUsers);

    clearFiltersBtn.addEventListener('click', function() {
        searchInput.value = '';
        roleFilter.value = '';
        statusFilter.value = '';
        filterUsers();
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

    // Add User Modal functionality
    const addUserModal = document.getElementById('addUserModal');
    const addUserForm = addUserModal.querySelector('form');

    addUserModal.addEventListener('shown.bs.modal', function() {
        // Focus on first input
        addUserForm.querySelector('input').focus();
    });

    // Form validation and submission
    addUserForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Basic validation
        const firstName = this.querySelector('input[placeholder*="first name"]').value.trim();
        const lastName = this.querySelector('input[placeholder*="last name"]').value.trim();
        const email = this.querySelector('input[type="email"]').value.trim();
        const password = this.querySelector('input[type="password"]').value.trim();

        if (!firstName || !lastName || !email || !password) {
            alert('Please fill in all required fields.');
            return;
        }

        if (!email.includes('@')) {
            alert('Please enter a valid email address.');
            return;
        }

        // Simulate adding user
        alert('User added successfully!');

        // Reset form and close modal
        this.reset();
        bootstrap.Modal.getInstance(addUserModal).hide();
    });

    // Action buttons (view, edit, delete, approve, reject)
    tbody.addEventListener('click', function(e) {
        const button = e.target.closest('button');
        if (!button) return;

        const row = button.closest('tr');
        const name = row.children[1].textContent.trim();
        const email = row.children[2].textContent.trim();

        if (button.querySelector('.bi-eye')) {
            // View profile
            alert(`Viewing profile for ${name} (${email})`);
        } else if (button.querySelector('.bi-pencil')) {
            // Edit user
            alert(`Editing ${name} (${email})`);
        } else if (button.querySelector('.bi-trash')) {
            // Delete user
            if (confirm(`Are you sure you want to delete ${name}?`)) {
                row.remove();
                alert('User deleted successfully!');
            }
        } else if (button.querySelector('.bi-check-circle')) {
            // Approve pending user
            row.children[5].innerHTML = '<span class="badge bg-success">Active</span>';
            row.children[6].textContent = 'Just now';
            button.closest('td').innerHTML = `
                <button class="btn btn-sm btn-light" data-bs-toggle="tooltip" title="View Profile"><i class="bi bi-eye"></i></button>
                <button class="btn btn-sm btn-light" data-bs-toggle="tooltip" title="Edit"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-light text-danger" data-bs-toggle="tooltip" title="Delete"><i class="bi bi-trash"></i></button>
            `;
            alert(`${name} has been approved!`);
        } else if (button.querySelector('.bi-x-circle')) {
            // Reject pending user
            if (confirm(`Are you sure you want to reject ${name}?`)) {
                row.remove();
                alert('User rejected and removed!');
            }
        }
    });
});