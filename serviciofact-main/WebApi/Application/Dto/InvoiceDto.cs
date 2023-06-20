using System;

namespace WebApi.Application.Dto
{
    public class InvoiceDto
    {
        public int IdTable { get; set; }
        public string DocumentId { get; set; }
        public int EnterpriseId { get; set; }        
        public string InvoiceUuid { get; set; }
        public string InvoiceUuidType { get; set; }        
        public DateTime InvoiceIssuedate { get; set; }
        public string SupplierTypeIdentification { get; set; }
        public string SupplierIdentification { get; set; }
        public string CustomerTypeIdentification { get; set; }
        public string CustomerIdentification { get; set; }
        public string PathFileXml { get; set; }
        public string NameFileXml { get; set; }
        public decimal PayableAmount { get; set; }
        public string PaymentMeansID { get; set; }
        public string PaymentMeansCode { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public DateTime? ValidateDianDatetime { get; set; }
    }
}