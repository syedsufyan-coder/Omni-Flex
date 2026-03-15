using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface IAnnouncementRepository
    {
        Task<IEnumerable<Announcement>> GetAllSystemWideAsync();
        Task<IEnumerable<Announcement>> GetBySectionAsync(string sectionId);
        Task<int> CreateAsync(Announcement announcement);
        Task<int> DeleteAsync(string announcementId);
        Task<int> TogglePinAsync(string announcementId);
    }
}
