using System;

namespace WebApi.Domain.Models
{
    public class InvoiceDocumentResponse
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public bool IsValid { get; set; }

        public string DocumentId { get; set; }

        public string InvoiceUuid { get; set; }

        public string XmlApplicationResponse { get; set; }

        public string SupplierIdentification { get; set; }

        public string SupplierTypeIdentification { get; set; }

        public string CustomerIdentification { get; set; }

        public string CustomerTypeIdentification { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
