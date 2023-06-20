using FeCoEventos.Clients.Signed;
using FeCoEventos.Domain.Entity;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface ISignedClient
    {
        SignedInternalResponse SignedXml(string xml, Certificate certificate, ILogAzure log);
    }
}
