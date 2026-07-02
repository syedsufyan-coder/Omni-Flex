$(document).ready(function () {
    const offeringId = document.querySelector('[data-offering-id]')?.dataset.offeringId || '';

    const partialMap = {
        stream: '/Instructor/GetStreamPartial',
        classwork: '/Instructor/GetClassworkPartial',
        people: '/Instructor/GetPeoplePartial',
        grades: '/Instructor/GetGradesPartial'
    };

    $('#classroomTabs .nav-link').on('click', function (e) {
        e.preventDefault();
        const tab = $(this).data('tab');

        $('#classroomTabs .nav-link').removeClass('active');
        $(this).addClass('active');

        history.replaceState(null, '', `/Instructor/Classroom/${offeringId}/${tab}`);

        $('#classroomTabContent').html('<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>');

        $.ajax({
            url: partialMap[tab],
            type: 'POST',
            data: { offeringId: offeringId },
            success: function (result) {
                $('#classroomTabContent').html(result);
                initInstructorClassroom();
            },
            error: function () {
                $('#classroomTabContent').html('<div class="alert alert-danger m-3">Unable to load tab content.</div>');
            }
        });
    });

    $(document).on('change', '#assessmentDeliveryMode', function () {
        const mode = $(this).val();
        if (mode === 'Onsite') {
            $('#assessmentCategoryText').addClass('d-none').val('');
            $('#assessmentCategorySelect').removeClass('d-none');
        } else {
            $('#assessmentCategorySelect').addClass('d-none').val('');
            $('#assessmentCategoryText').removeClass('d-none');
        }
    });

    // Removing original code for adding post
    // Because we replace with a new and custom one that matches our new model and API contract
    /*
    $(document).on('click', '#btnSubmitPost', function () {
        const payload = {
            OfferingId: $('#postOfferingId').val(),
            PostType: $('#postType').val(),
            Title: $('#postTitle').val(),
            Body: $('#postBody').val()
        };

        if (!payload.PostType || !payload.Title || !payload.Body) {
            alert('Please complete all required fields.');
            return;
        }

        $.ajax({
            url: '/Instructor/AddPost',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (response) {
                if (response.success) {
                    $('#addPostModal').modal('hide');
                    $('#postType, #postTitle, #postBody').val('');
                    $('#classroomTabs .nav-link[data-tab="stream"]').trigger('click');
                } else {
                    alert(response.message || 'Unable to add post.');
                }
            },
            error: function () {
                alert('Unable to add post.');
            }
        });
    });
    */

    //Critical Section because we replace with a new and custom code for adding post
    $(document).on('click', '#btnSubmitPost', function () {
        const $btn = $(this);

        const payload = {
            OfferingId: $('#postOfferingId').val(),
            PostType: $('#postType').val(),
            Title: $('#postTitle').val(),
            Body: $('#postBody').val()
        };

        // Simple Client-side Validation
        if (!payload.PostType || !payload.Title || !payload.Body) {
            alert('Please complete all required fields.');
            return;
        }

        // Visual feedback
        $btn.prop('disabled', true).text('Posting...');

        $.ajax({
            url: '/Instructor/AddPost',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (response) {
                if (response.success) {
                    // Close Modal
                    $('#addPostModal').modal('hide');

                    // Clear inputs for next time
                    $('#postType, #postTitle, #postBody').val('');

                    // Refresh the stream (triggers your existing tab click logic)
                    $('#classroomTabs .nav-link[data-tab="stream"]').trigger('click');
                } else {
                    alert(response.message || 'Error occurred.');
                    $btn.prop('disabled', false).text('Post');
                }
            },
            error: function () {
                alert('Server communication error.');
                $btn.prop('disabled', false).text('Post');
            }
        });
    });

    // Critical Section because we add the jQuery custom
    // =========================
    // Assessment UI Rules Engine
    // =========================

    function resetAssessmentFields() {
        $('#assessmentActualWtg').prop('disabled', false);
        $('#assessmentGradingGroup').prop('disabled', false).val('');
        $('#assessmentCountBestOf').prop('disabled', false).val('');
        $('#assessmentIsGraded').prop('disabled', false);
    }

    function applyOnsiteRules() {
        const category = $('#assessmentCategorySelect').val();

        resetAssessmentFields();

        if (category === 'Quiz') {
            // Free weight
            $('#assessmentActualWtg').val('').prop('disabled', false);

            // Lock grading group
            $('#assessmentGradingGroup')
                .val('Quiz')
                .prop('disabled', true);

            $('#assessmentCountBestOf').prop('disabled', false);
            $('#assessmentIsGraded').prop('disabled', false);
        }

        else if (category === 'Mid I' || category === 'Mid II') {
            $('#assessmentActualWtg')
                .val(15)
                .prop('disabled', true);

            $('#assessmentGradingGroup')
                .val('')
                .prop('disabled', true);

            $('#assessmentCountBestOf')
                .val('')
                .prop('disabled', true);

            $('#assessmentIsGraded')
                .val('Y')
                .prop('disabled', true);
        }

        else if (category === 'Final') {
            $('#assessmentActualWtg')
                .val(50)
                .prop('disabled', true);

            $('#assessmentGradingGroup')
                .val('')
                .prop('disabled', true);

            $('#assessmentCountBestOf')
                .val('')
                .prop('disabled', true);

            $('#assessmentIsGraded')
                .val('Y')
                .prop('disabled', true);
        }
    }

    function handleDeliveryModeChange() {
        const mode = $('#assessmentDeliveryMode').val();

        if (mode === 'Onsite') {
            $('#assessmentCategorySelect').removeClass('d-none');
            $('#assessmentCategoryText').addClass('d-none');

            applyOnsiteRules();
        } else {
            // ONLINE → everything free
            $('#assessmentCategorySelect').addClass('d-none');
            $('#assessmentCategoryText').removeClass('d-none');

            resetAssessmentFields();
        }
    }

    // Events
    $(document).on('change', '#assessmentDeliveryMode', handleDeliveryModeChange);
    $(document).on('change', '#assessmentCategorySelect', applyOnsiteRules);


    //

    $(document).on('click', '#btnSubmitAssessment', function () {
        const deliveryMode = $('#assessmentDeliveryMode').val();
        const category = deliveryMode === 'Onsite'
            ? $('#assessmentCategorySelect').val()
            : $('#assessmentCategoryText').val();

        // Critical Section -- Custom validation logic

        // Clean disabled fields before submit
        if ($('#assessmentDeliveryMode').val() === 'Onsite') {
            const category = $('#assessmentCategorySelect').val();

            if (category === 'Mid I' || category === 'Mid II' || category === 'Final') {
                $('#assessmentGradingGroup').val('');
                $('#assessmentCountBestOf').val('');
            }

            if (category === 'Quiz') {
                $('#assessmentGradingGroup').val('Quiz');
            }
        }

        // =============================



        const payload = {
            OfferingId: $('#assessmentOfferingId').val(),
            Title: $('#assessmentTitle').val(),
            Description: $('#assessmentDescription').val(),
            DeliveryMode: deliveryMode,
            Category: category,
            DueDate: $('#assessmentDueDate').val() || null,
            TotalMarks: parseFloat($('#assessmentTotalMarks').val()) || 0,
            ActualWtg: parseFloat($('#assessmentActualWtg').val()) || 0,
            IsGraded: $('#assessmentIsGraded').val(),
            GradingGroup: $('#assessmentGradingGroup').val(),
            CountBestOf: $('#assessmentCountBestOf').val() ? parseInt($('#assessmentCountBestOf').val()) : null
        };

        if (!payload.Title || !payload.DeliveryMode || !payload.TotalMarks) {
            alert('Please fill all required fields.');
            return;
        }

        $.ajax({
            url: '/Instructor/AddAssessment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (response) {
                if (response.success) {
                    $('#addAssessmentModal').modal('hide');
                    $('#classroomTabs .nav-link[data-tab="classwork"]').trigger('click');
                } else {
                    alert(response.message || 'Unable to add assessment.');
                }
            },
            error: function () {
                alert('Unable to add assessment.');
            }
        });
    });

    $(document).on('click', '.btn-view-assignment', function () {
        const assignmentId = $(this).data('assignment-id');
        $('#assignmentDetailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>');
        const offcanvas = new bootstrap.Offcanvas(document.getElementById('assignmentDetailOffcanvas'));
        offcanvas.show();

        $.get(`/Instructor/AssignmentDetail/${assignmentId}`, function (result) {
            $('#assignmentDetailContent').html(result);
        }).fail(function () {
            $('#assignmentDetailContent').html('<div class="alert alert-danger">Unable to load assignment detail.</div>');
        });
    });

    // We remove this original code
    /*$(document).on('click', '.btn-grade-assignment', function () {
        const assignmentId = $(this).data('assignment-id');
        window.location.href = `/Instructor/Grade/${assignmentId}`;
    });*/

    // Critical Section because we replace with a new and custom one

    $(document).on('click', '.btn-grade-assignment', function () {
        const assignmentId = $(this).data('assignment-id');

        $('#assignmentDetailOffcanvas').offcanvas('show');
        $('#assignmentDetailContent').html(`
        <div class="text-center py-5 text-muted">
            <div class="spinner-border"></div>
        </div>`);

        $.get(`/Instructor/GradeAssignment/${assignmentId}`, function (html) {
            $('#assignmentDetailContent').html(html);
        });
    });
    // ==================

    // =============== NEW FUNCTION FOR GRADING ONSITE EXAMS ===============
    $(document).on('click', '#btnSaveAllOnsite', function () {

        const rows = [];
        let hasError = false;

        $('#assignmentDetailContent tbody tr').each(function () {

            const row = $(this);

            const enrollId = row.data('enroll-id');
            const entryId = row.data('entry-id') || 0;

            const marksInput = row.find('.exam-marks');
            const marks = parseFloat(marksInput.val());
            const max = parseFloat(marksInput.attr('max'));

            // Skip empty rows (optional)
            if (!marks && marks !== 0) return;

            if (marks > max) {
                marksInput.addClass('is-invalid');
                hasError = true;
                return;
            }

            marksInput.removeClass('is-invalid');

            rows.push({
                EntryId: entryId,
                AssignmentId: parseInt($('#gradingAssignmentId').val()),
                EnrollmentId: enrollId, // This maps to ENROLL_ID in my SQL above
                MarksObtained: marks,
                ExamDate: row.find('.exam-date').val() || null, // Handle empty dates
                Remarks: row.find('.exam-remarks').val()
            });

        });

        if (hasError) {
            alert('Some marks exceed allowed total.');
            return;
        }

        if (rows.length === 0) {
            alert('Nothing to save.');
            return;
        }

        const btn = $(this);
        btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: '/Instructor/BulkGradeOnsite',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(rows),

            success: function (res) {

                btn.text('Saved');

                // Update UI
                res.updated.forEach(x => {
                    const row = $(`tr[data-enroll-id="${x.enrollmentId}"]`);

                    // update entryId if newly inserted
                    row.attr('data-entry-id', x.entryId);

                    // update badge
                    row.find('.status-badge')
                        .removeClass('bg-secondary')
                        .addClass('bg-success')
                        .text('Graded');
                });
            },

            error: function () {
                btn.prop('disabled', false).text('Save All');
                alert('Error saving grades');
            }
        });
    });

    // =====================

    $(document).on('click', '.btn-toggle-post', function () {
        const targetId = $(this).data('target');
        const $body = $('#' + targetId);
        const expanded = $body.css('max-height') !== '60px';
        if (expanded) {
            $body.css('max-height', '60px');
            $(this).find('.toggle-label').text('View more');
            $(this).find('i').removeClass('bi-chevron-up').addClass('bi-chevron-down');
        } else {
            $body.css('max-height', '1000px');
            $(this).find('.toggle-label').text('View less');
            $(this).find('i').removeClass('bi-chevron-down').addClass('bi-chevron-up');
        }
    });

    function initInstructorClassroom() {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }

    initInstructorClassroom();
});
