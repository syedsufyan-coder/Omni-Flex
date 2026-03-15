// landing.js - Typed.js usage + hover glow
$(function(){
  // Typed.js initialization if present
  if(window.Typed){
    new Typed('#typed-subtitle', {strings:["Your Learning. Streamlined.", "Attendance. Grades. Courses.", "Built for FAST-NUCES.", "One Portal. Every Role."], typeSpeed:45, backSpeed:25, backDelay:2000, loop:true});
  }

  // Card hover glow
  $('.role-card').hover(function(){
    var glow = $(this).data('glow') || 'rgba(59,130,246,0.32)';
    $(this).css('box-shadow', '0 20px 50px ' + glow);
  }, function(){
    $(this).css('box-shadow', '');
  });

  // Card click press
  $('.role-card').on('click', function(e){
    var href = $(this).data('href') || $(this).find('a').attr('href') || '/auth/login?role=student';
    $(this).addClass('card-pressed');
    setTimeout(function(){ window.location = href; }, 220);
  });

  // Staggered animate.css delays
  $('.role-card').each(function(i){
    $(this).css('animation-delay', (0.1 * (i+1)) + 's');
  });
});
