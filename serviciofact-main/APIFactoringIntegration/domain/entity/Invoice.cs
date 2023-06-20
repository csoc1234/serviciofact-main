namespace APIFactoringIntegration.Domain.Entity
{
    public class Invoice
    {
        public string Xml { get; set; }
        public string DocumentId { get; set; }
        public string Cufe { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal PayableAmount { get; set; }
        public List<string> EventCode { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
