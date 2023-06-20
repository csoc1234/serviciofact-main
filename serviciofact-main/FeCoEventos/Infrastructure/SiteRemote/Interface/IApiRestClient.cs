using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface IApiRestClient
    {
        ResponseHttp<T> Get<T>(string url, string api, string tokenJwt, ILogAzure log);

        ResponseHttp<T> Post<T>(string url, string api, object body, string tokenJwt, ILogAzure log);
    }
}
