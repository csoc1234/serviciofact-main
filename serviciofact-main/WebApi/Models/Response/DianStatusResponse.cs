using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Response
{
    public class DianStatusResponse: ResponseBase
    {
        [JsonProperty("codigo")]
        public int Code;

        [JsonProperty("mensaje")]
        public String Message;

        [JsonProperty("mensajeExcepcion")]
        public String messageException { get; set; }

        [JsonProperty("TFHKAErrores")]
        public List<String> TFHKAErrors { get; set; }

        [JsonProperty("DIANErrores")]
        public List<String> DIANErrors { get; set; }

        [JsonProperty("DIANNotificaciones")]
        public List<String> DianNotifications { get; set; }

        [JsonProperty("estatusCodigo")]
        public String statusCode { get; set; }

        [JsonProperty("estatusDescripcion")]
        public String statusDescription { get; set; }

        [JsonProperty("estatusMensaje")]
        public String messageStatus { get; set; }

        [JsonProperty("esValido")]
        public Boolean isValid { get; set; }

        [JsonProperty("nombreArchivo")]
        public String filename { get; set; }

        [JsonProperty("sesion")]
        public String session { get; set; }

        [JsonProperty("entregado")]
        public Boolean delivered { get; set; }

        public String UUID { get; set; }

        public String trackID { get; set; }

        [JsonProperty("DIANFechaHoraRespuesta")]
        public String DianDateTimeResponse { get; set; }

        public String applicationResponse { get; set; }

        [JsonProperty("codigoEmisor")]
        public String issuerCode { get; set; }
    }
}
