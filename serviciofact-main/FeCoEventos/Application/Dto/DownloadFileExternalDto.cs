namespace FeCoEventos.Application.Dto
{
    public class DownloadFileExternalDto
    {
        public string uuid { get; set; }

        public string DocumentId { get; set; }

        public string EventType { get; set; }

        public int FileType { get; set; }
    }
}
