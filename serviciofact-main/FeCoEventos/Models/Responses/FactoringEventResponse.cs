using System;
using System.Text.Json.Serialization;

namespace FeCoEventos.Models.Responses
{
    public class FactoringEventResponse : ResponseBase
    {
        public FactoringEvent Event { get; set; }

        public partial class FactoringEvent
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("documentId")]
            public string DocumentId { get; set; }

            [JsonPropertyName("invoiceUUID")]
            public string InvoiceUUID { get; set; }

            [JsonPropertyName("eventType")]
            public string EventType { get; set; }

            [JsonPropertyName("eventUUID")]
            public string EventUUID { get; set; }

            [JsonPropertyName("active")]
            public bool Active { get; set; }

            [JsonPropertyName("status")]
            public Int16 Status { get; set; }

            [JsonPropertyName("eventPathFile")]
            public string EventPathFile { get; set; }

            [JsonPropertyName("eventNameFile")]
            public string EventNameFile { get; set; }

            [JsonPropertyName("eventXML")]
            public string EventXML { get; set; }

            [JsonPropertyName("eventHash")]
            public string EventHash { get; set; }

            [JsonPropertyName("triesSend")]
            public int TriesSend { get; set; }

            [JsonPropertyName("dianStatus")]
            public Int16? DianStatus { get; set; }

            [JsonPropertyName("dianMessage")]
            public string DianMessage { get; set; }

            [JsonPropertyName("dianEventResult")]
            public string DianEventResult { get; set; }

            [JsonPropertyName("dianResponseDatetime")]
            public DateTime? DianResponseDatetime { get; set; }

            [JsonPropertyName("dianResultValidation")]
            public string DianResultValidation { get; set; }

            [JsonPropertyName("dianResultPathFile")]
            public string DianResultPathFile { get; set; }

            [JsonPropertyName("dianResultNameFile")]
            public string DianResultNameFile { get; set; }

            [JsonPropertyName("dianXML")]
            public string DianXML { get; set; }

            [JsonPropertyName("dianHash")]
            public string DianHash { get; set; }

            [JsonPropertyName("environment")]
            public int Environment { get; set; }

            [JsonPropertyName("createdBy")]
            public Int16? CreatedBy { get; set; }
        }
    }
}
