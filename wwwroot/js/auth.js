// auth.js - login page interactions
$(function(){
  // Determine role from query params
  var params = new URLSearchParams(window.location.search);
  var role = (params.get('role') || 'student').toLowerCase();
  var map = {
    admin:{badge:'Administrator', icon:'bi-shield-check'},
    instructor:{badge:'Instructor', icon:'bi-person-workspace'},
    'teaching assistant':{badge:'Teaching Assistant', icon:'bi-clipboard2-check'},
    ta:{badge:'Teaching Assistant', icon:'bi-clipboard2-check'},
    student:{badge:'Student', icon:'bi-book-half'}
  };
  var info = map[role] || map['student'];
  $('#roleBadge').text(info.badge);
  $('#roleIcon').addClass(info.icon);

  // Password toggle
  $('#togglePassword').on('click', function(){
    var $input = $('#passwordInput');
    var type = $input.attr('type') === 'text' ? 'password' : 'text';
    $input.attr('type', type);
    $(this).find('i').toggleClass('bi-eye bi-eye-slash');
  });

  // Form submit simulation
  $('#loginForm').on('submit', function(e){
    e.preventDefault();
    var email = $('#identifier').val().trim();
    var pw = $('#passwordInput').val().trim();
    if(!email || !pw){
      if(!email) $('#identifier').addClass('is-invalid'); else $('#identifier').removeClass('is-invalid');
      if(!pw) $('#passwordInput').addClass('is-invalid'); else $('#passwordInput').removeClass('is-invalid');
      return;
    }
    var $btn = $('#signInBtn');
    $btn.prop('disabled', true);
    $btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Signing in...');
    setTimeout(function(){ window.location = '/admin/dashboard'; }, 1500);
  });
});
