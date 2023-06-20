namespace APIValidateEvents.Application.Dto
{
    public class InvoiceDto
    {
        public string Xml { get; set; }

        public string Cufe { get; set; }

        public string DocumentId { get; set; }

        public string DatePayment { get; set; }

        public string TypeIdentificationSupplier { get; set; }

        public string NumberIdentificationSupplier { get; set; }

        public int IdEnterprise { get; set; }
    }
}
