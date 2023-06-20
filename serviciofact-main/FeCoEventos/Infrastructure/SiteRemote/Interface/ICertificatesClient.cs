using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface ICertificatesClient
    {
        CertificatesResponse GetCertificate(string tokenJWT, ILogAzure log);
    }
}
