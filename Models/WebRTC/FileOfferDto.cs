namespace Whispr.Models.WebRTC
{
    public class FileOfferDto
    {
        public string Type { get; set; }
        public required string Name { get; set; }
        public required long Size { get; set; }
        public required string MimeType { get; set; }
        public required int TotalChunks { get; set; }
    }
}
