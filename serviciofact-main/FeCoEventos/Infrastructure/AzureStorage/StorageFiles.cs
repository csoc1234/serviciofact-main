using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Infrastructure.AzureStorage
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

        public StorageFileResponse GetFile(string filePath, string fileName, string storageNameConfiguration, ILogAzure log)
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

                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Mensaje, LevelMsn.Info, timeT.ElapsedMilliseconds);

                return response;
            }
            catch (Exception eLog)
            {

                response.Code = 500;
                response.Message = eLog.Message;

                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", LogAzure.ConvertToJson(eLog), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage SaveFile(byte[] filebyte, string filePath, string fileName, string storageNameConfiguration, ILogAzure log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.UploadFile(storageNameConfiguration, filebyte, filePath, fileName);

                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Mensaje, LevelMsn.Info, timeT.ElapsedMilliseconds);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", LogAzure.ConvertToJson(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage DeleteFile(string filePath, string fileName, string storageNameConfiguration, ILogAzure log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.DeleteFile(storageNameConfiguration, filePath, fileName);

                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Mensaje, LevelMsn.Info, timeT.ElapsedMilliseconds);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", LogAzure.ConvertToJson(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }

        public ResponseBaseStorage RenameFile(string filePath, string fileName, string newFileName, string storageNameConfiguration, ILogAzure log)
        {
            ResponseBaseStorage response = new ResponseBaseStorage();
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var result = _fileShare.RenameFile(storageNameConfiguration, filePath, fileName, newFileName);

                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Mensaje, LevelMsn.Info, timeT.ElapsedMilliseconds);

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
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", LogAzure.ConvertToJson(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);
                return response;
            }
        }
    }
}
