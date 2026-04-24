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

    $(document).on('click', '#btnSubmitAssessment', function () {
        const deliveryMode = $('#assessmentDeliveryMode').val();
        const category = deliveryMode === 'Onsite'
            ? $('#assessmentCategorySelect').val()
            : $('#assessmentCategoryText').val();

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

    $(document).on('click', '.btn-grade-assignment', function () {
        const assignmentId = $(this).data('assignment-id');
        window.location.href = `/Instructor/Grade/${assignmentId}`;
    });

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
