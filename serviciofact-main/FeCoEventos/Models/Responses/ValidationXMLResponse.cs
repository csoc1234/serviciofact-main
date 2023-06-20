using Newtonsoft.Json;

namespace FeCoEventos.Models.Responses
{
    public class ValidationXMLResponse : ResponseBase
    {
        public string ApplicationResponse { get; set; }

        [JsonProperty("codigo")]
        public int Code { get; set; }

        [JsonProperty("mensaje")]
        public string Message { get; set; } 
        
        [JsonProperty("estatusDescripcion")]
        public string EstatusDescripcion { get; set; }



    }
}
