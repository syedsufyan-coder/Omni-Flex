using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class ResultRepository : IResultRepository
    {
        private readonly DbConnectionFactory _factory;

        public ResultRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            RESULT_ID        AS ResultId,
            STUDENT_ID       AS StudentId,
            ''               AS SectionId,
            OFFERING_ID      AS OfferingId,
            FINAL_GRADE      AS FinalGrade,
            FINAL_PERCENTAGE AS FinalPercentage";

        public async Task<Result?> GetByStudentAndSectionAsync(string studentId, string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Result>(
                $@"SELECT {SELECT_COLUMNS} FROM RESULTS 
                   WHERE STUDENT_ID = :StudentId AND OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )",
                new { StudentId = studentId, SectionId = sectionId });
        }

        public async Task<IEnumerable<Result>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Result>(
                $"SELECT {SELECT_COLUMNS} FROM RESULTS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }

        public async Task<IEnumerable<Result>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Result>(
                $@"SELECT {SELECT_COLUMNS} FROM RESULTS 
                   WHERE OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )",
                new { SectionId = sectionId });
        }

        public async Task<int> SaveResultAsync(Result result)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO RESULTS (STUDENT_ID, OFFERING_ID, FINAL_GRADE)
                VALUES (:StudentId, :OfferingId, :FinalGrade)", result);
        }

        public async Task<int> UpdateGradeAsync(string studentId, string sectionId, string grade, decimal percentage)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE RESULTS SET FINAL_GRADE = :Grade, FINAL_PERCENTAGE = :Percentage
                WHERE STUDENT_ID = :StudentId AND OFFERING_ID IN (
                    SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                )",
                new { StudentId = studentId, SectionId = sectionId, Grade = grade, Percentage = percentage });
        }
    }
}
