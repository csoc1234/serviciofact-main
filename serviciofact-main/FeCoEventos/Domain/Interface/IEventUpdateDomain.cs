using FeCoEventos.Application.Dto;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Domain.Interface
{
    public interface IEventUpdateDomain
    {
        EventUpdatingResponse UpdateDianResult(string eventId, string trackId, EventUpdatingRequest eventBodyReq, ILogAzure log);

        ResponseBase UpdateAsyncDelivery(EventDto eventData, EventDeliveryAsyncDto request, ILogAzure log);
    }
}
