using APIValidateEvents.Domain.Entity;

namespace APIValidateEvents.Domain.Interface
{
    public interface IValidationEvents
    {
        Task<InvoiceState> Validation(string cufe, string supplierIdentification, string documentId);
    }
}
