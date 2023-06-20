using System.Collections.Generic;

namespace FeCoEventos.Models.Responses
{
    public class EventsSummaryResponse : ResponseBase
    {
        public List<EventSummary> Events { get; set; }
    }

    public class EventSummary
    {
        public string CodeEvent { get; set; }

        public Summary Summary { get; set; }

        public List<EventItem> List { get; set; }
    }

    public class Summary
    {
        public int Success { get; set; }

        public int Error { get; set; }

        public int Rejected { get; set; }

        public int Pending { get; set; }
    }

    public class EventItem
    {
        public string InvoiceId { get; set; }

        public string EventId { get; set; }

        public string InvoiceCufe { get; set; }

        public string EventCufe { get; set; }

        public string TrackId { get; set; }

        public int Status { get; set; }

        public int StatusDian { get; set; }

        public string Type { get; set; }
    }
}
