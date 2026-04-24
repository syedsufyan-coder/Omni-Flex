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

            if (assignedClasses != null)
            {
                foreach (var assignedClass in assignedClasses)
                {
                    classCards.Add(new InstructorAssignedClassCard
                    {
                        OfferingId = assignedClass.OfferingId,
                        CourseCode = assignedClass.CourseCode,
                        CourseName = assignedClass.CourseName,
                        Section = SectionHelper.GetFormattedSectionLabel(assignedClass.Degree, assignedClass.Section, assignedClass.Batch),
                        CreditHrs = assignedClass.CreditHrs,
                        CourseType = assignedClass.CourseType,
                        EnrolledStudents = assignedClass.EnrolledStudents,
                        Status = assignedClass.Status
                    });
                }
            }

            var courseCards = await GetCoursesAsync(instructorId);
            if (courseCards != null)
            {
                foreach (var course in courseCards)
                {
                    course.BannerColorClass = EnrollmentHelper.GetRandomDarkHex();
                    course.Section = SectionHelper.GetFormattedSectionLabel(course.Degree, course.Section, course.Batch);
                }
            }

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
                        SO.OFFERING_ID    AS OfferingId,
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
                        SO.OFFERING_ID,
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

        public async Task<InstructorClassroomViewModel> GetInstructorClassroomAsync(string offeringId)
        {
            const string sql_classroom = @"SELECT
                    SO.OFFERING_ID     AS OfferingId,
                    C.COURSE_ID        AS CourseId,
                    C.COURSE_NAME      AS CourseName,
                    S.SECTION_LABEL    AS SectionName,
                    SM.SEMESTER_ID     AS Semester,
                    SO.TEACHER_ID      AS InstructorId,

                    COUNT(E.ENROLL_ID) AS EnrolledCount

                FROM SECTION_OFFERINGS SO

                JOIN COURSES C 
                    ON C.COURSE_ID = SO.COURSE_ID

                JOIN SECTIONS S 
                    ON S.SECTION_ID = SO.SECTION_ID

                JOIN SEMESTERS SM 
                    ON SM.SEMESTER_ID = SO.SEMESTER_ID

                LEFT JOIN ENROLLMENTS E 
                    ON E.OFFERING_ID = SO.OFFERING_ID AND E.STATUS = 'Registered'

                WHERE SO.OFFERING_ID = :OfferingId

                GROUP BY
                    SO.OFFERING_ID,
                    C.COURSE_ID,
                    C.COURSE_NAME,
                    S.SECTION_LABEL,
                    SM.SEMESTER_ID,
                    SO.TEACHER_ID";

            const string sql_posts = @"SELECT
                        CP.POST_ID        AS PostId,
                        CP.POSTED_BY      AS PostedBy,
                        CP.POST_TYPE      AS PostType,
                        CP.TITLE          AS Title,
                        CP.CONTENT        AS Body,
                        CP.CREATED_AT     AS PostedAt,
                        U.FIRST_NAME || ' ' || U.LAST_NAME AS PostedByName

                    FROM COURSE_POSTS CP

                    LEFT JOIN USERS U 
                        ON U.USER_ID = CP.POSTED_BY

                    WHERE CP.OFFERING_ID = :OfferingId

                    ORDER BY CP.CREATED_AT DESC";

            const string sql_assignments = @"SELECT
                                A.ASSIGNMENT_ID    AS AssignmentId,
                                A.OFFERING_ID      AS OfferingId,
                                A.TITLE            AS Title,
                                A.DESCRIPTION      AS Description,
                                A.DELIVERY_MODE    AS DeliveryMode,
                                A.DUE_DATE         AS DueDate,
                                A.CATEGORY         AS Category,
                                A.TOTAL_MARKS      AS TotalMarks,
                                A.ACTUAL_WTG       AS ActualWtg,
                                A.IS_GRADED        AS IsGraded,
                                A.GRADING_GROUP    AS GradingGroup,
                                A.COUNT_BEST_OF    AS CountBestOf,
                                A.CREATED_AT       AS CreatedAt,

                                /* Total submissions for this assignment */
                                COUNT(DISTINCT SUB.SUBMISSION_ID) AS SubmissionCount,

                                /* Total enrolled students in offering */
                                (
                                    SELECT COUNT(*)
                                    FROM ENROLLMENTS E
                                    WHERE E.OFFERING_ID = A.OFFERING_ID
                                    AND E.STATUS = 'Registered'
                                ) AS TotalEnrolled

                            FROM ASSIGNMENTS A

                            LEFT JOIN SUBMISSIONS SUB 
                                ON SUB.ASSIGNMENT_ID = A.ASSIGNMENT_ID

                            WHERE A.OFFERING_ID = :OfferingId

                            GROUP BY
                                A.ASSIGNMENT_ID,
                                A.OFFERING_ID,
                                A.TITLE,
                                A.DESCRIPTION,
                                A.DELIVERY_MODE,
                                A.DUE_DATE,
                                A.CATEGORY,
                                A.TOTAL_MARKS,
                                A.ACTUAL_WTG,
                                A.IS_GRADED,
                                A.GRADING_GROUP,
                                A.COUNT_BEST_OF,
                                A.CREATED_AT

                                ORDER BY A.CREATED_AT DESC";

            const string sql_submissions = @"SELECT
                                SUB.ASSIGNMENT_ID AS AssignmentId,
                                SUB.SUBMISSION_ID     AS SubmissionId,
                                ST.USER_ID            AS StudentId,
                                ST.FIRST_NAME || ' ' || ST.LAST_NAME AS StudentName,
                                SUB.SUBMIT_DATE       AS SubmitDate,
                                SUB.OBTAINED_MARKS    AS ObtainedMarks,

                                CASE WHEN SUB.IS_LATE = 'Y' THEN 'Yes' ELSE 'No' END AS IsLate,
                                CASE WHEN SUB.LOCKED = 'Y' THEN 'Yes' ELSE 'No' END AS Locked

                            FROM SUBMISSIONS SUB

                            JOIN ENROLLMENTS E 
                                ON E.ENROLL_ID = SUB.ENROLL_ID

                            JOIN USERS ST 
                                ON ST.USER_ID = E.STUDENT_ID

                            WHERE SUB.ASSIGNMENT_ID IN :AssignmentIds
                            ORDER BY ST.FIRST_NAME";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var classroomInfo = await conn.QueryFirstOrDefaultAsync<InstructorClassroomViewModel>(sql_classroom, new { OfferingId = offeringId });
            if (classroomInfo == null)
            {
                return new InstructorClassroomViewModel();
            }
            var posts = await conn.QueryAsync<InstructorCoursePostViewModel>(sql_posts, new { OfferingId = offeringId });
            var assignments = await conn.QueryAsync<InstructorAssignmentViewModel>(sql_assignments, new { OfferingId = offeringId });
            // 1. Extract IDs
            var assignmentIds = assignments.Select(a => a.AssignmentId).ToArray();

            // 2. Guard against empty collections to prevent SQL syntax errors
            IEnumerable<InstructorSubmissionSummaryViewModel> submissions = new List<InstructorSubmissionSummaryViewModel>();

            if (assignmentIds.Any())
            {
                // Use @AssignmentIds so Dapper recognizes the expansion point
                submissions = await conn.QueryAsync<InstructorSubmissionSummaryViewModel>(sql_submissions, new { AssignmentIds = assignmentIds });
            }

            // 3. Map submissions back to assignments
            foreach (var assignment in assignments)
            {
                assignment.Submissions = submissions.Where(s => s.AssignmentId == assignment.AssignmentId).ToList();
            }
            classroomInfo.Posts = posts.ToList();
            classroomInfo.Assignments = assignments.ToList();

            return classroomInfo;
        }

        public async Task<InstructorPeopleViewModel> GetInstructorPeopleAsync(string offeringId)
        {
            const string sql_teacher = @"SELECT 
                        U.FIRST_NAME || ' ' || U.LAST_NAME AS TeacherName,
                        U.EMAIL AS TeacherEmail
                    FROM SECTION_OFFERINGS SO
                    JOIN USERS U ON SO.TEACHER_ID = U.USER_ID
                    WHERE SO.OFFERING_ID = :Offering_ID";
            const string sql_students = @"SELECT 
                            U.USER_ID AS StudentId,
                            U.FIRST_NAME || ' ' || U.LAST_NAME AS FullName,
                            U.EMAIL AS Email
                        FROM ENROLLMENTS E
                        JOIN USERS U ON E.STUDENT_ID = U.USER_ID
                        WHERE E.OFFERING_ID = :Offering_ID
                        ORDER BY U.LAST_NAME, U.FIRST_NAME";
            const string sql_ta = @"SELECT 
                        U.USER_ID AS StudentId,
                        U.FIRST_NAME || ' ' || U.LAST_NAME AS FullName,
                        U.EMAIL AS Email
                    FROM SECTION_OFFERINGS SO
                    JOIN SECTION_TAS TA ON SO.SECTION_ID = TA.SECTION_ID 
                        AND SO.COURSE_ID = TA.COURSE_ID 
                        AND SO.SEMESTER_ID = TA.SEMESTER_ID
                    JOIN USERS U ON TA.TA_ID = U.USER_ID
                    WHERE SO.OFFERING_ID = :Offering_ID";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var teacher = await conn.QueryFirstOrDefaultAsync<InstructorPeopleViewModel>(sql_teacher, new { Offering_ID = offeringId });
            var students = await conn.QueryAsync<StudentsViewModel>(sql_students, new { Offering_ID = offeringId });
            var ta = await conn.QueryFirstOrDefaultAsync<StudentsViewModel>(sql_ta, new { Offering_ID = offeringId });

            var model = new InstructorPeopleViewModel
            {
                TeacherName = teacher?.TeacherName ?? "N/A",
                TeacherEmail = teacher?.TeacherEmail ?? "N/A",
                Students = students.ToList(),
                TeachingAssistant = ta ?? null
            };
            return model;
        }

        public async Task<InstructorAssignmentViewModel> GetInstructorAssignmentDetailsAsync(int assignmentId)
        {
            const string sql = @"SELECT
                    A.ASSIGNMENT_ID    AS AssignmentId,
                    A.OFFERING_ID      AS OfferingId,
                    A.TITLE            AS Title,
                    A.DESCRIPTION      AS Description,
                    A.DELIVERY_MODE    AS DeliveryMode,
                    A.DUE_DATE         AS DueDate,
                    A.CATEGORY         AS Category,
                    A.TOTAL_MARKS      AS TotalMarks,
                    A.ACTUAL_WTG       AS ActualWtg,
                    A.IS_GRADED        AS IsGraded,
                    A.GRADING_GROUP    AS GradingGroup,
                    A.COUNT_BEST_OF    AS CountBestOf,
                    A.CREATED_AT       AS CreatedAt,

                    /* Count unique submissions for this specific assignment */
                    (
                        SELECT COUNT(SUBMISSION_ID) 
                        FROM SUBMISSIONS 
                        WHERE ASSIGNMENT_ID = A.ASSIGNMENT_ID
                    ) AS SubmissionCount,

                    /* Total registered students for the parent offering */
                    (
                        SELECT COUNT(*)
                        FROM ENROLLMENTS E
                        WHERE E.OFFERING_ID = A.OFFERING_ID
                        AND E.STATUS = 'Registered'
                    ) AS TotalEnrolled

                FROM ASSIGNMENTS A
                WHERE A.ASSIGNMENT_ID = :assignmentId";

            using var conn = _factory.CreateConnection();
            conn.Open();
            var assignment = await conn.QueryFirstOrDefaultAsync<InstructorAssignmentViewModel>(sql, new { assignmentId = assignmentId });
            return assignment ?? new InstructorAssignmentViewModel();
        }

        public async Task<GradesViewModel> GetInstructorGradesGridAsync(string offeringId)
        {
            const string sql = @"SELECT 
                        U.USER_ID AS StudentId,
                        U.FIRST_NAME || ' ' || U.LAST_NAME AS FullName,
                        A.ASSIGNMENT_ID AS AssignmentId,
                        A.TITLE AS AssignmentTitle,
                        A.TOTAL_MARKS AS MaxMarks,
                        -- Get marks from either Online Submissions or Physical Exam Entries
                        COALESCE(S.OBTAINED_MARKS, E_ENT.MARKS_OBTAINED) AS ObtainedMarks
                    FROM ENROLLMENTS E
                    JOIN USERS U ON E.STUDENT_ID = U.USER_ID
                    -- Cross Join with Assignments ensures we get a slot for every student for every assignment
                    CROSS JOIN ASSIGNMENTS A 
                    LEFT JOIN SUBMISSIONS S 
                        ON S.ASSIGNMENT_ID = A.ASSIGNMENT_ID 
                        AND S.ENROLL_ID = E.ENROLL_ID
                    LEFT JOIN EXAM_ENTRIES E_ENT 
                        ON E_ENT.ASSIGNMENT_ID = A.ASSIGNMENT_ID 
                        AND E_ENT.ENROLL_ID = E.ENROLL_ID
                    WHERE E.OFFERING_ID = :OfferingId
                        AND A.OFFERING_ID = :OfferingId
                        AND E.STATUS = 'Registered'
                    ORDER BY U.LAST_NAME, U.FIRST_NAME, A.CREATED_AT";

            using var conn = _factory.CreateConnection();
            conn.Open();        

            var flatData = await conn.QueryAsync<GradeGridDTO>(sql, new { OfferingId = offeringId });

            var viewModel = new GradesViewModel();

            // 1. Extract Unique Assignments for Headers
            viewModel.Assignments = flatData
                .GroupBy(d => d.AssignmentId)
                .Select(g => new AssignmentHeaderViewModel
                {
                    AssignmentId = g.Key,
                    Title = g.First().AssignmentTitle,
                    MaxMarks = g.First().MaxMarks
                }).ToList();

            // 2. Group by Student to create Rows
            viewModel.StudentRows = flatData
                .GroupBy(d => d.StudentId)
                .Select(g => new StudentGradeRowViewModel
                {
                    StudentId = g.Key,
                    FullName = g.First().StudentName,
                    Grades = g.ToDictionary(x => x.AssignmentId, x => x.ObtainedMarks)
                }).ToList();

            // 3. Calculate Class Averages per Assignment
            foreach (var assn in viewModel.Assignments)
            {
                var allGradesForThisAssn = flatData
                    .Where(d => d.AssignmentId == assn.AssignmentId && d.ObtainedMarks.HasValue)
                    .Select(d => d.ObtainedMarks!.Value);

                if (allGradesForThisAssn.Any())
                    viewModel.AssignmentAverages[assn.AssignmentId] = allGradesForThisAssn.Average();
            }

            return viewModel;
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
