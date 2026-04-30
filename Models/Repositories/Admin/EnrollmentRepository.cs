using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

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
            
            return await conn.QueryFirstOrDefaultAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId });
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }
        public async Task<Enrollment?> GetRecordAsync(string studentId, string courseId)
        {
            using var conn = _factory.CreateConnection();

            // Student that's being appointed as TA must have completed the assigned course with requirements fulfilled
            string query = @"
                SELECT 
                    STUDENT_ID AS StudentId,
                    COURSE_ID  AS CourseId,
                    GRADE      AS Grade,
                    STATUS     AS Status
                FROM ENROLLMENTS 
                WHERE STUDENT_ID = :StudentId 
                AND COURSE_ID = :CourseId";

            return await conn.QueryFirstOrDefaultAsync<Enrollment>(query,
                new { StudentId = studentId, CourseId = courseId });
        }

        public async Task<IEnumerable<Enrollment>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId });
        }

        public async Task<int> EnrollAsync(Enrollment enrollment)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO ENROLLMENTS (STUDENT_ID, SECTION_ID, ENROLL_DATE, STATUS)
                VALUES (:StudentId, :SectionId, :EnrollDate, :Status)", enrollment);
        }

        public async Task<int> TransferAsync(int enrollId, string newSectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET SECTION_ID = :NewSectionId WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, NewSectionId = newSectionId });
        }

        public async Task<int> UpdateStatusAsync(int enrollId, string status)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET STATUS = :Status WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, Status = status });
        }

        public async Task<bool> IsEnrolledAsync(string studentId, string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ENROLLMENTS WHERE STUDENT_ID = :StudentId AND SECTION_ID = :SectionId",
                new { StudentId = studentId, SectionId = sectionId });
            return count > 0;
        }
    }
}
