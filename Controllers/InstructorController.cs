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

        public async Task<InstructorClassroomViewModel> GetClassroomMockData(string offeringId, string tab)
        {
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
                                IsLocked = false
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
        public async Task<IActionResult> AddAssessment([FromBody] OmniFlex.Models.Domain.Instructor.Assignment model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
                return Json(new { success = false, message = "Required fields are missing." });

            // TODO: Use real User ID from Auth
            const string hardcodedUserId = "U003";

            // Delivery Mode is handled here
            if(string.Equals(model.DeliveryMode,"onsite", StringComparison.OrdinalIgnoreCase))
            {
                model.DeliveryMode = "Physical"; // Because of DB constraint
            }

            try
            {
                var assessment = new OmniFlex.Models.Domain.Instructor.Assignment
                {
                    OfferingId = model.OfferingId,
                    Title = model.Title,
                    Description = model.Description,
                    DeliveryMode = model.DeliveryMode,
                    DueDate = model.DueDate,
                    Category = model.Category,
                    TotalMarks = model.TotalMarks,
                    ActualWtg = model.ActualWtg,
                    // Handling the 'Y'/'N' string to bool or keeping it as string 
                    // based on your table definition (CHAR(1))
                    IsGraded = model.IsGraded,
                    GradingGroup = model.GradingGroup,
                    CountBestOf = model.CountBestOf,
                    CreatedBy = hardcodedUserId,
                    CreatedAt = DateTime.Now
                };

                int newId = await _instructor.AddAssignmentAsync(assessment);

                return Json(new
                {
                    success = true,
                    message = "Assessment added successfully!",
                    assessmentId = newId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
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
                        IsLocked = false
                    }
                }
            };
            return PartialView("_InstructorAssignmentDetail", model);
        }

        [HttpGet("Instructor/GradeAssignment/{assignmentId}")]
        public async Task<IActionResult> GradeAssignment(int assignmentId)
        {
            var model = await _instructor.GetInstructorAssignmentDetailsAsync(assignmentId);
            if (model.DeliveryMode == "Physical")
            {
                // EnrollmentId is already present in the following model
                var examEntry = await _instructor.GetOnsiteAssignmentDetailsAsync(model.OfferingId, assignmentId);
                return PartialView("_GradeOnsite", examEntry);
            }

            return PartialView("_GradeOnline", model);
        }
        [HttpPost]
        public async Task<IActionResult> BulkGradeOnsite([FromBody] List<OmniFlex.Models.Domain.Instructor.Exam> models)
        {
            if (models == null || !models.Any()) return BadRequest();

            // 1. Prepare Metadata (Audit Fields)
            var currentUser = "U003";
            // TODO: We will remove this because this will be fetched from user identity once authentication is implemented

            var currentTime = DateTime.Now;

            foreach (var m in models)
            {
                // Set info for Inserts
                m.EnteredBy = currentUser;
                m.EnteredAt = currentTime;
                // Set info for Updates
                //m.UpdatedBy = currentUser;
                //m.UpdatedAt = currentTime;
            }

            // 2. Call Repository
            var updatedList = await _instructor.BulkUpsertGradesAsync(models);

            // 3. Project back to the specific JSON format your JS expects
            var response = updatedList.Select(x => new
            {
                enrollmentId = x.EnrollmentId,
                entryId = x.EntryId
            });

            return Json(new { success = true, updated = response });
        }
        [HttpPost]
        public async Task<IActionResult> AddPost([FromBody] PostDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Title))
                return Json(new { success = false, message = "Title is required." });

            // TODO: Remove hardcoded PostedBy once Auth is implemented
            const string hardcodedUserId = "U003";
            // TODO: Remove it once you implement Auth and fetch real user info

            try
            {
                var post = new OmniFlex.Models.Domain.Instructor.CoursePost
                {
                    OfferingId = model.OfferingId,
                    PostType = model.PostType,
                    Title = model.Title,
                    Content = model.Body, // Mapping JS 'Body' to Model 'Content'
                    PostedBy = hardcodedUserId, // Placeholder until you add Auth
                    CreatedAt = DateTime.Now
                };

                int newId = await _instructor.AddCoursePostAsync(post);

                return Json(new
                {
                    success = true,
                    message = "Post created successfully!",
                    postId = newId
                });
            }
            catch (Exception ex)
            {
                // For debugging, you might want to return ex.Message temporarily
                return Json(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        // Data Transfer Object to match your JSON payload
        public class PostDto
        {
            public string OfferingId { get; set; } = string.Empty;
            public string PostType { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
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
