using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DbConnectionFactory _factory;

        public CourseRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            COURSE_ID   AS CourseId,
            DEPT_ID     AS DeptId,
            COURSE_NAME AS CourseName,
            CREDIT_HRS  AS CreditHrs,
            COURSE_TYPE AS CourseType,
            COURSE_CAT  AS CourseCat,
            PRE_REQ_ID  AS PreReqId,
            IS_ACTIVE   AS IsActive";

        public async Task<Course?> GetByIdAsync(string courseId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryFirstOrDefaultAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId });
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES");
        }

        public async Task<IEnumerable<Course>> GetActiveAsync()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE IS_ACTIVE = 1");
        }

        public async Task<IEnumerable<Course>> GetByDeptAsync(string deptId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Course>(
                $"SELECT {SELECT_COLUMNS} FROM COURSES WHERE DEPT_ID = :DeptId",
                new { DeptId = deptId });
        }

        public async Task<int> CreateAsync(Course course)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO COURSES 
                (COURSE_ID, DEPT_ID, COURSE_NAME, CREDIT_HRS, COURSE_TYPE, 
                 COURSE_CAT, PRE_REQ_ID, IS_ACTIVE)
                VALUES 
                (:CourseId, :DeptId, :CourseName, :CreditHrs, :CourseType,
                 :CourseCat, :PreReqId, :IsActive)", course);
        }

        public async Task<int> UpdateAsync(Course course)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                UPDATE COURSES SET 
                DEPT_ID = :DeptId, COURSE_NAME = :CourseName, CREDIT_HRS = :CreditHrs,
                COURSE_TYPE = :CourseType, COURSE_CAT = :CourseCat, PRE_REQ_ID = :PreReqId,
                IS_ACTIVE = :IsActive
                WHERE COURSE_ID = :CourseId", course);
        }

        public async Task<int> DeleteAsync(string courseId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "DELETE FROM COURSES WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId });
        }

        public async Task<int> SetActiveStatusAsync(string courseId, int status)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "UPDATE COURSES SET IS_ACTIVE = :Status WHERE COURSE_ID = :CourseId",
                new { CourseId = courseId, Status = status });
        }
    }
}
