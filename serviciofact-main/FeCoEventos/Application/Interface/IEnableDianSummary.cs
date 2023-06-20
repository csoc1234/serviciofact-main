using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IEnableDianSummary
    {
        EventsSummaryResponse GetList(string nit, LogRequest logRequest);
    }
}
