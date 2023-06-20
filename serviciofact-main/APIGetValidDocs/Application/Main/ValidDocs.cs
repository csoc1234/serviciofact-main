using APIGetValidDocs.Application.Dto;
using APIGetValidDocs.Application.Interface;
using APIGetValidDocs.Application.Validation;
using APIGetValidDocs.Domain.Entity;
using APIGetValidDocs.Domain.Interface;
using AutoMapper;
using FluentValidation.Results;

namespace APIGetValidDocs.Application.Main
{
    public class ValidDocs : IValidDocs
    {
        private readonly IGetInvoiceDomain _invoiceDomain;

        private readonly IMapper _mapper;

        public ValidDocs(IGetInvoiceDomain invoiceDomain, IMapper mapper)
        {

            _invoiceDomain = invoiceDomain;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetListValidDocs(ValidDocsRequestDto request)
        {
            try
            {
                //Fluent Validation
                ValidDocsRequestDtoValidator validator = new ValidDocsRequestDtoValidator();

                ValidationResult validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    return new ResponseDto
                    {
                        Code = 400,
                        Message = validationResult.Errors[0].ErrorMessage
                    };
                }

                //Call Domain
                List<Invoice> result = await _invoiceDomain.GetList(request.SupplierIdentificationType, request.SupplierIdentification, request.CustomerIdentificationType, request.CustomerIdentification, DateTime.Parse(request.DateFrom), DateTime.Parse(request.DateTo));

                //AutoMapper
                List<ValidDocsResponseDto> responseDto = new();
                _mapper.Map(result, responseDto);

                return new ResponseDto
                {
                    Code = 200,
                    Message = "",
                    List = responseDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Code = 500,
                    Message = "Se ha producido un error durante la transaccion"
                };
            }
        }
    }
}
