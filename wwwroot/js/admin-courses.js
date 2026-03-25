// admin-courses.js - Courses page functionality

document.addEventListener('DOMContentLoaded', function () {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Search functionality
    const searchInput = document.getElementById('courseSearch');
    const table = document.getElementById('coursesTable');
    const tbody = table.querySelector('tbody');

    searchInput.addEventListener('input', function () {
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
        header.addEventListener('click', function () {
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

    addCourseModal.addEventListener('shown.bs.modal', function () {
        // Focus on first input
        addCourseForm.querySelector('input').focus();
    });

    // Form validation and submission
    addCourseForm.addEventListener('submit', function (e) {
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
    tbody.addEventListener('click', function (e) {
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
})

// Called when user changes the filter type dropdown
// Shows the value input and updates placeholder to guide the user
function onFilterTypeChange() {
    var filterType = document.getElementById('filterType').value;
    var valueRow = document.getElementById('filterValueRow');
    var filterValue = document.getElementById('filterValue');
    var filterLabel = document.getElementById('filterValueLabel');

    // Hide any previous error when user changes selection
    hideFilterError();

    // If nothing selected, hide the value input row
    if (!filterType) {
        valueRow.style.display = 'none';
        return;
    }

    // Show the value input row
    valueRow.style.display = 'flex';

    // Each filter type gets its own label and placeholder
    // This tells the user exactly what to type
    var placeholders = {
        'TeacherId': { label: 'Teacher ID', placeholder: 'e.g. INS1' },
        'DeptId': { label: 'Department ID', placeholder: 'e.g. CS' },
        'CourseType': { label: 'Course Type', placeholder: 'Theory  or  Lab' },
        'CourseCat': { label: 'Course Category', placeholder: 'Core or  Elective' },
        'SectionId': { label: 'Section ID', placeholder: 'e.g. BCS-4G' },
        'PreReqId': { label: 'Prerequisite Course ID', placeholder: 'e.g. CS2001' },
        'CreditHrs': { label: 'Credit Hours', placeholder: '1, 2, 3 or 4' }
    };

    // Apply correct label and placeholder for the selected filter
    filterLabel.textContent = placeholders[filterType].label;
    filterValue.placeholder = placeholders[filterType].placeholder;
    filterValue.value = ''; // Clear previous value
    filterValue.focus();   // Auto focus so user can type immediately
}

// Called when user clicks the Search button
function filterCourses() {
    var filterType = document.getElementById('filterType').value;
    var filterValue = document.getElementById('filterValue').value.trim();

    // CLIENT SIDE VALIDATION 1 — No filter type selected
    if (!filterType) {
        showFilterError('Please Select a Filter Type First');
        return;
    }

    // CLIENT SIDE VALIDATION 2 — Value box is empty
    if (!filterValue) {
        showFilterError('Please Enter a Search Value');
        return;
    }

    // CLIENT SIDE VALIDATION 3 — CreditHrs must be a number between 1 and 4
    if (filterType === 'CreditHrs') {
        var num = parseInt(filterValue);
        if (isNaN(num)) {
            showFilterError('Credits Must be a Positive Number');
            return;
        }
        if (num < 1 || num > 4) {
            showFilterError('Credit Hours Must be Between 1 and 4');
            return;
        }
    }

    // Build the request object — only fill the ONE field that matches filterType
    var requestData = {};

    // CreditHrs must be sent as a number, all others as string
    if (filterType === 'CreditHrs') {
        requestData[filterType] = parseInt(filterValue);
    } else {
        requestData[filterType] = filterValue;
    }

    // Show loading spinner on button so user knows request is in progress
    var btn = document.getElementById('filterBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Searching...';

    // Hide any previous error
    hideFilterError();

    // AJAX call using jQuery — sends requestData as JSON to AdminController.FilterCourses()
    $.ajax({
        url: '/Admin/FilterCourses', // URL of our controller action
        type: 'POST',                 // HTTP POST because we are sending data
        contentType: 'application/json',     // Tell server we are sending JSON
        data: JSON.stringify(requestData), // Convert JS object to JSON string

        // Called when server responds successfully
        success: function (response) {
            // Restore button to normal
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-search me-1"></i>Search';

            if (response.success) {
                // Server returned filtered courses — update the table
                renderFilteredCourses(response.data);
            } else {
                // Server returned a validation error — show it
                showFilterError(response.message);
            }
        },

        // Called when network error or server crash happens
        error: function () {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-search me-1"></i>Search';
            showFilterError('Ops ! Something Went Wrong. Please Try Again.');
        }
    });
}

// Rebuilds the table rows with filtered courses received from the server
function renderFilteredCourses(courses) {
    var tbody = document.querySelector('#coursesTable tbody');

    // No courses found — show friendly empty state
    if (!courses || courses.length === 0) {
        tbody.innerHTML =
            '<tr>' +
            '<td colspan="8" class="text-center text-muted py-4">' +
            '<i class="bi bi-inbox fs-4 d-block mb-2"></i>' +
            'No courses found for this filter' +
            '</td>' +
            '</tr>';
        return;
    }

    // Build HTML rows from the courses array
    // CourseDto properties: courseId, courseName, creditHours, courseType, courseCat, preRequisite
    var html = '';
    $.each(courses, function (index, course) {
        html +=
            '<tr>' +
            '<td>' + (index + 1) + '</td>' +
            '<td>' + course.courseId + '</td>' +
            '<td>' + course.courseName + '</td>' +
            '<td>' + course.creditHours + '</td>' +
            '<td>' + course.courseType + '</td>' +
            '<td>' + course.courseCat + '</td>' +
            '<td><span class="badge bg-success">Active</span></td>' +
            '<td>' +
            '<button class="btn btn-sm btn-outline-secondary" data-bs-toggle="tooltip" title="View"><i class="bi bi-eye"></i></button> ' +
            '<button class="btn btn-sm btn-outline-warning"   data-bs-toggle="tooltip" title="Edit"><i class="bi bi-pencil"></i></button> ' +
            '<button class="btn btn-sm btn-outline-danger"    data-bs-toggle="tooltip" title="Delete"><i class="bi bi-trash"></i></button>' +
            '</td>' +
            '</tr>';
    });

    // Replace all existing rows with the filtered results
    tbody.innerHTML = html;
}

// Resets filter form and reloads the original full course list
function clearFilter() {
    document.getElementById('filterType').value = '';
    document.getElementById('filterValue').value = '';
    hideFilterError();

    // Hide value input row until user selects a filter again
    document.getElementById('filterValueRow').style.display = 'none';

    // Reload page to restore the original unfiltered course list
    location.reload();
}

// Shows error message below the filter form
function showFilterError(message) {
    var errorDiv = document.getElementById('filterError');
    errorDiv.textContent = message;
    errorDiv.style.display = 'block';
}

// Hides the error message
function hideFilterError() {
    var errorDiv = document.getElementById('filterError');
    errorDiv.style.display = 'none';
}

// COURSES CRUD
var currentDeleteCourseId = null;
// EDIT button click — load course data into edit modal
document.addEventListener('DOMContentLoaded', function () {

    // Edit button click
    document.querySelector('#coursesTable tbody').addEventListener('click', function (e) {
        var btn = e.target.closest('button');
        if (!btn) return;

        var row = btn.closest('tr');
        var courseId = row.children[1].textContent.trim();

        if (btn.querySelector('.bi-pencil')) {
            openEditCourseModal(courseId);
        }

        if (btn.querySelector('.bi-trash')) {
            openDeleteCourseModal(courseId, row.children[2].textContent.trim());
        }
    });
});

// Open Edit Modal — fetch current data from server
function openEditCourseModal(courseId) {
    // Clear previous values
    document.getElementById('editCourseError').classList.add('d-none');
    document.getElementById('editCourseSuccess').classList.add('d-none');

    $.ajax({
        url: '/Admin/GetCourse',
        type: 'GET',
        data: { id: courseId },
        success: function (response) {
            if (response.success) {
                var c = response.data;
                // Fill current value fields
                document.getElementById('edit_courseId').value = c.courseId;
                document.getElementById('edit_current_courseId').value = c.courseId;
                document.getElementById('edit_current_courseName').value = c.courseName;
                document.getElementById('edit_current_deptId').value = c.deptId;
                document.getElementById('edit_current_creditHrs').value = c.creditHrs;
                document.getElementById('edit_current_courseType').value = c.courseType;
                document.getElementById('edit_current_courseCat').value = c.courseCat;
                document.getElementById('edit_current_preReqId').value = c.preReqId || 'None';

                // Clear new value fields
                document.getElementById('edit_new_courseName').value = '';
                document.getElementById('edit_new_deptId').value = '';
                document.getElementById('edit_new_creditHrs').value = '';
                document.getElementById('edit_new_courseType').value = '';
                document.getElementById('edit_new_courseCat').value = '';
                document.getElementById('edit_new_preReqId').value = '';

                // Open modal
                new bootstrap.Modal(document.getElementById('editCourseModal')).show();
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function () {
            alert('Failed to Load Course Data !');
        }
    });
}

// Submit Create Course
function submitCreateCourse() {
    var errorDiv = document.getElementById('createCourseError');
    var successDiv = document.getElementById('createCourseSuccess');
    errorDiv.classList.add('d-none');
    successDiv.classList.add('d-none');

    // Read form values
    var courseId = document.getElementById('create_courseId').value.trim();
    var courseName = document.getElementById('create_courseName').value.trim();
    var deptId = document.getElementById('create_deptId').value;
    var creditHrs = document.getElementById('create_creditHrs').value;
    var courseType = document.getElementById('create_courseType').value;
    var courseCat = document.getElementById('create_courseCat').value;
    var preReqId = document.getElementById('create_preReqId').value.trim();

    // Client side validation
    if (!courseId) return showModalError(errorDiv, 'Course Code is required');
    if (!courseName) return showModalError(errorDiv, 'Course Name is required');
    if (!deptId) return showModalError(errorDiv, 'Department is required');
    if (!creditHrs) return showModalError(errorDiv, 'Credit Hours is required');
    if (!courseType) return showModalError(errorDiv, 'Course Type is required');
    if (!courseCat) return showModalError(errorDiv, 'Course Category is required');

    var data = {
        courseId: courseId,
        courseName: courseName,
        deptId: deptId,
        creditHrs: parseInt(creditHrs),
        courseType: courseType,
        courseCat: courseCat,
        preReqId: preReqId || null,
        isActive: 1
    };

    $.ajax({
        url: '/Admin/CreateCourse',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                successDiv.textContent = response.message;
                successDiv.classList.remove('d-none');
                // Reload page after 1.5 seconds
                setTimeout(function () { location.reload(); }, 1500);
            } else {
                showModalError(errorDiv, response.message);
            }
        },
        error: function () {
            showModalError(errorDiv, 'Something went wrong | Please try again.');
        }
    });
}

// Submit Edit Course
function submitEditCourse() {
    var errorDiv = document.getElementById('editCourseError');
    var successDiv = document.getElementById('editCourseSuccess');
    errorDiv.classList.add('d-none');
    successDiv.classList.add('d-none');

    var courseId = document.getElementById('edit_courseId').value;

    // New values — if empty use current values
    var courseName = document.getElementById('edit_new_courseName').value.trim()
        || document.getElementById('edit_current_courseName').value.trim();
    var deptId = document.getElementById('edit_new_deptId').value
        || document.getElementById('edit_current_deptId').value;
    var creditHrs = document.getElementById('edit_new_creditHrs').value
        || document.getElementById('edit_current_creditHrs').value;
    var courseType = document.getElementById('edit_new_courseType').value
        || document.getElementById('edit_current_courseType').value;
    var courseCat = document.getElementById('edit_new_courseCat').value
        || document.getElementById('edit_current_courseCat').value;
    var preReqId = document.getElementById('edit_new_preReqId').value.trim()
        || document.getElementById('edit_current_preReqId').value.trim();

    if (preReqId === 'None') preReqId = null;

    var data = {
        courseId: courseId,
        courseName: courseName,
        deptId: deptId,
        creditHrs: parseInt(creditHrs),
        courseType: courseType,
        courseCat: courseCat,
        preReqId: preReqId || null,
        isActive: 1
    };

    $.ajax({
        url: '/Admin/UpdateCourse',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                successDiv.textContent = response.message;
                successDiv.classList.remove('d-none');
                setTimeout(function () { location.reload(); }, 1500);
            } else {
                showModalError(errorDiv, response.message);
            }
        },
        error: function () {
            showModalError(errorDiv, 'Something went wrong. Please try again.');
        }
    });
}

// Open Delete Modal 
function openDeleteCourseModal(courseId, courseName) {
    currentDeleteCourseId = courseId;

    let cleanName = courseName.replace(/[^\x20-\x7E]/g, '').trim();

    document.getElementById('delete_courseName').innerHTML =
        `<div class="p-3 mb-2 bg-light rounded border-start border-4 border-danger">
            <span class="text-muted small d-block">COURSE TO BE REMOVED:</span>
            <strong class="text-primary">${courseId}</strong> 
            <span class="mx-2 text-muted">|</span> 
            <span class="text-dark">${cleanName}</span>
        </div>`;

    const errorDiv = document.getElementById('deleteCourseError');
    if (errorDiv) {
        errorDiv.classList.add('d-none');
    }

    var deleteModal = new bootstrap.Modal(document.getElementById('deleteCourseModal'));
    deleteModal.show();
}

// submit DELETION handling
function submitDeleteCourse() {
    const errorDiv = document.getElementById('deleteCourseError');
    const btn = document.querySelector('#deleteCourseModal .btn-danger');
    const originalText = '<i class="bi bi-trash me-1"></i> Confirm Delete';

    errorDiv.classList.add('d-none');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';

    $.ajax({
        url: '/Admin/DeleteCourse',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(currentDeleteCourseId),
        success: function (response) {
            if (response.success) {
                btn.innerHTML = '<i class="bi bi-check-circle me-1"></i> Deleted';
                btn.classList.replace('btn-danger', 'btn-success');
                setTimeout(() => { location.reload(); }, 800);
            } else {
                btn.disabled = false;
                btn.innerHTML = originalText;

                errorDiv.innerHTML = `<i class="bi bi-exclamation-triangle me-2"></i> ${response.message || "Cannot Delete: Course is assigned to a section."}`;
                errorDiv.classList.remove('d-none', 'alert-danger');
                errorDiv.classList.add('alert-warning', 'animate__animated', 'animate__shakeX');
            }
        },
        error: function () {
            btn.disabled = false;
            btn.innerHTML = originalText;
            showModalError(errorDiv, 'Server connection lost. Please try again.');
        }
    });
}

// Helper — show error in modal
function showModalError(div, message) {
    div.textContent = message;
    div.classList.remove('d-none');
}