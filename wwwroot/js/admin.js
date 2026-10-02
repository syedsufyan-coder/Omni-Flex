// admin.js - Dashboard interactions
$(function() {
  // Stat counter animation
  function animateCounter(el, target, duration = 1500) {
    let start = 0;
    const step = target / (duration / 16);
    const timer = setInterval(() => {
      start = Math.min(start + step, target);
      el.textContent = Math.floor(start).toLocaleString();
      if (start >= target) clearInterval(timer);
    }, 16);
  }

  // Animate stats on load
  $('.stat-number').each(function() {
    const target = parseInt($(this).data('value'));
    animateCounter(this, target);
  });

  // Table search filter
  $('#tableSearch').on('keyup', function() {
    const val = $(this).val().toLowerCase();
    $('#coursesTable tbody tr').filter(function() {
      $(this).toggle($(this).text().toLowerCase().includes(val));
    });
  });

  // Table column sort
  $('.sortable').on('click', function() {
    const table = $(this).closest('table');
    const index = $(this).index();
    const rows = table.find('tbody tr').toArray();
    const isAsc = $(this).hasClass('asc');

    rows.sort((a, b) => {
      const aVal = $(a).find('td').eq(index).text().toLowerCase();
      const bVal = $(b).find('td').eq(index).text().toLowerCase();
      return isAsc ? aVal.localeCompare(bVal) : bVal.localeCompare(aVal);
    });

    table.find('tbody').empty().append(rows);
    $('.sortable').removeClass('asc desc');
    $(this).addClass(isAsc ? 'desc' : 'asc');
    $(this).find('.sort-icon').remove();
    $(this).append(`<i class="bi bi-sort-${isAsc ? 'down' : 'up'} sort-icon ms-1"></i>`);
  });

  // Delete modal trigger
  $('.btn-delete').on('click', function() {
    const rowName = $(this).data('course-name');
    $('#deleteModalLabel').text('Delete ' + rowName + '?');
    $('#confirmDeleteModal').modal('show');
  });

  // Dynamic greeting
  const hour = new Date().getHours();
  const greeting = hour < 12 ? 'Good Morning' : hour < 17 ? 'Good Afternoon' : 'Good Evening';
  $('#greeting-text').text(greeting);

  // Dynamic date
  $('#currentDate').text(new Date().toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }));

  // Ripple effect on buttons
  $('.btn').on('click', function(e) {
    const btn = $(this);
    const x = e.pageX - btn.offset().left;
    const y = e.pageY - btn.offset().top;

    const ripple = $('<span class="ripple"></span>');
    ripple.css({
      left: x - 10,
      top: y - 10
    });

    btn.append(ripple);
    setTimeout(() => ripple.remove(), 500);
  });

  // Sidebar mobile toggle
  $('#sidebarToggle').on('click', function() {
    $('body').toggleClass('sidebar-open');
  });

  $('.sidebar-overlay').on('click', function() {
    $('body').removeClass('sidebar-open');
  });

  // Active nav link
  const currentPath = window.location.pathname;
  $('.sidebar .nav-link').each(function() {
    if ($(this).attr('href') === currentPath) {
      $(this).addClass('active');
    }
  });
});

<div class="col-lg-5 mb-3">
  <div class="card shadow-sm border-0">
    <div class="card-header bg-white border-0 py-3">
        <h6 class="mb-0 fw-bold" style="font-family:Sora; color: #2d3436;">Quick Actions</h6>
    </div>
    <div class="card-body d-grid gap-2 pt-0">
      <button class="btn btn-action btn-light-blue text-start py-3 mb-1" data-bs-toggle="modal" data-bs-target="#addCourseModal">
        <i class="bi bi-plus-circle-fill me-2 text-primary"></i> Add New Course
      </button>
      
      <a href="/admin/users" class="btn btn-action btn-light-purple text-start py-3 mb-1">
        <i class="bi bi-person-plus-fill me-2 text-purple"></i> Add New User
      </a>
      
      <a href="/admin/sections/assign" class="btn btn-action btn-light-orange text-start py-3 mb-1">
        <i class="bi bi-diagram-3-fill me-2 text-warning"></i> Assign Instructor
      </a>
      
      <a href="/admin/reports" class="btn btn-action btn-light-green text-start py-3">
        <i class="bi bi-bar-chart-fill me-2 text-success"></i> Generate Reports
      </a>
    </div>
  </div>
</div>
