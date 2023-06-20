using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;

namespace FeCoEventos.Infrastructure.SiteRemote.Interface
{
    public interface IValidationXMLClient
    {
        ValidationXMLResponse GetValidationXML(string cufe, ILogAzure log);
    }
}
