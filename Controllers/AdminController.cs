using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels.Admin;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Services;
using OmniFlex.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Dapper;

namespace OmniFlex.Controllers
{
    public class AdminController(
        IUserRepository users,
        ICourseRepository courses,
        ISectionRepository sections,
        IEnrollmentRepository enrollments,
        IAnnouncementRepository announcements,
        Infrastructure.DbConnectionFactory factory) : Controller
    {
        private readonly IUserRepository _users = users;
        private readonly ICourseRepository _courses = courses;
        private readonly ISectionRepository _sections = sections;
        private readonly IEnrollmentRepository _enrollments = enrollments;
        private readonly IAnnouncementRepository _announcements = announcements;
        private readonly Infrastructure.DbConnectionFactory _factory = factory;

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var vm = new AdminDashboardViewModel
                {
                    TotalStudents = await _users.GetCountByRoleAsync("Student"),
                    TotalInstructors = await _users.GetCountByRoleAsync("Instructor"),
                    TotalTAs = 0,
                    ActiveCourses = (await _courses.GetActiveAsync())?.Count() ?? 0,
                };

                var courses = await _courses.GetAllAsync();
                var summary = new List<CourseViewModel>();

                if (courses != null)
                {
                    foreach (var c in courses.Take(5))
                    {
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
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                    ViewData["ErrorMessage"] += $" | Inner: {ex.InnerException.Message}";

                return View(new AdminDashboardViewModel());
            }
        }

        public async Task<IActionResult> Courses(int page = 1)
        {
            try
            {
                const int pageSize = 15;
                var courses = await _courses.GetAllAsync();
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

                // Apply pagination
                var totalItems = viewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedCourses = viewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationViewModel = new PaginationViewModel<CourseViewModel>
                {
                    Items = paginatedCourses,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                };

                ViewData["PageTitle"] = "Courses";
                return View(paginationViewModel);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Courses";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new PaginationViewModel<CourseViewModel>());
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
                            SectionName = SectionHelper.GetFormattedSectionLabel(s.Degree ?? "", s.SectionLabel ?? "", s.Batch) ?? "",
                            SectionLabel = s.SectionLabel ?? "",
                            Degree = s.Degree ?? "",
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

        public async Task<IActionResult> Users(int page = 1)
        {
            try
            {
                const int pageSize = 15;
                var users = await _users.GetAllAsync();
                var allViewModels = AdminController.BuildUserViewModels(users);

                // Apply pagination
                var totalItems = allViewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedUsers = allViewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationViewModel = new PaginationViewModel<UserViewModel>
                {
                    Items = paginatedUsers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                };

                ViewData["PageTitle"] = "Users";
                return View(paginationViewModel);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Users";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View(new PaginationViewModel<UserViewModel>());
            }
        }

        public async Task<IActionResult> Instructors(int page = 1)
        {
            try
            {
                const int pageSize = 15;
                var users = await _users.GetByRoleAsync("Instructor");
                var allViewModels = AdminController.BuildUserViewModels(users);

                // Apply pagination
                var totalItems = allViewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedUsers = allViewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationViewModel = new PaginationViewModel<UserViewModel>
                {
                    Items = paginatedUsers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                };

                ViewData["PageTitle"] = "Instructors";
                return View("Users", paginationViewModel);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Instructors";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new PaginationViewModel<UserViewModel>());
            }
        }

        public async Task<IActionResult> TAs(int page = 1)
        {
            try
            {
                const int pageSize = 15;
                // TAs are Students identified via SECTION_TAS table not by Role
                var users = await _users.GetByRoleAsync("Student");
                var allViewModels = AdminController.BuildUserViewModels(users);

                // Apply pagination
                var totalItems = allViewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedUsers = allViewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationViewModel = new PaginationViewModel<UserViewModel>
                {
                    Items = paginatedUsers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                };

                ViewData["PageTitle"] = "Teaching Assistants";
                return View("Users", paginationViewModel);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Teaching Assistants";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new PaginationViewModel<UserViewModel>());
            }
        }

        public async Task<IActionResult> Students(int page = 1)
        {
            try
            {
                const int pageSize = 15;
                var users = await _users.GetByRoleAsync("Student");
                var allViewModels = AdminController.BuildUserViewModels(users);

                // Apply pagination
                var totalItems = allViewModels.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedUsers = allViewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationViewModel = new PaginationViewModel<UserViewModel>
                {
                    Items = paginatedUsers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize
                };

                ViewData["PageTitle"] = "Students";
                return View("Users", paginationViewModel);
            }
            catch (Exception ex)
            {
                ViewData["PageTitle"] = "Students";
                ViewData["ErrorMessage"] = $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
                return View("Users", new PaginationViewModel<UserViewModel>());
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
                            TimeAgo = AdminController.GetTimeAgo(a.PostDate)
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
                // CHECK 1 � Make sure at least one filter field is filled
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

                // CHECK 2 Only ONE filter is allowed at a time
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

                // CHECK 3 � Length Validation on string fields
                // No field should be unreasonably long
                if ((filter.DeptId?.Length > 20) ||
                    (filter.CourseType?.Length > 20) ||
                    (filter.CourseCat?.Length > 20) ||
                    (filter.TeacherId?.Length > 20) ||
                    (filter.SectionId?.Length > 20) ||
                    (filter.PreReqId?.Length > 20))
                {
                    return Json(new { success = false, message = "Your Input is Unreasonably Too Long" });
                }
                // CHECK 4 - SQL Injection Protection on all string fields
                var allStringValues = new[]
                {   // Collecting all the filter values into an array to loop through it for protective measures
                    filter.DeptId, filter.CourseType, filter.CourseCat,
                    filter.TeacherId, filter.SectionId, filter.PreReqId
                };

                foreach (var val in allStringValues)
                {
                    if (val == null) continue;
                    if (val.Contains('\'') || val.Contains(';') || val.Contains("--"))
                        return Json(new { success = false, message = "Your Input Posses Invalid Characters" });
                }

                // CHECK 5 � Range Validation for Credit Hours
                if (filter.CreditHrs.HasValue && (filter.CreditHrs < 1 || filter.CreditHrs > 4))
                    return Json(new { success = false, message = "Credit Hours Must be Between 1 and 4" });

                // CHECK 6 � Trim whitespace from all string fields silently if in case user has entered mistakenly
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
                    if (field.Contains('\'') || field.Contains(';') || field.Contains("--"))
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

        // POST - Update an existing course
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
        // POST - Delete course
        [HttpPost]
        public async Task<IActionResult> DeleteCourse([FromBody] string courseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(courseId))
                    return Json(new { success = false, message = "Course ID is required" });

                // Check if course exists
                var course = await _courses.GetByIdAsync(courseId);
                if (course == null)
                    return Json(new { success = false, message = "Course not found" });

                // Check if course is used as a prerequisite by other courses
                var coursesUsingAsPrereq = await _courses.GetByPreRequisiteWithDetailsAsync(courseId);
                if (coursesUsingAsPrereq != null && coursesUsingAsPrereq.Any())
                {
                    var courseNames = string.Join(", ", coursesUsingAsPrereq.Select(c => $"{c.CourseId} - {c.CourseName}"));
                    return Json(new { 
                        success = false, 
                        message = $"Cannot delete: This course is a prerequisite for: {courseNames}. Remove the prerequisite relationship first." 
                    });
                }

                // Check if course is assigned to any section offerings
                var sectionOfferings = await _sections.GetByCourseAsync(courseId);
                if (sectionOfferings != null && sectionOfferings.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = $"Cannot delete: Course is offered in {sectionOfferings.Count()} section(s). Remove the course from all sections first." 
                    });
                }

                var result = await _courses.DeleteAsync(courseId);
                if (result > 0)
                    return Json(new { success = true, message = "Course deleted successfully" });

                return Json(new { success = false, message = "Course not found or could not be deleted" });
            }
            catch (Exception ex)
            {
                // Foreign key constraint - course is being used
                if (ex.Message.Contains("ORA-02292") || ex.Message.Contains("integrity constraint"))
                    return Json(new { success = false, message = "Cannot delete: Course is referenced by other records. Please remove all associations first." });

                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST - Toggle active status
        [HttpPost]
        public async Task<IActionResult> ToggleCourseStatus([FromBody] string courseId)
        {
            try
            {
                var course = await _courses.GetByIdAsync(courseId);
                if (course == null)
                    return Json(new { success = false, message = "Course not found" });

                // Toggle � if active make inactive, if inactive make active
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

        // Users CRUD Operations

        [HttpGet]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _users.GetByIdAsync(id);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading user: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.UserId))
                    return Json(new { success = false, message = "User ID is required" });

                if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
                    return Json(new { success = false, message = "Full Name is required" });

                if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
                    return Json(new { success = false, message = "Valid Email is required" });

                if (string.IsNullOrWhiteSpace(user.Role))
                    return Json(new { success = false, message = "User Role must be assigned" });

                // Basic SQL Injection protection
                var suspicious = new[] { "'", ";", "--", "/*", "xp_" };
                var fields = new[] { user.UserId, user.FirstName, user.LastName, user.Email, user.PhoneNumber,
                            user.Address, user.City, user.Country, user.Degree, user.Designation };

                foreach (var field in fields)
                {
                    if (field != null && suspicious.Any(s => field.Contains(s)))
                        return Json(new { success = false, message = "Invalid characters detected" });
                }

                if (await _users.ExistsAsync(user.UserId))
                    return Json(new { success = false, message = "User ID already exists" });

                // Normalization
                user.UserId = user.UserId.Trim().ToUpper();
                user.FirstName = user.FirstName.Trim();
                user.LastName = user.LastName.Trim();
                user.Email = user.Email.Trim().ToLower();
                user.PasswordHash = "ateebchandio"; // Default password (change in production)
                user.Status = "Active";

                var result = await _users.CreateAsync(user);

                if (result > 0)
                    return Json(new { success = true, message = $"User {user.UserId} created successfully!" });

                return Json(new { success = false, message = "Failed to create user" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-00001") || ex.Message.Contains("unique"))
                    return Json(new { success = false, message = "User ID or Email already exists" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.UserId))
                    return Json(new { success = false, message = "User Id Is Required For Update" });

                var stringFields = new[] {
                    user.FirstName, user.LastName, user.Email, user.DeptId,
                    user.PhoneNumber, user.Address, user.City, user.Country,
                    user.Degree, user.Designation, user.OfficeRoom, user.Specialization
                };

                foreach (var field in stringFields)
                {
                    if (field != null && (field.Contains('\'') || field.Contains(';') || field.Contains("--")))
                        return Json(new { success = false, message = "Invalid Characters Detected In Update Fields" });
                }

                if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
                    return Json(new { success = false, message = "Name Fields Cannot Be Empty" });

                if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
                    return Json(new { success = false, message = "A Valid Email Is Required" });

                user.UserId = user.UserId.Trim().ToUpper();
                user.FirstName = user.FirstName.Trim();
                user.LastName = user.LastName.Trim();
                user.Email = user.Email.Trim().ToLower();

                var result = await _users.UpdateAsync(user);

                if (result > 0)
                    return Json(new { success = true, message = $"User {user.UserId} Updated Successfully!" });

                return Json(new { success = false, message = "No Changes Were Made Or User Not Found" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("unique") || ex.Message.Contains("ORA-00001"))
                    return Json(new { success = false, message = "This Email Is Already In Use By Another User" });

                return Json(new { success = false, message = "Update Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return Json(new { success = false, message = "User Id Is Required For Deletion" });

                if (string.Equals(userId?.Trim(), "ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Critical Error: Primary Admin Account Cannot Be Deleted" });
                }
                var user = await _users.GetByIdAsync(userId!);
                if (user == null)
                    return Json(new { success = false, message = "User Not Found Or Already Deleted" });

                var result = await _users.DeleteAsync(userId!);

                if (result > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"User {userId} ({user.Role}) Has Been Successfully Removed From The System"
                    });
                }

                return Json(new { success = false, message = "Delete Failed: The Record Might Be In Use" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("integrity constraint") || ex.Message.Contains("ORA-02292"))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot Delete: This User Has Linked Records (Enrollments, Grades, Or Sections). Try Deactivating The User Instead"
                    });
                }

                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }

        /*
        [HttpPost]
        public async Task<IActionResult> AssignTA([FromBody] TAAssignmentDto request)
        {
            try
            {
                // 1. Check if Student exists and is Active Student
                var student = await _users.GetByIdAsync(request.StudentId);
                if (student == null)
                    return Json(new { success = false, message = "Student not found" });

                if (student.Role != "Student")
                    return Json(new { success = false, message = "Only Students can be assigned as TA" });

                if (student.Status != "Active")
                    return Json(new { success = false, message = "TA must be an Active Student" });

                // 2. Check if Section exists
                var sectionExists = await _sections.ExistsAsync(request.SectionId);
                if (!sectionExists)
                    return Json(new { success = false, message = "Section not found" });

                // 3. Check Batch Seniority (at least 1 year senior)
                if (!student.Batch.HasValue)
                    return Json(new { success = false, message = "Student batch information is missing" });

                if (request.Batch - student.Batch.Value < 1)
                    return Json(new { success = false, message = "TA must be at least one year senior to the assigned section batch" });

                // 4. Check if student has studied this course and got minimum B+ grade
                var enrollment = await _enrollments.GetRecordAsync(request.StudentId, request.CourseId);
                if (enrollment == null)
                    return Json(new { success = false, message = "Student has not enrolled in this course" });

                var eligibleGrades = new[] { "A+", "A", "A-", "B+" };
                if (!eligibleGrades.Contains(enrollment.Grade?.Trim()))
                    return Json(new { success = false, message = "Student must have at least B+ grade in this course to be TA" });

                // 5. If all passed then finally assign TA
                var result = await _users.AddTaToSectionAsync(request);

                if (result > 0)
                    return Json(new { success = true, message = "Teaching Assistant assigned successfully!" });

                return Json(new { success = false, message = "Failed to assign TA | Please try again." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }
        */

        // Get all active courses for dropdown
        [HttpGet]
        public async Task<IActionResult> GetActiveCourses()
        {
            try
            {
                var courses = await _courses.GetAllAsync();
                var activeCourses = courses
                    .Where(c => c.IsActive == 1)
                    .Select(c => new
                    {
                        courseId = c.CourseId,
                        courseName = c.CourseName,
                        deptId = c.DeptId
                    })
                    .OrderBy(c => c.courseId)
                    .ToList();

                return Json(new { success = true, data = activeCourses });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get sections for a specific course (with instructor info)
        [HttpGet]
        public async Task<IActionResult> GetSectionsByCourse(string courseId)
        {
            try
            {
                // Custom query to get sections with instructor information
                using var conn = _factory.CreateConnection();
                const string sql = @"
                    SELECT 
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        S.BATCH AS Batch,
                        SO.TEACHER_ID AS InstructorId,
                        U.FIRST_NAME AS InstructorFirstName,
                        U.LAST_NAME AS InstructorLastName
                    FROM SECTIONS S
                    JOIN SECTION_OFFERINGS SO ON SO.SECTION_ID = S.SECTION_ID
                    JOIN SEMESTERS SM ON SO.SEMESTER_ID = SM.SEMESTER_ID
                    LEFT JOIN USERS U ON SO.TEACHER_ID = U.USER_ID
                    WHERE SO.COURSE_ID = :CourseId AND SM.IS_CURRENT = 1
                    ORDER BY S.SECTION_ID";
                
                var sections = await conn.QueryAsync(sql, new { CourseId = courseId });
                var sectionList = sections.Select(s => new
                {
                    sectionId = s.SectionId,
                    sectionLabel = s.SectionLabel,
                    degree = s.Degree,
                    batch = s.Batch,
                    instructorId = s.InstructorId as string,
                    instructorName = !string.IsNullOrEmpty(s.InstructorId as string) ? $"{s.InstructorFirstName} {s.InstructorLastName}" : null
                }).ToList();

                return Json(new { success = true, data = sectionList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get instructor's currently assigned courses and sections
        [HttpGet]
        public async Task<IActionResult> GetInstructorAssignments(string instructorId)
        {
            try
            {
                // Custom query to get assignments with course information
                using var conn = _factory.CreateConnection();
                const string sql = @"
                    SELECT 
                        SO.COURSE_ID AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        S.BATCH AS Batch
                    FROM SECTIONS S
                    JOIN SECTION_OFFERINGS SO ON SO.SECTION_ID = S.SECTION_ID
                    JOIN SEMESTERS SM ON SO.SEMESTER_ID = SM.SEMESTER_ID
                    JOIN COURSES C ON C.COURSE_ID = SO.COURSE_ID
                    WHERE SO.TEACHER_ID = :InstructorId AND SM.IS_CURRENT = 1
                    ORDER BY C.COURSE_ID, S.SECTION_ID";
                
                var assignments = await conn.QueryAsync(sql, new { InstructorId = instructorId });
                var assignmentList = assignments.Select(s => new
                {
                    courseId = s.CourseId,
                    courseName = s.CourseName,
                    sectionId = s.SectionId,
                    sectionLabel = s.SectionLabel,
                    degree = s.Degree,
                    batch = s.Batch
                }).ToList();

                return Json(new { success = true, data = assignmentList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Assign instructor to a specific section
        [HttpPost]
        public async Task<IActionResult> AssignInstructorToSection([FromBody] InstructorSectionAssignmentDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.InstructorId))
                    return Json(new { success = false, message = "Instructor ID is required" });

                if (string.IsNullOrWhiteSpace(request.SectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                // Verify instructor exists and is active
                var instructor = await _users.GetByIdAsync(request.InstructorId);
                if (instructor == null)
                    return Json(new { success = false, message = "Instructor not found" });

                if (instructor.Role != "Instructor" || instructor.Status != "Active")
                    return Json(new { success = false, message = "Selected user is not an active instructor" });

                // Assign instructor to the section
                var result = await _sections.AssignTeacherAsync(request.SectionId, request.InstructorId);

                if (result > 0)
                    return Json(new { success = true, message = $"Instructor {instructor.FirstName} {instructor.LastName} assigned to section successfully" });

                return Json(new { success = false, message = "Failed to assign instructor to section" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get all instructors for dropdown
        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            try
            {
                var users = await _users.GetByRoleAsync("Instructor");
                var instructors = users.Where(u => u.Status == "Active").Select(u => new
                {
                    userId = u.UserId,
                    fullName = $"{u.FirstName} {u.LastName}",
                    email = u.Email,
                    designation = u.Designation
                }).ToList();

                return Json(new { success = true, data = instructors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Assign instructor to course (creates sections with this instructor)
        [HttpPost]
        public async Task<IActionResult> AssignInstructorToCourse([FromBody] InstructorAssignmentDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CourseId))
                    return Json(new { success = false, message = "Course ID is required" });

                if (string.IsNullOrWhiteSpace(request.InstructorId))
                    return Json(new { success = false, message = "Instructor ID is required" });

                // Verify course exists
                var course = await _courses.GetByIdAsync(request.CourseId);
                if (course == null)
                    return Json(new { success = false, message = "Course not found" });

                // Verify instructor exists and is active
                var instructor = await _users.GetByIdAsync(request.InstructorId);
                if (instructor == null)
                    return Json(new { success = false, message = "Instructor not found" });

                if (instructor.Role != "Instructor" || instructor.Status != "Active")
                    return Json(new { success = false, message = "Selected user is not an active instructor" });

                // Assign instructor to all sections of this course
                var result = await _sections.AssignInstructorToCourseSectionsAsync(request.CourseId, request.InstructorId);

                if (result > 0)
                    return Json(new { success = true, message = $"Instructor {instructor.FirstName} {instructor.LastName} assigned to {result} section(s) of {course.CourseName}" });

                return Json(new { success = false, message = "No sections found for this course, or instructor already assigned" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Change user password
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                    return Json(new { success = false, message = "User ID is required" });

                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    return Json(new { success = false, message = "Current password is required" });

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    return Json(new { success = false, message = "New password is required" });

                if (request.NewPassword.Length < 6)
                    return Json(new { success = false, message = "New password must be at least 6 characters long" });

                var user = await _users.GetByIdAsync(request.UserId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                // Verify current password
                if (user.PasswordHash != request.CurrentPassword)
                    return Json(new { success = false, message = "Current password is incorrect" });

                // Update password
                user.PasswordHash = request.NewPassword;
                var result = await _users.UpdateAsync(user);

                if (result > 0)
                    return Json(new { success = true, message = "Password changed successfully" });

                return Json(new { success = false, message = "Failed to change password" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get enrolled students for a specific section
        [HttpGet]
        public async Task<IActionResult> GetSectionStudents(string sectionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                var students = await _sections.GetEnrolledStudentsAsync(sectionId);
                var studentList = students.Select(s => new
                {
                    studentId = s.StudentId,
                    firstName = s.FirstName,
                    lastName = s.LastName,
                    fullName = $"{s.FirstName} {s.LastName}",
                    email = s.Email,
                    degree = s.Degree,
                    batch = s.Batch,
                    status = s.Status,
                    department = s.Department
                }).ToList();

                return Json(new { success = true, data = studentList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // ═══════════════════════════════════════════════════
        //  SECTIONS CRUD
        // ═══════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> GetSection(string id)
        {
            try
            {
                var section = await _sections.GetByIdAsync(id);
                if (section == null)
                    return Json(new { success = false, message = "Section not found" });

                // Get enrolled count
                var enrolledCount = await _sections.GetEnrolledCountAsync(id);

                // Get CR name if assigned
                string crName = "Not assigned";
                if (!string.IsNullOrWhiteSpace(section.CrId))
                {
                    var cr = await _users.GetByIdAsync(section.CrId);
                    if (cr != null)
                        crName = $"{cr.FirstName} {cr.LastName}";
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        sectionId = section.SectionId,
                        sectionLabel = section.SectionLabel,
                        degree = section.DegreeProgram,
                        department = section.DepartmentId,
                        batchYear = section.BatchYear,
                        crName = crName,
                        crId = section.CrId,
                        enrolledStudents = enrolledCount,
                        seats = 50,
                        seatsLeft = 50 - enrolledCount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSection([FromBody] Section section)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(section.SectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                if (string.IsNullOrWhiteSpace(section.SectionLabel))
                    return Json(new { success = false, message = "Section Label is required" });

                if (string.IsNullOrWhiteSpace(section.DepartmentId))
                    return Json(new { success = false, message = "Department is required" });

                if (string.IsNullOrWhiteSpace(section.DegreeProgram))
                    return Json(new { success = false, message = "Degree is required" });

                if (section.BatchYear < 2020 || section.BatchYear > 2035)
                    return Json(new { success = false, message = "Batch year must be between 2020 and 2035" });

                // SQL Injection protection
                var fields = new[] { section.SectionId, section.SectionLabel, section.DepartmentId, section.DegreeProgram };
                foreach (var f in fields)
                {
                    if (f != null && (f.Contains('\'') || f.Contains(';') || f.Contains("--")))
                        return Json(new { success = false, message = "Invalid characters in input" });
                }

                if (await _sections.ExistsAsync(section.SectionId.Trim().ToUpper()))
                    return Json(new { success = false, message = "Section ID already exists" });

                section.SectionId = section.SectionId.Trim().ToUpper();
                section.SectionLabel = section.SectionLabel.Trim().ToUpper();
                section.DepartmentId = section.DepartmentId.Trim().ToUpper();

                var result = await _sections.CreateAsync(section);
                if (result > 0)
                    return Json(new { success = true, message = $"Section {section.SectionId} created successfully!" });

                return Json(new { success = false, message = "Failed to create section" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-00001") || ex.Message.Contains("unique"))
                    return Json(new { success = false, message = "Section ID already exists" });

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSection([FromBody] UpdateSectionRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.SectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                var existing = await _sections.GetByIdAsync(req.SectionId);
                if (existing == null)
                    return Json(new { success = false, message = "Section not found" });

                // Apply updates — keep existing value if new value not provided
                existing.SectionLabel = string.IsNullOrWhiteSpace(req.SectionLabel)
                    ? existing.SectionLabel : req.SectionLabel.Trim().ToUpper();

                existing.DepartmentId = string.IsNullOrWhiteSpace(req.Department)
                    ? existing.DepartmentId : req.Department.Trim().ToUpper();

                existing.DegreeProgram = string.IsNullOrWhiteSpace(req.Degree)
                    ? existing.DegreeProgram : req.Degree.Trim();

                if (req.Batch.HasValue && req.Batch.Value >= 2020)
                    existing.BatchYear = req.Batch.Value;

                var result = await _sections.UpdateAsync(existing);
                if (result > 0)
                    return Json(new { success = true, message = "Section updated successfully" });

                return Json(new { success = false, message = "No changes made or section not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSection([FromBody] DeleteSectionRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.SectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                // Check enrolled students
                var count = await _sections.GetEnrolledCountAsync(req.SectionId);
                if (count > 0)
                    return Json(new
                    {
                        success = false,
                        message = $"Cannot delete: {count} student(s) are enrolled. Remove enrollments first."
                    });

                var result = await _sections.DeleteAsync(req.SectionId);
                if (result > 0)
                    return Json(new { success = true, message = "Section deleted successfully" });

                return Json(new { success = false, message = "Section not found" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-02292") || ex.Message.Contains("integrity constraint"))
                    return Json(new { success = false, message = "Cannot delete: Section has linked records. Remove all associations first." });

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignCR([FromBody] AssignCRRequest req)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.SectionId))
                    return Json(new { success = false, message = "Section ID is required" });

                if (string.IsNullOrWhiteSpace(req.StudentId))
                    return Json(new { success = false, message = "Please select a student" });

                // Verify student exists and is active
                var student = await _users.GetByIdAsync(req.StudentId);
                if (student == null)
                    return Json(new { success = false, message = "Student not found" });

                if (student.Role != "Student" || student.Status != "Active")
                    return Json(new { success = false, message = "Only active students can be assigned as CR" });

                var result = await _sections.AssignCRAsync(req.SectionId, req.StudentId);
                if (result > 0)
                    return Json(new
                    {
                        success = true,
                        message = $"{student.FirstName} {student.LastName} assigned as Class Representative"
                    });

                return Json(new { success = false, message = "Failed to assign CR" });
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

        private static List<UserViewModel> BuildUserViewModels(IEnumerable<User>? users)
        {
            if (users == null) return [];

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

        private static string GetTimeAgo(DateTime dt)
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
