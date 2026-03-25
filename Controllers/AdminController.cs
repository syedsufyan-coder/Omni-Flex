using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels.Admin;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Services;
using System.Diagnostics;

namespace OmniFlex.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _users;
        private readonly ICourseRepository _courses;
        private readonly ISectionRepository _sections;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IAnnouncementRepository _announcements;

        public AdminController(
            IUserRepository users,
            ICourseRepository courses,
            ISectionRepository sections,
            IEnrollmentRepository enrollments,
            IAnnouncementRepository announcements)
        {
            _users = users;
            _courses = courses;
            _sections = sections;
            _enrollments = enrollments;
            _announcements = announcements;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var vm = new AdminDashboardViewModel
                {
                    TotalStudents = await _users.GetCountByRoleAsync("Student"),
                    TotalInstructors = await _users.GetCountByRoleAsync("Instructor"),
                    TotalTAs = await _users.GetCountByRoleAsync("TA"),
                    ActiveCourses = (await _courses.GetActiveAsync())?.Count() ?? 0,
                };

                // Build CourseSummary list
                var courses = await _courses.GetAllAsync();
                var summary = new List<CourseViewModel>();

                if (courses != null)
                {
                    foreach (var c in courses.Take(5))
                    {
                        var sections = await _sections.GetByCourseAsync(c.CourseId);
                        summary.Add(new CourseViewModel
                        {
                            CourseId = c.CourseId,
                            CourseName = c.CourseName,
                            CreditHrs = c.CreditHrs,
                            CourseType = c.CourseType,
                            CourseCat = c.CourseCat,
                            PreReqId = c.PreReqId,
                            IsActive = c.IsActive == 1,
                            Status = c.IsActive == 1 ? "Active" : "Inactive"
                        });
                    }
                }

                vm.Courses = summary;
                ViewData["PageTitle"] = "Dashboard";
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Dashboard";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new AdminDashboardViewModel());
            }
        }

        public async Task<IActionResult> Courses()
        {
            try
            {
                var courses  = await _courses.GetAllAsync();
                var viewModels = new List<CourseViewModel>();

                if (courses != null)
                {
                    foreach (var c in courses)
                    {
                        viewModels.Add(new CourseViewModel
                        {
                            CourseId   = c.CourseId,
                            CourseName = c.CourseName,
                            CreditHrs  = c.CreditHrs,
                            CourseType = c.CourseType,
                            CourseCat  = c.CourseCat,
                            PreReqId   = c.PreReqId,
                            IsActive   = c.IsActive == 1,
                            Status     = c.IsActive == 1 ? "Active" : "Inactive"
                        });
                    }
                }

                ViewData["PageTitle"] = "Courses";
                return View(viewModels);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Courses";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new List<CourseViewModel>());
            }
        }

        public async Task<IActionResult> Sections()
        {
            try
            {
                var sections = await _sections.GetAllDetailedAsync();
                var viewModels = new List<SectionViewModel>();

                if (sections != null)
                {
                    foreach (var s in sections)
                    {
                        viewModels.Add(new SectionViewModel
                        {
                            SectionId = s.SectionId,
                            SectionName = SectionHelper.GetFormattedSectionLabel(s.Degree, s.SectionLabel, s.Batch) ?? "",
                            Department = s.Department ?? "",
                            EnrolledStudents = s.EnrolledStudents,
                            Seats = 50,
                            SeatsLeft = 50 - s.EnrolledStudents,
                            CrName = $"{s.CrFirstName} {s.CrLastName}",
                            BatchYear = s.Batch
                        });
                    }
                }

                ViewData["PageTitle"] = "Sections";
                return View(viewModels);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Sections";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new List<SectionViewModel>());
            }
        }

        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _users.GetAllAsync();
                var vm = BuildUserViewModels(users);
                ViewData["PageTitle"] = "Users";
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Users";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new List<UserViewModel>());
            }
        }

        public async Task<IActionResult> Instructors()
        {
            try
            {
                var users = await _users.GetByRoleAsync("Instructor");
                var vm = BuildUserViewModels(users);
                ViewData["PageTitle"] = "Instructors";
                return View("Users", vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Instructors";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new List<UserViewModel>());
            }
        }

        public async Task<IActionResult> TAs()
        {
            try
            {
                // TAs are Students — identified via SECTION_TAS table not by Role
                var users = await _users.GetByRoleAsync("Student");
                var vm = BuildUserViewModels(users);
                ViewData["PageTitle"] = "Teaching Assistants";
                return View("Users", vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Teaching Assistants";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new List<UserViewModel>());
            }
        }

        public async Task<IActionResult> Students()
        {
            try
            {
                var users = await _users.GetByRoleAsync("Student");
                var vm = BuildUserViewModels(users);
                ViewData["PageTitle"] = "Students";
                return View("Users", vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Students";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new List<UserViewModel>());
            }
        }

        public async Task<IActionResult> Announcements()
        {
            try
            {
                var announcements = await _announcements.GetAllSystemWideAsync();
                var vm = new List<AnnouncementViewModel>();

                if (announcements != null)
                {
                    foreach (var a in announcements)
                    {
                        var poster = await _users.GetByIdAsync(a.PostedBy);
                        vm.Add(new AnnouncementViewModel
                        {
                            AnnouncementId = a.AnnouncementId,
                            Title = a.Title,
                            Content = a.Content,
                            PostedByName = poster != null
                                ? $"{poster.FirstName} {poster.LastName}"
                                : "System",
                            Audience = a.Audience,
                            Priority = a.Priority,
                            IsPinned = a.IsPinned == 1,
                            SectionId = a.SectionId,
                            PostDate = a.PostDate,
                            TimeAgo = GetTimeAgo(a.PostDate)
                        });
                    }
                }

                vm = vm.OrderByDescending(x => x.IsPinned)
                       .ThenByDescending(x => x.PostDate)
                       .ToList();

                ViewData["PageTitle"] = "Announcements";
                return View(vm);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Announcements";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new List<AnnouncementViewModel>());
            }
        }

        public IActionResult Reports()
        {
            ViewData["PageTitle"] = "Reports & Monitoring";
            return View();
        }

        // Courses Fileration 
        [HttpPost] // ASP.NET is informed to accept only POST requests for this method defined
        // POST requests carry data in the request body, not in the URL
        public async Task<IActionResult> FilterCourses([FromBody] CourseFilterRequest filter)
        {
            try
            {
                // CHECK 1 — Make sure at least one filter field is filled
                // All fields cannot be null/empty at the same time
                bool hasFilter =
                    !string.IsNullOrWhiteSpace(filter.DeptId) ||
                    !string.IsNullOrWhiteSpace(filter.CourseType) ||
                    !string.IsNullOrWhiteSpace(filter.CourseCat) ||
                    !string.IsNullOrWhiteSpace(filter.TeacherId) ||
                    !string.IsNullOrWhiteSpace(filter.SectionId) ||
                    !string.IsNullOrWhiteSpace(filter.PreReqId) ||
                    filter.CreditHrs.HasValue;

                if (!hasFilter)
                    return Json(new { success = false, message = "Please Select a Filter and Enter a Value" });

                // CHECK 2 — Only ONE filter is allowed at a time
                // Count how many fields are filled
                int filterCount = 0;
                if (!string.IsNullOrWhiteSpace(filter.DeptId)) filterCount++;
                if (!string.IsNullOrWhiteSpace(filter.CourseType)) filterCount++;
                if (!string.IsNullOrWhiteSpace(filter.CourseCat)) filterCount++;
                if (!string.IsNullOrWhiteSpace(filter.TeacherId)) filterCount++;
                if (!string.IsNullOrWhiteSpace(filter.SectionId)) filterCount++;
                if (!string.IsNullOrWhiteSpace(filter.PreReqId)) filterCount++;
                if (filter.CreditHrs.HasValue) filterCount++;

                if (filterCount > 1)
                    return Json(new { success = false, message = "Only One Filter is Allowed at a Time" });

                // CHECK 3 — Length Validation on string fields
                // No field should be unreasonably long
                if ((filter.DeptId?.Length > 20) ||
                    (filter.CourseType?.Length > 20) ||
                    (filter.CourseCat?.Length > 20) ||
                    (filter.TeacherId?.Length > 20) ||
                    (filter.SectionId?.Length > 20) ||
                    (filter.PreReqId?.Length > 20))
                    return Json(new { success = false, message = "Your Input is Unreasonably Too Long" });

                // CHECK 4 — SQL Injection Protection on all string fields
                var allStringValues = new[]
                {   // Collecting all the filter values into an array to loop through it for protective measures
                    filter.DeptId, filter.CourseType, filter.CourseCat,
                    filter.TeacherId, filter.SectionId, filter.PreReqId
                };

                foreach (var val in allStringValues)
                {
                    if (val == null) continue;
                    if (val.Contains("'") || val.Contains(";") || val.Contains("--"))
                        return Json(new { success = false, message = "Your Input Posses Invalid Characters" });
                }

                // CHECK 5 — Range Validation for Credit Hours
                if (filter.CreditHrs.HasValue && (filter.CreditHrs < 1 || filter.CreditHrs > 4))
                    return Json(new { success = false, message = "Credit Hours Must be Between 1 and 4" });

                // CHECK 6 — Trim whitespace from all string fields silently if in case user has entered mistakenly
                filter.DeptId = filter.DeptId?.Trim();
                filter.CourseType = filter.CourseType?.Trim();
                filter.CourseCat = filter.CourseCat?.Trim();
                filter.TeacherId = filter.TeacherId?.Trim();
                filter.SectionId = filter.SectionId?.Trim();
                filter.PreReqId = filter.PreReqId?.Trim();

                // check which field is filled and call the respective repository method
                IEnumerable<CourseDto> result; // a list returning the courses filtered 

                if (!string.IsNullOrWhiteSpace(filter.TeacherId))
                {
                    result = await _courses.GetByTeacherWithDetailsAsync(filter.TeacherId);
                }
                else if (!string.IsNullOrWhiteSpace(filter.DeptId))
                {
                    result = await _courses.GetByDeptWithDetailsAsync(filter.DeptId);
                }
                else if (!string.IsNullOrWhiteSpace(filter.CourseType))
                {
                    result = await _courses.GetByCourseTypeWithDetailsAsync(filter.CourseType);
                }
                else if (!string.IsNullOrWhiteSpace(filter.CourseCat))
                {
                    result = await _courses.GetByCourseCatWithDetailsAsync(filter.CourseCat);
                }
                else if (!string.IsNullOrWhiteSpace(filter.SectionId))
                {
                    result = await _courses.GetBySectionWithDetailsAsync(filter.SectionId);
                }
                else if (!string.IsNullOrWhiteSpace(filter.PreReqId))
                {
                    result = await _courses.GetByPreRequisiteWithDetailsAsync(filter.PreReqId);
                }
                else
                {
                    result = await _courses.GetByCreditsWithDetailsAsync(filter.CreditHrs!.Value);
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Courses CRUD Operations

        [HttpGet]
        public async Task<IActionResult> GetCourse(string id)
        {
            try
            {
                var course = await _courses.GetByIdAsync(id);
                if(course == null)
                {
                    return Json(new { success = false, message = "Course Not Found" });
                }
                return Json(new { success = true, data = course });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message});
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(course.CourseId))
                    return Json(new { success = false, message = "Course ID is Required" });

                if (string.IsNullOrWhiteSpace(course.CourseName))
                    return Json(new { success = false, message = "Course Name is Required" });

                if (string.IsNullOrWhiteSpace(course.DeptId))
                    return Json(new { success = false, message = "Department is Required" });

                if (string.IsNullOrWhiteSpace(course.CourseType))
                    return Json(new { success = false, message = "Course Type is Required" });

                if (string.IsNullOrWhiteSpace(course.CourseCat))
                    return Json(new { success = false, message = "Course Category is Required" });

                if (course.CreditHrs < 1 || course.CreditHrs > 4)
                    return Json(new { success = false, message = "Credit Hours must be between 1 and 4" });

                // SQL Injection Protection, Security Measure
                var stringFields = new[] { course.CourseId, course.CourseName, course.CourseCat, course.DeptId, course.CourseType, course.PreReqId };
                foreach (var field in stringFields)
                {
                    if (field == null) continue;
                    if (field.Contains("'") || field.Contains(";") || field.Contains("--"))
                        return Json(new { success = false, message = "Invalid characters found in input" });
                }
                // Trim whitespaces if any
                course.CourseId = course.CourseId.Trim().ToUpper();
                course.CourseName = course.CourseName.Trim();
                course.DeptId = course.DeptId.Trim().ToUpper();
                course.IsActive = 1; // new course is active by default

                var result = await _courses.CreateAsync(course);
                if (result > 0)
                    return Json(new { success = true, message = "Course Created Successfully" });

                return Json(new { success = false, message = "Failed to Create Course" });
            }
            catch (Exception ex)
            {
                // Duplicate primary key
                if (ex.Message.Contains("unique") || ex.Message.Contains("ORA-00001"))
                    return Json(new { success = false, message = "Course ID already exists" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST — Update an existing course
        [HttpPost]
        public async Task<IActionResult> UpdateCourse([FromBody] Course course)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(course.CourseId))
                    return Json(new { success = false, message = "Course ID is required" });

                if (string.IsNullOrWhiteSpace(course.CourseName))
                    return Json(new { success = false, message = "Course Name is required" });

                if (string.IsNullOrWhiteSpace(course.DeptId))
                    return Json(new { success = false, message = "Department is required" });

                if (course.CreditHrs < 1 || course.CreditHrs > 4)
                    return Json(new { success = false, message = "Credit Hours must be between 1 and 4" });

                // Trim
                course.CourseName = course.CourseName.Trim();
                course.DeptId = course.DeptId.Trim().ToUpper();

                var result = await _courses.UpdateAsync(course);
                if (result > 0)
                    return Json(new { success = true, message = "Course Updated Successfully" });

                return Json(new { success = false, message = "Course not found or no changes made" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // POST — Delete course
        [HttpPost]
        public async Task<IActionResult> DeleteCourse([FromBody] string courseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(courseId))
                    return Json(new { success = false, message = "Course ID is required" });

                var result = await _courses.DeleteAsync(courseId);
                if (result > 0)
                    return Json(new { success = true, message = "Course deleted successfully" });

                return Json(new { success = false, message = "Course not found" });
            }
            catch (Exception ex)
            {
                // Foreign key constraint — course is being used
                if (ex.Message.Contains("ORA-02292") || ex.Message.Contains("integrity constraint"))
                    return Json(new { success = false, message = "Cannot delete — course is assigned to sections or has enrollments" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST — Toggle active status
        [HttpPost]
        public async Task<IActionResult> ToggleCourseStatus([FromBody] string courseId)
        {
            try
            {
                var course = await _courses.GetByIdAsync(courseId);
                if (course == null)
                    return Json(new { success = false, message = "Course not found" });

                // Toggle — if active make inactive, if inactive make active
                int newStatus = course.IsActive == 1 ? 0 : 1;
                await _courses.SetActiveStatusAsync(courseId, newStatus);

                string statusText = newStatus == 1 ? "activated" : "deactivated";
                return Json(new { success = true, message = $"Course {statusText} successfully", newStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Settings()
        {
            ViewData["PageTitle"] = "Settings";
            return View();
        }

        private List<UserViewModel> BuildUserViewModels(IEnumerable<User>? users)
        {
            if (users == null) return new List<UserViewModel>();

            return users.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                Role = u.Role,
                Department = u.DeptId,
                Status = u.Status,
                Initials = $"{u.FirstName[0]}{u.LastName[0]}".ToUpper(),
                Batch = u.Batch,
                Degree = u.Degree,
                Designation = u.Designation,
                Specialization = u.Specialization
            }).ToList();
        }

        private string GetTimeAgo(DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours}h ago";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays}d ago";
            return dt.ToString("MMM dd, yyyy");
        }
    }
}
