using APIValidateEvents.Common;

namespace APIValidateEvents.Domain.Entity
{
    public class InvoiceStatusDian : ResponseBase
    {
        public int InvoiceStatusCode { get; set; }

        public string InvoiceStatusDesc { get; set; }

        public InvoiceStatus StatusDian { get; set; }

        public List<EventsDocumentResponse> Events { get; set; }

        public bool TieneEventos { get; set; }

        public int CantidadEventos { get; set; }

        public bool EsRecibida { get; set; }

        public bool EsReclamada { get; set; }

        public bool EsBienoServicio { get; set; }

        public bool EsAceptada { get; set; }

        public bool EsAceptadaTacitamente { get; set; }

        public bool EsTituloValor { get; set; }

        public bool EstaEndosada { get; set; }
    }

    public class InvoiceStatus
    {
        public int Status { get; set; }

        public bool IsValid { get; set; }

        public string Uuid { get; set; }

        public string DocumentId { get; set; }

        public string InvoiceSupplierIdentification { get; set; }

        public string InvoiceSupplierTypeIdentification { get; set; }

        public string InvoiceCustomerIdentification { get; set; }

        public string InvoiceCustomerTypeIdentification { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
