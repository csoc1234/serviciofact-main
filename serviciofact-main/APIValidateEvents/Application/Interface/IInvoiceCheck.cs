using APIValidateEvents.Application.Dto;

namespace APIValidateEvents.Application.Interface
{
    public interface IInvoiceCheck
    {
        Task<ResponseDto> ValidateAsync(InvoiceDto request);
    }
}
