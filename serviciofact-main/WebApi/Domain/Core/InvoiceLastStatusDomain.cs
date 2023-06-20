using DocumentBuildCO.ClassXSD;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApi.Domain.Entity;
using WebApi.Domain.Interface;
using WebApi.Infrastructure.ComunicationDian;
using WebApi.Infrastructure.Data.Context;
using TFHKA.LogsMongo;

namespace WebApi.Domain.Core
{
    public class InvoiceLastStatusDomain : IInvoiceLastStatusDomain
    {
        private readonly IConfiguration _configuration;
        private readonly IFactoringDbContext _factoringDbContext;
        private readonly IDianStatusRestClient _dianStatusClient;
        private readonly ICheckStatusEventsDomain _checkStatusEvents;

        public InvoiceLastStatusDomain(IDianStatusRestClient dianStatusClient,
            IFactoringDbContext factoringDbContext,
            IConfiguration configuration,
            ICheckStatusEventsDomain checkStatusEvents)
        {
            _factoringDbContext = factoringDbContext;
            _dianStatusClient = dianStatusClient;
            _configuration = configuration;
            _checkStatusEvents = checkStatusEvents;
        }

        public InvoiceEventsStatusDian GetStatusInvoice(string identification, string documentId, string cufe, ILogMongo log)
        {
            //Consulto en DB el estatus de la factura
            List<InvoiceStatusLast> invoiceStatusLastTableList = _factoringDbContext.GetInvoiceStatusLastAsync(documentId, identification, cufe, log).Result;

            if (invoiceStatusLastTableList.Count > 0)
            {
                InvoiceStatusLast invoiceStatusLastTable = invoiceStatusLastTableList.OrderByDescending(x => x.EventDatetime).FirstOrDefault();

                //Si el updated_at tiene un tiempo mayor a tiempo T voy a la DIAN
                int BackLastStatusValidMinute = Int32.Parse(_configuration["ConfigurationTime:BackLastStatusValid.Minute"]);

                if (invoiceStatusLastTable.UpdatedAt >= DateTime.UtcNow.AddHours(-5).AddMinutes(BackLastStatusValidMinute * -1))
                {
                    //Valida los escenarios de eventos
                    InvoiceEventsStatusDian response = _checkStatusEvents.Valid(invoiceStatusLastTableList);

                    response.Code = 201;
                    response.Message = "Se retorna el ultimo estatus legal de la factura";

                    //Sino retorno
                    return response;
                }
                else
                {
                    //Consulto a la DIAN
                    InvoiceEventsStatusDian resultStatusDian = GetDianStatus(cufe, identification, documentId, log);

                    //Busco el estatus mas reciente
                    List<InvoiceStatusLast> invoiceStatusLastDian = GetLastStatusFromDocumentType(resultStatusDian, log);

                    if (invoiceStatusLastDian != null)
                    {
                        foreach (InvoiceStatusLast item in invoiceStatusLastDian)
                        {
                            item.InvoiceDocumentId = documentId;
                            item.InvoiceUuid = cufe;

                            //Seteamos la fecha de la tabla
                            item.InvoiceCreatedAt = invoiceStatusLastTable.InvoiceCreatedAt;
                            item.InvoiceUpdatedAt = invoiceStatusLastTable.InvoiceUpdatedAt;

                            //Si es el mismo estatus o no
                            if (invoiceStatusLastTableList.Exists(x => x.EventType == item.EventType))
                            {
                                //Actualizo el updated_at del estatus 
                                bool resultUpdate = _factoringDbContext.InvoiceStatusEventHistUpdateAsync(invoiceStatusLastTable.Id, DateTime.UtcNow.AddHours(-5), log).Result;
                            }
                            else
                            {
                                //Si tiene un evento se registra en DB el ultimo estatus
                                //Inserta el nuevo estatus
                                int id = 0;

                                bool resultInsert = _factoringDbContext.InvoiceLastStatusCreateAsync(id, item, log).Result;
                            }
                        }

                        //Valida los escenarios de eventos
                        InvoiceEventsStatusDian response = _checkStatusEvents.Valid(invoiceStatusLastDian);

                        response.Code = 200;
                        response.Message = "Se retorna el ultimo estatus legal de la factura";

                        return response;
                    }
                    else
                    {
                        InvoiceEventsStatusDian response = _checkStatusEvents.Valid(invoiceStatusLastTableList);

                        response.Code = 201;
                        response.Message = "Se retorna el ultimo estatus legal de la factura";

                        return response;
                    }
                }
            }
            else
            {
                //Consultar Estatus DIAN
                InvoiceEventsStatusDian resultStatusDian = GetDianStatus(cufe, identification, documentId, log);

                if (resultStatusDian.Code == 200)
                {
                    //Busco el estatus mas reciente
                    List<InvoiceStatusLast> invoiceStatusLastDian = GetLastStatusFromDocumentType(resultStatusDian, log);

                    //Verifico si a pesar de que existe tiene un estatus o solo existe la factura
                    if (invoiceStatusLastDian != null)
                    {
                        //Si tiene un evento se registra en DB el ultimo estatus

                        foreach (InvoiceStatusLast item in invoiceStatusLastDian)
                        {
                            item.InvoiceDocumentId = documentId;
                            item.InvoiceUuid = cufe;

                            //S Inserta el estatus                        
                            int id = 0;

                            bool resultInsert = _factoringDbContext.InvoiceLastStatusCreateAsync(id, item, log).Result;
                        }

                        //Return 200 (Estatus viene de la DIAN)

                        InvoiceEventsStatusDian response = _checkStatusEvents.Valid(invoiceStatusLastDian);

                        response.Code = 200;
                        response.Message = "Se retorna el ultimo estatus legal de la factura";

                        return response;
                    }
                    else
                    {
                        return new InvoiceEventsStatusDian
                        {
                            Code = 101,
                            Message = "No se ha encontrado el estatus de la factura"
                        };
                    }
                }
                else
                {
                    return new InvoiceEventsStatusDian
                    {
                        Code = resultStatusDian.Code,
                        Message = resultStatusDian.Message,
                        StatusCode = resultStatusDian.StatusCode,
                        StatusMessage = resultStatusDian.StatusMessage
                    };
                }
            }
        }

        public Entity.InvoiceEventsStatusDian GetDianStatus(string cufe, string identification, string documentId, ILogMongo log)
        {
            //Consultar API Consultar/CUFE
            WebApi.Models.Response.DianStatusResponse resultConsultaDian = _dianStatusClient.GetDianStatus(cufe, log);

            if (resultConsultaDian.Code == 200)
            {
                //Verificar el Document Id y el NIT Emisor
                if (!string.IsNullOrEmpty(resultConsultaDian.applicationResponse))
                {
                    //Serializar
                    ApplicationResponseType applicationResponseSerialize = DocumentBuild.ApplicationResponseSerialize(resultConsultaDian.applicationResponse);

                    if (applicationResponseSerialize != null)
                    {
                        //Comparar valores

                        //DocumentId
                        if (applicationResponseSerialize.DocumentResponse?[0].DocumentReference?[0].ID != null)
                        {
                            //Se comenta validacion motivado a que la DIAN ya no esta cumpliendo esto
                            /* if (applicationResponseSerialize.DocumentResponse?[0].DocumentReference?[0].ID?.Value != documentId)
                             {
                                 return new Entity.InvoiceEventsStatusDian
                                 {
                                     Code = 97,
                                     Message = "El numero de documento no corresponde al asociado a la factura consultada por el CUFE"
                                 };
                             }*/
                        }

                        //Identification Supplier
                        if (applicationResponseSerialize.ReceiverParty.PartyTaxScheme[0].CompanyID.Value != identification)
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                Code = 96,
                                Message = "El numero de identificacion del emisor no corresponde al asociado a la factura consultada por el CUFE"
                            };
                        }


                        //Consulto en la DIAN
                        WebApi.Models.Response.DianStatusResponse resultStatusDian = _dianStatusClient.GetStatusEvent(cufe, log);

                        if (resultStatusDian.Code == 200)
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                Code = 200,
                                Message = "Se retorna el ultimo estatus",
                                Invoice = new Models.InvoiceDocumentResponse
                                {
                                    XmlApplicationResponse = resultStatusDian.applicationResponse,
                                    StatusCode = resultStatusDian.statusCode == "00" ? 200 : int.Parse(resultStatusDian.statusCode),
                                    StatusDescription = resultStatusDian.statusDescription
                                }
                            };
                        }
                        else if (resultStatusDian.Code == 99)
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                Code = 99,
                                Message = resultStatusDian.Message
                            };
                        }
                        else if (resultStatusDian.Code == 67)
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                StatusCode = 205,
                                StatusMessage = "Documento Valido",
                                Code = 205,
                                Message = "La factura existe en el DIAN pero no posee eventos registrados"
                            };
                        }
                        else if (resultStatusDian.Code == 66)
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                Code = 98,
                                Message = "No existe el documento en la DIAN"
                            };
                        }
                        else
                        {
                            return new Entity.InvoiceEventsStatusDian
                            {
                                Code = resultStatusDian.Code,
                                Message = resultStatusDian.Message
                            };
                        }
                    }
                    else
                    {
                        return new Entity.InvoiceEventsStatusDian
                        {
                            Code = 500,
                            Message = "No se logro leer el xml Application Response de la DIAN"
                        };
                    }
                }
                else
                {
                    return new Entity.InvoiceEventsStatusDian
                    {
                        Code = 500,
                        Message = "La DIAN no retorna el evento de aceptacion de la factura"
                    };
                }
            }
            else
            {
                return new Entity.InvoiceEventsStatusDian
                {
                    Code = resultConsultaDian.Code,
                    Message = resultConsultaDian.messageStatus
                };
            }
        }

        public List<Entity.InvoiceStatusLast> GetLastStatusFromDocumentType(Entity.InvoiceEventsStatusDian invoiceEventsStatus, ILogMongo log)
        {
            try
            {
                List<Entity.InvoiceStatusLast> response = new List<Entity.InvoiceStatusLast>();

                if (invoiceEventsStatus.Invoice == null)
                {
                    return null;
                }

                Models.InvoiceDocumentResponse statusDian = invoiceEventsStatus.Invoice;

                ApplicationResponseType applicationResponse = DocumentBuild.ApplicationResponseProcess(statusDian.XmlApplicationResponse);

                if (applicationResponse == null)
                {
                    return null;
                }

                //Se selecciona el ultimo estatus de evento
                //Order by cbc:EffectiveDate DESC cbc:EffectiveTime DESC
                List<DocumentResponseType> documentResponseLast = applicationResponse.DocumentResponse.ToList();

                //Emisor y Receptor del documento electronico
                DocumentResponseType eventsList = documentResponseLast.Where(x => x.Response.ResponseCode.Value == "030").FirstOrDefault();
                string invoiceSupplierIdentification = eventsList.RecipientParty.PartyTaxScheme[0].CompanyID.Value;
                string invoiceSupplierTypeIdentification = eventsList.RecipientParty.PartyTaxScheme[0].CompanyID.schemeName;
                string invoiceCustomerIdentification = eventsList.IssuerParty.PartyTaxScheme[0].CompanyID.Value;
                string invoiceCustomerTypeIdentification = eventsList.IssuerParty.PartyTaxScheme[0].CompanyID.schemeName;

                foreach (DocumentResponseType documentItem in documentResponseLast)
                {
                    //Creo la Entity a partir de la Respuesta de la DIAN

                    string fecha = documentItem.Response.EffectiveDate.Value.ToShortDateString();
                    string hora = documentItem.Response.EffectiveTime.Value.ToLongTimeString();

                    DateTime timeNow = DateTime.UtcNow.AddHours(-5);

                    Entity.InvoiceStatusLast invoiceStatusLast = new Entity.InvoiceStatusLast
                    {
                        InvoiceStatus = 200,
                        InvoiceSupplierIdentification = invoiceSupplierIdentification,
                        InvoiceSupplierTypeIdentification = invoiceSupplierTypeIdentification,
                        InvoiceCustomerIdentification = invoiceCustomerIdentification,
                        InvoiceCustomerTypeIdentification = invoiceCustomerTypeIdentification,

                        DianStatus = (short)statusDian.StatusCode,
                        DianMessage = statusDian.StatusDescription,

                        EventId = documentItem.DocumentReference[0].ID.Value,
                        EventUuid = documentItem.DocumentReference[0].UUID.Value,
                        EventType = documentItem.Response.ResponseCode.Value,
                        EventUuidType = documentItem.DocumentReference[0].UUID.schemeName,
                        ReferenceId = documentItem.Response.ReferenceID.Value,

                        Id = 0,
                        EventDatetime = DateTime.Parse(fecha + " " + hora),
                        CreatedAt = timeNow,
                        UpdatedAt = timeNow,
                        InvoiceCreatedAt = timeNow,
                        InvoiceUpdatedAt = timeNow,
                        Status = 200,
                        SupplierIdentification = documentItem.IssuerParty.PartyTaxScheme[0].CompanyID.Value,
                        SupplierTypeIdentification = documentItem.IssuerParty.PartyTaxScheme[0].CompanyID.schemeName,
                        SupplierRegistrationName = documentItem.IssuerParty.PartyTaxScheme[0].RegistrationName.Value,

                        CustomerIdentification = documentItem.RecipientParty.PartyTaxScheme[0].CompanyID.Value,
                        CustomerTypeIdentification = documentItem.RecipientParty.PartyTaxScheme[0].CompanyID.schemeName,
                        CustomerRegistrationName = documentItem.RecipientParty.PartyTaxScheme[0].RegistrationName.Value
                    };

                    response.Add(invoiceStatusLast);
                }

                return response;
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                return null;
            }
        }
    }
}
