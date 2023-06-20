namespace APIGetValidDocs.Application.Dto
{
    public class ValidDocsResponseDto
    {
        public string XML { get; set; }
        public string DocumentId { get; set; }
        public string Cufe { get; set; }
        public DateTime IssueDate { get; set; }
        public double PayableAmount { get; set; }
        public List<string> EventCode { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
