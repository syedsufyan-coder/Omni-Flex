using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.Repositories.Instructor;
using OmniFlex.Models.ViewModels.Instructor;

namespace OmniFlex.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IInstructorRepository _instructorRepository;

        public InstructorController(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = await _instructorRepository.GetDashboardAsync("I005");
            ViewData["ActivePage"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";
            return View(model);
        }

        public async Task<IActionResult> Courses()
        {
            var model = await _instructorRepository.GetCoursesAsync("I005");
            ViewData["ActivePage"] = "Courses";
            ViewData["PageTitle"] = "Courses";
            return View(model);
        }

        public async Task<IActionResult> WeeklyCalendar()
        {
            var model = await _instructorRepository.GetWeeklyCalendarAsync("I005");
            ViewData["ActivePage"] = "WeeklyCalendar";
            ViewData["PageTitle"] = "Weekly Calendar";
            return View(model);
        }

        public async Task<IActionResult> ManageAttendance(string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null)
        {
            var model = await _instructorRepository.GetManageAttendanceModelAsync("I005", selectedCourseId, selectedSectionId, selectedMonth);
            ViewData["ActivePage"] = "ManageAttendance";
            ViewData["PageTitle"] = "Manage Attendance";
            return View(model);
        }
    }
}
