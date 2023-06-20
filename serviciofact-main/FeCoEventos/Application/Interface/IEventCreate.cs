using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Application.Interface
{
    public interface IEventCreate
    {
        EventsBuildResponse Generate(EventsBuildRequest request, LogRequest logRequest, string tokenJWT);
    }
}
