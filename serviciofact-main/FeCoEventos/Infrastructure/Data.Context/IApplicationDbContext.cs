using FeCoEventos.Domain.Entity;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Infrastructure.Data.Context
{
    public interface IApplicationDbContext
    {
        List<InvoiceEventTable> GetAnysInvoiceEvents(int status, string evenCode, DateTime dateFrom, DateTime dateEnd, ILogAzure log, IConfiguration configuration);

        int SaveUpdateEventInvoice(InvoiceEventTable invoiceEvent, byte operationType, IConfiguration configuration, ILogAzure log);

        Task<Event> GetEventStatus(string eventId, string trackId, IConfiguration configuration, ILogAzure log);

        Task<EventCount> GetInvoiceEventsReadByEventUuid(string eventId, string eventUuid, IConfiguration configuration, ILogAzure log);

        int UpdateEventAsyncSend(InvoiceEventTable invoiceEvent, string dianTrackId, IConfiguration configuration, ILogAzure log);

        List<EventSummary> GetAllEventsEnable(string numberIdentification, IConfiguration configuration, ILogAzure log);

        int UpdateAttachedDocument(InvoiceEventTable invoiceEvent, IConfiguration configuration, ILogAzure log);

        int UpdateFileResponseDian(InvoiceEventTable invoiceEvent, IConfiguration configuration, ILogAzure log);

    }
}
