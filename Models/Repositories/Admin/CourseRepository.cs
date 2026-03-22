using Dapper;
using OmniFlex.Models.DTOs;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DbConnectionFactory _factory;

        public CourseRepository(DbConnectionFactory factory)
            => _factory = factory;

        // Column mapping for basic Course domain object
        private const string SELECT_COLUMNS = @"
            COURSE_ID   AS CourseId,
            DEPT_ID     AS DeptId,
            COURSE_NAME AS CourseName,
            CREDIT_HRS  AS CreditHrs,
            COURSE_TYPE AS CourseType,
            COURSE_CAT  AS CourseCat,
            PRE_REQ_ID  AS PreReqId,
            IS_ACTIVE   AS IsActive";

        // Returns a single course by its ID
        public async Task<Course?> GetByIdAsync(string courseId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId });
        }

        // Returns all courses in the database
        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES");
        }

        // Returns only active courses
        public async Task<IEnumerable<Course>> GetActiveAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE IS_ACTIVE = 1");
        }

        // Returns courses by department (basic — no JOIN)
        public async Task<IEnumerable<Course>> GetByDeptAsync(string deptId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE DEPT_ID = :DeptId",
                new { DeptId = deptId });
        }

        // Returns courses offered in a department in the current semester with full details
        // DISTINCT is used because one course can be offered in multiple sections of the same department
        public async Task<IEnumerable<CourseDto>> GetByDeptWithDetailsAsync(string deptId)
        {
            const string sql = @"SELECT DISTINCT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    JOIN SECTION_OFFERINGS SO  ON SO.COURSE_ID   = C.COURSE_ID
                    JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                    LEFT JOIN COURSES PRE      ON PRE.COURSE_ID  = C.PRE_REQ_ID
                    WHERE C.DEPT_ID = :Dept_id AND SM.IS_CURRENT = 1";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Dept_id = deptId });
        }

        // Returns courses taught by a specific teacher in the current semester
        // DISTINCT is used because a teacher can teach the same course in multiple sections
        public async Task<IEnumerable<CourseDto>> GetByTeacherWithDetailsAsync(string teacherId)
        {
            const string sql = @"SELECT DISTINCT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    JOIN SECTION_OFFERINGS SO  ON SO.COURSE_ID   = C.COURSE_ID
                    JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                    LEFT JOIN COURSES PRE      ON PRE.COURSE_ID  = C.PRE_REQ_ID
                    WHERE SO.TEACHER_ID = :Teacher_id AND SM.IS_CURRENT = 1";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Teacher_id = teacherId });
        }

        // Returns all courses offered to a specific section in the current semester
        // DISTINCT is used to avoid duplicate rows from multiple offerings
        public async Task<IEnumerable<CourseDto>> GetBySectionWithDetailsAsync(string sectionId)
        {
            const string sql = @"SELECT DISTINCT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    JOIN SECTION_OFFERINGS SO  ON SO.COURSE_ID   = C.COURSE_ID
                    JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                    LEFT JOIN COURSES PRE      ON PRE.COURSE_ID  = C.PRE_REQ_ID
                    WHERE SO.SECTION_ID = :Section_id AND SM.IS_CURRENT = 1
                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Section_id = sectionId });
        }

        // Returns active courses filtered by credit hours
        // No DISTINCT needed — no SECTION_OFFERINGS JOIN, filters directly from COURSES table
        public async Task<IEnumerable<CourseDto>> GetByCreditsWithDetailsAsync(int creditHrs)
        {
            const string sql = @"SELECT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    LEFT JOIN COURSES PRE ON PRE.COURSE_ID = C.PRE_REQ_ID
                    WHERE C.CREDIT_HRS = :Credit_Hours AND C.IS_ACTIVE = 1
                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Credit_Hours = creditHrs });
        }

        // Returns active courses filtered by course type — Theory or Lab
        // No DISTINCT needed — filters directly from COURSES table
        public async Task<IEnumerable<CourseDto>> GetByCourseTypeWithDetailsAsync(string courseType)
        {
            const string sql = @"SELECT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    LEFT JOIN COURSES PRE ON PRE.COURSE_ID = C.PRE_REQ_ID
                    WHERE C.COURSE_TYPE = :Coursetype AND C.IS_ACTIVE = 1
                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Coursetype = courseType });
        }

        // Returns active courses filtered by course category — Core or Elective
        // No DISTINCT needed — filters directly from COURSES table
        public async Task<IEnumerable<CourseDto>> GetByCourseCatWithDetailsAsync(string courseCat)
        {
            const string sql = @"SELECT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    LEFT JOIN COURSES PRE ON PRE.COURSE_ID = C.PRE_REQ_ID
                    WHERE C.COURSE_CAT = :Course_cat AND C.IS_ACTIVE = 1
                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { Course_cat = courseCat });
        }

        // Returns active courses that have a specific course as prerequisite
        // No DISTINCT needed — filters directly from COURSES table
        public async Task<IEnumerable<CourseDto>> GetByPreRequisiteWithDetailsAsync(string preRequisiteId)
        {
            const string sql = @"SELECT
                        C.COURSE_ID   AS CourseId,
                        C.COURSE_NAME AS CourseName,
                        C.CREDIT_HRS  AS CreditHours,
                        C.COURSE_TYPE AS CourseType,
                        C.COURSE_CAT  AS CourseCat,
                        NVL(PRE.COURSE_NAME, 'None') AS PreRequisite
                    FROM COURSES C
                    LEFT JOIN COURSES PRE ON PRE.COURSE_ID = C.PRE_REQ_ID
                    WHERE C.PRE_REQ_ID = :PreReqId AND C.IS_ACTIVE = 1
                    ORDER BY C.COURSE_NAME";

            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<CourseDto>(sql, new { PreReqId = preRequisiteId });
        }

        // Inserts a new course into the database
        public async Task<int> CreateAsync(Course course)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                INSERT INTO COURSES
                (COURSE_ID, DEPT_ID, COURSE_NAME, CREDIT_HRS, COURSE_TYPE,
                 COURSE_CAT, PRE_REQ_ID, IS_ACTIVE)
                VALUES
                (:CourseId, :DeptId, :CourseName, :CreditHrs, :CourseType,
                 :CourseCat, :PreReqId, :IsActive)", course);
        }

        // Updates an existing course by its CourseId
        public async Task<int> UpdateAsync(Course course)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                UPDATE COURSES SET
                DEPT_ID     = :DeptId,
                COURSE_NAME = :CourseName,
                CREDIT_HRS  = :CreditHrs,
                COURSE_TYPE = :CourseType,
                COURSE_CAT  = :CourseCat,
                PRE_REQ_ID  = :PreReqId,
                IS_ACTIVE   = :IsActive
                WHERE COURSE_ID = :CourseId", course);
        }

        // Deletes a course by its CourseId
        public async Task<int> DeleteAsync(string courseId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                "DELETE FROM COURSES WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId });
        }

        // Updates the active/inactive status of a course
        public async Task<int> SetActiveStatusAsync(string courseId, int status)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                "UPDATE COURSES SET IS_ACTIVE = :Status WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId, Status = status });
        }
    }
}