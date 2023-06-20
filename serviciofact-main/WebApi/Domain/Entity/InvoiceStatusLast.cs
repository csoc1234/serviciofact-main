using System;

namespace WebApi.Domain.Entity
{
    public class InvoiceStatusLast
    {
        public int Id { get; set; }
        public string? InvoiceDocumentId { get; set; }
        public string? InvoiceUuid { get; set; }

        public string InvoiceSupplierIdentification { get; set; }

        public string InvoiceSupplierTypeIdentification { get; set; }

        public string InvoiceCustomerIdentification { get; set; }

        public string InvoiceCustomerTypeIdentification { get; set; }

        public Int16 InvoiceStatus { get; set; }

        public DateTime InvoiceCreatedAt { get; set; }

        public DateTime InvoiceUpdatedAt { get; set; }

        public string EventId { get; set; }

        public string EventUuid { get; set; }

        public string EventType { get; set; }

        public DateTime EventDatetime { get; set; }

        public Int16 DianStatus { get; set; }

        public string DianMessage { get; set; }

        public string SupplierTypeIdentification { get; set; }

        public string SupplierIdentification { get; set; }

        public string CustomerTypeIdentification { get; set; }

        public string CustomerIdentification { get; set; }

        public Int16 Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string SupplierRegistrationName { get; set; }
        public string CustomerRegistrationName { get; set; }
        public string ReferenceId { get; set; }
        public string EventUuidType { get; set; }
    }
}
