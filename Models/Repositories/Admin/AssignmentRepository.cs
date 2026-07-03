using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly DbConnectionFactory _factory;

        public AssignmentRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ASSIGNMENT_ID AS AssignmentId,
            ''            AS SectionId,
            OFFERING_ID   AS OfferingId,
            TITLE         AS Title,
            DESCRIPTION   AS Description,
            DUE_DATE      AS DueDate,
            CATEGORY      AS Category,
            TOTAL_MARKS   AS TotalMarks,
            ACTUAL_WTG    AS ActualWtg,
            CREATED_BY    AS CreatedBy,
            CREATED_AT    AS CreatedAt";

        public async Task<Assignment?> GetByIdAsync(string assignmentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Assignment>(
                $"SELECT {SELECT_COLUMNS} FROM ASSIGNMENTS WHERE ASSIGNMENT_ID = :AssignmentId",
                new { AssignmentId = assignmentId });
        }

        public async Task<IEnumerable<Assignment>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Assignment>(
                $@"SELECT {SELECT_COLUMNS} FROM ASSIGNMENTS 
                   WHERE OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )
                   ORDER BY DUE_DATE",
                new { SectionId = sectionId });
        }

        public async Task<int> CreateAsync(Assignment assignment)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO ASSIGNMENTS 
                (ASSIGNMENT_ID, OFFERING_ID, TITLE, DESCRIPTION, DUE_DATE, 
                 CATEGORY, TOTAL_MARKS, ACTUAL_WTG, CREATED_BY, CREATED_AT)
                VALUES 
                (:AssignmentId, :OfferingId, :Title, :Description, :DueDate,
                 :Category, :TotalMarks, :ActualWtg, :CreatedBy, :CreatedAt)", assignment);
        }

        public async Task<int> UpdateAsync(Assignment assignment)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE ASSIGNMENTS SET 
                TITLE = :Title, DESCRIPTION = :Description, DUE_DATE = :DueDate,
                CATEGORY = :Category, TOTAL_MARKS = :TotalMarks, ACTUAL_WTG = :ActualWtg
                WHERE ASSIGNMENT_ID = :AssignmentId", assignment);
        }

        public async Task<int> DeleteAsync(string assignmentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "DELETE FROM ASSIGNMENTS WHERE ASSIGNMENT_ID = :AssignmentId",
                new { AssignmentId = assignmentId });
        }
    }
}
