using DocumentBuildCO.ClassXSD;
using DocumentBuildCO.DocumentClass.UBL2._1;
using DocumentBuildCO.Models.Request;
using DocumentBuildCO.Response;
using FeCoEventos.Models.Requests;

namespace FeCoEventos.Domain.Interface
{
    public interface IDocumentBuild
    {
        string GetInvoiceXml(string env, string xml);

        BaseDocument21 SerializeDocument(string env, string xmlInvoicePlain);

        ValidateXMLDocumentResponse ValidateXSD(string env, string xml);

        ApplicationResponseType SerializeApplicationResponse(string xml);

        DocumentBuildCO.Response.BuildResponse BuildApplicationResponse(string env, EventsBuildRequest eventRequest);

        DocumentBuildCO.Request.Ambiente GetEnvironmentDocumentBuild(string env);

        string GetEventName(string eventCode);

        string BOMFomartXml(string xml);

        BuildResponse BuildXML(string xml, ApplicationResponseInformationRequest request);

        BuildResponse BuildXML21(string xml, ApplicationResponseInformationRequest request);
    }
}
