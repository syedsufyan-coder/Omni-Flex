using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Repositories.Admin;
using OmniFlex.Models.Services;
using OmniFlex.Models.ViewModels.Instructor;

namespace OmniFlex.Models.Repositories.Instructor
{
    public class InstructorRepository : IInstructorRepository
    {

        private readonly DbConnectionFactory _factory;

        public InstructorRepository(DbConnectionFactory factory)
            => _factory = factory;

        public async Task<InstructorDashboardViewModel> GetDashboardAsync(string instructorId)
        {
            var profile = await BuildProfileAsync(instructorId);
            var assignedClasses = await GetAssignedClassesAsync(instructorId);
            var classCards = new List<InstructorAssignedClassCard>();

            if(assignedClasses != null)
            {
                foreach(var assignedClass in assignedClasses){
                classCards.Add(new InstructorAssignedClassCard
                {
                    CourseCode = assignedClass.CourseCode,
                    CourseName = assignedClass.CourseName,
                    Section = SectionHelper.GetFormattedSectionLabel(assignedClass.Degree,assignedClass.Section,assignedClass.Batch),
                    CreditHrs = assignedClass.CreditHrs,
                    CourseType = assignedClass.CourseType,
                    EnrolledStudents = assignedClass.EnrolledStudents,
                    Status = assignedClass.Status
                });
                }
            }

            var courseCards = await GetCoursesAsync(instructorId);

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
                AssignedClasses = classCards,
                WeeklySummary = weeklySummary,
                Courses = courseCards
            };
        }

        public async Task<IEnumerable<AssignedClassesDTO>> GetAssignedClassesAsync(string instructorId)
        {
            const string sql = @"SELECT 
                    c.COURSE_ID            AS CourseCode,
                    c.COURSE_NAME          AS CourseName,
                    s.SECTION_LABEL        AS Section,
                    s.BATCH                AS Batch,
                    s.DEGREE               AS Degree,
                    TO_CHAR(c.CREDIT_HRS)  AS CreditHrs,
                    c.COURSE_TYPE          AS CourseType,
    
                    COUNT(e.ENROLL_ID)     AS EnrolledStudents,
    
                CASE 
                    WHEN c.IS_ACTIVE = 1 THEN 'Active'
                ELSE 'Inactive'
                END                    AS Status

                FROM SECTION_OFFERINGS so

                JOIN COURSES c 
                    ON c.COURSE_ID = so.COURSE_ID

                JOIN SECTIONS s 
                    ON s.SECTION_ID = so.SECTION_ID

                LEFT JOIN ENROLLMENTS e 
                ON e.OFFERING_ID = so.OFFERING_ID
                AND e.STATUS = 'Registered'   -- only count active enrollments

                -- Filter: only current semester
                JOIN SEMESTERS sem 
                    ON sem.SEMESTER_ID = so.SEMESTER_ID
                    AND sem.IS_CURRENT = 1

                -- Filter: classes assigned to instructor
                WHERE so.TEACHER_ID = :InstructorId

                GROUP BY 
                c.COURSE_ID,
                c.COURSE_NAME,
                s.SECTION_LABEL,
                s.BATCH,
                s.DEGREE,
                c.CREDIT_HRS,
                c.COURSE_TYPE,
                c.IS_ACTIVE";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var result = await conn.QueryAsync<AssignedClassesDTO>(sql, new { InstructorId = instructorId });
            return result ?? new List<AssignedClassesDTO>();
        }

        public async Task<List<InstructorCourseCard>> GetCoursesAsync(string instructorId)
        {
            const string sql = @"SELECT
                        C.COURSE_ID       AS CourseCode,
                        C.COURSE_NAME     AS CourseName,
                        S.SECTION_LABEL   AS Section,
                        S.BATCH           AS Batch,
                        S.DEGREE          AS Degree,
                        C.CREDIT_HRS       AS CreditHours,
                        C.COURSE_TYPE     AS CourseType,
                        'FALSE'              AS IsArchived, -- Assuming all courses are active for now
                        COUNT(E.ENROLL_ID) AS StudentsCount

                    FROM SECTION_OFFERINGS SO

                    JOIN COURSES C 
                        ON C.COURSE_ID = SO.COURSE_ID

                    JOIN SECTIONS S 
                        ON S.SECTION_ID = SO.SECTION_ID

                    JOIN SEMESTERS SM 
                        ON SM.SEMESTER_ID = SO.SEMESTER_ID AND SM.IS_CURRENT = 1

                    LEFT JOIN ENROLLMENTS E 
                        ON E.OFFERING_ID = SO.OFFERING_ID AND E.STATUS = 'Registered'

                    WHERE SO.TEACHER_ID = :TeacherId

                    GROUP BY 
                        C.COURSE_ID,
                        C.COURSE_NAME,
                        S.SECTION_LABEL,
                        S.BATCH,
                        S.DEGREE,
                        C.CREDIT_HRS,
                        C.COURSE_TYPE,
                        C.IS_ACTIVE

                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var result = await conn.QueryAsync<InstructorCourseCard>(sql, new { TeacherId = instructorId });
            return result.ToList();
        }
/*
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
*/
        private static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour < 12 ? "Good morning"
                 : hour < 17 ? "Good afternoon"
                 : "Good evening";
        }
        private async Task<InstructorProfileInfo> BuildProfileAsync(string instructorId)
        {
            const string sql = @"SELECT 
                    USER_ID AS InstructorId,
                    FIRST_NAME || ' ' || LAST_NAME AS FullName,
                    DESIGNATION AS Designation,
                    OFFICE_ROOM AS OfficeRoom,
                    SPECIALIZATION AS Specialization,
                    STATUS AS Status,
                    GENDER AS Gender,
                    EMAIL AS Email,
                    DOB AS DOB, -- Formats Date to String
                    PHONE_NUMBER AS MobileNo,
                    'N/A' AS BloodGroup,
                    'Pakistani' AS Nationality,
                    ADDRESS AS Address,
                    'N/A' AS HomePhone,
                    'N/A' AS PostalCode,
                    CITY AS City,
                    COUNTRY AS Country
                FROM USERS
                WHERE USER_ID = :InstructorId AND ROLE = 'Instructor'";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var result = await conn.QueryFirstOrDefaultAsync<InstructorProfileInfo>(sql, new { InstructorId = instructorId });
            return result ?? new InstructorProfileInfo();
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
