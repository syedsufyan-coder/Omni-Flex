//  OMNI-FLEX — Admin Users CRUD  (admin-users.js)

document.addEventListener('DOMContentLoaded', function () {

    // User Search 
    const searchInput = document.getElementById('userSearch');
    const tbody = document.querySelector('#usersTable tbody');

    if (searchInput && tbody) {
        searchInput.addEventListener('input', function () {
            const q = this.value.toLowerCase();
            tbody.querySelectorAll('tr').forEach(row => {
                row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
            });
        });
    }

    // Role filter 
    const roleFilter = document.getElementById('roleFilter');
    if (roleFilter) {
        roleFilter.addEventListener('change', function () {
            const role = this.value.toLowerCase();
            tbody.querySelectorAll('tr').forEach(row => {
                const rowRole = (row.getAttribute('data-role') || '').toLowerCase();
                row.style.display = (!role || rowRole === role) ? '' : 'none';
            });
        });
    }
});

// Clear filters 
function clearFilters() {
    document.getElementById('userSearch').value = '';
    document.getElementById('roleFilter').value = '';
    document.querySelectorAll('#usersTable tbody tr').forEach(r => r.style.display = '');
}

//  Add User
function openAddUserModal() {
    // Clear all inputs
    document.getElementById('add_userId').value      = '';
    document.getElementById('add_firstName').value  = '';
    document.getElementById('add_lastName').value   = '';
    document.getElementById('add_email').value      = '';
    document.getElementById('add_phone').value      = '';
    document.getElementById('add_address').value    = '';
    document.getElementById('add_city').value       = '';
    document.getElementById('add_dob').value        = '';
    document.getElementById('add_deptId').value     = '';
    document.getElementById('add_role').value       = '';
    document.getElementById('add_gender').value     = '';
    document.getElementById('add_status').value     = 'Active';
    document.getElementById('add_country').value    = 'Pakistan';

    // Hide role sections
    document.getElementById('add_roleDetails').style.display      = 'none';
    document.getElementById('add_studentFields').style.display    = 'none';
    document.getElementById('add_instructorFields').style.display = 'none';
    document.getElementById('add_adminFields').style.display      = 'none';

    hideAlert('addUserError');
    hideAlert('addUserSuccess');

    new bootstrap.Modal(document.getElementById('addUserModal')).show();
}

// Role dropdown change --> show relevant fields
function onAddRoleChange() {
    const role = document.getElementById('add_role').value;

    document.getElementById('add_roleDetails').style.display      = 'none';
    document.getElementById('add_studentFields').style.display    = 'none';
    document.getElementById('add_instructorFields').style.display = 'none';
    document.getElementById('add_adminFields').style.display      = 'none';

    if (!role) return;

    document.getElementById('add_roleDetails').style.display = 'block';

    if (role === 'Student')
        document.getElementById('add_studentFields').style.display = 'block';
    else if (role === 'Instructor')
        document.getElementById('add_instructorFields').style.display = 'block';
    else if (role === 'Admin')
        document.getElementById('add_adminFields').style.display = 'block';
}

function submitAddUser() {
    hideAlert('addUserError');
    hideAlert('addUserSuccess');

    const role = document.getElementById('add_role').value;

    const payload = {
        userId:      document.getElementById('add_userId').value.trim(),
        firstName:   document.getElementById('add_firstName').value.trim(),
        lastName:    document.getElementById('add_lastName').value.trim(),
        email:       document.getElementById('add_email').value.trim(),
        deptId:      document.getElementById('add_deptId').value,
        role:        role,
        status:      document.getElementById('add_status').value,
        gender:      document.getElementById('add_gender').value,
        dOB:         document.getElementById('add_dob').value || null,
        phoneNumber: document.getElementById('add_phone').value.trim(),
        address:     document.getElementById('add_address').value.trim(),
        city:        document.getElementById('add_city').value.trim(),
        country:     document.getElementById('add_country').value.trim(),
        passwordHash: 'default@123'
    };

    // Role-specific fields
    if (role === 'Student') {
        payload.batch  = parseInt(document.getElementById('add_batch').value) || null;
        payload.degree = document.getElementById('add_degree').value;
    }
    if (role === 'Instructor') {
        payload.designation    = document.getElementById('add_designation').value;
        payload.officeRoom     = document.getElementById('add_officeRoom').value.trim();
        payload.specialization = document.getElementById('add_specialization').value.trim();
    }
    if (role === 'Admin') {
        payload.designation = document.getElementById('add_adminDesignation').value.trim();
        payload.officeRoom  = document.getElementById('add_adminOfficeRoom').value.trim();
    }

    // Validation
    if (!payload.userId)                            return showAlert('addUserError', 'User ID is required');
    if (!payload.firstName)                         return showAlert('addUserError', 'First Name is required');
    if (!payload.lastName)                          return showAlert('addUserError', 'Last Name is required');
    if (!payload.email || !payload.email.includes('@')) return showAlert('addUserError', 'Valid email is required');
    if (!payload.deptId)                            return showAlert('addUserError', 'Department is required');
    if (!payload.role)                              return showAlert('addUserError', 'Role is required');
    if (role === 'Student' && !payload.batch)       return showAlert('addUserError', 'Batch Year is required');
    if (role === 'Student' && !payload.degree)      return showAlert('addUserError', 'Degree is required');
    if (role === 'Instructor' && !payload.designation) return showAlert('addUserError', 'Designation is required');

    const btn = document.querySelector('#addUserModal .btn-primary');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Saving...';

    $.ajax({
        url: '/Admin/CreateUser',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.success) {
                showAlert('addUserSuccess', res.message);
                setTimeout(() => {
                    bootstrap.Modal.getInstance(document.getElementById('addUserModal')).hide();
                    location.reload();
                }, 1000);
            } else {
                showAlert('addUserError', res.message);
            }
        },
        error: function () {
            showAlert('addUserError', 'Network error. Please try again.');
        },
        complete: function () {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-person-plus me-1"></i>Save User';
        }
    });
}

//  View User Profile
function viewUser(userId) {
    document.getElementById('viewUserBody').innerHTML = `
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">Loading profile...</p>
        </div>`;

    new bootstrap.Modal(document.getElementById('viewUserModal')).show();

    $.ajax({
        url: '/Admin/GetUser',
        type: 'GET',
        data: { id: userId },
        success: function (res) {
            if (!res.success) {
                document.getElementById('viewUserBody').innerHTML =
                    `<div class="alert alert-danger">${res.message}</div>`;
                return;
            }

            const u = res.data;
            const initials = `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
            const badgeColor = u.role === 'Admin' ? 'danger'
                : u.role === 'Instructor' ? 'warning text-dark' : 'info text-dark';

            let roleSection = '';
            if (u.role === 'Student') {
                roleSection = `
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Batch Year</span>
                        <p class="fw-semibold">${u.batch ?? '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Degree</span>
                        <p class="fw-semibold">${u.degree ?? '—'}</p>
                    </div>`;
            } else if (u.role === 'Instructor' || u.role === 'Admin') {
                roleSection = `
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Designation</span>
                        <p class="fw-semibold">${u.designation ?? '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Office Room</span>
                        <p class="fw-semibold">${u.officeRoom ?? '—'}</p>
                    </div>
                    <div class="col-12">
                        <span class="text-muted small d-block">Specialization</span>
                        <p class="fw-semibold">${u.specialization ?? '—'}</p>
                    </div>`;
            }

            document.getElementById('viewUserBody').innerHTML = `
                <div class="d-flex align-items-center gap-3 p-3 mb-4 rounded"
                     style="background:#f0f4ff;">
                    <div class="rounded-circle bg-primary text-white fw-bold
                                d-flex align-items-center justify-content-center"
                         style="width:70px;height:70px;font-size:22px;flex-shrink:0;">
                        ${initials}
                    </div>
                    <div>
                        <h5 class="mb-1">${u.firstName} ${u.lastName}</h5>
                        <span class="badge bg-${badgeColor} me-1">${u.role}</span>
                        <span class="badge ${u.status === 'Active' ? 'bg-success' : 'bg-secondary'}">
                            ${u.status}
                        </span>
                        <p class="text-muted mb-0 mt-1 small"><code>${u.userId}</code></p>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Email</span>
                        <p class="fw-semibold">${u.email}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Department</span>
                        <p class="fw-semibold">${u.deptId || '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Phone</span>
                        <p class="fw-semibold">${u.phoneNumber || '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Gender</span>
                        <p class="fw-semibold">${u.gender || '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Date of Birth</span>
                        <p class="fw-semibold">${u.dOB || u.dob || u.DOB ? new Date(u.dOB || u.dob || u.DOB).toLocaleDateString() : '—'}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">City</span>
                        <p class="fw-semibold">${u.city || '—'}</p>
                    </div>
                    ${roleSection}
                </div>`;
        },
        error: function () {
            document.getElementById('viewUserBody').innerHTML =
                `<div class="alert alert-danger">Failed to load profile.</div>`;
        }
    });
}

//  Edit User
function openEditUserModal(userId) {
    hideAlert('editUserError');
    hideAlert('editUserSuccess');

    // Hide all role-specific rows
    document.querySelectorAll('.edit_studentRow, .edit_instructorRow, .edit_adminRow')
            .forEach(r => r.style.display = 'none');

    $.ajax({
        url: '/Admin/GetUser',
        type: 'GET',
        data: { id: userId },
        success: function (res) {
            if (!res.success) {
                alert('Error loading user: ' + res.message);
                return;
            }

            const u = res.data;

            // Store hidden values
            document.getElementById('edit_userId').value = u.userId;
            document.getElementById('edit_role').value   = u.role;

            // Fill current value columns (disabled — read only display)
            document.getElementById('edit_cur_userId').value    = u.userId;
            document.getElementById('edit_cur_firstName').value = u.firstName;
            document.getElementById('edit_cur_lastName').value  = u.lastName;
            document.getElementById('edit_cur_email').value     = u.email;
            document.getElementById('edit_cur_dept').value      = u.deptId || '';
            document.getElementById('edit_cur_phone').value     = u.phoneNumber || '';
            document.getElementById('edit_cur_gender').value    = u.gender || '';
            document.getElementById('edit_cur_city').value      = u.city || '';
            document.getElementById('edit_cur_status').value    = u.status || '';

            // Pre-fill new value columns with current values of respective user
            document.getElementById('edit_new_firstName').value = u.firstName;
            document.getElementById('edit_new_lastName').value  = u.lastName;
            document.getElementById('edit_new_email').value     = u.email;
            document.getElementById('edit_new_dept').value      = u.deptId || '';
            document.getElementById('edit_new_phone').value     = u.phoneNumber || '';
            document.getElementById('edit_new_gender').value    = u.gender || '';
            document.getElementById('edit_new_city').value      = u.city || '';
            document.getElementById('edit_new_status').value    = u.status || 'Active';

            // Role-specific rows - show only the rows relevant to this user's role
            if (u.role === 'Student') {
                // Show Student-specific rows
                document.querySelectorAll('.edit_studentRow')
                        .forEach(r => r.style.display = '');
                document.getElementById('edit_cur_batch').value  = u.batch  || '';
                document.getElementById('edit_cur_degree').value = u.degree || '';
                document.getElementById('edit_new_batch').value  = u.batch  || '';
                document.getElementById('edit_new_degree').value = u.degree || '';
            } else if (u.role === 'Instructor') {
                // Show Instructor-specific rows only
                document.querySelectorAll('.edit_instructorRow')
                        .forEach(r => r.style.display = '');
                document.getElementById('edit_cur_designation').value = u.designation    || '';
                document.getElementById('edit_cur_office').value      = u.officeRoom     || '';
                document.getElementById('edit_cur_spec').value        = u.specialization || '';
                document.getElementById('edit_new_designation').value = u.designation    || '';
                document.getElementById('edit_new_office').value      = u.officeRoom     || '';
                document.getElementById('edit_new_spec').value        = u.specialization || '';
            } else if (u.role === 'Admin') {
                // Show Admin-specific rows only
                document.querySelectorAll('.edit_adminRow')
                        .forEach(r => r.style.display = '');
                document.getElementById('edit_cur_adminDesignation').value = u.designation    || '';
                document.getElementById('edit_cur_adminOffice').value      = u.officeRoom     || '';
                document.getElementById('edit_new_adminDesignation').value = u.designation    || '';
                document.getElementById('edit_new_adminOffice').value      = u.officeRoom     || '';
            }

            new bootstrap.Modal(document.getElementById('editUserModal')).show();
        },
        error: function () {
            alert('Network error loading user.');
        }
    });
}

function submitEditUser() {
    hideAlert('editUserError');
    hideAlert('editUserSuccess');

    const role   = document.getElementById('edit_role').value;
    const userId = document.getElementById('edit_userId').value;

    const payload = {
        userId:      userId,
        role:        role,
        firstName:   document.getElementById('edit_new_firstName').value.trim(),
        lastName:    document.getElementById('edit_new_lastName').value.trim(),
        email:       document.getElementById('edit_new_email').value.trim(),
        deptId:      document.getElementById('edit_new_dept').value,
        phoneNumber: document.getElementById('edit_new_phone').value.trim(),
        gender:      document.getElementById('edit_new_gender').value,
        city:        document.getElementById('edit_new_city').value.trim(),
        status:      document.getElementById('edit_new_status').value
    };

    // Role-specific Attributes
    if (role === 'Student') {
        payload.batch  = parseInt(document.getElementById('edit_new_batch').value) || null;
        payload.degree = document.getElementById('edit_new_degree').value;
    } else if (role === 'Instructor') {
        payload.designation    = document.getElementById('edit_new_designation').value;
        payload.officeRoom     = document.getElementById('edit_new_office').value.trim();
        payload.specialization = document.getElementById('edit_new_spec').value.trim();
    } else if (role === 'Admin') {
        payload.designation    = document.getElementById('edit_new_adminDesignation').value.trim();
        payload.officeRoom     = document.getElementById('edit_new_adminOffice').value.trim();
        payload.specialization = null; // Admin doesn't have specialization
    }

    // Validation
    if (!payload.firstName)
        return showAlert('editUserError', 'First Name cannot be empty');
    if (!payload.lastName)
        return showAlert('editUserError', 'Last Name cannot be empty');
    if (!payload.email || !payload.email.includes('@'))
        return showAlert('editUserError', 'Valid email is required');

    const btn = document.querySelector('#editUserModal .btn-warning');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Updating...';

    $.ajax({
        url: '/Admin/UpdateUser',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.success) {
                showAlert('editUserSuccess', res.message);
                setTimeout(() => {
                    bootstrap.Modal.getInstance(document.getElementById('editUserModal')).hide();
                    location.reload();
                }, 1000);
            } else {
                showAlert('editUserError', res.message);
            }
        },
        error: function () {
            showAlert('editUserError', 'Network error. Please try again.');
        },
        complete: function () {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-pencil me-1"></i>Update User';
        }
    });
}

//  Delete User
function openDeleteUserModal(userId, fullName) {
    document.getElementById('delete_userId').value        = userId;
    document.getElementById('delete_userName').textContent = `${fullName} (${userId})`;
    hideAlert('deleteUserError');
    new bootstrap.Modal(document.getElementById('deleteUserModal')).show();
}

function submitDeleteUser() {
    const userId = document.getElementById('delete_userId').value;

    const btn = document.querySelector('#deleteUserModal .btn-danger');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Deleting...';

    $.ajax({
        url: '/Admin/DeleteUser',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(userId),
        success: function (res) {
            if (res.success) {
                bootstrap.Modal.getInstance(document.getElementById('deleteUserModal')).hide();
                // Remove row from DOM without full page reload
                const row = document.querySelector(`#usersTable tr[data-user-id="${userId}"]`);
                if (row) {
                    row.style.transition = 'opacity 0.3s';
                    row.style.opacity    = '0';
                    setTimeout(() => row.remove(), 300);
                }
                showToast(res.message, 'success');
            } else {
                showAlert('deleteUserError', res.message);
                btn.disabled = false;
                btn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
            }
        },
        error: function () {
            showAlert('deleteUserError', 'Network error. Please try again.');
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
        }
    });
}

// ASSIGN COURSE TO INSTRUCTOR FUNCTIONALITY - to be continued after merge

function openAssignCourseModal(instructorId) {
    document.getElementById('assign_instructorId').value = instructorId;
    document.getElementById('assign_instructorName').textContent = 'Loading...';
    document.getElementById('assign_courseId').innerHTML = '<option value="">Loading courses...</option>';
    document.getElementById('sectionSelectionDiv').style.display = 'none';
    document.getElementById('currentAssignedCourses').innerHTML = '<p class="text-muted small">Loading...</p>';
    hideAlert('assignCourseError');
    hideAlert('assignCourseSuccess');

    // Get instructor details
    $.ajax({
        url: '/Admin/GetUser',
        type: 'GET',
        data: { id: instructorId },
        success: function (res) {
            if (res.success) {
                document.getElementById('assign_instructorName').textContent = 
                    `${res.data.firstName} ${res.data.lastName} (${res.data.userId})`;
            }
        }
    });

    // Get all active courses
    $.ajax({
        url: '/Admin/GetActiveCourses',
        type: 'GET',
        success: function (res) {
            if (res.success && res.data) {
                let options = '<option value="">-- Select a Course --</option>';
                res.data.forEach(function (course) {
                    options += `<option value="${course.courseId}">${course.courseId} - ${course.courseName}</option>`;
                });
                document.getElementById('assign_courseId').innerHTML = options;
            } else {
                document.getElementById('assign_courseId').innerHTML = 
                    '<option value="">No courses available</option>';
            }
        },
        error: function () {
            document.getElementById('assign_courseId').innerHTML = 
                '<option value="">Error loading courses</option>';
        }
    });

    // Get instructor's current assignments
    $.ajax({
        url: '/Admin/GetInstructorAssignments',
        type: 'GET',
        data: { instructorId: instructorId },
        success: function (res) {
            if (res.success && res.data && res.data.length > 0) {
                let html = '<ul class="list-group">';
                res.data.forEach(function (assignment) {
                    html += `<li class="list-group-item d-flex justify-content-between align-items-center">
                        <div>
                            <strong>${assignment.courseId}</strong> - ${assignment.courseName}
                            <br><small class="text-muted">Section: ${assignment.sectionLabel} (${assignment.degree} - Batch ${assignment.batch})</small>
                        </div>
                    </li>`;
                });
                html += '</ul>';
                document.getElementById('currentAssignedCourses').innerHTML = html;
            } else {
                document.getElementById('currentAssignedCourses').innerHTML = 
                    '<p class="text-muted small">No courses assigned yet.</p>';
            }
        },
        error: function () {
            document.getElementById('currentAssignedCourses').innerHTML = 
                '<p class="text-muted small">Unable to load assignments.</p>';
        }
    });

    new bootstrap.Modal(document.getElementById('assignCourseModal')).show();
}

function loadSectionsForCourse() {
    const courseId = document.getElementById('assign_courseId').value;
    const sectionDiv = document.getElementById('sectionSelectionDiv');
    const sectionSelect = document.getElementById('assign_sectionId');

    if (!courseId) {
        sectionDiv.style.display = 'none';
        return;
    }

    sectionSelect.innerHTML = '<option value="">Loading sections...</option>';
    sectionDiv.style.display = 'block';

    $.ajax({
        url: '/Admin/GetSectionsByCourse',
        type: 'GET',
        data: { courseId: courseId },
        success: function (res) {
            if (res.success && res.data && res.data.length > 0) {
                let options = '<option value="">-- Select a Section --</option>';
                res.data.forEach(function (section) {
                    const isAssigned = section.instructorId !== null;
                    const disabled = isAssigned ? 'disabled' : '';
                    const label = isAssigned 
                        ? `${section.sectionLabel} (${section.degree} - Batch ${section.batch}) - Assigned to: ${section.instructorName}`
                        : `${section.sectionLabel} (${section.degree} - Batch ${section.batch})`;
                    options += `<option value="${section.sectionId}" ${disabled}>${label}</option>`;
                });
                sectionSelect.innerHTML = options;
            } else {
                sectionSelect.innerHTML = '<option value="">No sections available for this course</option>';
            }
        },
        error: function () {
            sectionSelect.innerHTML = '<option value="">Error loading sections</option>';
        }
    });
}

function submitAssignCourse() {
    hideAlert('assignCourseError');
    hideAlert('assignCourseSuccess');

    const instructorId = document.getElementById('assign_instructorId').value;
    const sectionId = document.getElementById('assign_sectionId').value;

    if (!sectionId) {
        return showAlert('assignCourseError', 'Please select a section');
    }

    const btn = document.querySelector('#assignCourseModal .btn-primary');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Assigning...';

    $.ajax({
        url: '/Admin/AssignInstructorToSection',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ instructorId: instructorId, sectionId: sectionId }),
        success: function (res) {
            if (res.success) {
                showAlert('assignCourseSuccess', res.message);
                setTimeout(() => {
                    bootstrap.Modal.getInstance(document.getElementById('assignCourseModal')).hide();
                    location.reload();
                }, 1000);
            } else {
                showAlert('assignCourseError', res.message);
            }
        },
        error: function () {
            showAlert('assignCourseError', 'Network error. Please try again.');
        },
        complete: function () {
            btn.disabled = false;
            btn.innerHTML = '<i class="bi bi-book me-1"></i>Assign Course';
        }
    });
}

// Helper functionalities

// Helper functions for alerts
function showAlert(id, msg) {
    const el = document.getElementById(id);
    if (!el) return;
    el.textContent = msg;
    el.classList.remove('d-none');
}

function hideAlert(id) {
    const el = document.getElementById(id);
    if (el) el.classList.add('d-none');
}

// Toast notification (top-right, auto-dismiss)
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} alert-dismissible fade show position-fixed shadow`;
    toast.style.cssText = 'top:20px;right:20px;z-index:9999;min-width:300px;';
    toast.innerHTML = `${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 4000);
}

// TA Functions — DISABLED until DB migration complete
/*
function assignTA(studentId) { ... }
function handleTASubmit(e) { ... }
function updateUserRoleInTable(userId, newRole) { ... }
*/