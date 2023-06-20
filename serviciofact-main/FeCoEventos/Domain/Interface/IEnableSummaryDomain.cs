using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Domain.Interface
{
    public interface IEnableSummaryDomain
    {
        EventsSummaryResponse GetList(string nit, ILogAzure log);
    }
}
