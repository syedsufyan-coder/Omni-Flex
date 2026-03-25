using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.ViewModels.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DbConnectionFactory _factory;

        public EnrollmentRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ENROLL_ID   AS EnrollId,
            STUDENT_ID  AS StudentId,
            SECTION_ID  AS SectionId,
            ENROLL_DATE AS EnrollDate,
            STATUS      AS Status";

        public async Task<Enrollment?> GetByIdAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryFirstOrDefaultAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId });
        }

        public async Task<List<ClassCard>> GetEnrolledClassesAsync(string studentId)
        {
            const string sql = @"SELECT
                        C.COURSE_ID       AS CourseCode,
                        C.COURSE_NAME     AS CourseName,
                        T.FIRST_NAME || ' ' || T.LAST_NAME AS TeacherName,
                        S.SECTION_LABEL   AS Section,
                        S.BATCH AS Batch,
                        S.DEGREE AS Degree
                FROM ENROLLMENTS E
                JOIN SECTION_OFFERINGS SO ON SO.OFFERING_ID = E.OFFERING_ID
                JOIN SEMESTERS SM        ON SM.SEMESTER_ID = SO.SEMESTER_ID
                JOIN COURSES C           ON C.COURSE_ID    = SO.COURSE_ID
                JOIN SECTIONS S          ON SO.SECTION_ID  = S.SECTION_ID
                LEFT JOIN USERS T        ON T.USER_ID      = SO.TEACHER_ID
                WHERE E.STUDENT_ID = :StudentId AND E.STATUS = 'Registered' AND SM.IS_CURRENT = 1
                ORDER BY C.COURSE_NAME";


            using var conn = _factory.CreateConnection();
            conn.Open();
            return (await conn.QueryAsync<ClassCard>(sql, new { StudentId = studentId })).ToList();
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }

        public async Task<IEnumerable<Enrollment>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId });
        }

        public async Task<int> EnrollAsync(Enrollment enrollment)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO ENROLLMENTS (STUDENT_ID, SECTION_ID, ENROLL_DATE, STATUS)
                VALUES (:StudentId, :SectionId, :EnrollDate, :Status)", enrollment);
        }

        public async Task<int> TransferAsync(int enrollId, string newSectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET SECTION_ID = :NewSectionId WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, NewSectionId = newSectionId });
        }

        public async Task<int> UpdateStatusAsync(int enrollId, string status)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET STATUS = :Status WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, Status = status });
        }

        public async Task<bool> IsEnrolledAsync(string studentId, string sectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ENROLLMENTS WHERE STUDENT_ID = :StudentId AND SECTION_ID = :SectionId",
                new { StudentId = studentId, SectionId = sectionId });
            return count > 0;
        }
    }
}
