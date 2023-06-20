using TFHKA.Storage.Fileshare.Client.Models;
using TFHKA.LogsMongo;

namespace WebApi.Infrastructure.AzureStorage.Interface
{
    public interface IStorageFiles
    {
        StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration, ILogMongo log);

        ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration, ILogMongo log);

        ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration, ILogMongo log);

        ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration, ILogMongo log);
    }
}
