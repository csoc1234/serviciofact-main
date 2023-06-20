using WebApi.Domain.Entity;
using TFHKA.LogsMongo;

namespace WebApi.Domain.Interface
{
    public interface IInvoiceLastStatusDomain
    {
        InvoiceEventsStatusDian GetStatusInvoice(string identification, string documentId, string cufe, ILogMongo log);
    }
}
