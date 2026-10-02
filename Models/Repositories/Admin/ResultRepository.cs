using System.Runtime.ExceptionServices;
using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Student;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.Services;
using OmniFlex.Models.ViewModels.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public class ResultRepository : IResultRepository
    {
        private readonly DbConnectionFactory _factory;

        public ResultRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            RESULT_ID        AS ResultId,
            STUDENT_ID       AS StudentId,
            ''               AS SectionId,
            OFFERING_ID      AS OfferingId,
            FINAL_GRADE      AS FinalGrade,
            FINAL_PERCENTAGE AS FinalPercentage";

        public async Task<Result?> GetByStudentAndSectionAsync(string studentId, string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Result>(
                $@"SELECT {SELECT_COLUMNS} FROM RESULTS 
                   WHERE STUDENT_ID = :StudentId AND OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )",
                new { StudentId = studentId, SectionId = sectionId });
        }

        public async Task<IEnumerable<Result>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Result>(
                $"SELECT {SELECT_COLUMNS} FROM RESULTS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }

        public async Task<IEnumerable<Result>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Result>(
                $@"SELECT {SELECT_COLUMNS} FROM RESULTS 
                   WHERE OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )",
                new { SectionId = sectionId });
        }

        public async Task<TranscriptViewModel?> GetTranscriptHeaderData(string studentId)
        {
            using var conn = _factory.CreateConnection();
            const string sql = @"SELECT 
                        FIRST_NAME || ' ' || LAST_NAME AS StudentName,
                        BATCH AS Batch, 
                        DEGREE AS DegreeProgram
                        FROM USERS 
                        WHERE USER_ID = :StudentId";
            var headerData = await conn.QueryAsync<TranscriptViewModel>(sql, new { StudentId = studentId });
            return headerData.FirstOrDefault();
        }

        public async Task<TranscriptViewModel> GetDetailedTranscriptAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();

            const string MetadataSql = @"WITH SEM_STATS AS (
                        SELECT
                        SM.SEMESTER_ID,
                        SM.SEMESTER_NAME,
                        SM.START_DATE,
        
                    -- Semester Attempted: Status is Dropped, Registered, or Completed
                        SUM(CASE 
                        WHEN E.STATUS IN ('Dropped', 'Registered', 'Completed') THEN C.CREDIT_HRS 
                        ELSE 0 
                        END) AS SEM_CR_ATT,
        
                    -- Semester Earned: Status must be 'Completed'
                        SUM(CASE 
                        WHEN E.STATUS = 'Completed' THEN C.CREDIT_HRS 
                        ELSE 0 
                        END) AS SEM_CR_ERND

                    FROM ENROLLMENTS E
                    JOIN SECTION_OFFERINGS SO ON E.OFFERING_ID = SO.OFFERING_ID
                    JOIN COURSES C           ON SO.COURSE_ID   = C.COURSE_ID
                    JOIN SEMESTERS SM        ON SO.SEMESTER_ID = SM.SEMESTER_ID
                    WHERE E.STUDENT_ID = :student_id
                    GROUP BY SM.SEMESTER_ID, SM.SEMESTER_NAME, SM.START_DATE
                    )
                        SELECT
                        SEMESTER_ID,
                        SEMESTER_NAME,
    
                -- Cumulative Attempted: Running total of credits touched
                        SUM(SEM_CR_ATT) OVER (ORDER BY START_DATE 
                            ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CUM_CR_ATT,
    
                -- Cumulative Earned: Running total of credits finished
                        SUM(SEM_CR_ERND) OVER (ORDER BY START_DATE 
                            ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CUM_CR_ERND

                    FROM SEM_STATS
                    ORDER BY START_DATE";

            const string CoursesSql = @"
                WITH STUDENT_COURSE_DATA AS (
            -- Step 1: Flatten all enrollment, course, and grade data
                SELECT
                SM.SEMESTER_ID,
                SM.SEMESTER_NAME,
                SM.START_DATE,
                C.COURSE_ID AS CourseCode,
                C.COURSE_NAME AS CourseName,
                S.SECTION_LABEL AS Section,
                S.BATCH AS Batch,
                S.DEGREE AS Degree,
                C.CREDIT_HRS,
                C.COURSE_TYPE AS Type,
                E.STATUS AS ENROLL_STATUS,
                NVL(R.FINAL_GRADE, 'I') AS Grade, -- Defaults to 'I' if no result entry exists
                NVL(GS.GPA_POINTS, 0) AS Points,
                NVL(GS.IS_PASSING, 'N') AS IS_PASSING,
            -- Quality Points for this specific course
                (NVL(GS.GPA_POINTS, 0) * C.CREDIT_HRS) AS CourseQP
            FROM ENROLLMENTS E
            JOIN SECTION_OFFERINGS SO ON E.OFFERING_ID = SO.OFFERING_ID
            JOIN SECTIONS S           ON SO.SECTION_ID  = S.SECTION_ID
            JOIN COURSES C           ON SO.COURSE_ID   = C.COURSE_ID
            JOIN SEMESTERS SM        ON SO.SEMESTER_ID = SM.SEMESTER_ID
            LEFT JOIN RESULTS R      ON R.OFFERING_ID  = E.OFFERING_ID 
                                    AND R.STUDENT_ID   = E.STUDENT_ID
            LEFT JOIN GRADE_SCALE GS ON R.FINAL_GRADE  = GS.GRADE
            WHERE E.STUDENT_ID = :student_id
            ),
                SEM_TOTALS AS (
            -- Step 2: Calculate Semester-level aggregates
                SELECT 
                SEMESTER_ID,
                SUM(CASE WHEN ENROLL_STATUS IN ('Dropped', 'Registered', 'Completed') THEN CREDIT_HRS ELSE 0 END) AS SEM_ATT,
                SUM(CASE WHEN ENROLL_STATUS = 'Completed' AND IS_PASSING = 'Y' THEN CREDIT_HRS ELSE 0 END) AS SEM_ERND,
                SUM(CASE WHEN ENROLL_STATUS = 'Completed' THEN CourseQP ELSE 0 END) AS SEM_QP,
                SUM(CASE WHEN ENROLL_STATUS = 'Completed' THEN CREDIT_HRS ELSE 0 END) AS SEM_GRADED_CR
            FROM STUDENT_COURSE_DATA
            GROUP BY SEMESTER_ID
            )
                SELECT
                D.SEMESTER_ID,
                D.SEMESTER_NAME,
                D.CourseCode,
                D.CourseName,
                D.Section,
                D.Batch,
                D.Degree,
                D.CREDIT_HRS AS Credits,
                D.Grade,
                D.Points,
                D.Type,
            -- Semester Summaries (Repeated for each course in that semester for easy C# grouping)
                T.SEM_ATT,
                T.SEM_ERND,
                ROUND(T.SEM_QP / NULLIF(T.SEM_GRADED_CR, 0), 2) AS SGPA,
            -- Cumulative Summaries using Window Functions
                SUM(T.SEM_ATT) OVER (ORDER BY D.START_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CUM_ATT,
                SUM(T.SEM_ERND) OVER (ORDER BY D.START_DATE ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CUM_ERND,
            ROUND(
                SUM(T.SEM_QP) OVER (ORDER BY D.START_DATE)/NULLIF(SUM(T.SEM_GRADED_CR) OVER (ORDER BY D.START_DATE), 0),2) AS CGPA
            FROM STUDENT_COURSE_DATA D
            JOIN SEM_TOTALS T ON D.SEMESTER_ID = T.SEMESTER_ID
            ORDER BY D.START_DATE, D.CourseCode";

            var rawData = await conn.QueryAsync<dynamic>(CoursesSql, new { student_id = studentId });
            var semesterSummaries = await conn.QueryAsync<dynamic>(MetadataSql, new { student_id = studentId });
            var Headerdata = await GetTranscriptHeaderData(studentId);
            if (Headerdata == null)
            {
                throw new Exception($"No header data found for student ID: {studentId}");
            }
            var summaryDict = semesterSummaries.ToDictionary(
                s => (string)s.SEMESTER_ID,
                s => s
            );

            var viewModel = new TranscriptViewModel
            {
                StudentId = studentId,
                StudentName = Headerdata.StudentName,
                DegreeProgram = CourseHelper.GetFullDegreeName(Headerdata.DegreeProgram),
                Batch = Headerdata.Batch,
                Semesters = rawData
                    .GroupBy(row => new
                    {
                        row.SEMESTER_ID,
                        row.SEMESTER_NAME,
                        //row.SEM_ATT,
                        //row.SEM_ERND,
                        row.SGPA,
                        row.CGPA,
                        row.START_DATE
                    })
                    .OrderBy(g => g.Key.START_DATE) // Ensure chronological order
                    .Select(g => 
                    {
                        var summary = summaryDict.ContainsKey(g.Key.SEMESTER_ID) 
                            ? summaryDict[g.Key.SEMESTER_ID] : null;

                        return new SemesterSection
                        {
                            SemesterName = summary?.SEMESTER_NAME ?? "Unknown",
                            SemCrAtt = Convert.ToDecimal(summary?.CUM_CR_ATT ?? 0),
                            SemCrErnd = Convert.ToDecimal(summary?.CUM_CR_ERND ?? 0),
                            SGPA = Convert.ToDecimal(g.Key.SGPA ?? 0),
                            CGPA = Convert.ToDecimal(g.Key.CGPA ?? 0),
                            Courses = g.Select(c => new TranscriptCourseRow
                            {
                                Code = c.COURSECODE,
                                CourseName = c.COURSENAME,
                                Batch = Convert.ToInt32(c.BATCH),
                                Degree = c.DEGREE,
                                Section = SectionHelper.GetFormattedSectionLabel(c.DEGREE, c.SECTION, Convert.ToInt32(c.BATCH)),
                                Credits = Convert.ToDecimal(c.CREDITS),
                                Grade = c.GRADE,
                                Points = Convert.ToDecimal(c.POINTS),
                                Type = c.TYPE
                            }).ToList()
                        };
                    }).ToList()
            };

            return viewModel;
        }

        public async Task<int> SaveResultAsync(Result result)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO RESULTS (STUDENT_ID, OFFERING_ID, FINAL_GRADE)
                VALUES (:StudentId, :OfferingId, :FinalGrade)", result);
        }

        public async Task<int> UpdateGradeAsync(string studentId, string sectionId, string grade, decimal percentage)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE RESULTS SET FINAL_GRADE = :Grade, FINAL_PERCENTAGE = :Percentage
                WHERE STUDENT_ID = :StudentId AND OFFERING_ID IN (
                    SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                )",
                new { StudentId = studentId, SectionId = sectionId, Grade = grade, Percentage = percentage });
        }
    }
}
