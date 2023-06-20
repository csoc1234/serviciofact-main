using APIFactoringIntegration.Application.Dto;
using APIFactoringIntegration.Application.Interface;
using APIFactoringIntegration.Application.Validation;
using APIFactoringIntegration.Domain.Entity;
using APIFactoringIntegration.Domain.Interface;
using AutoMapper;
using FluentValidation.Results;

namespace APIFactoringIntegration.Application.Main
{
    public class Document : IDocument
    {
        private readonly IValidDocsList _validDocsList;
        private readonly IMapper _mapper;

        public Document(IValidDocsList validDocsList, IMapper mapper)
        {
            _validDocsList = validDocsList;
            _mapper = mapper;
        }

        public async Task<ResponseDto> ValidDocument(DocumentRequestDto request)
        {
            try
            {
                //Fluent Validation
                DocumentRequestDtoValidator validator = new();
                ValidationResult validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    return new ResponseDto
                    {
                        Codigo = 400,
                        Mensaje = "Request Invalido"
                    };
                }

                //Domain

                List<Invoice> result = await _validDocsList.Get(
                    request.Usuario,
                    request.Clave,
                    request.TipoDocumentoProveedor,
                    request.IdProveedor,
                    request.TipoDocumentoPagador,
                    request.IdPagador,
                    request.FechaInicio,
                    request.FechaFin);

                //AutoMapper
                ResponseDto response = new ResponseDto { Codigo = 200 };
                List<FacturaDto> facturas = new List<FacturaDto>();

                _mapper.Map(result, facturas);
                response.Facturas = facturas;

                return response;
            }
            catch (Exception)
            {
                return new ResponseDto
                {
                    Codigo = 400,
                    Mensaje = "Request Invalido"
                };
            }
        }
    }
}
