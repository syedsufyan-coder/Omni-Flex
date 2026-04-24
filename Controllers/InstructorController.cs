using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.Repositories.Instructor;
using OmniFlex.Models.ViewModels.Instructor;
using OmniFlex.Models.ViewModels.Admin;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Services;
using System.Threading.Tasks;

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

        public async Task<InstructorClassroomViewModel> GetClassroomMockData(string offeringId, string tab){
            var model = new InstructorClassroomViewModel
            {
                OfferingId = offeringId,
                CourseId = offeringId,
                CourseName = offeringId == "" ? "Course Title" : offeringId,
                SectionName = "BCS-1A",
                Semester = "Fall 2025",
                InstructorId = "U003",
                ActiveTab = tab,
                EnrolledCount = 32,
                Posts = new List<InstructorCoursePostViewModel>
                {
                    new InstructorCoursePostViewModel
                    {
                        PostId = 1,
                        PostType = "ANNOUNCEMENT",
                        Title = "Welcome to the course",
                        Body = "Please review the syllabus and complete the pre-course survey.",
                        PostedAt = DateTime.Now.AddDays(-2),
                        PostedByName = "Dr. Asad Khan",
                        AttachedFileNames = new List<string> { "Syllabus.pdf", "Week1Notes.docx" }
                    }
                },
                Assignments = new List<InstructorAssignmentViewModel>
                {
                    new InstructorAssignmentViewModel
                    {
                        AssignmentId = 1,
                        OfferingId = offeringId,
                        Title = "Assignment 1",
                        Description = "Complete the coding exercise.",
                        DeliveryMode = "Online",
                        DueDate = DateTime.Now.AddDays(7),
                        Category = "Assignment",
                        TotalMarks = 10,
                        ActualWtg = 5,
                        IsGraded = "Y",
                        GradingGroup = "Assignments",
                        CountBestOf = 1,
                        CreatedAt = DateTime.Now.AddDays(-5),
                        SubmissionCount = 18,
                        TotalEnrolled = 32,
                        Submissions = new List<InstructorSubmissionSummaryViewModel>
                        {
                            new InstructorSubmissionSummaryViewModel
                            {
                                SubmissionId = 1,
                                StudentName = "Amina Bibi",
                                StudentId = "21K-0012",
                                SubmitDate = DateTime.Now.AddDays(-1),
                                ObtainedMarks = null,
                                IsLate = "N",
                                Locked = "N"
                            }
                        }
                    },
                    new InstructorAssignmentViewModel
                    {
                        AssignmentId = 2,
                        OfferingId = offeringId,
                        Title = "Mid I Quiz",
                        Description = "Midterm quiz covering weeks 1-4.",
                        DeliveryMode = "Onsite",
                        DueDate = DateTime.Now.AddDays(14),
                        Category = "Mid I",
                        TotalMarks = 30,
                        ActualWtg = 20,
                        IsGraded = "N",
                        GradingGroup = "Exams",
                        CountBestOf = null,
                        CreatedAt = DateTime.Now.AddDays(-3),
                        SubmissionCount = 0,
                        TotalEnrolled = 32
                    }
                }
            };
            
            return model;
        }

        public async Task<IActionResult> Classroom(string offeringId, string tab = "stream")
        {
            var model = await _instructor.GetInstructorClassroomAsync(offeringId);
            ViewData["PageTitle"] = $"{model.CourseName} — Instructor";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetStreamPartial(string offeringId)
        {
            var model = await _instructor.GetInstructorClassroomAsync(offeringId);
            return PartialView("_InstructorStream", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetClassworkPartial(string offeringId)
        {
            var model = await _instructor.GetInstructorClassroomAsync(offeringId);
            return PartialView("_InstructorClasswork", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPeoplePartial(string offeringId)
        {
            var model = await _instructor.GetInstructorPeopleAsync(offeringId);
            return PartialView("_InstructorPeople", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetGradesPartial(string offeringId)
        {
            var model = await _instructor.GetInstructorGradesGridAsync(offeringId);
            return PartialView("_InstructorGrades", model);
        }

        [HttpPost]
        public IActionResult AddPost([FromBody] CreatePostViewModel model)
        {
            return Json(new { success = true, message = "Post added (stub)." });
        }

        [HttpPost]
        public IActionResult AddAssessment([FromBody] CreateAssignmentViewModel model)
        {
            return Json(new { success = true, message = "Assessment added (stub)." });
        }

        [HttpGet("Instructor/AssignmentDetail/{assignmentId}")]
        public async Task<IActionResult> AssignmentDetail(int assignmentId)
        {
            var model = await _instructor.GetInstructorAssignmentDetailsAsync(assignmentId);
            var modeldummy = new InstructorAssignmentViewModel
            {
                AssignmentId = assignmentId,
                Title = "Assignment Detail",
                Description = "Detailed instructions will appear here.",
                DeliveryMode = "Online",
                Category = "Assignment",
                TotalMarks = 10,
                ActualWtg = 5,
                IsGraded = "Y",
                GradingGroup = "Assignments",
                CountBestOf = 1,
                DueDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now.AddDays(-10),
                SubmissionCount = 12,
                TotalEnrolled = 32,
                Submissions = new List<InstructorSubmissionSummaryViewModel>
                {
                    new InstructorSubmissionSummaryViewModel
                    {
                        SubmissionId = 1,
                        StudentName = "Ali Khan",
                        StudentId = "21K-0090",
                        SubmitDate = DateTime.Now.AddDays(-1),
                        ObtainedMarks = null,
                        IsLate = "N",
                        Locked = "N"
                    }
                }
            };
            return PartialView("_InstructorAssignmentDetail", model);
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
