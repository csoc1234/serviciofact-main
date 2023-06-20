using FeCoEventos.Util.TableLog;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Infrastructure.AzureStorage.Interface
{
    public interface IStorageFiles
    {
        StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration, ILogAzure log);

        ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration, ILogAzure log);

        ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration, ILogAzure log);

        ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration, ILogAzure log);
    }
}
