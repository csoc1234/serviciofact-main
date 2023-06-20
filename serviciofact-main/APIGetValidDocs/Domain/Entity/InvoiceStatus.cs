namespace APIGetValidDocs.Domain.Entity
{
    public class InvoiceStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Valid { get; set; }
        public List<string> EventCode { get; set; }
    }
}
