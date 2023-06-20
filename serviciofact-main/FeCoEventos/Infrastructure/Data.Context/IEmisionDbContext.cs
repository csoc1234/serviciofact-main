using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;

namespace FeCoEventos.Infrastructure.Data.Context
{
    public interface IEmisionDbContext
    {
        int UpdateInvoiceHistoryEvent(int dianStatus, string eventId, string trackId, bool active, IConfiguration configuration, ILogAzure log);

        int UpdateReceptionStatus(string uuid, int estatus, IConfiguration configuration);
    }
}
