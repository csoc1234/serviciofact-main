using FeCoEventos.Application.Dto;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TFHKA.EventsDian.Infrastructure.Data.Context;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Core
{
    public class EventUpdateDomain : IEventUpdateDomain
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IEmisionDbContext _emisionContext;
        private readonly IStorageFiles _storageFiles;
        private readonly INotificationEmail _notificationEmail;
        private readonly IConfiguration _configuration;
        private readonly IAttachedDocumentClient _attachedDocumentClient;

        public EventUpdateDomain(
            IApplicationDbContext dbContext,
            IEmisionDbContext emisionContext,
            IStorageFiles storageFiles,
            INotificationEmail notificationEmail,
            IConfiguration configuration,
            IAttachedDocumentClient attachedDocumentClient)
        {
            _dbContext = dbContext;
            _emisionContext = emisionContext;
            _storageFiles = storageFiles;
            _notificationEmail = notificationEmail;
            _configuration = configuration;
            _attachedDocumentClient = attachedDocumentClient;
        }

        public EventUpdatingResponse UpdateDianResult(string eventId, string trackId, EventUpdatingRequest eventBodyReq, ILogAzure log)
        {
            Entity.Event? eventRow = _dbContext.GetEventStatus(eventId, trackId, _configuration, log).Result;

            if (eventRow == null)
            {
                return new EventUpdatingResponse
                {
                    Code = 404,
                    Message = "Evento no encontrado"
                };
            }

            EventUpdatingResponse response = new EventUpdatingResponse();
            try
            {
                InvoiceEventTable? invoiceEventEntity = new InvoiceEventTable();
                invoiceEventEntity.event_id = eventId;
                invoiceEventEntity.document_id = eventBodyReq.DocumentId;
                invoiceEventEntity.status = eventBodyReq.Status;
                invoiceEventEntity.active = eventBodyReq.Active;
                invoiceEventEntity.dian_status = eventBodyReq.DianStatus;
                invoiceEventEntity.dian_message = eventBodyReq.DianMessage;
                invoiceEventEntity.dian_response_datetime = DateTime.Parse(eventBodyReq.DianResponseDateTime);
                invoiceEventEntity.track_id = trackId;
                invoiceEventEntity.dian_result_validation = eventBodyReq.DianResultValidation == null ? "" : eventBodyReq.DianResultValidation;
                invoiceEventEntity.tries_send = eventBodyReq.TriesSend;
                invoiceEventEntity.dian_result_pathfile = "";
                invoiceEventEntity.dian_result_namefile = "";
                invoiceEventEntity.try_query = eventBodyReq.TryQuery;

                //Default
                ResponseBaseStorage resultStorage = new ResponseBaseStorage { Code = 0 };
                if (!string.IsNullOrEmpty(eventBodyReq.DianApplicationResponse))
                {
                    invoiceEventEntity.dian_result_pathfile = eventRow.PathFile;
                    invoiceEventEntity.dian_result_namefile = Guid.NewGuid().ToString() + ".xml";

                    resultStorage = _storageFiles.SaveFile(Convert.FromBase64String(eventBodyReq.DianApplicationResponse), invoiceEventEntity.dian_result_pathfile, invoiceEventEntity.dian_result_namefile, StorageConfiguration.FactoringFileShare, log);
                }

                if (resultStorage.Code == 200 || resultStorage.Code == 0)
                {
                    int pin = _dbContext.SaveUpdateEventInvoice(invoiceEventEntity, 1, _configuration, log);

                    if (pin < 0)
                    {
                        response.Code = 500;
                        response.Message = "Error al intentar actualizar el Evento de Factura.";

                        return response;
                    }
                    else if (pin == 0)
                    {
                        response.Code = 204;
                        response.Message = "No se encontro ningun Evento de Factura con id = " + eventId + " activo";

                        return response;
                    }
                    else
                    {
                        if ((invoiceEventEntity.dian_status == 200 || invoiceEventEntity.dian_status == 0) && invoiceEventEntity.active)
                        {
                            if (!string.IsNullOrEmpty(eventRow.Email))
                            {
                                //Buscamos el xml Evento
                                StorageFileResponse? resultEventXml = _storageFiles.GetFile(eventRow.PathFile, eventRow.NameFile, StorageConfiguration.FactoringFileShare, log);

                                //Genero el Attached Document
                                AttachedDocumentRequest? requestAttachedDocument = new AttachedDocumentRequest
                                {

                                    Xml = resultEventXml.File,
                                    XmlDian = eventBodyReq.DianApplicationResponse
                                };
                                AttachedDocumentResponse? resultAttachedDocument = _attachedDocumentClient.GenerateXmlByIdentification(requestAttachedDocument, eventRow.IdEnterprise, eventRow.CustomerIdentification, log);

                                if (resultAttachedDocument.Code == 200)
                                {
                                    //Envio de Correo de Notificacion
                                    //Si es exitoso el evento ante la DIAN
                                    _notificationEmail.Send(eventId, trackId, resultAttachedDocument, resultEventXml, eventRow, log);
                                }
                                else
                                {
                                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "No se envia el correo por error en el AttachedDocument", LevelMsn.Warning);
                                }
                            }
                        }

                        //En el Appsetings de define la lista de codigos que si van a indicar que se actualice Recepion
                        string[]? updateReceptionCode = _configuration["Recepcion:UpdateReceptionCode"].Split(';');

                        List<string> listCodeAllowed = new List<string>();
                        listCodeAllowed.AddRange(updateReceptionCode);

                        if (listCodeAllowed.Contains(invoiceEventEntity.status.ToString()))
                        {

                            //Casos bordes de codigos para Recepcion
                            //Si es un rechazo de la DIAN se transforma a 99
                            //Si no es de la DIAN, 103
                            short codeResult;

                            List<int> listCodeStatusReception = new List<int> { 200, 201, 99, 103 };

                            if (listCodeStatusReception.Contains(invoiceEventEntity.status))
                            {
                                codeResult = invoiceEventEntity.status;
                            }
                            else
                            {
                                codeResult = 99;
                            }

                            if (eventRow.CreatedBy == 1)
                            {
                                pin = _emisionContext.UpdateInvoiceHistoryEvent(codeResult, eventId, trackId, true, _configuration, log);

                                if (eventBodyReq.ReceptionNewStatus >= 0)
                                {
                                    _emisionContext.UpdateReceptionStatus(eventRow.InvoiceUuid, eventBodyReq.ReceptionNewStatus.Value, _configuration);
                                }
                            }
                        }
                        else
                        {
                            pin = 201;
                        }

                        if (pin == 201)
                        {
                            response.Code = 201;
                            response.Message = "El evento con id = " + eventId + " fue actualizado; se omite la actualizacion a recepcion";

                            return response;
                        }
                        else if (pin < 0)
                        {
                            response.Code = 205;
                            response.Message = "El evento con id = " + eventId + " fue actualizado, aunque se produjo un fallo al actualizar la tabla de log, invoice_reception_history";

                            return response;
                        }
                        else if (pin == 0)
                        {
                            response.Code = 206;
                            response.Message = "El evento con id = " + eventId + " fue actualizado, aunque no se encontro ningun evento activo con ese id en la tabla de log, invoice_reception_history";

                            return response;
                        }
                        else
                        {
                            response.Code = 200;
                            response.Message = "El evento con id = " + eventId + " fue actualizado";

                            return response;
                        }
                    }
                }
                else
                {
                    response.Code = resultStorage.Code;
                    response.Message = resultStorage.Message;

                    return response;
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response.Code = 500;
                response.Message = "Error al intentar actualizar el Evento de Factura.";

                return response;
            }
        }


        public ResponseBase UpdateAsyncDelivery(EventDto eventData, EventDeliveryAsyncDto request, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                InvoiceEventTable? eventTable = new InvoiceEventTable
                {
                    event_id = eventData.EventId,
                    track_id = eventData.TrackId,
                    status = (short)request.Status,
                    //active = request.Status == 204 ? true : false
                };

                request.TrackIdDian = !string.IsNullOrEmpty(request.TrackIdDian) ? request.TrackIdDian : eventData.TrackId;

                int resultUpdate = _dbContext.UpdateEventAsyncSend(eventTable, request.TrackIdDian, _configuration, log);

                if (resultUpdate == 0)
                {
                    return new ResponseBase
                    {
                        Code = 404,
                        Message = "No se ha encontrado el evento a actualizar"
                    };
                }
                else
                {
                    return new ResponseBase
                    {
                        Code = 200,
                        Message = "Se actualiza el evento con exito"
                    };
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                ResponseBase response = new ResponseBase
                {
                    Code = 500,
                    Message = "Se genero error al momento de realizar la transaccion"
                };

                log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }
    }
}
