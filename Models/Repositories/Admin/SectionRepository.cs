using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class SectionRepository : ISectionRepository
    {
        private readonly DbConnectionFactory _factory;

        public SectionRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            SECTION_ID    AS SectionId,
            COURSE_ID     AS CourseId,
            SEMESTER_ID   AS SemesterId,
            TEACHER_ID    AS TeacherId,
            SEATS         AS Seats,
            SECTION_LABEL AS SectionLabel,
            ROOM_NO       AS RoomNo,
            TIME_SLOT     AS TimeSlot";

        public async Task<Section?> GetByIdAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryFirstOrDefaultAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId });
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS");
        }

        public async Task<IEnumerable<Section>> GetByCourseAsync(string courseId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId });
        }

        public async Task<IEnumerable<Section>> GetByTeacherAsync(string teacherId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS WHERE TEACHER_ID = :TeacherId",
                new { TeacherId = teacherId });
        }

        public async Task<IEnumerable<Section>> GetBySemesterAsync(string semesterId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Section>(
                $"SELECT {SELECT_COLUMNS} FROM SECTIONS WHERE SEMESTER_ID = :SemesterId",
                new { SemesterId = semesterId });
        }

        public async Task<int> CreateAsync(Section section)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO SECTIONS 
                (SECTION_ID, COURSE_ID, SEMESTER_ID, TEACHER_ID, SEATS, 
                 SECTION_LABEL, ROOM_NO, TIME_SLOT)
                VALUES 
                (:SectionId, :CourseId, :SemesterId, :TeacherId, :Seats,
                 :SectionLabel, :RoomNo, :TimeSlot)", section);
        }

        public async Task<int> UpdateAsync(Section section)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                UPDATE SECTIONS SET 
                COURSE_ID = :CourseId, SEMESTER_ID = :SemesterId, TEACHER_ID = :TeacherId,
                SEATS = :Seats, SECTION_LABEL = :SectionLabel, ROOM_NO = :RoomNo,
                TIME_SLOT = :TimeSlot
                WHERE SECTION_ID = :SectionId", section);
        }

        public async Task<int> AssignTeacherAsync(string sectionId, string teacherId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "UPDATE SECTIONS SET TEACHER_ID = :TeacherId WHERE SECTION_ID = :SectionId",
                new { SectionId = sectionId, TeacherId = teacherId });
        }

        public async Task<int> AddTaAsync(string sectionId, string taId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO SECTION_TAS (SECTION_ID, TA_ID)
                VALUES (:SectionId, :TaId)",
                new { SectionId = sectionId, TaId = taId });
        }

        public async Task<int> RemoveTaAsync(string sectionId, string taId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "DELETE FROM SECTION_TAS WHERE SECTION_ID = :SectionId AND TA_ID = :TaId",
                new { SectionId = sectionId, TaId = taId });
        }

        public async Task<int> GetEnrolledCountAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ENROLLMENTS WHERE SECTION_ID = :SectionId AND STATUS = 'Registered'",
                new { SectionId = sectionId });
        }
    }
}
