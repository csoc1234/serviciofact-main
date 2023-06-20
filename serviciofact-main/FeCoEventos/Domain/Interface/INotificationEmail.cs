using FeCoEventos.Domain.Entity;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Interface
{
    public interface INotificationEmail
    {
        ResponseBase Send(string eventId, string trackId, AttachedDocumentResponse attachedDocument, StorageFileResponse storageFile, Event eventRow, ILogAzure log);
    }
}
