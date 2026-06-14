var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<OmniFlex.Infrastructure.DbConnectionFactory>();

builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IUserRepository, OmniFlex.Models.Repositories.Admin.UserRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.ICourseRepository, OmniFlex.Models.Repositories.Admin.CourseRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.ISectionRepository, OmniFlex.Models.Repositories.Admin.SectionRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IEnrollmentRepository, OmniFlex.Models.Repositories.Admin.EnrollmentRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IAnnouncementRepository, OmniFlex.Models.Repositories.Admin.AnnouncementRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IAssignmentRepository, OmniFlex.Models.Repositories.Admin.AssignmentRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.ISubmissionRepository, OmniFlex.Models.Repositories.Admin.SubmissionRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IAttendanceRepository, OmniFlex.Models.Repositories.Admin.AttendanceRepository>();
builder.Services.AddScoped<OmniFlex.Models.Repositories.Admin.IResultRepository, OmniFlex.Models.Repositories.Admin.ResultRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
