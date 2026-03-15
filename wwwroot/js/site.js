// site.js - Global site interactions
$(function() {
  // Bootstrap tooltip initialization
  $('[data-bs-toggle="tooltip"]').tooltip();

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

  // Active nav link detection
  const currentPath = window.location.pathname;
  $('.sidebar .nav-link').each(function() {
    if ($(this).attr('href') === currentPath) {
      $(this).addClass('active');
    }
  });

  // Current date/time display (for any elements with id="currentDate")
  if ($('#currentDate').length) {
    $('#currentDate').text(new Date().toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    }));
  }
});
