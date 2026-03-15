namespace OmniFlex.Models.Domain.Admin
{
    public class Semester
    {
        public string SemesterId { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IsCurrent { get; set; }
    }
}
