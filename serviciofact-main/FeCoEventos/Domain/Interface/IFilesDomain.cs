using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Interface
{
    public interface IFilesDomain
    {
        StorageFileResponse GetXmlDian(string uuid, ILogAzure log);

        StorageFileResponse GetXmlValidationDian(string uuid, ILogAzure log);

        FileXmlResponse ParseResponse(StorageFileResponse resultFile, string prefix, string supplierIdentification);

        FileXmlResponse BuildAttachedDocument(AttachedDocumentRequest request, string tokenJwt, string supplierIdentification, ILogAzure log);

        string GetNameFile(string prefix, string SupplierIdentification, string fileXml);
    }
}
