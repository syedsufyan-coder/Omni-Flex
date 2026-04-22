namespace OmniFlex.Models.Domain.Student
{
    public class AddComment
    {
        public long PostId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}