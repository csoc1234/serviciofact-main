using APIGetValidDocs.Infraestructure.Interface;
using System.Diagnostics;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Client.Models;

namespace APIGetValidDocs.Infraestructure.AzureStorage
{
    public class StorageFiles : IStorageFiles
    {
        private static IConfiguration _configuration;

        private static IFileShareClass _fileShare;

        public StorageFiles(IConfiguration configuration, IFileShareClass fileShare)
        {
            _configuration = configuration;
            _fileShare = fileShare;
        }

        public StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration)
        {
            StorageFileResponse response = new();
            Stopwatch timeT = new();
            timeT.Start();

            try
            {
                TFHKA.Storage.Fileshare.Models.FileSystemGetFileResponse result = _fileShare.GetFile(storageNameConfiguration, filePath, fileName);

                response = new StorageFileResponse
                {
                    Code = result.Codigo,
                    Message = result.Mensaje
                };

                if (result.Archivo != null)
                {
                    response.File = Convert.ToBase64String(result.Archivo);
                }

                timeT.Stop();

                return response;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;

                timeT.Stop();
                return response;
            }
        }

        public ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration)
        {
            ResponseBaseStorage response = new();
            Stopwatch timeT = new();
            timeT.Start();

            try
            {
                TFHKA.Storage.Fileshare.Models.FileSystemResponse result = _fileShare.UploadFile(storageNameConfiguration, filebyte, filePath, fileName);

                timeT.Stop();

                return new ResponseBaseStorage
                {
                    Code = result.Codigo,
                    Message = result.Mensaje
                };
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;

                timeT.Stop();
                return response;
            }
        }

        public ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration)
        {
            ResponseBaseStorage response = new();
            Stopwatch timeT = new();
            timeT.Start();

            try
            {
                TFHKA.Storage.Fileshare.Models.FileSystemResponse result = _fileShare.DeleteFile(storageNameConfiguration, filePath, fileName);

                timeT.Stop();

                return new ResponseBaseStorage
                {
                    Code = result.Codigo,
                    Message = result.Mensaje
                };
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;

                timeT.Stop();
                return response;
            }
        }

        public ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration)
        {
            ResponseBaseStorage response = new();
            Stopwatch timeT = new();
            timeT.Start();

            try
            {
                TFHKA.Storage.Fileshare.Models.FileSystemResponse result = _fileShare.RenameFile(storageNameConfiguration, filePath, fileName, newFileName);

                timeT.Stop();

                return new ResponseBaseStorage
                {
                    Code = result.Codigo,
                    Message = result.Mensaje
                };
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;

                timeT.Stop();
                return response;
            }
        }
    }
}
