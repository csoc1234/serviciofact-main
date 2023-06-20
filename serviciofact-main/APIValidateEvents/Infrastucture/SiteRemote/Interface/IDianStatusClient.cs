using APIValidateEvents.Domain.Entity;

namespace APIValidateEvents.Infrastucture.SiteRemote.Interface
{
    public interface IDianStatusClient
    {
        Task<InvoiceStatusDian> Get (string cufe, string supplierIdentification, string documentId);
    }
}
