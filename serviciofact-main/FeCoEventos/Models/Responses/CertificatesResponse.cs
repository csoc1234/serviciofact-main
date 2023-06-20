using FeCoEventos.Domain.Entity;
using Newtonsoft.Json;

namespace FeCoEventos.Models.Responses
{
    public class CertificatesResponse : ResponseBase
    {
        [JsonProperty("codigo")]
        public int Code;

        [JsonProperty("mensaje")]
        public string Message;

        [JsonProperty("certificado")]
        public Certificate Certificate { get; set; }
    }
}
