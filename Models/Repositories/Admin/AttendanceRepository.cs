using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.ViewModels.Student;
using OmniFlex.Models.Services;
using OmniFlex.Models.DTOs;

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
            MARKED_BY        AS MarkedBy,
            DURATION       AS Duration";

        public async Task<IEnumerable<Attendance>> GetByEnrollmentAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Attendance>(
                $"SELECT {SELECT_COLUMNS} FROM ATTENDANCE WHERE ENROLL_ID = :EnrollId ORDER BY ATTENDANCE_DATE DESC",
                new { EnrollId = enrollId });
        }

        public async Task<IEnumerable<Attendance>> GetBySectionAndDateAsync(string sectionId, DateTime date)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Attendance>(
                $@"SELECT {SELECT_COLUMNS} FROM ATTENDANCE a
                  JOIN ENROLLMENTS e ON a.ENROLL_ID = e.ENROLL_ID
                  WHERE e.SECTION_ID = :SectionId AND TRUNC(a.ATTENDANCE_DATE) = TRUNC(:Date)",
                new { SectionId = sectionId, Date = date });
        }
        public async Task<AttendanceViewModel> GetStudentAttendanceAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            const string sql = @"SELECT
                    C.COURSE_ID       AS CourseCode,
                    C.COURSE_NAME AS CourseName,
                    S.SECTION_LABEL AS Section,
                    S.BATCH AS Batch,
                    S.DEGREE AS Degree,
                    A.ATTENDANCE_DATE AS Attendance_Date,
                    TO_CHAR(A.ATTENDANCE_DATE, 'Day') AS Attendance_Day,
                    A.DURATION AS Duration,
                    A.STATUS AS Status
                FROM ENROLLMENTS E
                JOIN SECTION_OFFERINGS SO ON E.OFFERING_ID = SO.OFFERING_ID
                JOIN SEMESTERS SM ON SM.SEMESTER_ID = SO.SEMESTER_ID
                JOIN COURSES C ON C.COURSE_ID = SO.COURSE_ID
                JOIN SECTIONS S ON SO.SECTION_ID = S.SECTION_ID
                JOIN ATTENDANCE A ON A.ENROLL_ID = E.ENROLL_ID
                WHERE E.STUDENT_ID = :UserId AND SM.IS_CURRENT = 1
                ORDER BY A.ATTENDANCE_DATE ASC";

            // 1. Get flat data from Oracle
            var flatData = await conn.QueryAsync<AttendanceDto>(sql, new { UserId = userId });

            // 2. Group by Course to fill your ViewModel structure
            var courses = flatData
                .GroupBy(row => new { row.CourseCode, row.CourseName, row.Section, row.Batch, row.Degree })
                .Select(group =>
                {
                    var records = group.Select(r => new AttendanceRecord
                    {
                        Attendance_Date = r.Attendance_Date,
                        Attendance_Day = r.Attendance_Day.Trim(),
                        Duration = Convert.ToDouble(r.Duration),
                        Status = (string)r.Status == "P" ? "Present" :
                                 (string)r.Status == "L" ? "Late" : "Absent"
                    }).ToList();

                    int total = records.Count;
                    int attended = records.Count(r => r.Status == "Present" || r.Status == "Late");
                    string sectionLabel = SectionHelper.GetFormattedSectionLabel(group.Key.Degree, group.Key.Section, group.Key.Batch);

                    return new CourseAttendance
                    {
                        CourseCode = group.Key.CourseCode,
                        CourseName = group.Key.CourseName,
                        Section = sectionLabel,
                        Records = records,
                        TotalClasses = total,
                        ClassesAttended = attended,
                        ClassesMissed = total - attended,
                        AttendancePercentage = total > 0 ? Math.Round((double)attended / total * 100, 2) : 0
                    };
                }).ToList();

            return new AttendanceViewModel { Courses = courses };
        }

        public async Task<int> MarkAsync(Attendance attendance)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO ATTENDANCE (ENROLL_ID, ATTENDANCE_DATE, STATUS, MARKED_BY)
                VALUES (:EnrollId, :AttendanceDate, :Status, :MarkedBy)", attendance);
        }

        public async Task<int> UpdateAsync(Attendance attendance)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE ATTENDANCE SET STATUS = :Status, MARKED_BY = :MarkedBy
                WHERE ATTENDANCE_ID = :AttendanceId", attendance);
        }

        public async Task<decimal> GetAttendancePercentageAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteScalarAsync<decimal>(
                @"SELECT ROUND((SUM(CASE WHEN STATUS = 'P' THEN 1 ELSE 0 END) / 
                  NULLIF(COUNT(*), 0) * 100), 2) FROM ATTENDANCE WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId });
        }
    }
}
