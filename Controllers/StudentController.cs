using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels.Student;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Services;
using OmniFlex.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using OmniFlex.Models.Domain.Admin;
using System.Threading.Tasks;

namespace OmniFlex.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserRepository _users;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IAttendanceRepository _attendance;
        private readonly IResultRepository _results;

        public StudentController(
            IUserRepository users,
            IEnrollmentRepository enrollments,
            IAttendanceRepository attendance,
            IResultRepository results
            )
        {
            _users = users;
            _enrollments = enrollments;
            _attendance = attendance;
            _results = results;
        }

        private string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour < 12 ? "Good morning"
                 : hour < 17 ? "Good afternoon"
                 : "Good evening";
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private string? GetCurrentUserId()
            => HttpContext.Session.GetString("UserId");

        private IActionResult RedirectToLogin()
            => RedirectToAction("Login", "Auth", new { role = "student" });

        public class AddPostCommentRequest
        {
            public long PostId { get; set; }
            public string Content { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment([FromBody] AddPostCommentRequest request)
        {
            if (request == null || request.PostId <= 0 || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { success = false, message = "Comment content is required." });
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var rowsAffected = await _enrollments.AddPostCommentAsync(request.PostId, userId, request.Content.Trim());
            if (rowsAffected <= 0)
            {
                return StatusCode(500, new { success = false, message = "Unable to save comment." });
            }

            return Ok(new
            {
                success = true,
                comment = new
                {
                    PostId = request.PostId,
                    CommentorName = "You",
                    Content = request.Content.Trim(),
                    CommentedTimeAndDate = DateTime.Now.ToString("t")
                }
            });
        }

        // TO-DO: Remove -Theory concatenation from course names in and handle it in view instead based on credit hours or course type
        public async Task<IActionResult> Dashboard()
        {
            // 1. Get the current User ID from session
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            // 2. Fetch data from your service/repository
            var studentDetails = await _users.GetDetailsByIdAsync(userId);
            var enrolledCourses = await _users.GetEnrolledCoursesAsync(userId);

            if (studentDetails == null)
            {
                return NotFound();
            }

            // 3. Map Data to DashboardViewModel
            var model = new DashboardViewModel
            {
                // Student Info
                StudentName = $"{studentDetails.FirstName} {studentDetails.LastName}",
                FullName = $"{studentDetails.FirstName} {studentDetails.LastName}",
                GreetingMessage = GetGreeting(),
                RollNo = studentDetails.UserId,
                Status = studentDetails.Status,
                BloodGroup = 0.ToString(), // Placeholder, as it's not in StudentDto
                Nationality = "Pakistani", // Placeholder, as it's not in StudentDto

                // Academic Info
                Degree = studentDetails.DegreeProgram,
                Batch = studentDetails.BatchYear,

                // Contact Info
                Email = studentDetails.Email,
                Gender = studentDetails.Gender,
                DateOfBirth = studentDetails.DOB.ToString("dd-MMM-yyyy"),
                MobileNo = studentDetails.PhoneNumber,
                Address = studentDetails.Address,
                City = studentDetails.City,
                Country = studentDetails.Country,

                // Courses List
                EnrolledCourses = enrolledCourses,

                // Additional Info (Map from first course if available, or leave defaults)
                Section = enrolledCourses.FirstOrDefault()?.SectionLabel.ToString() ?? "N/A"
            };
            if (model.EnrolledCourses != null)
            {
                foreach (var course in model.EnrolledCourses)
                {
                    course.SectionLabel = SectionHelper.GetFormattedSectionLabel(course.Degree, course.SectionLabel.ToString(), course.Batch);
                    course.CourseName += course.CreditHours == 1 ? " - Lab" : " - Theory";
                }
                model.Section = model.EnrolledCourses.FirstOrDefault()?.SectionLabel ?? "N/A";
            }

            ViewData["ActivePage"] = "Dashboard";
            ViewData["PageTitle"] = "Dashboard";

            return View(model);
        }

        public IActionResult CourseRegistration()
        {
            ViewData["ActivePage"] = "CourseRegistration";
            ViewData["PageTitle"] = "Course Registration";
            return View();
        }

        public async Task<IActionResult> Enrolled()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            var enrolledCourses = await _enrollments.GetEnrolledClassesAsync(userId);

            var model = new EnrolledViewModel
            {
                ActiveClasses = enrolledCourses
            };
            if (model.ActiveClasses != null)
            {
                foreach (var course in model.ActiveClasses)
                {
                    course.Section = SectionHelper.GetFormattedSectionLabel(course.Degree, course.Section, course.Batch);
                    course.BannerColorClass = EnrollmentHelper.GetRandomDarkHex();
                }
            }
            ViewData["ActivePage"] = "Enrolled";
            ViewData["PageTitle"] = "Enrolled";
            return View(model);
        }

        public async Task<IActionResult> CourseDetails(string courseId, string tab = "stream")
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            var enrolledCourses = await _enrollments.GetEnrolledClassesAsync(userId);
            var course = enrolledCourses.FirstOrDefault(c => string.Equals(c.CourseCode, courseId, StringComparison.OrdinalIgnoreCase));
            if (course == null)
            {
                return NotFound();
            }

            var normalizedTab = string.IsNullOrWhiteSpace(tab) ? "stream" : tab.Trim().ToLowerInvariant();
            if (normalizedTab != "stream" && normalizedTab != "classwork" && normalizedTab != "people")
            {
                normalizedTab = "stream";
            }

            // TODO: Fetch real data for Course Description, Upcoming Due Count, Classwork Items, and People from the database instead of using placeholders.
            var model = new CourseDetailsViewModel
            {
                CourseId = course.CourseCode,
                CourseName = course.CourseName,
                InstructorName = course.TeacherName,
                Section = course.Section,
                Degree = course.Degree,
                Batch = course.Batch,
                ActiveTab = normalizedTab,
                CourseDescription = "A modern cohort course designed for strong participation, full transparency, and easy access to classwork.",
                UpcomingDueCount = 3,
                ClassworkItems = new List<string>
                {
                    "Assignment 1: Requirements Document",
                    "Quiz 1: Fundamentals of the course",
                    "Group Project Proposal"
                },
                People = new List<string>
                {
                    "Ayesha Khan",
                    "Bilal Ahmed",
                    "Fatima Noor",
                    "Omar Saeed"
                }
            };

            // 2. Fetch Posts ONLY if we are on the 'stream' tab
            if (normalizedTab == "stream")
            {
                var offeringId = await _enrollments.GetOfferingIdByCourseCodeAsync(course.CourseCode, userId);
                // NOTE: Ensure your service method uses the correct ID (OfferingId vs CourseCode)
                model.PostData = await _enrollments.GetCoursePostsAsync(offeringId);
            }

            if (IsAjaxRequest())
            {
                var partialName = normalizedTab switch
                {
                    "classwork" => "_CourseClasswork",
                    "people" => "_CoursePeople",
                    _ => "_CourseStream"
                };
                return PartialView(partialName, model);
            }

            ViewData["ActivePage"] = "Enrolled";
            ViewData["PageTitle"] = course.CourseName;
            return View(model);
        }

        // TO-DO: Implement the following action and related repository method for the TODO Page feature.
        public IActionResult ToDo()
        {
            ViewData["ActivePage"] = "ToDo";
            ViewData["PageTitle"] = "TO-DO";
            var model = new ToDoViewModel
            {
                AssignedItems = new List<ToDoItem>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", CourseColorClass="bg-primary", AssignmentTitle="SRS Document", DueDate=DateTime.Now.AddDays(2), Status="Assigned" },
                    new() { CourseCode="CS3014", CourseName="Database Systems", CourseColorClass="bg-success", AssignmentTitle="ER Diagram", DueDate=DateTime.Now.AddDays(3), Status="Assigned" },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", CourseColorClass="bg-warning", AssignmentTitle="Process Scheduling", DueDate=DateTime.Now.AddDays(5), Status="Assigned" }
                },
                MissingItems = new List<ToDoItem>
                {
                    new() { CourseCode="CL3012", CourseName="Software Engineering Lab", CourseColorClass="bg-danger", AssignmentTitle="Unit Testing", DueDate=DateTime.Now.AddDays(-1), Status="Missing" },
                    new() { CourseCode="CS3018", CourseName="Computer Networks", CourseColorClass="bg-info", AssignmentTitle="Subnetting Quiz", DueDate=DateTime.Now.AddDays(-2), Status="Missing" }
                },
                DoneItems = new List<ToDoItem>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", CourseColorClass="bg-primary", AssignmentTitle="Version Control Setup", DueDate=DateTime.Now.AddDays(-10), Status="Done" },
                    new() { CourseCode="CS3014", CourseName="Database Systems", CourseColorClass="bg-success", AssignmentTitle="Normalization Exercise", DueDate=DateTime.Now.AddDays(-8), Status="Done" },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", CourseColorClass="bg-warning", AssignmentTitle="Memory Management", DueDate=DateTime.Now.AddDays(-6), Status="Done" },
                    new() { CourseCode="CS3018", CourseName="Computer Networks", CourseColorClass="bg-info", AssignmentTitle="Network Layers", DueDate=DateTime.Now.AddDays(-4), Status="Done" }
                }
            };
            return View(model);
        }

        public IActionResult Calendar()
        {
            ViewData["ActivePage"] = "Calendar";
            ViewData["PageTitle"] = "Calendar";
            var now = DateTime.Now;
            var model = new CalendarViewModel
            {
                CurrentYear = now.Year,
                CurrentMonth = now.Month,
                Events = new List<CalendarEvent>
                {
                    new() { Title="SRS Deadline", DueDate= new DateTime(now.Year, now.Month, 5), CourseCode="CS3012", ColorClass="bg-primary", Type="Assignment" },
                    new() { Title="Database Quiz", DueDate= new DateTime(now.Year, now.Month, 9), CourseCode="CS3014", ColorClass="bg-success", Type="Quiz" },
                    new() { Title="OS Lab", DueDate= new DateTime(now.Year, now.Month, 14), CourseCode="CS3016", ColorClass="bg-warning text-dark", Type="Lab" },
                    new() { Title="Networks Test", DueDate= new DateTime(now.Year, now.Month, 21), CourseCode="CS3018", ColorClass="bg-danger", Type="Exam" },
                    new() { Title="Midterm", DueDate= new DateTime(now.Year, now.Month, 25), CourseCode="CS3012", ColorClass="bg-info", Type="Exam" },
                    new() { Title="Final Project", DueDate= new DateTime(now.Year, now.Month, 28), CourseCode="CL3012", ColorClass="bg-secondary", Type="Assignment" }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> Attendance()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            ViewData["ActivePage"] = "Attendance";
            ViewData["PageTitle"] = "Attendance";
            var model = await _attendance.GetStudentAttendanceAsync(userId);
            return View(model);
        }

        public IActionResult Marks()
        {
            ViewData["ActivePage"] = "Marks";
            ViewData["PageTitle"] = "Marks";
            var model = new MarksViewModel
            {
                Courses = new List<CourseMarks>
                {
                    new() { CourseCode="CS3012", CourseName="Software Engineering", Section="BCS-2G", EnrollmentStatus="Enrolled",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=18, TotalMarks=20, Remarks="Good" },
                            new() { Component="Mid", ObtainedMarks=45, TotalMarks=50, Remarks="Very Good" },
                            new() { Component="Final", ObtainedMarks=80, TotalMarks=100, Remarks="Excellent" }
                        },
                        TotalObtained=143, TotalMax=170
                    },
                    new() { CourseCode="CS3014", CourseName="Database Systems", Section="BCS-2G", EnrollmentStatus="Registered",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=15, TotalMarks=20, Remarks="Satisfactory" },
                            new() { Component="Mid", ObtainedMarks=40, TotalMarks=50, Remarks="Good" },
                            new() { Component="Final", ObtainedMarks=70, TotalMarks=100, Remarks="Good" }
                        },
                        TotalObtained=125, TotalMax=170
                    },
                    new() { CourseCode="CS3016", CourseName="Operating Systems", Section="BCS-2G", EnrollmentStatus="Enrolled",
                        Assessments = new List<AssessmentMark>
                        {
                            new() { Component="Quiz 1", ObtainedMarks=12, TotalMarks=20, Remarks="Needs Improvement" },
                            new() { Component="Mid", ObtainedMarks=34, TotalMarks=50, Remarks="Average" },
                            new() { Component="Final", ObtainedMarks=60, TotalMarks=100, Remarks="Satisfactory" }
                        },
                        TotalObtained=106, TotalMax=170
                    }
                }
            };
            return View(model);
        }

        public async Task<IActionResult> Transcript()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToLogin();
            }

            ViewData["ActivePage"] = "Transcript";
            ViewData["PageTitle"] = "Transcript";
            var model = await _results.GetDetailedTranscriptAsync(userId);
            return View(model);
        }

        public IActionResult ArchivedClasses()
        {
            ViewData["ActivePage"] = "ArchivedClasses";
            ViewData["PageTitle"] = "Archived Classes";
            var model = new EnrolledViewModel
            {
                ActiveClasses = new List<ClassCard>
                {
                    new() { CourseCode="CS2010", CourseName="Intro to Python", TeacherName="Ms. Nadia Shah", Section="BCS-1G", BannerColorClass="#6f42c1", IsArchived=true },
                    new() { CourseCode="CS2020", CourseName="Web Technologies", TeacherName="Mr. Salman Rehman", Section="BCS-1G", BannerColorClass="#fd7e14", IsArchived=true }
                }
            };
            return View(model);
        }

        public IActionResult Settings()
        {
            ViewData["ActivePage"] = "Settings";
            ViewData["PageTitle"] = "Settings";
            return View();
        }
    }
}
