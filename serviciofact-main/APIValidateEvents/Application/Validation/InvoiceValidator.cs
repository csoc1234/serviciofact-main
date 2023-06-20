using APIValidateEvents.Application.Dto;
using APIValidateEvents.Common;
using FluentValidation;

namespace APIValidateEvents.Application.Validation
{
    public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
    {
        public InvoiceDtoValidator()
        {
            RuleFor(x => x.Xml)
                .NotEmpty().WithMessage("El campo Xml es requerido.")
                .NotNull().WithMessage("El Xml no puede ser vacio")
                .Must(x => UtilitiesString.IsValidXML(x)).WithMessage("El Xml no es un XML permitido");

            RuleFor(x => x.Cufe)
                .NotEmpty().WithMessage("El campo Uuid es requerido.")
                .NotNull().WithMessage("El campo Uuid es requerido.")
                .Length(96, 96).WithMessage("El campo Uuid debe contener 96 caracteres")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("El campo Uuid tiene caracteres no validos");

            RuleFor(x => x.DocumentId)
                .NotEmpty().WithMessage("El campo Numero de Documento es requerido.")
                .NotNull().WithMessage("El campo Numero de Documento es requerido.")
                .Length(1, 30).WithMessage("El campo Numero de Documento debe contener de 1 a 30 caracteres")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("El campo Numero de Documento tiene caracteres no validos");

            RuleFor(x => x.DatePayment)
                .Matches(@"^(?:$|(?:\d{4})-(?:0?[1-9]|1[0-2])-(?:0?[1-9]|[12]\d|3[01]))$").WithMessage("El campo Fecha no tiene un formato valido")
                .Must(x => UtilitiesString.IsDateValid(x)).WithMessage("El campo Fecha es valido");

            RuleFor(x => x.TypeIdentificationSupplier)
                .NotEmpty().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .Matches("(11|12|13|21|22|31|41|42|47|48|50|50|91)$").WithMessage("El campo Tipo de Identificacion Emisor no es soportado");

            RuleFor(x => x.NumberIdentificationSupplier)
                .NotEmpty().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .Length(3, 20).WithMessage("El campo Numero de Identificacion Emisor debe contener de 3 a 20 caracteres")
                .Matches(@"^[0-9]+$").WithMessage("El campo Numero de Identificacion Emisor tiene caracteres no validos");

            RuleFor(x => x.IdEnterprise)
                .GreaterThan(0).WithMessage("El campo IdEnterprise debe ser mayor que cero.");
        }
    }
}
