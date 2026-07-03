using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly DbConnectionFactory _factory;

        public AnnouncementRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ANNOUNCEMENT_ID AS AnnouncementId,
            POSTED_BY       AS PostedBy,
            ''              AS SectionId,
            OFFERING_ID     AS OfferingId,
            TITLE           AS Title,
            CONTENT         AS Content,
            IS_PINNED       AS IsPinned,
            PRIORITY        AS Priority,
            POST_DATE       AS PostDate";

        public async Task<IEnumerable<Announcement>> GetAllSystemWideAsync()
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Announcement>(
                $"SELECT {SELECT_COLUMNS} FROM ANNOUNCEMENTS WHERE OFFERING_ID IS NULL ORDER BY POST_DATE DESC");
        }

        public async Task<IEnumerable<Announcement>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Announcement>(
                $@"SELECT {SELECT_COLUMNS} FROM ANNOUNCEMENTS 
                   WHERE OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )
                   ORDER BY POST_DATE DESC",
                new { SectionId = sectionId });
        }

        public async Task<int> CreateAsync(Announcement announcement)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO ANNOUNCEMENTS 
                (ANNOUNCEMENT_ID, POSTED_BY, OFFERING_ID, TITLE, CONTENT, 
                 IS_PINNED, PRIORITY, POST_DATE)
                VALUES 
                (:AnnouncementId, :PostedBy, :OfferingId, :Title, :Content,
                 :IsPinned, :Priority, :PostDate)", announcement);
        }

        public async Task<int> DeleteAsync(string announcementId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "DELETE FROM ANNOUNCEMENTS WHERE ANNOUNCEMENT_ID = :AnnouncementId",
                new { AnnouncementId = announcementId });
        }

        public async Task<int> TogglePinAsync(string announcementId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE ANNOUNCEMENTS SET IS_PINNED = CASE WHEN IS_PINNED = 1 THEN 0 ELSE 1 END WHERE ANNOUNCEMENT_ID = :AnnouncementId",
                new { AnnouncementId = announcementId });
        }
    }
}
