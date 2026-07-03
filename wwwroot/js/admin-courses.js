// admin-courses.js - Courses page functionality

document.addEventListener("DOMContentLoaded", function () {
  // Initialize tooltips
  const tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]'),
  );
  const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Search functionality
  const searchInput = document.getElementById("courseSearch");
  const table = document.getElementById("coursesTable");
  const tbody = table.querySelector("tbody");

  searchInput.addEventListener("input", function () {
    const searchTerm = this.value.toLowerCase();
    const rows = tbody.querySelectorAll("tr");

    rows.forEach((row) => {
      const text = row.textContent.toLowerCase();
      row.style.display = text.includes(searchTerm) ? "" : "none";
    });
  });

  // Sort functionality
  const sortableHeaders = document.querySelectorAll(".sortable");

  sortableHeaders.forEach((header) => {
    header.addEventListener("click", function () {
      const columnIndex = Array.from(header.parentElement.children).indexOf(
        header,
      );
      const rows = Array.from(tbody.querySelectorAll("tr"));
      const isAscending = header.classList.contains("asc");

      // Remove sort classes from all headers
      sortableHeaders.forEach((h) => {
        h.classList.remove("asc", "desc");
        h.querySelector(".sort-icon").className = "bi bi-sort sort-icon ms-1";
      });

      // Sort rows
      rows.sort((a, b) => {
        const aText = a.children[columnIndex].textContent.trim();
        const bText = b.children[columnIndex].textContent.trim();

        if (isAscending) {
          header.classList.add("desc");
          header.querySelector(".sort-icon").className =
            "bi bi-sort-down sort-icon ms-1";
          return bText.localeCompare(aText);
        } else {
          header.classList.add("asc");
          header.querySelector(".sort-icon").className =
            "bi bi-sort-up sort-icon ms-1";
          return aText.localeCompare(bText);
        }
      });

      // Re-append sorted rows
      rows.forEach((row) => tbody.appendChild(row));
    });
  });
});

// Called when user changes the filter type dropdown
function onFilterTypeChange() {
  var filterType = document.getElementById("filterType").value;
  var valueRow = document.getElementById("filterValueRow");
  var filterValue = document.getElementById("filterValue");
  var filterLabel = document.getElementById("filterValueLabel");

  // Hide any previous error when user changes selection
  hideFilterError();

  // If nothing selected, hide the value input row
  if (!filterType) {
    valueRow.style.display = "none";
    return;
  }

  // Show the value input row
  valueRow.style.display = "flex";

  // Each filter type gets its own label and placeholder
  var placeholders = {
    TeacherId: { label: "Teacher ID", placeholder: "e.g. INS1" },
    DeptId: { label: "Department ID", placeholder: "e.g. CS" },
    CourseType: { label: "Course Type", placeholder: "Theory  or  Lab" },
    CourseCat: { label: "Course Category", placeholder: "Core or  Elective" },
    SectionId: { label: "Section ID", placeholder: "e.g. BCS-4G" },
    PreReqId: { label: "Prerequisite Course ID", placeholder: "e.g. CS2001" },
    CreditHrs: { label: "Credit Hours", placeholder: "1, 2, 3 or 4" },
  };

  // Apply correct label and placeholder for the selected filter
  filterLabel.textContent = placeholders[filterType].label;
  filterValue.placeholder = placeholders[filterType].placeholder;
  filterValue.value = ""; // Clear previous value
  filterValue.focus(); // Auto focus so user can type immediately
}

// Called when user clicks the Search button
function filterCourses() {
  var filterType = document.getElementById("filterType").value;
  var filterValue = document.getElementById("filterValue").value.trim();

  // CLIENT SIDE VALIDATION 1 — No filter type selected
  if (!filterType) {
    showFilterError("Please Select a Filter Type First");
    return;
  }

  // CLIENT SIDE VALIDATION 2 — Value box is empty
  if (!filterValue) {
    showFilterError("Please Enter a Search Value");
    return;
  }

  // CLIENT SIDE VALIDATION 3 — CreditHrs must be a number between 1 and 4
  if (filterType === "CreditHrs") {
    var num = parseInt(filterValue);
    if (isNaN(num)) {
      showFilterError("Credits Must be a Positive Number");
      return;
    }
    if (num < 1 || num > 4) {
      showFilterError("Credit Hours Must be Between 1 and 4");
      return;
    }
  }

  // Build the request object — only fill the ONE field that matches filterType
  var requestData = {};

  // CreditHrs must be sent as a number, all others as string
  if (filterType === "CreditHrs") {
    requestData[filterType] = parseInt(filterValue);
  } else {
    requestData[filterType] = filterValue;
  }

  // Show loading spinner on button so user knows request is in progress
  var btn = document.getElementById("filterBtn");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Searching...';

  // Hide any previous error
  hideFilterError();

  // AJAX call using jQuery
  $.ajax({
    url: "/Admin/FilterCourses",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(requestData),

    success: function (response) {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-search me-1"></i>Search';

      if (response.success) {
        renderFilteredCourses(response.data);
        // Update the total count shown in the header to reflect filtered results
        try {
          var totalSpan = document.querySelector(
            "div.d-flex.justify-content-between h4 .text-muted",
          );
          var uniqueCourses = response.data.length;
          var totalAssigned = 0;
          response.data.forEach(function (c) {
            if (c.assignedSections)
              totalAssigned += parseInt(c.assignedSections);
          });
          if (totalSpan) {
            if (totalAssigned && totalAssigned !== uniqueCourses) {
              totalSpan.textContent = `(${uniqueCourses} unique / ${totalAssigned} assigned)`;
            } else {
              totalSpan.textContent = `(${uniqueCourses} total)`;
            }
          }

          // Hide server-side pagination when showing filtered results
          var paginationNav = document.querySelector(
            "nav[aria-label='Courses pagination']",
          );
          if (paginationNav) paginationNav.style.display = "none";

          // Update the small summary line under pagination
          var showingDiv = document.querySelector(
            ".text-center.text-muted.small",
          );
          if (showingDiv) {
            if (totalAssigned && totalAssigned !== uniqueCourses) {
              showingDiv.textContent = `Showing ${uniqueCourses} unique courses (${totalAssigned} assigned across sections) (Page 1 of 1)`;
            } else {
              showingDiv.textContent = `Showing ${uniqueCourses} of ${uniqueCourses} courses (Page 1 of 1)`;
            }
          }
        } catch (e) {
          // Swallow any UI update errors so filtering still works
          console.warn("Error updating pagination UI after filtering", e);
        }
      } else {
        showFilterError(response.message);
      }
    },

    error: function () {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-search me-1"></i>Search';
      // Show detailed error to help debugging (temporary)
      try {
        var args = arguments; // xhr, status, error
        var xhr = args[0];
        var status = args[1];
        var err = args[2];
        console.error("FilterCourses AJAX error", xhr, status, err);
        var respText =
          xhr && xhr.responseText ? xhr.responseText : err || "Network error";
        // Try to parse JSON message if present
        try {
          var j = JSON.parse(respText);
          if (j && j.message) respText = j.message;
        } catch (e) {}
        showFilterError("Error " + (xhr.status || "") + ": " + respText);
      } catch (e) {
        showFilterError("Ops ! Something Went Wrong. Please Try Again.");
      }
    },
  });
}

// Rebuilds the table rows with filtered courses received from the server
function renderFilteredCourses(courses) {
  var tbody = document.querySelector("#coursesTable tbody");

  // No courses found — show friendly empty state
  if (!courses || courses.length === 0) {
    tbody.innerHTML =
      "<tr>" +
      '<td colspan="8" class="text-center text-muted py-4">' +
      '<i class="bi bi-inbox fs-4 d-block mb-2"></i>' +
      "No courses found for this filter" +
      "</td>" +
      "</tr>";
    return;
  }

  // Build HTML rows from the courses array
  var html = "";
  $.each(courses, function (index, course) {
    var sectionBadge = "";
    if (course.assignedSections && parseInt(course.assignedSections) > 1) {
      sectionBadge = ` <span class="badge bg-secondary ms-1">Sections: ${course.assignedSections}</span>`;
    }

    html +=
      "<tr>" +
      "<td>" +
      (index + 1) +
      "</td>" +
      "<td>" +
      course.courseId +
      sectionBadge +
      "</td>" +
      "<td>" +
      course.courseName +
      "</td>" +
      "<td>" +
      course.creditHours +
      "</td>" +
      "<td>" +
      course.courseType +
      "</td>" +
      "<td>" +
      course.courseCat +
      "</td>" +
      '<td><span class="badge bg-success">Active</span></td>' +
      "<td>" +
      '<button class="btn btn-sm btn-outline-info" onclick="viewCourse(\'' +
      course.courseId +
      '\')" title="View"><i class="bi bi-eye"></i></button> ' +
      '<button class="btn btn-sm btn-outline-warning" onclick="openEditCourseModal(\'' +
      course.courseId +
      '\')" title="Edit"><i class="bi bi-pencil"></i></button> ' +
      '<button class="btn btn-sm btn-outline-danger" onclick="openDeleteCourseModal(\'' +
      course.courseId +
      "', '" +
      course.courseName +
      '\')" title="Delete"><i class="bi bi-trash"></i></button>' +
      "</td>" +
      "</tr>";
  });

  // Replace all existing rows with the filtered results
  tbody.innerHTML = html;
}

// Resets filter form and reloads the original full course list
function clearFilter() {
  document.getElementById("filterType").value = "";
  document.getElementById("filterValue").value = "";
  hideFilterError();

  // Hide value input row until user selects a filter again
  document.getElementById("filterValueRow").style.display = "none";

  // Reload page to restore the original unfiltered course list
  location.reload();
}

// Shows error message below the filter form
function showFilterError(message) {
  var errorDiv = document.getElementById("filterError");
  errorDiv.textContent = message;
  errorDiv.style.display = "block";
}

// Hides the error message
function hideFilterError() {
  var errorDiv = document.getElementById("filterError");
  errorDiv.style.display = "none";
}

// ==================== VIEW COURSE FUNCTIONALITY ====================

// Store modal instances to properly dispose them
var modalInstances = {};

// Helper function to open a modal and store its instance
function openModal(modalId) {
  // Dispose existing instance if any
  if (modalInstances[modalId]) {
    modalInstances[modalId].dispose();
  }

  var modalEl = document.getElementById(modalId);
  var modal = new bootstrap.Modal(modalEl);
  modalInstances[modalId] = modal;

  // Remove backdrop and modal when hidden
  modalEl.addEventListener(
    "hidden.bs.modal",
    function () {
      var backdrop = document.querySelector(".modal-backdrop");
      if (backdrop) {
        backdrop.remove();
      }
      document.body.classList.remove("modal-open");
      document.body.style.removeProperty("padding-right");
      document.body.style.removeProperty("overflow");
    },
    { once: true },
  );

  modal.show();
  return modal;
}

// Helper function to close a modal properly
function closeModal(modalId) {
  if (modalInstances[modalId]) {
    modalInstances[modalId].hide();
    modalInstances[modalId].dispose();
    delete modalInstances[modalId];
  }
}

function viewCourse(courseId) {
  document.getElementById("viewCourseBody").innerHTML = `
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">Loading course details...</p>
        </div>`;

  openModal("viewCourseModal");

  $.ajax({
    url: "/Admin/GetCourse",
    type: "GET",
    data: { id: courseId },
    success: function (res) {
      if (!res.success) {
        document.getElementById("viewCourseBody").innerHTML =
          `<div class="alert alert-danger">${res.message}</div>`;
        return;
      }

      const c = res.data;

      document.getElementById("viewCourseBody").innerHTML = `
                <div class="d-flex align-items-center gap-3 p-3 mb-4 rounded"
                     style="background:#f0f4ff;">
                    <div class="rounded-circle bg-primary text-white fw-bold
                                d-flex align-items-center justify-content-center"
                         style="width:70px;height:70px;font-size:22px;flex-shrink:0;">
                        <i class="bi bi-book"></i>
                    </div>
                    <div>
                        <h5 class="mb-1">${c.courseName}</h5>
                        <span class="badge bg-primary me-1">${c.courseId}</span>
                        <span class="badge ${c.isActive == 1 ? "bg-success" : "bg-secondary"}">
                            ${c.isActive == 1 ? "Active" : "Inactive"}
                        </span>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Department</span>
                        <p class="fw-semibold">${c.deptId} — ${c.deptName || c.deptId}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Credit Hours</span>
                        <p class="fw-semibold">${c.creditHrs} Credit(s)</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Course Type</span>
                        <p class="fw-semibold">${c.courseType}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Course Category</span>
                        <p class="fw-semibold">${c.courseCat}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Prerequisite</span>
                        <p class="fw-semibold">${c.preReqId ? c.preReqId : "None"}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Assigned Instructors</span>
                        <p class="fw-semibold" id="viewCourseInstructors">Loading...</p>
                    </div>
                </div>`;
      loadCourseAssignedInstructors(courseId);
    },
    error: function () {
      document.getElementById("viewCourseBody").innerHTML =
        `<div class="alert alert-danger">Failed to load course details.</div>`;
    },
  });
}

function loadCourseAssignedInstructors(courseId) {
  const target = document.getElementById("viewCourseInstructors");
  if (target) {
    target.textContent = "Loading...";
  }

  $.ajax({
    url: "/Admin/GetSectionsByCourse",
    type: "GET",
    data: { courseId: courseId },
    success: function (res) {
      if (!res.success) {
        if (target) {
          target.textContent = "Unable to load assigned instructors.";
        }
        return;
      }

      const sections = res.data || [];
      const uniqueInstructorNames = [];
      const seenIds = {};

      sections.forEach(function (section) {
        if (section.instructorId && !seenIds[section.instructorId]) {
          seenIds[section.instructorId] = true;
          uniqueInstructorNames.push(
            section.instructorName || section.instructorId,
          );
        }
      });

      if (target) {
        if (uniqueInstructorNames.length === 0) {
          target.textContent = "No instructors assigned yet.";
        } else {
          target.textContent = uniqueInstructorNames.join(", ");
        }
      }
    },
    error: function () {
      if (target) {
        target.textContent = "Unable to load assigned instructors.";
      }
    },
  });
}

function loadAssignCourseInstructors(courseId) {
  const container = document.getElementById("currentInstructors");
  if (container) {
    container.innerHTML = '<p class="text-muted small">Loading...</p>';
  }

  $.ajax({
    url: "/Admin/GetSectionsByCourse",
    type: "GET",
    data: { courseId: courseId },
    success: function (res) {
      if (!res.success) {
        if (container) {
          container.innerHTML =
            '<p class="text-muted small">Unable to load assigned instructors.</p>';
        }
        return;
      }

      const sections = res.data || [];
      const uniqueInstructorNames = [];
      const seenIds = {};

      sections.forEach(function (section) {
        if (section.instructorId && !seenIds[section.instructorId]) {
          seenIds[section.instructorId] = true;
          uniqueInstructorNames.push(
            section.instructorName || section.instructorId,
          );
        }
      });

      if (container) {
        if (uniqueInstructorNames.length === 0) {
          container.innerHTML =
            '<p class="text-muted small">No instructors assigned to this course yet.</p>';
        } else {
          container.innerHTML =
            '<p class="fw-semibold mb-2">Currently Assigned Instructor' +
            (uniqueInstructorNames.length > 1 ? "s" : "") +
            "</p>" +
            '<p class="mb-0">' +
            uniqueInstructorNames.join(", ") +
            "</p>";
        }
      }
    },
    error: function () {
      if (container) {
        container.innerHTML =
          '<p class="text-muted small">Unable to load assigned instructors.</p>';
      }
    },
  });
}

// assign Instructor Functionality - to be continued after main merge

function openAssignInstructorModal(courseId) {
  document.getElementById("assign_courseId").value = courseId;
  document.getElementById("assign_courseName").textContent = "Loading...";
  document.getElementById("assign_instructorId").innerHTML =
    '<option value="">Loading instructors...</option>';
  document.getElementById("currentInstructors").innerHTML =
    '<p class="text-muted small">Loading...</p>';
  hideAlert("assignInstructorError");
  hideAlert("assignInstructorSuccess");

  // Get course details
  $.ajax({
    url: "/Admin/GetCourse",
    type: "GET",
    data: { id: courseId },
    success: function (res) {
      if (res.success) {
        document.getElementById("assign_courseName").textContent =
          `${res.data.courseId} - ${res.data.courseName}`;
      }
    },
  });

  // Get all instructors
  $.ajax({
    url: "/Admin/GetInstructors",
    type: "GET",
    success: function (res) {
      if (res.success && res.data) {
        let options = '<option value="">-- Select an Instructor --</option>';
        res.data.forEach(function (instructor) {
          options += `<option value="${instructor.userId}">${instructor.fullName} (${instructor.userId})</option>`;
        });
        document.getElementById("assign_instructorId").innerHTML = options;
      } else {
        document.getElementById("assign_instructorId").innerHTML =
          '<option value="">No instructors available</option>';
      }
    },
    error: function () {
      document.getElementById("assign_instructorId").innerHTML =
        '<option value="">Error loading instructors</option>';
    },
  });

  loadAssignCourseInstructors(courseId);
  openModal("assignInstructorModal");
}

function submitAssignInstructor() {
  hideAlert("assignInstructorError");
  hideAlert("assignInstructorSuccess");

  const courseId = document.getElementById("assign_courseId").value;
  const instructorId = document.getElementById("assign_instructorId").value;

  if (!instructorId) {
    return showAlert("assignInstructorError", "Please select an instructor");
  }

  const btn = document.querySelector("#assignInstructorModal .btn-primary");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Assigning...';

  // This will assign the instructor to sections of this course
  // For now, we'll show a success message
  $.ajax({
    url: "/Admin/AssignInstructorToCourse",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify({ courseId: courseId, instructorId: instructorId }),
    success: function (res) {
      if (res.success) {
        showAlert("assignInstructorSuccess", res.message);
        setTimeout(() => {
          bootstrap.Modal.getInstance(
            document.getElementById("assignInstructorModal"),
          ).hide();
          location.reload();
        }, 1000);
      } else {
        showAlert("assignInstructorError", res.message);
      }
    },
    error: function () {
      showAlert("assignInstructorError", "Network error. Please try again.");
    },
    complete: function () {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-person-plus me-1"></i>Assign Instructor';
    },
  });
}

// COURSE CRUD OPERATIONS

var currentDeleteCourseId = null;

// Edit button click
document.addEventListener("DOMContentLoaded", function () {
  document
    .querySelector("#coursesTable tbody")
    .addEventListener("click", function (e) {
      var btn = e.target.closest("button");
      if (!btn) return;

      var row = btn.closest("tr");
      var courseId = row.children[1].textContent.trim();

      if (btn.querySelector(".bi-pencil")) {
        openEditCourseModal(courseId);
      }

      if (btn.querySelector(".bi-trash")) {
        openDeleteCourseModal(courseId, row.children[2].textContent.trim());
      }
    });
});

// Open Edit Modal — fetch current data from server
function openEditCourseModal(courseId) {
  // Clear previous values
  document.getElementById("editCourseError").classList.add("d-none");
  document.getElementById("editCourseSuccess").classList.add("d-none");

  $.ajax({
    url: "/Admin/GetCourse",
    type: "GET",
    data: { id: courseId },
    success: function (response) {
      if (response.success) {
        var c = response.data;
        // Fill current value fields
        document.getElementById("edit_courseId").value = c.courseId;
        document.getElementById("edit_current_courseId").value = c.courseId;
        document.getElementById("edit_current_courseName").value = c.courseName;
        document.getElementById("edit_current_deptId").value = c.deptId;
        document.getElementById("edit_current_creditHrs").value = c.creditHrs;
        document.getElementById("edit_current_courseType").value = c.courseType;
        document.getElementById("edit_current_courseCat").value = c.courseCat;
        document.getElementById("edit_current_preReqId").value =
          c.preReqId || "None";

        // Clear new value fields
        document.getElementById("edit_new_courseName").value = "";
        document.getElementById("edit_new_deptId").value = "";
        document.getElementById("edit_new_creditHrs").value = "";
        document.getElementById("edit_new_courseType").value = "";
        document.getElementById("edit_new_courseCat").value = "";
        document.getElementById("edit_new_preReqId").value = "";

        // Open modal
        openModal("editCourseModal");
      } else {
        alert("Error: " + response.message);
      }
    },
    error: function () {
      alert("Failed to Load Course Data !");
    },
  });
}

// Submit Create Course
function submitCreateCourse() {
  var errorDiv = document.getElementById("createCourseError");
  var successDiv = document.getElementById("createCourseSuccess");
  errorDiv.classList.add("d-none");
  successDiv.classList.add("d-none");

  var courseId = document.getElementById("create_courseId").value.trim();
  var courseName = document.getElementById("create_courseName").value.trim();
  var deptId = document.getElementById("create_deptId").value;
  var creditHrs = document.getElementById("create_creditHrs").value;
  var courseType = document.getElementById("create_courseType").value;
  var courseCat = document.getElementById("create_courseCat").value;
  var preReqId = document.getElementById("create_preReqId").value.trim();

  // Client side validation
  if (!courseId) return showModalError(errorDiv, "Course Code is required");
  if (!courseName) return showModalError(errorDiv, "Course Name is required");
  if (!deptId) return showModalError(errorDiv, "Department is required");
  if (!creditHrs) return showModalError(errorDiv, "Credit Hours is required");
  if (!courseType) return showModalError(errorDiv, "Course Type is required");
  if (!courseCat)
    return showModalError(errorDiv, "Course Category is required");

  var data = {
    courseId: courseId,
    courseName: courseName,
    deptId: deptId,
    creditHrs: parseInt(creditHrs),
    courseType: courseType,
    courseCat: courseCat,
    preReqId: preReqId || null,
    isActive: 1,
  };

  $.ajax({
    url: "/Admin/CreateCourse",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(data),
    success: function (response) {
      if (response.success) {
        successDiv.textContent = response.message;
        successDiv.classList.remove("d-none");
        setTimeout(function () {
          location.reload();
        }, 1500);
      } else {
        showModalError(errorDiv, response.message);
      }
    },
    error: function () {
      showModalError(errorDiv, "Something went wrong | Please try again.");
    },
  });
}

// Submit Edit Course
function submitEditCourse() {
  var errorDiv = document.getElementById("editCourseError");
  var successDiv = document.getElementById("editCourseSuccess");
  errorDiv.classList.add("d-none");
  successDiv.classList.add("d-none");

  var courseId = document.getElementById("edit_courseId").value;

  // New values — if empty use current values
  var courseName =
    document.getElementById("edit_new_courseName").value.trim() ||
    document.getElementById("edit_current_courseName").value.trim();
  var deptId =
    document.getElementById("edit_new_deptId").value ||
    document.getElementById("edit_current_deptId").value;
  var creditHrs =
    document.getElementById("edit_new_creditHrs").value ||
    document.getElementById("edit_current_creditHrs").value;
  var courseType =
    document.getElementById("edit_new_courseType").value ||
    document.getElementById("edit_current_courseType").value;
  var courseCat =
    document.getElementById("edit_new_courseCat").value ||
    document.getElementById("edit_current_courseCat").value;
  var preReqId =
    document.getElementById("edit_new_preReqId").value.trim() ||
    document.getElementById("edit_current_preReqId").value.trim();

  if (preReqId === "None") preReqId = null;

  var data = {
    courseId: courseId,
    courseName: courseName,
    deptId: deptId,
    creditHrs: parseInt(creditHrs),
    courseType: courseType,
    courseCat: courseCat,
    preReqId: preReqId || null,
    isActive: 1,
  };

  $.ajax({
    url: "/Admin/UpdateCourse",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(data),
    success: function (response) {
      if (response.success) {
        successDiv.textContent = response.message;
        successDiv.classList.remove("d-none");
        setTimeout(function () {
          location.reload();
        }, 1500);
      } else {
        showModalError(errorDiv, response.message);
      }
    },
    error: function () {
      showModalError(errorDiv, "Something went wrong. Please try again.");
    },
  });
}

// Open Delete Modal
function openDeleteCourseModal(courseId, courseName) {
  currentDeleteCourseId = courseId;

  let cleanName = courseName.replace(/[^\x20-\x7E]/g, "").trim();

  document.getElementById("delete_courseName").innerHTML =
    `<div class="p-3 mb-2 bg-light rounded border-start border-4 border-danger">
            <span class="text-muted small d-block">COURSE TO BE REMOVED:</span>
            <strong class="text-primary">${courseId}</strong> 
            <span class="mx-2 text-muted">|</span> 
            <span class="text-dark">${cleanName}</span>
        </div>`;

  const errorDiv = document.getElementById("deleteCourseError");
  if (errorDiv) {
    errorDiv.classList.add("d-none");
  }

  openModal("deleteCourseModal");
}

// Submit DELETION handling
function submitDeleteCourse() {
  const errorDiv = document.getElementById("deleteCourseError");
  const btn = document.querySelector("#deleteCourseModal .btn-danger");
  const originalText = '<i class="bi bi-trash me-1"></i> Confirm Delete';

  errorDiv.classList.add("d-none");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';

  $.ajax({
    url: "/Admin/DeleteCourse",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(currentDeleteCourseId),
    success: function (response) {
      if (response.success) {
        btn.innerHTML = '<i class="bi bi-check-circle me-1"></i> Deleted';
        btn.classList.replace("btn-danger", "btn-success");
        setTimeout(() => {
          location.reload();
        }, 800);
      } else {
        btn.disabled = false;
        btn.innerHTML = originalText;

        errorDiv.innerHTML = `<i class="bi bi-exclamation-triangle me-2"></i> ${response.message || "Cannot Delete: Course is assigned to a section."}`;
        errorDiv.classList.remove("d-none", "alert-danger");
        errorDiv.classList.add(
          "alert-warning",
          "animate__animated",
          "animate__shakeX",
        );
      }
    },
    error: function () {
      btn.disabled = false;
      btn.innerHTML = originalText;
      showModalError(errorDiv, "Server connection lost. Please try again.");
    },
  });
}

// Helper — show error in modal
function showModalError(div, message) {
  div.textContent = message;
  div.classList.remove("d-none");
}

// Helper functions for alerts
function showAlert(id, msg) {
  const el = document.getElementById(id);
  if (!el) return;
  el.textContent = msg;
  el.classList.remove("d-none");
}

function hideAlert(id) {
  const el = document.getElementById(id);
  if (el) el.classList.add("d-none");
}

// Toast notification (top-right, auto-dismiss)
function showToast(message, type = "success") {
  const toast = document.createElement("div");
  toast.className = `alert alert-${type} alert-dismissible fade show position-fixed shadow`;
  toast.style.cssText = "top:20px;right:20px;z-index:9999;min-width:300px;";
  toast.innerHTML = `${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 4000);
}
// Open Toggle Modal — replaces confirm()
function toggleCourseStatus(courseId, isActive) {
  document.getElementById("toggle_courseId").value = courseId;
  document.getElementById("toggle_newStatus").value = isActive ? "0" : "1";

  const title = document.getElementById("toggleCourseTitle");
  const name = document.getElementById("toggle_courseName");
  const alert = document.getElementById("toggleCourseAlert");
  const icon = document.getElementById("toggleAlertIcon");
  const text = document.getElementById("toggleAlertText");
  const btn = document.getElementById("toggleConfirmBtn");

  name.textContent = courseId;

  if (isActive) {
    // Deactivate
    title.textContent = "Deactivate Course";
    title.className = "modal-title fw-bold text-warning";
    alert.className =
      "alert alert-warning d-flex align-items-center border-0 small";
    icon.className = "bi bi-exclamation-triangle-fill fs-5 me-2";
    text.innerHTML =
      "<strong>This will deactivate the course.</strong> Students will no longer see it as active.";
    btn.className = "btn btn-warning px-4 shadow-sm";
    btn.innerHTML = '<i class="bi bi-toggle-off me-1"></i> Deactivate';
  } else {
    // Activate
    title.textContent = "Activate Course";
    title.className = "modal-title fw-bold text-success";
    alert.className =
      "alert alert-success d-flex align-items-center border-0 small";
    icon.className = "bi bi-check-circle-fill fs-5 me-2";
    text.innerHTML =
      "<strong>This will activate the course.</strong> It will be visible and available.";
    btn.className = "btn btn-success px-4 shadow-sm";
    btn.innerHTML = '<i class="bi bi-toggle-on me-1"></i> Activate';
  }

  openModal("toggleCourseModal");
}

// Submit toggle
function submitToggleCourse() {
  const courseId = document.getElementById("toggle_courseId").value;
  const btn = document.getElementById("toggleConfirmBtn");

  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Processing...';

  $.ajax({
    url: "/Admin/ToggleCourseStatus",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(courseId),
    success: function (res) {
      if (res.success) {
        closeModal("toggleCourseModal");
        showToast(res.message, "success");
        setTimeout(() => location.reload(), 800);
      } else {
        showToast(res.message, "danger");
        btn.disabled = false;
      }
    },
    error: function () {
      showToast("Network error. Please try again.", "danger");
      btn.disabled = false;
    },
  });
}
