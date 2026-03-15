using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels.Admin;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository         _users;
        private readonly ICourseRepository       _courses;
        private readonly ISectionRepository      _sections;
        private readonly IEnrollmentRepository   _enrollments;
        private readonly IAnnouncementRepository _announcements;

        public AdminController(
            IUserRepository         users,
            ICourseRepository       courses,
            ISectionRepository      sections,
            IEnrollmentRepository   enrollments,
            IAnnouncementRepository announcements)
        {
            _users         = users;
            _courses       = courses;
            _sections      = sections;
            _enrollments   = enrollments;
            _announcements = announcements;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var vm = new AdminDashboardViewModel
                {
                    TotalStudents    = await _users.GetCountByRoleAsync("Student"),
                    TotalInstructors = await _users.GetCountByRoleAsync("Instructor"),
                    TotalTAs         = await _users.GetCountByRoleAsync("TA"),
                    ActiveCourses    = (await _courses.GetActiveAsync())?.Count() ?? 0,
                };

                // Build CourseSummary list
                var courses  = await _courses.GetAllAsync();
                var summary  = new List<CourseSummaryItem>();

                if (courses != null)
                {
                    foreach (var c in courses.Take(5))
                    {
                        var sections = await _sections.GetByCourseAsync(c.CourseId);
                        var sec      = sections?.FirstOrDefault();
                        string instructorName = "Unassigned";
                        
                        if (sec != null)
                        {
                            var teacher = await _users.GetByIdAsync(sec.TeacherId);
                            instructorName = teacher != null 
                                ? $"{teacher.FirstName} {teacher.LastName}" 
                                : "Unassigned";
                        }

                        summary.Add(new CourseSummaryItem
                        {
                            CourseId    = c.CourseId,
                            CourseName  = c.CourseName,
                            Department  = c.DeptId,
                            Instructor  = instructorName,
                            Enrolled    = sec != null ? await _sections.GetEnrolledCountAsync(sec.SectionId) : 0,
                            Seats       = sec?.Seats ?? 0,
                            Status      = c.IsActive == 1 ? "Active" : "Inactive"
                        });
                    }
                }

                vm.CoursesSummary = summary;
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
                        var sections = await _sections.GetByCourseAsync(c.CourseId);
                        var sec      = sections?.FirstOrDefault();
                        string instructorName = "Unassigned";
                        int enrolled = 0;

                        if (sec != null)
                        {
                            var teacher = await _users.GetByIdAsync(sec.TeacherId);
                            if (teacher != null)
                                instructorName = $"{teacher.FirstName} {teacher.LastName}";
                            enrolled = await _sections.GetEnrolledCountAsync(sec.SectionId);
                        }

                        viewModels.Add(new CourseViewModel
                        {
                            CourseId   = c.CourseId,
                            CourseName = c.CourseName,
                            Department = c.DeptId,
                            CreditHrs  = c.CreditHrs,
                            CourseType = c.CourseType,
                            CourseCat  = c.CourseCat,
                            PreReqId   = c.PreReqId,
                            IsActive   = c.IsActive == 1,
                            Instructor = instructorName,
                            Enrolled   = enrolled,
                            Seats      = sec?.Seats ?? 0,
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
                var sections   = await _sections.GetAllAsync();
                var viewModels = new List<SectionViewModel>();

                if (sections != null)
                {
                    foreach (var s in sections)
                    {
                        var course  = await _courses.GetByIdAsync(s.CourseId);
                        var teacher = await _users.GetByIdAsync(s.TeacherId);
                        var enrolled = await _sections.GetEnrolledCountAsync(s.SectionId);

                        viewModels.Add(new SectionViewModel
                        {
                            SectionId    = s.SectionId,
                            SectionLabel = s.SectionLabel ?? "",
                            CourseName   = course?.CourseName ?? "",
                            CourseId     = s.CourseId,
                            Instructor   = teacher != null 
                                ? $"{teacher.FirstName} {teacher.LastName}" 
                                : "Unassigned",
                            TeacherId    = s.TeacherId,
                            RoomNo       = s.RoomNo,
                            TimeSlot     = s.TimeSlot,
                            Enrolled     = enrolled,
                            Seats        = s.Seats,
                            Semester     = s.SemesterId
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
                var vm    = BuildUserViewModels(users);
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
                var vm    = BuildUserViewModels(users);
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
                var users = await _users.GetByRoleAsync("TA");
                var vm    = BuildUserViewModels(users);
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
                var vm    = BuildUserViewModels(users);
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
                            Title          = a.Title,
                            Content        = a.Content,
                            PostedByName   = poster != null 
                                ? $"{poster.FirstName} {poster.LastName}" 
                                : "System",
                            Audience       = a.Audience,
                            Priority       = a.Priority,
                            IsPinned       = a.IsPinned == 1,
                            SectionId      = a.SectionId,
                            PostDate       = a.PostDate,
                            TimeAgo        = GetTimeAgo(a.PostDate)
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
                UserId         = u.UserId,
                FullName       = $"{u.FirstName} {u.LastName}",
                Email          = u.Email,
                Role           = u.Role,
                Department     = u.DeptId,
                Status         = u.Status,
                Initials       = $"{u.FirstName[0]}{u.LastName[0]}".ToUpper(),
                Batch          = u.Batch,
                Degree         = u.Degree,
                Designation    = u.Designation,
                Specialization = u.Specialization
            }).ToList();
        }

        private string GetTimeAgo(DateTime dt)
        {
            var diff = DateTime.Now - dt;
            if (diff.TotalMinutes < 60)  
                return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalHours   < 24)  
                return $"{(int)diff.TotalHours}h ago";
            if (diff.TotalDays    < 7)   
                return $"{(int)diff.TotalDays}d ago";
            return dt.ToString("MMM dd, yyyy");
        }
    }
}
