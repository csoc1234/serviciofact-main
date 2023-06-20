using System;

namespace FeCoEventos.Domain.Entity
{
    public class EventSummary
    {
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string InvoiceUuid { get; set; }
        public string EventType { get; set; }
        public string EventId { get; set; }
        public string EventUuid { get; set; }
        public bool Active { get; set; }
        public Int16 Status { get; set; }
        public Int16? DianStatus { get; set; }
        public string TrackId { get; set; }
    }
}
