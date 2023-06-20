using System;

namespace FeCoEventos.Domain.Entity
{
    public class Event
    {
        public int Id { get; set; }
        public int IdEnterprise { get; set; }
        public string DocumentId { get; set; }
        public string InvoiceUuid { get; set; }
        public string SupplierTypeIdentification { get; set; }
        public string SupplierIdentification { get; set; }
        public string CustomerTypeIdentification { get; set; }
        public string CustomerIdentification { get; set; }
        public string EventType { get; set; }
        public string EventId { get; set; }
        public string EventUuid { get; set; }
        public bool Active { get; set; }
        public Int16 Status { get; set; }
        public Int16? DianStatus { get; set; }
        public string DianMessage { get; set; }
        public DateTime? DianResponseDatetime { get; set; }
        public string PathFile { get; set; }
        public string NameFile { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string TrackId { get; set; }
        public string DianResultValidation { get; set; }
        public string DianResultPathFile { get; set; }
        public string DianResultNameFile { get; set; }
        public int triesSend { get; set; }
        public string Email { get; set; }
        public int Environment { get; set; }
        public Int16? CreatedBy { get; set; }
    }
}
