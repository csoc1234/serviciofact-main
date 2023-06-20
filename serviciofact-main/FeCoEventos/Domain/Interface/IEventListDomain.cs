using FeCoEventos.Application.Dto;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using System.Collections.Generic;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Domain.Interface
{
    public interface IEventListDomain
    {
        EventsPendingResponse GetEventsByStatus(EventStatusDto request, ILogAzure log);        
    }
}
