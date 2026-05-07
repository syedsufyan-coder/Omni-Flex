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

        private string? GetCurrentUserId()
            => HttpContext.Session.GetString("UserId");

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "Auth", new { role = "instructor" });

        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            var model = await _instructor.GetDashboardAsync(userId);
            ViewData["ActivePage"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";
            return View(model);
        }

        public async Task<IActionResult> Courses()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            var model = await _instructor.GetCoursesAsync(userId);
            ViewData["ActivePage"] = "Courses";
            ViewData["PageTitle"] = "Courses";
            return View(model);
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

            var currentUser = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUser))
                return Unauthorized();

            // Delivery Mode is handled here
            if (string.Equals(model.DeliveryMode, "onsite", StringComparison.OrdinalIgnoreCase))
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
                    CreatedBy = currentUser,
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

            var currentUser = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUser)) return Unauthorized();

            // 1. Prepare Metadata (Audit Fields)

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

            var currentUser = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUser))
                return Unauthorized();

            try
            {
                var post = new OmniFlex.Models.Domain.Instructor.CoursePost
                {
                    OfferingId = model.OfferingId,
                    PostType = model.PostType,
                    Title = model.Title,
                    Content = model.Body, // Mapping JS 'Body' to Model 'Content'
                    PostedBy = currentUser, // Placeholder until you add Auth
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


        // TODO: Implement the following action and related repository method for the Weekly Calendar feature.
        /*public async Task<IActionResult> WeeklyCalendar()
        {
            var model = await _instructor.GetWeeklyCalendarAsync("I005");
            ViewData["ActivePage"] = "WeeklyCalendar";
            ViewData["PageTitle"] = "Weekly Calendar";
            return View(model);
        }*/

        public async Task<IActionResult> ManageAttendance(string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            var model = await _instructor.GetManageAttendanceModelAsync(userId, selectedCourseId, selectedSectionId, selectedMonth);
            ViewData["ActivePage"] = "ManageAttendance";
            ViewData["PageTitle"] = "Manage Attendance";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BulkAddAttendance([FromBody] BulkAddAttendanceRequest request)
        {
            // TODO: maybe define some logic in the following function called
            var enrollIds = await _instructor.GetEnrollmentIDsForAttendace(request.CourseId, request.SectionId);

            if (enrollIds == null || !enrollIds.Any())
            {
                return Json(new { success = false, message = "No students enrolled in this section." });
            }

            var currentUser = GetCurrentUserId(); // Hardcoded for now per your logic
            if (string.IsNullOrWhiteSpace(currentUser)) return Unauthorized();
            var currentTime = DateTime.Now;

            var attendances = enrollIds.Select(enrollId => new OmniFlex.Models.Domain.Instructor.Attendance
            {
                EnrollId = enrollId,
                AttendanceDate = currentTime,
                Status = request.DefaultStatus,
                MarkedBy = currentUser,
                Duration = request.Duration
            }).ToList();

            // Call the Repository
            var resultList = await _instructor.BulkAddAttendanceRecordsAsync(attendances);

            var returnedRecords = resultList.Select(a => new
            {
                enrollId = a.EnrollId,
                attendanceId = a.AttendanceId
            });

            return Json(new
            {
                success = true,
                dateLabel = currentTime.ToString("dd-MMM"),
                records = returnedRecords
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSingleStatus([FromBody] UpdateAttendanceStatusRequest request)
        {
            if (request == null || request.AttendanceId <= 0) return BadRequest();

            // Use the repository to update the status
            bool isUpdated = await _instructor.UpdateAttendanceStatusAsync(request.AttendanceId, request.Status);

            if (!isUpdated)
            {
                return NotFound(new { message = "Attendance record not found." });
            }

            return Ok();
        }
    }
}
