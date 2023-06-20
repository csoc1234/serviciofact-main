using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Client.Models;
using WebApi.Infrastructure.AzureStorage.Interface;
using TFHKA.LogsMongo;

namespace WebApi.Infrastructure.AzureStorage
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

        public StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration, ILogMongo log)
        {
            StorageFileResponse response = new StorageFileResponse();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.GetFile(storageNameConfiguration, filePath, fileName);

                response = new StorageFileResponse
                {
                    Code = result.Codigo,
                    Message = result.Mensaje
                };

                if (result.Archivo != null)
                {
                    response.File = Convert.ToBase64String(result.Archivo);
                }

                return response;
            }
            catch (Exception eLog)
            {
                response.Code = 500;
                response.Message = eLog.Message;
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(eLog), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration, ILogMongo log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.UploadFile(storageNameConfiguration, filebyte, filePath, fileName);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration, ILogMongo log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.DeleteFile(storageNameConfiguration, filePath, fileName);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration, ILogMongo log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.RenameFile(storageNameConfiguration, filePath, fileName, newFileName);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }
    }
}
