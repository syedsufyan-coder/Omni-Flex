using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _factory;

        public UserRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            USER_ID        AS UserId,
            DEPT_ID        AS DeptId,
            FIRST_NAME     AS FirstName,
            LAST_NAME      AS LastName,
            EMAIL          AS Email,
            GENDER         AS Gender,
            DOB            AS DOB,
            PHONE_NUMBER   AS PhoneNumber,
            ADDRESS        AS Address,
            CITY           AS City,
            COUNTRY        AS Country,
            PASSWORD_HASH  AS PasswordHash,
            ROLE           AS Role,
            STATUS         AS Status,
            BATCH          AS Batch,
            DEGREE         AS Degree,
            TA_PASSWORD_HASH AS TaPasswordHash,
            DESIGNATION    AS Designation,
            OFFICE_ROOM    AS OfficeRoom,
            SPECIALIZATION AS Specialization";

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
                $"SELECT {SELECT_COLUMNS} FROM USERS");
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<User>(
                $"SELECT {SELECT_COLUMNS} FROM USERS WHERE ROLE = :Role AND STATUS = 'Active'",
                new { Role = role });
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO USERS 
                (USER_ID, DEPT_ID, FIRST_NAME, LAST_NAME, EMAIL, PASSWORD_HASH, 
                 ROLE, STATUS, BATCH, DEGREE, TA_PASSWORD_HASH,
                 DESIGNATION, OFFICE_ROOM, SPECIALIZATION)
                VALUES 
                (:UserId, :DeptId, :FirstName, :LastName, :Email, :PasswordHash,
                 :Role, :Status, :Batch, :Degree, :TaPasswordHash,
                 :Designation, :OfficeRoom, :Specialization)", user);
        }

        public async Task<int> UpdateAsync(User user)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE USERS SET 
                DEPT_ID = :DeptId, FIRST_NAME = :FirstName, LAST_NAME = :LastName,
                EMAIL = :Email, PASSWORD_HASH = :PasswordHash, ROLE = :Role, STATUS = :Status,
                BATCH = :Batch, DEGREE = :Degree, TA_PASSWORD_HASH = :TaPasswordHash,
                DESIGNATION = :Designation, OFFICE_ROOM = :OfficeRoom, SPECIALIZATION = :Specialization
                WHERE USER_ID = :UserId", user);
        }

        public async Task<int> DeleteAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "DELETE FROM USERS WHERE USER_ID = :UserId",
                new { UserId = userId });
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            using var conn = _factory.CreateConnection();
            
            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM USERS WHERE USER_ID = :UserId",
                new { UserId = userId });
            return count > 0;
        }

        // Known issue: We need to implement getting TA Functionality after the change in DB Migration.
        public async Task<int> GetCountByRoleAsync(string role)
        {
            using var conn = _factory.CreateConnection();
            

            if(string.Equals(role, "TA", StringComparison.OrdinalIgnoreCase))
            {
                return await conn.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM 
                (SELECT DISTINCT TA_ASSIGNMENT_ID,SECTION_ID,TA_ID,COURSE_ID, SEMESTER_ID FROM SECTION_TAS)");
            }

            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM USERS WHERE ROLE = :Role",
                new { Role = role });
            return count;
        }
    }
}
