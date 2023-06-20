using TFHKA.Storage.Fileshare.Client.Models;

namespace APIGetValidDocs.Infraestructure.Interface
{
    public interface IStorageFiles
    {
        StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration);

        ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration);

        ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration);

        ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration);
    }
}
