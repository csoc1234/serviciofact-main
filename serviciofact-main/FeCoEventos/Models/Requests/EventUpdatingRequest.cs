using System;

namespace FeCoEventos.Models.Requests
{
    public class EventUpdatingRequest
    {
        public string DocumentId { get; set; }

        public Int16 Status { get; set; }

        public bool Active { get; set; }

        public Int16 DianStatus { get; set; }

        public string DianMessage { get; set; }

        public string DianResponseDateTime { get; set; }

        public string DianResultValidation { get; set; }

        public string DianApplicationResponse { get; set; }

        public int TriesSend { get; set; }

        public Byte TryQuery { get; set; }

        public int? ReceptionNewStatus { get; set; }
    }
}
