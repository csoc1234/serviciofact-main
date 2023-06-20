using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Domain.Interface
{
    public interface IEventStatusDomain
    {
        FactoringEventResponse GetStatus(string eventId, string trackId, ILogAzure log);

        FactoringEventResponse GetStatusByUuid(string eventId, string eventuuid, ILogAzure log);
    }
}
