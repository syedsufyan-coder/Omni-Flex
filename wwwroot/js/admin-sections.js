// Admin Sections CRUD  (admin-sections.js)

document.addEventListener("DOMContentLoaded", function () {
  // Search
  document
    .getElementById("sectionSearch")
    .addEventListener("input", function () {
      const q = this.value.toLowerCase();
      document.querySelectorAll("#sectionsTable tbody tr").forEach((row) => {
        row.style.display = row.textContent.toLowerCase().includes(q)
          ? ""
          : "none";
      });
    });

  // Sort
  const tbody = document.querySelector("#sectionsTable tbody");
  document.querySelectorAll(".sortable").forEach((header) => {
    header.addEventListener("click", function () {
      const idx = Array.from(header.parentElement.children).indexOf(header);
      const rows = Array.from(tbody.querySelectorAll("tr"));
      const asc = header.classList.contains("asc");

      document.querySelectorAll(".sortable").forEach((h) => {
        h.classList.remove("asc", "desc");
        h.querySelector(".sort-icon").className = "bi bi-sort sort-icon ms-1";
      });

      rows.sort((a, b) => {
        const aT = a.children[idx]?.textContent.trim() ?? "";
        const bT = b.children[idx]?.textContent.trim() ?? "";
        return asc ? bT.localeCompare(aT) : aT.localeCompare(bT);
      });

      if (asc) {
        header.classList.add("desc");
        header.querySelector(".sort-icon").className =
          "bi bi-sort-down sort-icon ms-1";
      } else {
        header.classList.add("asc");
        header.querySelector(".sort-icon").className =
          "bi bi-sort-up sort-icon ms-1";
      }

      rows.forEach((r) => tbody.appendChild(r));
    });
  });

  // Tooltips
  document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((el) => {
    new bootstrap.Tooltip(el);
  });
});

//  Add a Section
function openAddSectionModal() {
  document.getElementById("create_sectionId").value = "";
  document.getElementById("create_sectionLabel").value = "";
  document.getElementById("create_department").value = "";
  document.getElementById("create_degree").value = "";
  document.getElementById("create_batch").value = "";
  hideAlert("createSectionError");
  hideAlert("createSectionSuccess");
  new bootstrap.Modal(document.getElementById("addSectionModal")).show();
}

function submitCreateSection() {
  hideAlert("createSectionError");
  hideAlert("createSectionSuccess");

  const payload = {
    sectionId: document.getElementById("create_sectionId").value.trim(),
    sectionLabel: document.getElementById("create_sectionLabel").value.trim(),
    departmentId: document.getElementById("create_department").value, // ← department → departmentId
    degreeProgram: document.getElementById("create_degree").value, // ← degree → degreeProgram
    batchYear: parseInt(document.getElementById("create_batch").value) || 0, // ← batch → batchYear
  };

  // Validation bhi update karo:
  if (!payload.departmentId)
    return showAlert("createSectionError", "Department is required");
  if (!payload.degreeProgram)
    return showAlert("createSectionError", "Degree is required");
  if (!payload.batchYear || payload.batchYear < 2020)
    return showAlert("createSectionError", "Valid Batch Year is required");

  const btn = document.querySelector("#addSectionModal .btn-primary");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Creating...';

  $.ajax({
    url: "/Admin/CreateSection",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(payload),
    success: function (res) {
      if (res.success) {
        showAlert("createSectionSuccess", res.message);
        setTimeout(() => {
          bootstrap.Modal.getInstance(
            document.getElementById("addSectionModal"),
          ).hide();
          location.reload();
        }, 1000);
      } else {
        showAlert("createSectionError", res.message);
      }
    },
    error: function () {
      showAlert("createSectionError", "Network error. Please try again.");
    },
    complete: function () {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-plus-circle me-1"></i>Add Section';
    },
  });
}

// View Section's enrolled students
function openViewSectionModal(sectionId) {
  document.getElementById("viewSectionBody").innerHTML = `
        <div class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">Loading section details...</p>
        </div>`;

  new bootstrap.Modal(document.getElementById("viewSectionModal")).show();

  $.ajax({
    url: "/Admin/GetSection",
    type: "GET",
    data: { id: sectionId },
    success: function (res) {
      if (!res.success) {
        document.getElementById("viewSectionBody").innerHTML =
          `<div class="alert alert-danger">${res.message}</div>`;
        return;
      }

      const s = res.data;
      const seatsBar = Math.round((s.enrolledStudents / s.seats) * 100);
      const barColor =
        seatsBar >= 90
          ? "bg-danger"
          : seatsBar >= 70
            ? "bg-warning"
            : "bg-success";

      document.getElementById("viewSectionBody").innerHTML = `
                <div class="d-flex align-items-center gap-3 p-3 mb-4 rounded"
                     style="background:#f0f4ff;">
                    <div class="rounded-circle bg-primary text-white fw-bold
                                d-flex align-items-center justify-content-center"
                         style="width:65px;height:65px;font-size:20px;flex-shrink:0;">
                        <i class="bi bi-building"></i>
                    </div>
                    <div>
                        <h5 class="mb-1">${s.sectionId}</h5>
                        <span class="badge bg-primary me-1">${s.sectionLabel}</span>
                        <span class="badge bg-info text-dark me-1">${s.degree}</span>
                        <span class="badge bg-secondary">${s.department}</span>
                    </div>
                </div>

                <div class="row g-3 mb-3">
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Section ID</span>
                        <p class="fw-semibold"><code>${s.sectionId}</code></p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Section Label</span>
                        <p class="fw-semibold">${s.sectionLabel}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Degree Program</span>
                        <p class="fw-semibold">${s.degree}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Batch Year</span>
                        <p class="fw-semibold">${s.batchYear}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Department</span>
                        <p class="fw-semibold">${s.department}</p>
                    </div>
                    <div class="col-md-6">
                        <span class="text-muted small d-block">Class Representative</span>
                        <p class="fw-semibold">${s.crName || '<span class="text-muted">Not assigned</span>'}</p>
                    </div>
                    <div class="col-12">
                        <span class="text-muted small d-block">Enrollment</span>
                        <div class="d-flex align-items-center gap-2">
                            <div class="progress flex-grow-1" style="height:8px;">
                                <div class="progress-bar ${barColor}" style="width:${seatsBar}%"></div>
                            </div>
                            <span class="fw-semibold small">${s.enrolledStudents} / ${s.seats}</span>
                        </div>
                    </div>
                </div>

                <button class="btn btn-outline-info" onclick="openStudentsModal('${s.sectionId}')">
                    <i class="bi bi-people me-1"></i>View Enrolled Students
                </button>`;
    },
    error: function () {
      document.getElementById("viewSectionBody").innerHTML =
        `<div class="alert alert-danger">Failed to load section details.</div>`;
    },
  });
}

//  ENROLLED STUDENTS
function openStudentsModal(sectionId) {
  const loadingDiv = document.getElementById("studentsLoading");
  const errorDiv = document.getElementById("studentsError");
  const contentDiv = document.getElementById("studentsContent");
  const tableBody = document.getElementById("studentsTableBody");
  const noMsg = document.getElementById("noStudentsMessage");

  loadingDiv.classList.remove("d-none");
  errorDiv.classList.add("d-none");
  contentDiv.classList.add("d-none");
  noMsg.classList.add("d-none");
  tableBody.innerHTML = "";

  new bootstrap.Modal(document.getElementById("studentsModal")).show();

  $.ajax({
    url: "/Admin/GetSectionStudents",
    type: "GET",
    data: { sectionId: sectionId },
    success: function (res) {
      loadingDiv.classList.add("d-none");

      if (!res.success) {
        errorDiv.textContent = res.message;
        errorDiv.classList.remove("d-none");
        return;
      }

      contentDiv.classList.remove("d-none");

      if (!res.data || res.data.length === 0) {
        noMsg.classList.remove("d-none");
        return;
      }

      res.data.forEach((s) => {
        const badge =
          s.status === "Active"
            ? '<span class="badge bg-success">Active</span>'
            : '<span class="badge bg-secondary">Inactive</span>';

        tableBody.innerHTML += `
                    <tr>
                        <td><code>${s.studentId}</code></td>
                        <td>${s.fullName}</td>
                        <td>${s.email}</td>
                        <td>${s.degree || "�"}</td>
                        <td>${s.batch}</td>
                        <td>${s.department || "�"}</td>
                        <td>${badge}</td>
                    </tr>`;
      });
    },
    error: function () {
      loadingDiv.classList.add("d-none");
      errorDiv.textContent = "Failed to load students.";
      errorDiv.classList.remove("d-none");
    },
  });
}
//  EDIT SECTION
function openEditSectionModal(sectionId) {
  hideAlert("editSectionError");
  hideAlert("editSectionSuccess");

  $.ajax({
    url: "/Admin/GetSection",
    type: "GET",
    data: { id: sectionId },
    success: function (res) {
      if (!res.success) {
        alert("Error loading section: " + res.message);
        return;
      }

      const s = res.data;
      document.getElementById("edit_sectionId").value = s.sectionId;
      document.getElementById("edit_cur_sectionId").value = s.sectionId;
      document.getElementById("edit_cur_sectionLabel").value = s.sectionLabel;
      document.getElementById("edit_cur_department").value = s.department;
      document.getElementById("edit_cur_degree").value = s.degree;
      document.getElementById("edit_cur_batch").value = s.batchYear;
      document.getElementById("edit_cur_cr").value = s.crName || "Not assigned";

      // Pre-fill new value columns
      document.getElementById("edit_new_sectionLabel").value = s.sectionLabel;
      document.getElementById("edit_new_department").value = s.department;
      document.getElementById("edit_new_degree").value = s.degree;
      document.getElementById("edit_new_batch").value = s.batchYear;

      new bootstrap.Modal(document.getElementById("editSectionModal")).show();
    },
    error: function () {
      alert("Network error loading section.");
    },
  });
}

function submitEditSection() {
  hideAlert("editSectionError");
  hideAlert("editSectionSuccess");

  const payload = {
    sectionId: document.getElementById("edit_sectionId").value,
    sectionLabel: document.getElementById("edit_new_sectionLabel").value.trim(),
    departmentId: document.getElementById("edit_new_department").value, // ← fix
    degreeProgram: document.getElementById("edit_new_degree").value, // ← fix
    batchYear:
      parseInt(document.getElementById("edit_new_batch").value) || null,
  };

  if (!payload.departmentId)
    return showAlert("editSectionError", "Department is required");
  if (!payload.degreeProgram)
    return showAlert("editSectionError", "Degree is required");

  const btn = document.querySelector("#editSectionModal .btn-warning");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Updating...';

  $.ajax({
    url: "/Admin/UpdateSection",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(payload),
    success: function (res) {
      if (res.success) {
        showAlert("editSectionSuccess", res.message);
        setTimeout(() => {
          bootstrap.Modal.getInstance(
            document.getElementById("editSectionModal"),
          ).hide();
          location.reload();
        }, 1000);
      } else {
        showAlert("editSectionError", res.message);
      }
    },
    error: function () {
      showAlert("editSectionError", "Network error. Please try again.");
    },
    complete: function () {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-pencil me-1"></i>Update Section';
    },
  });
}

//  ASSIGN CR
function openAssignCRModal(sectionId) {
  document.getElementById("assign_sectionId").value = sectionId;
  document.getElementById("assign_sectionName").textContent = "Loading...";
  document.getElementById("assign_studentId").innerHTML =
    '<option value="">Loading...</option>';
  document.getElementById("currentCR").innerHTML =
    '<p class="text-muted small">Loading...</p>';
  hideAlert("assignCRError");
  hideAlert("assignCRSuccess");

  // Load section info
  $.ajax({
    url: "/Admin/GetSection",
    type: "GET",
    data: { id: sectionId },
    success: function (res) {
      if (res.success) {
        const s = res.data;
        document.getElementById("assign_sectionName").textContent =
          `${s.sectionId} � ${s.degree}, Batch ${s.batchYear}`;
        document.getElementById("currentCR").innerHTML = s.crName
          ? `<span class="badge bg-success"><i class="bi bi-person-check me-1"></i>${s.crName}</span>`
          : '<span class="text-muted small">No CR assigned yet.</span>';
      }
    },
  });

  // Load enrolled students
  $.ajax({
    url: "/Admin/GetSectionStudents",
    type: "GET",
    data: { sectionId: sectionId },
    success: function (res) {
      if (res.success && res.data && res.data.length > 0) {
        let opts = '<option value="">-- Select a Student --</option>';
        res.data.forEach((s) => {
          opts += `<option value="${s.studentId}">${s.fullName} (${s.studentId})</option>`;
        });
        document.getElementById("assign_studentId").innerHTML = opts;
      } else {
        document.getElementById("assign_studentId").innerHTML =
          '<option value="">No enrolled students found</option>';
      }
    },
    error: function () {
      document.getElementById("assign_studentId").innerHTML =
        '<option value="">Error loading students</option>';
    },
  });

  new bootstrap.Modal(document.getElementById("assignCRModal")).show();
}

function submitAssignCR() {
  hideAlert("assignCRError");
  hideAlert("assignCRSuccess");

  const payload = {
    sectionId: document.getElementById("assign_sectionId").value,
    studentId: document.getElementById("assign_studentId").value,
  };

  if (!payload.studentId)
    return showAlert("assignCRError", "Please select a student");

  const btn = document.querySelector("#assignCRModal .btn-primary");
  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Assigning...';

  $.ajax({
    url: "/Admin/AssignCR",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(payload),
    success: function (res) {
      if (res.success) {
        showAlert("assignCRSuccess", res.message);
        setTimeout(() => {
          bootstrap.Modal.getInstance(
            document.getElementById("assignCRModal"),
          ).hide();
          location.reload();
        }, 1000);
      } else {
        showAlert("assignCRError", res.message);
      }
    },
    error: function () {
      showAlert("assignCRError", "Network error. Please try again.");
    },
    complete: function () {
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-person-check me-1"></i>Assign CR';
    },
  });
}

//  DELETE SECTION
function openDeleteSectionModal(sectionId, displayName) {
  document.getElementById("delete_sectionId").value = sectionId;
  document.getElementById("delete_sectionName").textContent = displayName;
  hideAlert("deleteSectionError");

  const deleteBtn = document.querySelector("#deleteSectionModal .btn-danger");
  deleteBtn.disabled = true;
  deleteBtn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Checking...';

  new bootstrap.Modal(document.getElementById("deleteSectionModal")).show();

  // Check enrolled students before allowing delete
  $.ajax({
    url: "/Admin/GetSectionStudents",
    type: "GET",
    data: { sectionId: sectionId },
    success: function (res) {
      if (res.success && res.data && res.data.length > 0) {
        showAlert(
          "deleteSectionError",
          `Cannot delete: ${res.data.length} student(s) enrolled. Remove enrollments first.`,
        );
        deleteBtn.disabled = true;
        deleteBtn.innerHTML =
          '<i class="bi bi-exclamation-triangle me-1"></i>Cannot Delete';
      } else {
        deleteBtn.disabled = false;
        deleteBtn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
      }
    },
    error: function () {
      deleteBtn.disabled = false;
      deleteBtn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
    },
  });
}

function submitDeleteSection() {
  const sectionId = document.getElementById("delete_sectionId").value;
  const btn = document.querySelector("#deleteSectionModal .btn-danger");

  btn.disabled = true;
  btn.innerHTML =
    '<span class="spinner-border spinner-border-sm me-1"></span>Deleting...';

  $.ajax({
    url: "/Admin/DeleteSection",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify({ sectionId: sectionId }),
    success: function (res) {
      if (res.success) {
        bootstrap.Modal.getInstance(
          document.getElementById("deleteSectionModal"),
        ).hide();
        // Remove row from table without full reload
        const row = document.querySelector(
          `#sectionsTable tr[data-section-id="${sectionId}"]`,
        );
        if (row) {
          row.style.transition = "opacity 0.3s";
          row.style.opacity = "0";
          setTimeout(() => row.remove(), 300);
        }
        showToast(res.message, "success");
      } else {
        showAlert("deleteSectionError", res.message);
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
      }
    },
    error: function () {
      showAlert("deleteSectionError", "Network error. Please try again.");
      btn.disabled = false;
      btn.innerHTML = '<i class="bi bi-trash me-1"></i>Confirm Delete';
    },
  });
}

//  HELPERS
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

function showToast(message, type = "success") {
  const toast = document.createElement("div");
  toast.className = `alert alert-${type} alert-dismissible fade show position-fixed shadow`;
  toast.style.cssText = "top:20px;right:20px;z-index:9999;min-width:300px;";
  toast.innerHTML = `${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 4000);
}
