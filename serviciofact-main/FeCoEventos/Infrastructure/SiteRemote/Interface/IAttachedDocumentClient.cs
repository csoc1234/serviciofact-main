using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface IAttachedDocumentClient
    {
        AttachedDocumentResponse GenerateXml(AttachedDocumentRequest requestFile, string tokenJwt, ILogAzure log);

        AttachedDocumentResponse GenerateXmlByIdentification(AttachedDocumentRequest requestFile, int idEnterprise, string identification, ILogAzure log);
    }
}
