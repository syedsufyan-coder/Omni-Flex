using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;

namespace OmniFlex.Models.Repositories.Admin
{
    public class SectionRepository : ISectionRepository
    {
        private readonly DbConnectionFactory _factory;

        public SectionRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            SECTION_ID    AS SectionId,
            SECTION_LABEL AS SectionLabel,
            DEPARTMENT_ID AS DepartmentId,
            DEGREE        AS DegreeProgram,
            BATCH         AS BatchYear,
            CR_ID         AS CrId";

        public async Task<Section?> GetByIdAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId });
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS");
        }

        public async Task<IEnumerable<SectionsDto>> GetAllDetailedAsync()
        {
            const string sql = @"SELECT
                S.SECTION_ID    AS SectionId,
                S.SECTION_LABEL AS SectionLabel,
                S.DEGREE        AS Degree,
                COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                CR.FIRST_NAME   AS CrFirstName,
                CR.LAST_NAME    AS CrLastName,
                D.DEPT_NAME     AS Department,
                S.BATCH         AS Batch
            FROM SECTIONS S
            JOIN DEPARTMENTS D          ON D.DEPT_ID      = S.DEPARTMENT_ID
            LEFT JOIN USERS CR          ON CR.USER_ID     = S.CR_ID
            LEFT JOIN SECTION_OFFERINGS SO ON SO.SECTION_ID = S.SECTION_ID
            LEFT JOIN SEMESTERS SM      ON SM.SEMESTER_ID = SO.SEMESTER_ID
                AND SM.IS_CURRENT = 1
            LEFT JOIN ENROLLMENTS E     ON E.OFFERING_ID  = SO.OFFERING_ID
                AND E.STATUS = 'Registered'
            GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
            ORDER BY S.SECTION_ID";
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql);
        }

        public async Task<IEnumerable<SectionsDto>> GetByCourseCurrentSemesterAsync(string courseId)
        {
            const string sql = @"
                SELECT S.SECTION_LABEL AS SectionLabel, U.FIRST_NAME || ' ' || U.LAST_NAME AS TeacherName,
                SO.SEATS AS Seats, S.DEGREE AS Degree
                FROM SECTION_OFFERINGS SO
                JOIN SECTIONS S    ON SO.SECTION_ID  = S.SECTION_ID
                JOIN SEMESTERS SM ON SO.SEMESTER_ID = SM.SEMESTER_ID
                LEFT JOIN USERS U  ON SO.TEACHER_ID  = U.USER_ID
                WHERE SO.COURSE_ID = :CourseId 
                AND SM.IS_CURRENT = 1";
            using var conn = _factory.CreateConnection();
            
            // Dapper maps the flat result set directly to the DTO properties
            return await conn.QueryAsync<SectionsDto>(sql, new { CourseId = courseId });
        }

        public async Task<IEnumerable<SectionsDto>> GetByCourseAsync(string courseId)
        {
            const string sql = @"SELECT
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                        CR.FIRST_NAME AS CrFirstName,
                        CR.LAST_NAME AS CrLastName,
                        D.DEPT_NAME AS Department,
                        S.BATCH AS Batch
                        FROM SECTIONS S
                        JOIN SECTION_OFFERINGS SO  ON SO.SECTION_ID  = S.SECTION_ID
                        JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                        JOIN DEPARTMENTS D         ON D.DEPT_ID      = S.DEPARTMENT_ID
                        LEFT JOIN USERS CR         ON CR.USER_ID     = S.CR_ID
                        LEFT JOIN ENROLLMENTS E    ON E.OFFERING_ID  = SO.OFFERING_ID
                           AND E.STATUS      = 'Registered'
                        WHERE SO.COURSE_ID   = :CourseId AND SM.IS_CURRENT  = 1
                        GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                        CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
                        ORDER BY S.SECTION_ID";
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql, new { CourseId = courseId });
        }

        public async Task<IEnumerable<SectionsDto>> GetByTeacherAsync(string teacherId)
        {
            const string sql = @"SELECT
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                        CR.FIRST_NAME AS CrFirstName,
                        CR.LAST_NAME AS CrLastName,
                        D.DEPT_NAME AS Department,
                        S.BATCH AS Batch
                        FROM SECTIONS S
                        JOIN SECTION_OFFERINGS SO  ON SO.SECTION_ID  = S.SECTION_ID
                        JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                        JOIN DEPARTMENTS D         ON D.DEPT_ID      = S.DEPARTMENT_ID
                        LEFT JOIN USERS CR         ON CR.USER_ID     = S.CR_ID
                        LEFT JOIN ENROLLMENTS E    ON E.OFFERING_ID  = SO.OFFERING_ID
                           AND E.STATUS      = 'Registered'
                        WHERE SO.TEACHER_ID  = :TeacherId AND SM.IS_CURRENT  = 1
                        GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                        CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
                        ORDER BY S.SECTION_ID";
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql, new { TeacherId = teacherId });
        }

        public async Task<IEnumerable<SectionsDto>> GetByDepartmentAsync(string deptId)
        {
            const string sql = @"SELECT
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                        CR.FIRST_NAME AS CrFirstName,
                        CR.LAST_NAME AS CrLastName,
                        D.DEPT_NAME AS Department,
                        S.BATCH AS Batch
                        FROM SECTIONS S
                        JOIN SECTION_OFFERINGS SO  ON SO.SECTION_ID  = S.SECTION_ID
                        JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                        JOIN DEPARTMENTS D         ON D.DEPT_ID      = S.DEPARTMENT_ID
                        LEFT JOIN USERS CR         ON CR.USER_ID     = S.CR_ID
                        LEFT JOIN ENROLLMENTS E    ON E.OFFERING_ID  = SO.OFFERING_ID
                           AND E.STATUS      = 'Registered'
                        WHERE S.DEPARTMENT_ID = :DeptId AND SM.IS_CURRENT  = 1
                        GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                        CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
                        ORDER BY S.SECTION_ID";
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql, new { DeptId = deptId });
        }

        public async Task<IEnumerable<SectionsDto>> GetByTaAsync(string taId)
        {
            const string sql = @"SELECT
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                        CR.FIRST_NAME AS CrFirstName,
                        CR.LAST_NAME AS CrLastName,
                        D.DEPT_NAME AS Department,
                        S.BATCH AS Batch
                        FROM SECTIONS S
                        JOIN SECTION_TAS ST        ON ST.SECTION_ID  = S.SECTION_ID
                        JOIN SEMESTERS SM          ON SM.SEMESTER_ID = ST.SEMESTER_ID
                        JOIN DEPARTMENTS D         ON D.DEPT_ID      = S.DEPARTMENT_ID
                        LEFT JOIN USERS CR         ON CR.USER_ID     = S.CR_ID
                        LEFT JOIN SECTION_OFFERINGS SO ON SO.SECTION_ID  = S.SECTION_ID
                            AND SO.SEMESTER_ID = ST.SEMESTER_ID
                        LEFT JOIN ENROLLMENTS E    ON E.OFFERING_ID  = SO.OFFERING_ID
                            AND E.STATUS      = 'Registered'
                        WHERE ST.TA_ID = :TaId AND SM.IS_CURRENT  = 1
                        GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                        CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
                        ORDER BY S.SECTION_ID";
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql, new { TaId = taId });
        }

        public async Task<IEnumerable<SectionsDto>> GetBySemesterAsync(string semesterId)
        {
            const string sql = @"SELECT
                        S.SECTION_ID AS SectionId,
                        S.SECTION_LABEL AS SectionLabel,
                        S.DEGREE AS Degree,
                        COUNT(DISTINCT E.STUDENT_ID) AS EnrolledStudents,
                        CR.FIRST_NAME AS CrFirstName,
                        CR.LAST_NAME AS CrLastName,
                        D.DEPT_NAME AS Department,
                        S.BATCH AS Batch
                        FROM SECTIONS S
                        JOIN SECTION_OFFERINGS SO  ON SO.SECTION_ID  = S.SECTION_ID
                        JOIN SEMESTERS SM          ON SM.SEMESTER_ID = SO.SEMESTER_ID
                        JOIN DEPARTMENTS D         ON D.DEPT_ID      = S.DEPARTMENT_ID
                        LEFT JOIN USERS CR         ON CR.USER_ID     = S.CR_ID
                        LEFT JOIN ENROLLMENTS E    ON E.OFFERING_ID  = SO.OFFERING_ID
                        AND E.STATUS IN ('Registered','Completed')
                        WHERE SM.SEMESTER_ID = :SemesterId
                        GROUP BY S.SECTION_ID, S.DEGREE, S.SECTION_LABEL,
                        CR.FIRST_NAME, CR.LAST_NAME, D.DEPT_NAME, S.BATCH
                        ORDER BY S.SECTION_ID";

            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<SectionsDto>(sql, new { SemesterId = semesterId });
        }

        public async Task<int> CreateAsync(Section section)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                INSERT INTO SECTIONS 
                (SECTION_ID, SECTION_LABEL, DEPARTMENT_ID, DEGREE, BATCH)
                VALUES 
                (:SectionId, :SectionLabel, :DepartmentId, :DegreeProgram, :BatchYear)",
                section);
        }

        public async Task<int> UpdateAsync(Section section)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                UPDATE SECTIONS SET
                SECTION_LABEL = :SectionLabel,
                DEPARTMENT_ID = :DepartmentId,
                DEGREE        = :DegreeProgram,
                BATCH         = :BatchYear
                WHERE SECTION_ID = :SectionId",
                section);
        }

        public async Task<int> AssignTeacherAsync(string sectionId, string teacherId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE SECTIONS SET TEACHER_ID = :TeacherId WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId, TeacherId = teacherId });
        }
        public async Task<bool> ExistsAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();

            // Formal Title Case SQL Query
            const string sql = "SELECT COUNT(1) FROM SECTIONS WHERE SECTION_ID = :SectionId";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { SectionId = sectionId });
            return count > 0;
        }

        public async Task<int> AddTaAsync(string sectionId, string taId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO SECTION_TAS (SECTION_ID, TA_ID)
                VALUES (:SectionId, :TaId)",
                new { SectionId = sectionId, TaId = taId });
        }

        public async Task<int> RemoveTaAsync(string sectionId, string taId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "DELETE FROM SECTION_TAS WHERE SECTION_ID = :SectionId AND TA_ID = :TaId",
                new { SectionId = sectionId, TaId = taId });
        }

        // This method is now expired because of change in Database design.
        public async Task<int> GetEnrolledCountAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) FROM ENROLLMENTS 
                  WHERE OFFERING_ID IN (
                      SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                  ) AND STATUS = 'Registered'",
                new { SectionId = sectionId });
        }

        public async Task<int> AssignInstructorToCourseSectionsAsync(string courseId, string instructorId)
        {
            using var conn = _factory.CreateConnection();
            
            // Update all section offerings for this course in the current semester
            const string sql = @"
                UPDATE SECTION_OFFERINGS 
                SET TEACHER_ID = :InstructorId 
                WHERE COURSE_ID = :CourseId 
                AND SEMESTER_ID = (SELECT SEMESTER_ID FROM SEMESTERS WHERE IS_CURRENT = 1)";
            
            return await conn.ExecuteAsync(sql, new { CourseId = courseId, InstructorId = instructorId });
        }

        public async Task<IEnumerable<StudentEnrollmentDto>> GetEnrolledStudentsAsync(string sectionId)
        {
            const string sql = @"
                SELECT DISTINCT
                    U.USER_ID AS StudentId,
                    U.FIRST_NAME AS FirstName,
                    U.LAST_NAME AS LastName,
                    U.EMAIL AS Email,
                    U.DEGREE AS Degree,
                    U.BATCH AS Batch,
                    U.STATUS AS Status,
                    D.DEPT_NAME AS Department
                FROM ENROLLMENTS E
                JOIN SECTION_OFFERINGS SO ON E.OFFERING_ID = SO.OFFERING_ID
                JOIN USERS U ON E.STUDENT_ID = U.USER_ID
                LEFT JOIN DEPARTMENTS D ON D.DEPT_ID = U.DEPT_ID
                WHERE SO.SECTION_ID = :SectionId 
                    AND E.STATUS = 'Registered'
                ORDER BY U.USER_ID";
            
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<StudentEnrollmentDto>(sql, new { SectionId = sectionId });
        }

        // DELETE SECTION
        public async Task<int> DeleteAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                "DELETE FROM SECTIONS WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId });
        }

        // ASSIGN CR (Class Representative)
        public async Task<int> AssignCRAsync(string sectionId, string studentId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                "UPDATE SECTIONS SET CR_ID = :CrId WHERE SECTION_ID = :SectionId",
                new { CrId = studentId, SectionId = sectionId });
        }
    }
}
