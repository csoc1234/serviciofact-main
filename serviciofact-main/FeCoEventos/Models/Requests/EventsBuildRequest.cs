using DocumentBuildCO.DocumentClass.UBL2._1;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace FeCoEventos.Models.Requests
{
    public class EventsBuildRequest
    {
        [SwaggerSchema("Archivo Xml")]
        public string XmlBase64 { get; set; }

        [SwaggerSchema("Codigo de Evento")]
        public string EventCode { get; set; }

        [SwaggerSchema("Numero de Documento")]
        public string CorrelativeNumber { get; set; }

        [SwaggerSchema("Codigo de Rechazo")]
        public string RejectedCode { get; set; }

        public DocumentBuildCO.DocumentClass.UBL2._1.IssuerParty IssuerParty { get; set; }

        public DocumentBuildCO.DocumentClass.UBL2._1.CustomTagGeneral CustomTagGeneral { get; set; }

        public SenderParty SenderParty { get; set; }

        public ReceiverParty ReceiverParty { get; set; }

        //public DocumentBuildCO.DocumentClass.UBL2._1.PartyTaxSchemeAR PartyTaxSchemeARIssuerParty { get; set; }

        [SwaggerSchema("Certificado")]
        public Certificate Certificate { get; set; }

        [SwaggerSchema("Correo Electronico")]
        public string EmailAddress { get; set; }

        [SwaggerSchema("Notas del evento")]
        public List<string> Note { get; set; }

        [SwaggerSchema("Notas del evento")]
        public int? Environment { get; set; }

        [SwaggerSchema("Producto que genera el evento")]
        public int? CreatedBy { get; set; }

        public DocumentReferenceAR DocumentReference { get; set; }

        public string CustomizationID { get; set; }

        public string CustomizationIDSchemeID { get; set; }

        public string EffectiveDate { get; set; }

        public DocumentResponseMandato DocumentResponseMandato { get; set; }

        public List<LineResponse> LineResponse { get; set; }
    }

    public class Certificate
    {
        [SwaggerSchema("Numero de Identificacion")]
        public string Identification { get; set; }

        [SwaggerSchema("Tipo de Nombre")]
        public byte? TypeName { get; set; }

        [SwaggerSchema("Nombre de Archivo")]
        public string NameFile { get; set; }
    }
}