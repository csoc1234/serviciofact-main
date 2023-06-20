using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using TFHKA.EventsDian.Infrastructure.Data.Context;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Core
{
    public class DownloadFileDomain : IDownloadFileDomain
    {
        private readonly IApplicationDbContext _context;
        private readonly IStorageFiles _storageFiles;
        private readonly IFilesDomain _filesDomain;
        private readonly IConfiguration _configuration;

        public DownloadFileDomain(IApplicationDbContext context,
            IStorageFiles storageFiles,
            IFilesDomain filesDomain,
            IConfiguration configuration)
        {
            _context = context;
            _storageFiles = storageFiles;
            _filesDomain = filesDomain;
            _configuration = configuration;
        }

        public FileXmlResponse GetFile(string eventId, string trackId, int fileType, string tokenJwt, string identificationNumber, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                var eventRow = _context.GetEventStatus(eventId, trackId, _configuration, log).Result;

                if (eventRow == null)
                {
                    var result = new FileXmlResponse
                    {
                        Code = 204,
                        Message = "No se encontro ningun Evento de Factura con id = " + eventId + " y track Id = " + trackId
                    };

                    log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Message, LevelMsn.Warning, timeT.ElapsedMilliseconds);

                    return result;
                }

                //Busqueda de tipo de Archivo
                StorageFileResponse resultFile = new StorageFileResponse { };

                if (eventRow.Status == 200 || eventRow.Status == 201 || eventRow.Status == 99)
                {
                    switch (fileType)
                    {
                        case 1:
                            //Xml Event
                            resultFile = _storageFiles.GetFile(eventRow.PathFile, eventRow.NameFile, StorageConfiguration.FactoringFileShare, log);

                            break;
                        case 2:

                            if (eventRow.Status == 200 || eventRow.Status == 99)
                            {
                                //Xml Event                        
                                resultFile = _storageFiles.GetFile(eventRow.DianResultPathFile, eventRow.DianResultNameFile, StorageConfiguration.FactoringFileShare, log);

                                if (resultFile.Code != 200)
                                {
                                    //Intento buscarlo en la DIAN
                                    resultFile = _filesDomain.GetXmlValidationDian(eventRow.EventUuid, log);

                                    //Si existe lo guardo
                                    if (resultFile.Code == 200)
                                    {
                                        //Update DB
                                        InvoiceEventTable invoiceEventName = new InvoiceEventTable
                                        {
                                            event_id = eventId,
                                            track_id = trackId,
                                            dian_result_pathfile = eventRow.PathFile,
                                            dian_result_namefile = string.Format("{0}.xml", Guid.NewGuid().ToString()),
                                        };

                                        _context.UpdateFileResponseDian(invoiceEventName, _configuration, log);

                                        //Storage                                
                                        _storageFiles.SaveFile(Convert.FromBase64String(resultFile.File), invoiceEventName.dian_result_pathfile, invoiceEventName.dian_result_namefile, StorageConfiguration.FactoringFileShare, log);
                                    }
                                }
                            }
                            else
                            {
                                if (eventRow.Status == 201)
                                {
                                    return new FileXmlResponse
                                    {
                                        Code = eventRow.Status,
                                        Message = "Documento pendiente por entregar a la DIAN"
                                    };
                                }
                                else
                                {
                                    return new FileXmlResponse
                                    {
                                        Code = eventRow.Status,
                                        Message = "Documento no es valido"
                                    };
                                }
                            }

                            break;
                        case 3:
                            //Xml Attached Document

                            //Xml Event
                            var resultXmlEvent = _storageFiles.GetFile(eventRow.PathFile, eventRow.NameFile, StorageConfiguration.FactoringFileShare, log);

                            if (resultXmlEvent.Code != 200)
                            {
                                return new FileXmlResponse { Code = resultXmlEvent.Code, Message = resultXmlEvent.Message };
                            }

                            //Xml Dian
                            var resultXmlDian = _storageFiles.GetFile(eventRow.DianResultPathFile, eventRow.DianResultNameFile, StorageConfiguration.FactoringFileShare, log);

                            if (resultXmlDian.Code != 200)
                            {
                                return new FileXmlResponse { Code = resultXmlDian.Code, Message = resultXmlDian.Message };
                            }

                            //Genero de Attached Document
                            var requestAttachedDocument = new AttachedDocumentRequest
                            {
                                Xml = resultXmlEvent.File,
                                XmlDian = resultXmlDian.File
                            };
                            var resultFileAD = _filesDomain.BuildAttachedDocument(requestAttachedDocument, tokenJwt, identificationNumber, log);

                            if (resultFileAD.Code == 200)
                            {
                                //Update DB
                                InvoiceEventTable invoiceEvent = new InvoiceEventTable
                                {
                                    event_id = eventId,
                                    track_id = trackId,
                                    attacheddocument_pathfile = eventRow.PathFile,
                                    attacheddocument_namefile = string.Format("{0}.xml", Guid.NewGuid().ToString()),
                                };

                                _context.UpdateAttachedDocument(invoiceEvent, _configuration, log);
                            }

                            return resultFileAD;

                            break;
                    }

                    return _filesDomain.ParseResponse(resultFile, "ar", eventRow.SupplierIdentification);
                }
                else
                {
                    return new FileXmlResponse
                    {
                        Code = eventRow.Status,
                        Message = "Documento no es valido"
                    };
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return new FileXmlResponse
                {
                    Code = 500,
                    Message = "Error al intentar obtener la informacion del Evento de Factura"
                };
            }
        }

        public FileXmlResponse GetFileExternal(string uuid, string DocumentId, string EventType, int fileType, string identificationNumber, string tokenJwt, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            StorageFileResponse resultFile = new StorageFileResponse();

            try
            {
                switch (fileType)
                {
                    case 1:

                        resultFile = _filesDomain.GetXmlDian(uuid, log);
                        break;

                    case 2:

                        resultFile = _filesDomain.GetXmlValidationDian(uuid, log);
                        break;

                    case 3:

                        var resultXmlEvent = _filesDomain.GetXmlDian(uuid, log);

                        if (resultXmlEvent.Code != 200)
                        {
                            return new FileXmlResponse
                            {
                                Code = resultXmlEvent.Code,
                                Message = resultXmlEvent.Message,
                            };
                        }

                        var resultXmlDian = _filesDomain.GetXmlValidationDian(uuid, log);

                        if (resultXmlDian.Code != 200)
                        {
                            return new FileXmlResponse
                            {
                                Code = resultXmlDian.Code,
                                Message = resultXmlDian.Message,
                            };
                        }

                        //Genero de Attached Document
                        var requestAttachedDocument = new AttachedDocumentRequest
                        {
                            Xml = resultXmlEvent.File,
                            XmlDian = resultXmlDian.File
                        };

                        var resultFileAD = _filesDomain.BuildAttachedDocument(requestAttachedDocument, tokenJwt, identificationNumber, log);

                        return resultFileAD;
                        break;
                }

                return _filesDomain.ParseResponse(resultFile, "ar", identificationNumber);
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return new FileXmlResponse
                {
                    Code = 500,
                    Message = "Error al intentar obtener la informacion del Evento de Factura"
                };
            }
        }


    }
}
