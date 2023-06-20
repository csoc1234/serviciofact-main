using FeCoEventos.Application.Dto;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IEventStatus
    {
        FactoringEventResponse GetStatus(EventDto eventDto, LogRequest logRequest);

        FactoringEventResponse GetFactoringEvent(EventUuidDto eventUuidDto, LogRequest logRequest);

    }
}
