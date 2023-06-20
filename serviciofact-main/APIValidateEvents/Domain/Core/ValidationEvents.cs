using APIValidateEvents.Domain.Entity;
using APIValidateEvents.Domain.Interface;
using APIValidateEvents.Infrastucture.SiteRemote.Interface;

namespace APIValidateEvents.Domain.Core
{
    public class ValidationEvents : IValidationEvents
    {
        private readonly IDianStatusClient _statusClient;
        public ValidationEvents(IDianStatusClient statusClient)
        {
            _statusClient = statusClient;
        }
        //Duda ocupe async, es correcto o utilizamos sync
        public async Task<InvoiceState> Validation(string cufe, string supplierIdentification, string documentId)
        {
            try
            {
                // Consumir capa infraestructura metodo GET string cufe, string supplierIdentification, string documentId 
                InvoiceStatusDian invoiceStatus = await _statusClient.Get(cufe, supplierIdentification, documentId);

                return EventsCheck.Check(invoiceStatus);
            }
            catch (Exception)
            {
                return new InvoiceState { Valid = false };
            }
        }
    }
}
