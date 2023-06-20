using System;
using System.Collections.Generic;

namespace FeCoEventos.Models.Responses
{
    public class EventsPendingResponse
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public List<EventsPendingList> ListEvents { get; set; }
    }

    public partial class EventsPendingList
    {
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string InvoiceUuid { get; set; }
        public string EventType { get; set; }
        public string Event_id { get; set; }
        public string EventUuid { get; set; }
        public int Status { get; set; }
        public string PathFile { get; set; }
        public string NameFile { get; set; }
        public string TrackId { get; set; }
        public string Xml { get; set; }
        public string Hash { get; set; }
        public int TriesSend { get; set; }
        public int Environment { get; set; }
        public string SupplierIdentification { get; set; }
        public string SupplierTypeIdentification { get; set; }
        public int TryQuery { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
