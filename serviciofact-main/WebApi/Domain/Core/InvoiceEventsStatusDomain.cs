using DocumentBuildCO.ClassXSD;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using WebApi.Domain.Interface;
using WebApi.Domain.Models;
using WebApi.Infrastructure.ComunicationDian;
using TFHKA.LogsMongo;
using WebApi.Models.Response;


namespace WebApi.Domain.Core
{
    public class InvoiceEventsStatusDomain : IInvoiceEventsStatusDomain
    {
        private readonly IDianStatusRestClient _dianStatusRestClient;
        private readonly ICheckStatusEventsDomain _checkStatusEvents;

        public InvoiceEventsStatusDomain(IDianStatusRestClient dianStatusRestClient, ICheckStatusEventsDomain checkStatusEvents)
        {
            _dianStatusRestClient = dianStatusRestClient;
            _checkStatusEvents = checkStatusEvents;
        }

        public Entity.InvoiceEventsStatusDian GetEventAppResponse(string cufe, ILogMongo log)
        {
            Entity.InvoiceEventsStatusDian response = new Entity.InvoiceEventsStatusDian();
            response.Invoice = new InvoiceDocumentResponse();

            try
            {
                DianStatusResponse responseStatus = _dianStatusRestClient.GetDianStatus(cufe, log);

                if (responseStatus != null)
                {
                    if (responseStatus.Code == 200 && responseStatus.statusCode == "00")
                    {
                        //Si existe la factura
                        response.Invoice.StatusDescription = responseStatus.messageStatus;
                        response.Invoice.StatusCode = 200;
                        response.Invoice.IsValid = responseStatus.isValid;

                        response.Code = 200;
                        response.Message = "La factura es valida en la DIAN";

                        try
                        {
                            var responseStatusEvents = _dianStatusRestClient.GetStatusEvent(cufe, log);

                            if (responseStatusEvents != null)
                            {
                                if (responseStatusEvents.Code == 200 && responseStatusEvents.statusCode == "00")
                                {
                                    //case con eventos
                                    //response.StatusDian.StatusCodeEvents = 200;
                                    //response.StatusDian.StatusEvents = responseStatusEvents.statusDescription;
                                    response.Invoice.XmlApplicationResponse = responseStatusEvents.applicationResponse;

                                    response.Code = 200;
                                    response.Message = "La factura es valida en la DIAN y posee eventos";
                                }
                                else if (responseStatusEvents.Code == 67)
                                {
                                    //response.StatusDian.StatusCodeEvents = Int32.Parse(responseStatusEvents.statusCode);
                                    //response.StatusDian.StatusEvents = responseStatusEvents.statusDescription;

                                    response.Code = 205;
                                    response.Message = "La factura es valida en la DIAN pero no tiene eventos";
                                }
                                else
                                {
                                    //response.StatusDian.StatusCodeEvents = Int32.Parse(responseStatusEvents.statusCode);
                                    //response.StatusDian.StatusEvents = responseStatusEvents.statusDescription;

                                    response.Code = Int32.Parse(responseStatusEvents.statusCode);
                                    response.Message = "La factura es valida en la DIAN, verificar el estatus de los eventos";
                                }
                            }
                            else
                            {
                                throw new Exception("La respuesta esta vacia al momento de consultar GetStatusEvent");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                            //Se logro consultar el estatus en GetStatus de una factura
                            //Pero no se logro consultar GetStatusEvent de la factura
                            //Se podria considerar valida la factura
                            response = new Entity.InvoiceEventsStatusDian
                            {
                                Code = 502,
                                Message = "Documento con incidente técnico en la DIAN al momento de consultar los eventos de una factura"
                            };

                            response.Invoice = new InvoiceDocumentResponse
                            {
                                StatusCode = responseStatus.Code,
                                StatusDescription = responseStatus.messageStatus,
                                IsValid = responseStatus.isValid,
                                //StatusCodeEvents = response.Code,
                                //StatusEvents = response.Message
                            };
                        }
                    }
                    else if (responseStatus.Code == 99)
                    {
                        //Factura Rechazada
                        response.Message = responseStatus.messageStatus;
                        response.Code = Int32.Parse(responseStatus.statusCode);

                        //response.StatusDian.InvoiceStatusCode = response.StatusDian.StatusCodeEvents = response.Code;
                        //response.StatusDian.StatusDescription = response.StatusDian.StatusEvents = response.Message;
                    }
                    else
                    {
                        //No existe la factura en la DIAN
                        //Si existe la factura
                        //response.StatusDian.StatusDescription = response.StatusDian.StatusEvents = responseStatus.statusDescription;
                        //response.StatusDian.InvoiceStatusCode = response.StatusDian.StatusCodeEvents = Int32.Parse(responseStatus.statusCode);
                        response.Invoice.IsValid = responseStatus.isValid;

                        response.Code = 98;
                        response.Message = "CUFE no registrado ante la DIAN";
                    }
                }
                else
                {
                    throw new Exception("La respuesta esta vacia al momento de consultar GetStatus");
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                //Hubo errores al momento de consultar la factura
                response = new Entity.InvoiceEventsStatusDian
                {
                    Code = 501,
                    Message = "Documento con incidente técnico en la DIAN al momento de consultar la factura"
                };

                response.Invoice = new InvoiceDocumentResponse
                {
                    StatusCode = response.Code,
                    StatusDescription = response.Message
                };
            }

            return response;
        }
    }
}
