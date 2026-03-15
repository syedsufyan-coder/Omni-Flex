namespace OmniFlex.Models.Domain.Admin
{
    public class PostFile
    {
        public int PostFileId { get; set; }
        public string PostId { get; set; } = string.Empty;
        public int FileId { get; set; }
    }
}
