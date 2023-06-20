using WebApi.Domain.Models;
using TFHKA.LogsMongo;

namespace WebApi.Domain.Interface
{
    public interface IInvoiceEventsStatusDomain
    {
        Entity.InvoiceEventsStatusDian GetEventAppResponse(string cufe, ILogMongo log);
    }
}
