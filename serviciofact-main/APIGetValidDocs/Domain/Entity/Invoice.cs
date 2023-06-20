namespace APIGetValidDocs.Domain.Entity
{
    public class Invoice : InvoiceBase
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime IssueDate { get; set; }

        public decimal PayableAmount { get; set; }

        public List<string> EventCode { get; set; }
    }
}
