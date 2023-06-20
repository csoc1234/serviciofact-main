using WebApi.Application.Dto;
using WebApi.Domain.Entity;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Application.Interface
{
    public interface IInvoiceEventsStatus
    {
       
        InvoiceStatusDianDto GetLastStatus(string supplierId, string documentId, string cufe, LogRequest logRequest);
    }
}
