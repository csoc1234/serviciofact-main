using DocumentBuildCO.Request;
using FeCoEventos.Clients.SendEmail;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface IEmailClient
    {
        SendEmailInternalResponse Send(ParamSendEmail inputSendEmail, AttachedDocumentResponse attachedDocument, Ambiente enviroment, ILogAzure log);
    }
}
