using FeCoEventos.Models;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Domain.Interface
{
    public interface IEventCreateDomain
    {
        EventsBuildResponse Create(EventsBuildRequest request, CustomJwtTokenContext context, string tokenJWT, ILogAzure log);
    }
}
