namespace APIGetValidDocs.Domain.Entity
{
    public class InvoiceValidate : InvoiceBase
    {
        public string DatePayment { get; set; }

        public string TypeIdentificationSupplier { get; set; }

        public string NumberIdentificationSupplier { get; set; }

        public int IdEnterprise { get; set; }
    }
}
