using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface IEventXmlClient
    {
        EventXMLResponse GetEventXML(string cufe, ILogAzure log);
    }
}
