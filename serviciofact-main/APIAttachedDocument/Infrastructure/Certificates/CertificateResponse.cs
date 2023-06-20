using APIAttachedDocument.Domain.Entity;
using Newtonsoft.Json;

namespace APIAttachedDocument.Infrastructure
{
    public class CertificateResponse
    {
        [JsonProperty("codigo")]
        public int Code;

        [JsonProperty("mensaje")]
        public string Message;

        [JsonProperty("certificado")]
        public Certificate Certificate { get; set; }
    }
}
