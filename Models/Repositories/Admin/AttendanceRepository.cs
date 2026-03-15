using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DbConnectionFactory _factory;

        public AttendanceRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ATTENDANCE_ID    AS AttendanceId,
            ENROLL_ID        AS EnrollId,
            ATTENDANCE_DATE  AS AttendanceDate,
            STATUS           AS Status,
            MARKED_BY        AS MarkedBy";

        public async Task<IEnumerable<Attendance>> GetByEnrollmentAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Attendance>(
                $"SELECT {SELECT_COLUMNS} FROM ATTENDANCE WHERE ENROLL_ID = :EnrollId ORDER BY ATTENDANCE_DATE DESC",
                new { EnrollId = enrollId });
        }

        public async Task<IEnumerable<Attendance>> GetBySectionAndDateAsync(string sectionId, DateTime date)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Attendance>(
                $@"SELECT {SELECT_COLUMNS} FROM ATTENDANCE a
                  JOIN ENROLLMENTS e ON a.ENROLL_ID = e.ENROLL_ID
                  WHERE e.SECTION_ID = :SectionId AND TRUNC(a.ATTENDANCE_DATE) = TRUNC(:Date)",
                new { SectionId = sectionId, Date = date });
        }

        public async Task<int> MarkAsync(Attendance attendance)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO ATTENDANCE (ENROLL_ID, ATTENDANCE_DATE, STATUS, MARKED_BY)
                VALUES (:EnrollId, :AttendanceDate, :Status, :MarkedBy)", attendance);
        }

        public async Task<int> UpdateAsync(Attendance attendance)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                UPDATE ATTENDANCE SET STATUS = :Status, MARKED_BY = :MarkedBy
                WHERE ATTENDANCE_ID = :AttendanceId", attendance);
        }

        public async Task<decimal> GetAttendancePercentageAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteScalarAsync<decimal>(
                @"SELECT ROUND((SUM(CASE WHEN STATUS = 'P' THEN 1 ELSE 0 END) / 
                  NULLIF(COUNT(*), 0) * 100), 2) FROM ATTENDANCE WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId });
        }
    }
}
