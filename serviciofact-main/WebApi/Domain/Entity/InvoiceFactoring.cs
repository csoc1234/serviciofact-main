using System;

namespace WebApi.Domain.Entity
{
    public class InvoiceFactoring
    {
        public int InvoiceId { get; set; }
        public string DocumentId { get; set; }
        public int EnterpriseId { get; set; }
        public int InvoiceNumber { get; set; }
        public string InvoiceUuid { get; set; }
        public string InvoiceUuidType { get; set; }
        public string InvoiceAuthorization { get; set; }
        public DateTime InvoiceIssuedate { get; set; }
        public string SupplierTypeIdentification { get; set; }
        public string SupplierIdentification { get; set; }
        public string CustomerTypeIdentification { get; set; }
        public string CustomerIdentification { get; set; }
        public bool Active { get; set; }
        public Int16 Status { get; set; }
        public string PathFileXml { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PayableAmount { get; set; }
    }
}