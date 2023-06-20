using FeCoEventos.Application.Dto;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IEventUpdate
    {
        EventUpdatingResponse UpdateDianResult(EventDto eventDto, EventUpdatingRequest request, LogRequest logRequest);

        ResponseBase DeliveryAsyncDian(EventDto eventDto, EventDeliveryAsyncDto request, LogRequest logRequest);
    }
}
