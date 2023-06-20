using TFHKA.Storage.Fileshare.Client.Models;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Domain.Interface
{
    public interface IInvoiceDomain
    {
        ValidateXmlResponse ValidateDocument(string xml);

        StorageFileResponse FindXMLFileShare(string pathFile, string nameFile, ILogMongo log);
    }
}
