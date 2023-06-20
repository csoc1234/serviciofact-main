namespace WebApi.Infrastructure.Data.Context
{
    public class Invoice21Table
    {
        public int Id { get; set; }
        public int IdEnterprise { get; set; }
        public string DocumentId { get; set; }
        public string Uuid { get; set; }
        public string TrackId { get; set; }
        public string PathFile { get; set; }
        public string NameFile { get; set; }
    }
}