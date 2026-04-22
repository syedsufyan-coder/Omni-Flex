using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Services;
using OmniFlex.Models.ViewModels.Instructor;

namespace OmniFlex.Models.Repositories.Instructor
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly IUserRepository _users;
        private readonly ICourseRepository _courses;
        private readonly ISectionRepository _sections;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IAttendanceRepository _attendance;

        public InstructorRepository(
            IUserRepository users,
            ICourseRepository courses,
            ISectionRepository sections,
            IEnrollmentRepository enrollments,
            IAttendanceRepository attendance)
        {
            _users = users;
            _courses = courses;
            _sections = sections;
            _enrollments = enrollments;
            _attendance = attendance;
        }

        public async Task<InstructorDashboardViewModel> GetDashboardAsync(string instructorId)
        {
            var profile = await BuildProfileAsync(instructorId);
            var sections = (await _sections.GetByTeacherAsync(instructorId))?.ToList() ?? new List<SectionsDto>();
            var assignedClasses = new List<InstructorAssignedClassCard>();

            foreach (var section in sections.Take(6))
            {
                assignedClasses.Add(new InstructorAssignedClassCard
                {
                    CourseCode = section.SectionId.Replace("-", string.Empty).ToUpperInvariant(),
                    CourseName = section.Department != string.Empty ? section.Department : "Assigned Course",
                    Section = SectionHelper.GetFormattedSectionLabel(section.Degree, section.SectionLabel, section.Batch),
                    CreditHrs = "3",
                    CourseType = "Theory",
                    EnrolledStudents = await _sections.GetEnrolledCountAsync(section.SectionId),
                    Status = "In Progress"
                });
            }

            var courseCards = MapCourseCards(await _courses.GetByTeacherWithDetailsAsync(instructorId));

            var weeklySummary = new InstructorWeeklySummary
            {
                CurrentWeek = 5,
                Semester = "Spring 2026",
                QuizzesCompleted = 3,
                QuizzesPending = 0,
                AssignmentsCompleted = 3,
                AssignmentsPending = 0,
                MidsCompleted = 2,
                MidsPending = 0,
                FinalsCompleted = 1,
                FinalsPending = 0
            };

            return new InstructorDashboardViewModel
            {
                GreetingMessage = GetGreeting(),
                Profile = profile,
                AssignedClasses = assignedClasses,
                WeeklySummary = weeklySummary,
                Courses = courseCards
            };
        }

        public async Task<List<InstructorCourseCard>> GetCoursesAsync(string instructorId)
        {
            return MapCourseCards(await _courses.GetByTeacherWithDetailsAsync(instructorId));
        }

        public async Task<InstructorWeeklyCalendarViewModel> GetWeeklyCalendarAsync(string instructorId)
        {
            var dashboard = await GetDashboardAsync(instructorId);
            return new InstructorWeeklyCalendarViewModel
            {
                GreetingMessage = dashboard.GreetingMessage,
                Profile = dashboard.Profile,
                Summary = dashboard.WeeklySummary,
                AssignedCourses = dashboard.Courses
            };
        }

        public async Task<InstructorAttendanceViewModel> GetManageAttendanceModelAsync(string instructorId, string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null)
        {
            var profile = await BuildProfileAsync(instructorId);
            var courseDtos = (await _courses.GetByTeacherWithDetailsAsync(instructorId))?.ToList() ?? new List<CourseDto>();
            var sectionDtos = (await _sections.GetByTeacherAsync(instructorId))?.ToList() ?? new List<SectionsDto>();

            var courseOptions = courseDtos.Select(c => new SelectOption
            {
                Value = c.CourseId.ToString(),
                Label = $"{c.CourseName} ({c.CreditHours} cr)"
            }).ToList();

            var sectionOptions = sectionDtos.Select(s => new SelectOption
            {
                Value = s.SectionId,
                Label = SectionHelper.GetFormattedSectionLabel(s.Degree, s.SectionLabel, s.Batch)
            }).ToList();

            selectedCourseId ??= courseOptions.FirstOrDefault()?.Value ?? string.Empty;
            selectedSectionId ??= sectionOptions.FirstOrDefault()?.Value ?? string.Empty;
            selectedMonth ??= "May";

            var attendanceHeaders = new List<string>
            {
                "S.No.",
                "Roll No.",
                "Full Name",
                "Mon",
                "Tue",
                "Wed",
                "Thu",
                "Fri"
            };

            var students = new List<InstructorAttendanceStudentRow>
            {
                new InstructorAttendanceStudentRow { SNo = 1, RollNo = "STU-101", FullName = "Ayesha Khan", Statuses = new List<string>{ "Present", "Present", "Absent", "Present", "Present" } },
                new InstructorAttendanceStudentRow { SNo = 2, RollNo = "STU-107", FullName = "Bilal Ahmed", Statuses = new List<string>{ "Present", "Late", "Present", "Present", "Present" } },
                new InstructorAttendanceStudentRow { SNo = 3, RollNo = "STU-113", FullName = "Fatima Noor", Statuses = new List<string>{ "Absent", "Present", "Present", "Present", "Present" } },
                new InstructorAttendanceStudentRow { SNo = 4, RollNo = "STU-121", FullName = "Omar Saeed", Statuses = new List<string>{ "Present", "Present", "Present", "Present", "Late" } }
            };

            return new InstructorAttendanceViewModel
            {
                GreetingMessage = GetGreeting(),
                Profile = profile,
                SelectedSemester = selectedCourseId,
                SemesterOptions = new List<SelectOption>
                {
                    new SelectOption { Value = "Spring 2026", Label = "Spring 2026" },
                    new SelectOption { Value = "Fall 2025", Label = "Fall 2025" }
                },
                SelectedCourseId = selectedCourseId,
                CourseOptions = courseOptions,
                SelectedSectionId = selectedSectionId,
                SectionOptions = sectionOptions,
                SelectedMonth = selectedMonth,
                MonthOptions = new List<SelectOption>
                {
                    new SelectOption { Value = "January", Label = "January" },
                    new SelectOption { Value = "February", Label = "February" },
                    new SelectOption { Value = "March", Label = "March" },
                    new SelectOption { Value = "April", Label = "April" },
                    new SelectOption { Value = "May", Label = "May" },
                    new SelectOption { Value = "June", Label = "June" }
                },
                CurrentWeek = 5,
                SelectedDuration = "1",
                DurationOptions = new List<SelectOption>
                {
                    new SelectOption { Value = "1", Label = "1 hour" },
                    new SelectOption { Value = "1.5", Label = "1.5 hours" },
                    new SelectOption { Value = "2", Label = "2 hours" },
                    new SelectOption { Value = "2.5", Label = "2.5 hours" },
                    new SelectOption { Value = "3", Label = "3 hours" }
                },
                AttendanceHeaders = attendanceHeaders,
                Students = students
            };
        }

        private static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour < 12 ? "Good morning"
                 : hour < 17 ? "Good afternoon"
                 : "Good evening";
        }

        private async Task<InstructorProfileInfo> BuildProfileAsync(string instructorId)
        {
            var user = await _users.GetByIdAsync(instructorId);
            if (user == null)
            {
                return new InstructorProfileInfo
                {
                    InstructorId = "I000",
                    FullName = "Instructor User",
                    Designation = "Instructor",
                    OfficeRoom = "---",
                    Specialization = "---",
                    Status = "Active",
                    Gender = "N/A",
                    Email = "noreply@omniflex.edu",
                    DOB = "01-Jan-1980",
                    MobileNo = "+92 300 0000000",
                    BloodGroup = "O+",
                    Nationality = "Pakistani",
                    Address = "Campus Road",
                    HomePhone = "021-1234567",
                    PostalCode = "44000",
                    City = "Lahore",
                    Country = "Pakistan"
                };
            }

            return new InstructorProfileInfo
            {
                InstructorId = user.UserId,
                FullName = $"{user.FirstName} {user.LastName}",
                Designation = user.Designation ?? "Lecturer",
                OfficeRoom = user.OfficeRoom ?? "B-204",
                Specialization = user.Specialization ?? "Computer Science",
                Status = string.IsNullOrWhiteSpace(user.Status) ? "Active" : user.Status,
                Gender = user.Gender,
                Email = user.Email,
                DOB = user.DOB == default ? "N/A" : user.DOB.ToString("dd-MMM-yyyy"),
                MobileNo = user.PhoneNumber,
                BloodGroup = "O+",
                Nationality = "Pakistani",
                Address = user.Address,
                HomePhone = "021-3456789",
                PostalCode = "44000",
                City = string.IsNullOrWhiteSpace(user.City) ? "Lahore" : user.City,
                Country = string.IsNullOrWhiteSpace(user.Country) ? "Pakistan" : user.Country
            };
        }

        private static List<InstructorCourseCard> MapCourseCards(IEnumerable<CourseDto>? courses)
        {
            if (courses == null)
            {
                return new List<InstructorCourseCard>();
            }

            var courseCards = courses.Select(c => new InstructorCourseCard
            {
                CourseCode = $"C{c.CourseId:D3}",
                CourseName = c.CourseName,
                Section = "Section A",
                Degree = "BSCS",
                Batch = 2025,
                CreditHours = int.TryParse(c.CreditHours, out var creditHrs) ? creditHrs : 3,
                CourseType = c.CourseType,
                StudentsCount = 24,
                BannerColorClass = EnrollmentHelper.GetRandomDarkHex(),
                IsArchived = false
            }).ToList();

            return courseCards;
        }
    }
}
