using APIValidateEvents.Application.Dto;
using APIValidateEvents.Application.Interface;
using APIValidateEvents.Application.Validation;
using APIValidateEvents.Domain.Entity;
using APIValidateEvents.Domain.Interface;
using AutoMapper;
using FluentValidation.Results;

namespace APIValidateEvents.Application.Main
{
    public class InvoiceCheck : IInvoiceCheck
    {
        private readonly IValidationEvents _validationEvents;
        private readonly IMapper _mapper;

        public InvoiceCheck(IValidationEvents validationEvents, IMapper mapper)
        {
            _validationEvents = validationEvents;
            _mapper = mapper;
        }

        public async Task<ResponseDto> ValidateAsync(InvoiceDto request)
        {
            try
            {
                //Validations            
                InvoiceDtoValidator validator = new InvoiceDtoValidator();
                ValidationResult result = validator.Validate(request);

                if (!result.IsValid)
                {
                    return new ResponseDto
                    {
                        Code = 400,
                        Message = result.Errors[0].ErrorMessage
                    };
                }

                //Domain
                InvoiceState responseDomain = await _validationEvents.Validation(request.Cufe, request.NumberIdentificationSupplier, request.DocumentId);

                //AutoMapper
                ResponseDto responseDto = new();
                _mapper.Map(responseDomain, responseDto);

                //Result
                return responseDto;
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Code = 500,
                    Message = "Error al procesarlo",
                    Valid = false
                };
            }
        }
    }
}
