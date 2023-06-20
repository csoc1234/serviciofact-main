using FeCoEventos.Application.Dto;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Domain.ValueObjects;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FeCoEventos.Domain.Core
{
    public class EventListDomain : IEventListDomain
    {
        private readonly IEventFileDomain _eventFileDomain;
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EventListDomain(IEventFileDomain eventFileDomain, IApplicationDbContext context, IConfiguration configuration)
        {
            _eventFileDomain = eventFileDomain;
            _context = context;
            _configuration = configuration;
        }

        public EventsPendingResponse GetEventsByStatus(EventStatusDto request, ILogAzure log)
        {
            EventsPendingResponse response = new EventsPendingResponse();

            List<TFHKA.EventsDian.Infrastructure.Data.Context.InvoiceEventTable>? eventPendingList = _context.GetAnysInvoiceEvents(
                request.Status,
                request.EventCode,
                DateTimeTools.Parse(request.DateFrom, "00:00:00"),
                DateTimeTools.Parse(request.DateTo, "23:59:59"),
                log,
                _configuration);

            if (eventPendingList != null)
            {
                if (eventPendingList.Count > 0)
                {
                    response.ListEvents = new List<EventsPendingList>();

                    foreach (TFHKA.EventsDian.Infrastructure.Data.Context.InvoiceEventTable? item in eventPendingList)
                    {
                        string fileXml = _eventFileDomain.GetFileXml(item, log);

                        if (!string.IsNullOrEmpty(fileXml))
                        {
                            EventsPendingList row = new EventsPendingList
                            {
                                Id = item.id,
                                DocumentId = item.document_id,
                                InvoiceUuid = item.invoice_uuid,
                                EventType = item.event_type,
                                Event_id = item.event_id,
                                EventUuid = item.event_uuid,
                                Status = item.status,
                                TrackId = item.track_id,
                                NameFile = item.namefile,
                                PathFile = item.path_file,
                                TriesSend = item.tries_send,
                                Environment = item.environment,
                                SupplierIdentification = item.supplier_identification,
                                SupplierTypeIdentification = item.supplier_type_identification,
                                TryQuery = item.try_query,
                                CreatedAt = item.created_at
                            };

                            row.Xml = fileXml;
                            row.Hash = StringUtilies.GetSHA1(fileXml);

                            response.ListEvents.Add(row);
                        }
                        else
                        {
                            EventsPendingList row = new EventsPendingList
                            {
                                Id = item.id,
                                DocumentId = item.document_id,
                                InvoiceUuid = item.invoice_uuid,
                                EventType = item.event_type,
                                Event_id = item.event_id,
                                EventUuid = item.event_uuid,
                                Status = item.status,
                                TrackId = item.track_id,
                                NameFile = item.namefile,
                                PathFile = item.path_file,
                                TriesSend = item.tries_send,
                                Environment = item.environment,
                                SupplierIdentification = item.supplier_identification,
                                SupplierTypeIdentification = item.supplier_type_identification,
                                TryQuery = item.try_query,
                                CreatedAt = item.created_at
                            };

                            row.Xml = fileXml;

                            response.ListEvents.Add(row);

                        }
                    }

                    if (response.ListEvents.Count > 0)
                    {
                        response.Code = 200;
                        response.Message = "Se retorna la lista de eventos";
                    }
                    else
                    {
                        response = new EventsPendingResponse
                        {
                            Code = 100,
                            Message = "No se encontraron registros con archivos xml"
                        };
                    }
                }
                else
                {
                    response = new EventsPendingResponse
                    {
                        Code = 100,
                        Message = "No se encontraron registros"
                    };
                }
            }
            else
            {
                response = new EventsPendingResponse
                {
                    Code = 100,
                    Message = "Hubo un error al intentar consultar el listado de eventos"
                };
            }
            return response;
        }
    }
}
