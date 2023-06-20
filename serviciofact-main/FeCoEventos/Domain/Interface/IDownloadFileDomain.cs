using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Domain.Interface
{
    public interface IDownloadFileDomain
    {
        FileXmlResponse GetFile(string eventId, string trackId, int fileType, string tokenJwt, string identificationNumber, ILogAzure log);

        FileXmlResponse GetFileExternal(string uuid, string DocumentId, string EventType, int fileType, string tokenJwt, string identificationNumber, ILogAzure log);
    }
}
