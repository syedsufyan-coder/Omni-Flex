using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;

namespace OmniFlex.Models.Repositories.Admin
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _factory;

        public UserRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            USER_ID          AS UserId,
            DEPT_ID          AS DeptId,
            FIRST_NAME       AS FirstName,
            LAST_NAME        AS LastName,
            EMAIL            AS Email,
            GENDER           AS Gender,
            DOB              AS DOB,
            PHONE_NUMBER     AS PhoneNumber,
            ADDRESS          AS Address,
            CITY             AS City,
            COUNTRY          AS Country,
            PASSWORD_HASH    AS PasswordHash,
            ROLE             AS Role,
            STATUS           AS Status,
            BATCH            AS Batch,
            DEGREE           AS Degree,
            DESIGNATION      AS Designation,
            OFFICE_ROOM      AS OfficeRoom,
            SPECIALIZATION   AS Specialization";

        // READ 
        public async Task<User?> GetByIdAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(
                $"SELECT {SELECT_COLUMNS} FROM USERS WHERE USER_ID = :UserId",
                new { UserId = userId });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>(
                $"SELECT {SELECT_COLUMNS} FROM USERS WHERE EMAIL = :Email",
                new { Email = email });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<User>(
                $"SELECT {SELECT_COLUMNS} FROM USERS ORDER BY ROLE, LAST_NAME");
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<User>(
                $"SELECT {SELECT_COLUMNS} FROM USERS WHERE ROLE = :Role ORDER BY LAST_NAME",
                new { Role = role });
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM USERS WHERE USER_ID = :UserId",
                new { UserId = userId });
            return count > 0;
        }

        public async Task<int> GetCountByRoleAsync(string role)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM USERS WHERE ROLE = :Role",
                new { Role = role });
        }

        // CREATE 
        public async Task<int> CreateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                INSERT INTO USERS 
                (USER_ID, DEPT_ID, FIRST_NAME, LAST_NAME, EMAIL,
                 GENDER, DOB, PHONE_NUMBER, ADDRESS, CITY, COUNTRY,
                 PASSWORD_HASH, ROLE, STATUS,
                 BATCH, DEGREE,
                 DESIGNATION, OFFICE_ROOM, SPECIALIZATION)
                VALUES 
                (:UserId, :DeptId, :FirstName, :LastName, :Email,
                 :Gender, :DOB, :PhoneNumber, :Address, :City, :Country,
                 :PasswordHash, :Role, :Status,
                 :Batch, :Degree,
                 :Designation, :OfficeRoom, :Specialization)",
                user);
        }

        // UPDATE
        public async Task<int> UpdateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(@"
                UPDATE USERS SET
                    DEPT_ID          = :DeptId,
                    FIRST_NAME       = :FirstName,
                    LAST_NAME        = :LastName,
                    EMAIL            = :Email,
                    GENDER           = :Gender,
                    DOB              = :DOB,
                    PHONE_NUMBER     = :PhoneNumber,
                    ADDRESS          = :Address,
                    CITY             = :City,
                    COUNTRY          = :Country,
                    PASSWORD_HASH    = :PasswordHash,
                    ROLE             = :Role,
                    STATUS           = :Status,
                    BATCH            = :Batch,
                    DEGREE           = :Degree,
                    DESIGNATION      = :Designation,
                    OFFICE_ROOM      = :OfficeRoom,
                    SPECIALIZATION   = :Specialization
                WHERE USER_ID = :UserId",
                user);
        }

        // DELETE 
        public async Task<int> DeleteAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.ExecuteAsync(
                "DELETE FROM USERS WHERE USER_ID = :UserId",
                new { UserId = userId });
        }

        // ─── TA — Disabled for now, implement after DB migration ─────
        /*
        public async Task<int> AddTaToSectionAsync(TAAssignmentDto assignment)
        {
            using var conn = _factory.CreateConnection();
            const string sql = @"
                INSERT INTO SECTION_TAS 
                (TA_ASSIGNMENT_ID, SECTION_ID, TA_ID, COURSE_ID, SEMESTER_ID, ASSIGNED_BY) 
                VALUES 
                (:Id, :SectionId, :StudentId, :CourseId, :SemesterId, :AssignedBy)";

            return await conn.ExecuteAsync(sql, new
            {
                Id         = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(),
                SectionId  = assignment.SectionId,
                StudentId  = assignment.StudentId,
                CourseId   = assignment.CourseId,
                SemesterId = assignment.SemesterId,
                AssignedBy = assignment.AssignedBy
            });
        }

        public async Task<IEnumerable<User>> GetTasBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            string sql = $@"
                SELECT u.USER_ID AS UserId, u.FIRST_NAME AS FirstName, u.LAST_NAME AS LastName
                FROM USERS u
                JOIN SECTION_TAS st ON u.USER_ID = st.TA_ID
                WHERE st.SECTION_ID = :SectionId";
            return await conn.QueryAsync<User>(sql, new { SectionId = sectionId });
        }
        */

        // Stub implementations to satisfy interface — remove when TA enabled
        public Task<int> AddTaToSectionAsync(TAAssignmentDto assignment)
            => Task.FromResult(0);

        public Task<IEnumerable<User>> GetTasBySectionAsync(string sectionId)
            => Task.FromResult(Enumerable.Empty<User>());
    }
}