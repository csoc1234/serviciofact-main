using System.Collections.Generic;
using System.Linq;
using WebApi.Domain.Entity;
using WebApi.Domain.Interface;
using WebApi.Domain.Models;
using WebApi.Domain.ValueObjects;

namespace WebApi.Domain.Core
{
    public class CheckStatusEventsDomain : ICheckStatusEventsDomain
    {
        public static bool StatusDianExist(List<InvoiceStatusLast> list, string codeEvent)
        {
            return list.Where(x => x.EventType == codeEvent).Count() > 0;
        }

        public Entity.InvoiceEventsStatusDian Valid(List<InvoiceStatusLast> events)
        {
            //Verificar Estatus

            Entity.InvoiceEventsStatusDian result = new Entity.InvoiceEventsStatusDian();

            result.CantidadEventos = events == null ? 0 : events.Count;

            result.TieneEventos = events?.Count > 0;

            //Transformacion de Invoices
            if (events != null)
            {
                var eventFirst = events.FirstOrDefault();
                if (eventFirst != null)
                {
                    result.Invoice = new InvoiceDocumentResponse()
                    {
                        DocumentId = eventFirst.InvoiceDocumentId,
                        InvoiceUuid = eventFirst.InvoiceUuid,
                        SupplierIdentification = eventFirst.InvoiceSupplierIdentification,
                        SupplierTypeIdentification = eventFirst.InvoiceSupplierTypeIdentification,
                        CustomerIdentification = eventFirst.InvoiceCustomerIdentification,
                        CustomerTypeIdentification = eventFirst.InvoiceCustomerTypeIdentification,
                        StatusCode = eventFirst.InvoiceStatus,
                        StatusDescription = "",
                        CreatedAt = eventFirst.InvoiceCreatedAt,
                        UpdatedAt = eventFirst.InvoiceUpdatedAt,
                        IsValid = eventFirst.InvoiceStatus == 200
                    };
                }
            }

            //Transformacion de Eventos
            if (events != null)
            {
                result.Events = new List<EventsDocumentResponse>();

                foreach (var item in events)
                {
                    EventsDocumentResponse rowEvent = new EventsDocumentResponse
                    {
                        ID = item.Id,
                        EventId = item.EventId,
                        CreatedAt = item.CreatedAt,
                        Description = Events.GetDescription(item.EventType),
                        UpdatedAt = item.UpdatedAt,
                        UUID = item.EventUuid,
                        UUIDSchemeName = item.EventUuidType,
                        EffectiveDate = item.EventDatetime,
                        ReferenceID = item.ReferenceId,
                        ResponseCode = item.EventType,
                        Status = item.Status,
                        IssuerParty = new Party
                        {
                            CompanyID = item.SupplierIdentification,
                            CompanyIDSchemeName = item.SupplierTypeIdentification,
                            RegistrationName = item.SupplierRegistrationName
                        },
                        RecipientParty = new Party
                        {
                            CompanyID = item.CustomerIdentification,
                            CompanyIDSchemeName = item.CustomerTypeIdentification,
                            RegistrationName = item.CustomerRegistrationName
                        }
                    };

                    result.Events.Add(rowEvent);
                }
            }

            if (result.TieneEventos)
            {
                result.EsRecibida = CheckStatusEventsDomain.StatusDianExist(events, "030");

                result.EsReclamada = CheckStatusEventsDomain.StatusDianExist(events, "031");

                result.EsBienoServicio = CheckStatusEventsDomain.StatusDianExist(events, "032");

                result.EsAceptada = CheckStatusEventsDomain.StatusDianExist(events, "033");

                result.EsAceptadaTacitamente = CheckStatusEventsDomain.StatusDianExist(events, "034");

                result.EsTituloValor = CheckStatusEventsDomain.StatusDianExist(events, "036");

                if (CheckStatusEventsDomain.StatusDianExist(events, "037") ||
                    CheckStatusEventsDomain.StatusDianExist(events, "038") ||
                    CheckStatusEventsDomain.StatusDianExist(events, "039"))
                {
                    result.EstaEndosada = true;
                }

                //Escenario Reclamada tiene evento registrado de reclamo.
                if (result.EsReclamada)
                {
                    result.StatusCode = 206;
                    result.StatusMessage = "Documento reclamado";

                    result.EsRecibida = false;
                    result.EsBienoServicio = false;
                    result.EsAceptadaTacitamente = false;
                    result.EsAceptada = false;
                    result.EsTituloValor = false;
                    result.EstaEndosada = false;

                    return result;
                }

                //Escenario Recibido aun no aceptada
                if (result.EsRecibida && (!result.EsAceptada && !result.EsAceptadaTacitamente))
                {
                    result.StatusCode = 207;
                    result.StatusMessage = "Documento recibido";

                    return result;
                }

                //Escenario Documento aceptado, con 3 eventos (aceptación expresa, recibo, y recibo del bien o servicio).
                if (result.EsRecibida && result.EsBienoServicio && (result.EsAceptada || result.EsAceptadaTacitamente))
                {
                    result.StatusCode = 208;
                    result.StatusMessage = "Documento Aceptado";
                    result.EsAceptada = true;

                    //Escenario Titulo Valor No endosada
                    bool event37 = CheckStatusEventsDomain.StatusDianExist(events, "037");
                    bool event38 = CheckStatusEventsDomain.StatusDianExist(events, "038");
                    bool event39 = CheckStatusEventsDomain.StatusDianExist(events, "039");

                    if (result.EsTituloValor && (!event37 && !event38 && !event39))
                    {
                        result.StatusCode = 209;
                        result.StatusMessage = "Titulo Valor";
                    }

                    //Escenario Título Valor Endosada
                    if (result.EsTituloValor && (event37 || event38 || event39))
                    {
                        result.StatusCode = 210;
                        result.StatusMessage = "Titulo Valor Edosado";
                    }

                    return result;
                }

                result.StatusCode = 211;
                result.StatusMessage = "No Recibida con Eventos";

                result.EsRecibida = false;
                result.EsBienoServicio = false;
                result.EsAceptadaTacitamente = false;
                result.EsAceptada = false;
                result.EsTituloValor = false;
                result.EstaEndosada = false;
            }
            else
            {
                //Escenario Validada y no tiene eventos.
                result.StatusCode = 205;
                result.StatusMessage = "Documento Valido";
            }

            return result;
        }
    }
}
