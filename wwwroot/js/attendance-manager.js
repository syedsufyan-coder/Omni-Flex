const attendanceManager = {
    
    // 1. Trigger the bulk add
    addNewColumn: async function () {
        const courseId = document.getElementById('selectedCourseId').value;
        const sectionId = document.getElementById('selectedSectionId').value;
        const duration = document.getElementById('selectedDuration').value;

        if (!courseId || !sectionId) {
            alert("Please ensure Course and Section are selected.");
            return;
        }

        const payload = {
            CourseId: courseId,
            SectionId: sectionId,
            Duration: parseFloat(duration),
            DefaultStatus: 'P'
        };

        try {
            const response = await fetch('/Instructor/BulkAddAttendance', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (result.success) {
                this.injectColumnIntoTable(result.dateLabel, result.records);
            } else {
                alert("Could not generate attendance: " + result.message);
            }
        } catch (error) {
            console.error("Error creating bulk attendance:", error);
        }
    },

    // 2. Inject the dynamic column
    injectColumnIntoTable: function (dateLabel, records) {
        // Append Header
        const headerRow = document.getElementById('tableHeaderRow');
        const th = document.createElement('th');
        th.innerText = dateLabel;
        headerRow.appendChild(th);

        // Append Dropdowns to rows based on EnrollId
        const rows = document.querySelectorAll('#attendanceTable tbody tr');
        rows.forEach(row => {
            const enrollId = parseInt(row.getAttribute('data-enroll-id'));
            
            // Find the generated attendance ID for this specific enrollment
            const matchingRecord = records.find(r => r.enrollId === enrollId);

            if (matchingRecord) {
                const td = document.createElement('td');
                td.className = "attendance-cell text-center";
                td.innerHTML = `
                    <div class="d-flex align-items-center justify-content-center gap-1">
                        <select class="status-select" data-aid="${matchingRecord.attendanceId}" onchange="attendanceManager.showSaveIcon(this)">
                            <option value="P" selected>P</option>
                            <option value="A">A</option>
                            <option value="L">L</option>
                        </select>
                        <button class="btn btn-sm btn-link p-0 save-btn d-none" onclick="attendanceManager.saveStatus(this)">
                            <i class="bi bi-check-circle-fill text-success"></i>
                        </button>
                    </div>
                `;
                row.appendChild(td);
            }
        });
    },

    // 3. UI interaction
    showSaveIcon: function (selectDropdown) {
        const saveBtn = selectDropdown.nextElementSibling;
        saveBtn.classList.remove('d-none');
    },

    // 4. Save individual updates
    saveStatus: async function (btnElement) {
        const selectElement = btnElement.previousElementSibling;
        const attendanceId = selectElement.getAttribute('data-aid');
        const statusVal = selectElement.value;
        selectElement.disabled = true; // Prevent multiple clicks
        btnElement.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

        try {
            const response = await fetch('/Instructor/UpdateSingleStatus', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    AttendanceId: parseInt(attendanceId),
                    Status: statusVal
                })
            });

            if (response.ok) {
                // Hide the checkmark once successfully saved
                btnElement.classList.add('d-none'); 
                // ... after fetch ...
                selectElement.disabled = false;
                btnElement.innerHTML = '<i class="bi bi-check-circle-fill text-success"></i>';
            } else {
                alert("Failed to update status.");
            }
            // Final cleanup to ensure UI is never stuck
            selectElement.disabled = false;
            if (!btnElement.classList.contains('d-none')) {
                btnElement.innerHTML = '<i class="bi bi-check-circle-fill text-success"></i>';
            }
        } catch (error) {
            console.error("Error saving status:", error);
            selectElement.disabled = false;
            btnElement.innerHTML = '<i class="bi bi-check-circle-fill text-success"></i>';
        }
    }
};