using FeCoEventos.Application.Dto;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IEventList
    {
        EventsPendingResponse GetList(EventStatusDto request, LogRequest logRequest);
    }
}
