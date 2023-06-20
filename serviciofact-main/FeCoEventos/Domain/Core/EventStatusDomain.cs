using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using static FeCoEventos.Models.Responses.FactoringEventResponse;

namespace FeCoEventos.Domain.Core
{
    public class EventStatusDomain : IEventStatusDomain
    {
        private readonly IApplicationDbContext _context;
        private readonly IStorageFiles _storageFiles;
        private readonly IDocumentBuild _documentBuild;
        private readonly IConfiguration _configuration;

        public EventStatusDomain(IApplicationDbContext context, IStorageFiles storageFiles, IDocumentBuild documentBuild, IConfiguration configuration)
        {
            _context = context;
            _storageFiles = storageFiles;
            _documentBuild = documentBuild;
            _configuration = configuration;
        }

        public FactoringEventResponse GetStatus(string eventId, string trackId, ILogAzure log)
        {
            FactoringEventResponse response = new FactoringEventResponse();

            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                System.Threading.Tasks.Task<Entity.Event>? factoringEvent = _context.GetEventStatus(eventId, trackId, _configuration, log);

                if (factoringEvent.Result == null)
                {
                    response.Code = 204;
                    response.Message = "No se encontro ningun Evento de Factura con id = " + eventId + " y track Id = " + trackId;
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Warning, timeT.ElapsedMilliseconds);
                    return response;
                }
                else
                {
                    TFHKA.Storage.Fileshare.Client.Models.StorageFileResponse? storageEventfile = _storageFiles.GetFile(factoringEvent.Result.PathFile, factoringEvent.Result.NameFile, StorageConfiguration.FactoringFileShare, log);

                    if (storageEventfile.Code == 200)
                    {
                        response.Code = 200;
                        response.Message = "Se retorna el evento";

                        response.Event = new FactoringEvent
                        {
                            Id = factoringEvent.Result.Id,

                            DocumentId = factoringEvent.Result.DocumentId,
                            InvoiceUUID = factoringEvent.Result.InvoiceUuid,

                            EventType = factoringEvent.Result.EventType,
                            EventUUID = factoringEvent.Result.EventUuid,
                            Active = factoringEvent.Result.Active,
                            Status = factoringEvent.Result.Status,
                            EventPathFile = factoringEvent.Result.PathFile,
                            EventNameFile = factoringEvent.Result.NameFile,
                            EventXML = storageEventfile.File,
                            EventHash = StringUtilies.GetSHA1(storageEventfile.File),

                            TriesSend = factoringEvent.Result.triesSend,

                            DianStatus = factoringEvent.Result.DianStatus,
                            DianMessage = factoringEvent.Result.DianMessage,
                            DianResponseDatetime = factoringEvent.Result.DianResponseDatetime,
                            DianResultValidation = factoringEvent.Result.DianResultValidation,
                            DianResultPathFile = factoringEvent.Result.DianResultPathFile,
                            DianResultNameFile = factoringEvent.Result.DianResultNameFile,
                            Environment = factoringEvent.Result.Environment,
                            CreatedBy = factoringEvent.Result.CreatedBy
                        };

                        TFHKA.Storage.Fileshare.Client.Models.StorageFileResponse? storageDianfile = _storageFiles.GetFile(factoringEvent.Result.DianResultPathFile, factoringEvent.Result.DianResultNameFile, StorageConfiguration.FactoringFileShare, log);

                        if (storageDianfile.Code == 200)
                        {
                            response.Event.DianXML = storageDianfile.File;
                            response.Event.DianHash = StringUtilies.GetSHA1(storageDianfile.File);

                            //Extraccion del mensaje de resultado del evento
                            try
                            {
                                storageDianfile.File = _documentBuild.BOMFomartXml(storageDianfile.File);

                                DocumentBuildCO.ClassXSD.ApplicationResponseType? serializeApplicationResponse = _documentBuild.SerializeApplicationResponse(StringUtilies.Base64Decode(storageDianfile.File));

                                response.Event.DianEventResult = serializeApplicationResponse.DocumentResponse[0].LineResponse[1].Response[0].Description[0].Value;
                            }
                            catch (Exception ex)
                            {
                                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error);
                                response.Event.DianEventResult = null;
                            }
                        }

                        response.Event.DianEventResult = response.Event.DianEventResult == null ? response.Event.DianMessage : response.Event.DianEventResult;

                        log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info, timeT.ElapsedMilliseconds);
                        return response;
                    }
                    else
                    {
                        response.Code = 500;
                        response.Message = "Error al intentar obetener el Evento de Factura.";
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Error, timeT.ElapsedMilliseconds);

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = "Error al intentar obetener el Evento de Factura.";
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return response;
            }

        }

        public FactoringEventResponse GetStatusByUuid(string eventId, string eventuuid, ILogAzure log)
        {
            FactoringEventResponse response = new FactoringEventResponse();

            try
            {
                //Consulta en DB
                Entity.EventCount? result = _context.GetInvoiceEventsReadByEventUuid(eventId, eventuuid, _configuration, log).Result;

                //Analisis de resultado
                if (result != null)
                {
                    if (result.Id > 0)
                    {
                        response = new FactoringEventResponse
                        {
                            Code = 200,
                            Message = String.Format("El evento {0} ha sido encontrado", eventId)
                        };
                    }
                    else
                    {
                        response = new FactoringEventResponse
                        {
                            Code = 404,
                            Message = String.Format("El evento {0} no ha sido encontrado", eventId)
                        };
                    }
                }
                else
                {
                    response = new FactoringEventResponse
                    {
                        Code = 103,
                        Message = String.Format("Error no se ha logrado encontrar el evento")
                    };
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response = new FactoringEventResponse
                {
                    Code = 500,
                    Message = "Error al momento de consulta el evento"
                };
            }

            return response;
        }
    }
}
