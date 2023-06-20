using APIGetValidDocs.Domain.Entity;

namespace APIGetValidDocs.Infraestructure.Interface
{
    public interface IInvoiceClient
    {
        Task<InvoiceStatus> Post(InvoiceValidate request);
    }
}
