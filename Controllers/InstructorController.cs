using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.Repositories.Instructor;
using OmniFlex.Models.ViewModels.Instructor;
using OmniFlex.Models.ViewModels.Admin;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Services;

namespace OmniFlex.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IUserRepository _users;
        private readonly ICourseRepository _courses;
        private readonly ISectionRepository _sections;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IAttendanceRepository _attendance;
        private readonly IInstructorRepository _instructor;

        public InstructorController(
            IUserRepository users,
            ICourseRepository courses,
            ISectionRepository sections,
            IEnrollmentRepository enrollments,
            IAttendanceRepository attendance,
            IInstructorRepository instructor)
        {
            _users = users;
            _courses = courses;
            _sections = sections;
            _enrollments = enrollments;
            _attendance = attendance;
            _instructor = instructor;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = await _instructor.GetDashboardAsync("U003");
            ViewData["ActivePage"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";
            return View(model);
        }

        public async Task<IActionResult> Courses()
        {
            var model = await _instructor.GetCoursesAsync("U003");
            ViewData["ActivePage"] = "Courses";
            ViewData["PageTitle"] = "Courses";
            return View(model);
        }
/*
        public async Task<IActionResult> WeeklyCalendar()
        {
            var model = await _instructor.GetWeeklyCalendarAsync("I005");
            ViewData["ActivePage"] = "WeeklyCalendar";
            ViewData["PageTitle"] = "Weekly Calendar";
            return View(model);
        }

        public async Task<IActionResult> ManageAttendance(string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null)
        {
            var model = await _instructor.GetManageAttendanceModelAsync("I005", selectedCourseId, selectedSectionId, selectedMonth);
            ViewData["ActivePage"] = "ManageAttendance";
            ViewData["PageTitle"] = "Manage Attendance";
            return View(model);
        }*/
    }
}
