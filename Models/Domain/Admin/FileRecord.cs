namespace OmniFlex.Models.Domain.Admin
{
    // Named FileRecord to avoid conflict with System.IO.File
    public class FileRecord
    {
        public int FileId { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
