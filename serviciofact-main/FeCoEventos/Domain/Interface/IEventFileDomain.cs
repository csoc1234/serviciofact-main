using FeCoEventos.Util.TableLog;
using System.Reflection;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Domain.Interface
{
    public interface IEventFileDomain
    {
        string GetFileXml(InvoiceEventTable eventTable, ILogAzure log);
        
    }
}
