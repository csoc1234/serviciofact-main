using APIFactoringIntegration.Application.Dto;
using FluentValidation;

namespace APIFactoringIntegration.Application.Validation
{
    public class DocumentRequestDtoValidator : AbstractValidator<DocumentRequestDto>
    {
        public DocumentRequestDtoValidator()
        {
            RuleFor(x => x.Usuario)
                .NotEmpty().WithMessage("El campo user es requerido.")
                .NotNull().WithMessage("El campo user es requerido.")
                .Length(36, 36).WithMessage("El campo user debe contener 36 caracteres")
                .Matches(@"^[a-zA-Z0-9-]+$").WithMessage("El campo user tiene caracteres no validos");

            RuleFor(x => x.Clave)
                .NotEmpty().WithMessage("El campo password es requerido.")
                .NotNull().WithMessage("El campo password es requerido.")
                .Length(36, 36).WithMessage("El campo password debe contener 36 caracteres")
                .Matches(@"^[a-zA-Z0-9-]+$").WithMessage("El campo password tiene caracteres no validos");

            RuleFor(x => x.TipoDocumentoProveedor)
                .NotEmpty().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .Matches("(11|12|13|21|22|31|41|42|47|48|50|50|91)$").WithMessage("El campo Tipo de Identificacion Emisor no es soportado.");

            RuleFor(x => x.IdProveedor)
                .NotEmpty().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .Length(3, 20).WithMessage("El campo Numero de Identificacion Emisor debe contener de 3 a 20 caracteres.")
                .Matches(@"^[0-9]+$").WithMessage("El campo Numero de Identificacion Emisor tiene caracteres no validos.");

            RuleFor(x => x.TipoDocumentoPagador)
                .NotEmpty().WithMessage("El campo Tipo de Identificacion Receptor es requerido.")
                .NotNull().WithMessage("El campo Tipo de Identificacion Receptor es requerido.")
                .Matches("(11|12|13|21|22|31|41|42|47|48|50|50|91)$").WithMessage("El campo Tipo de Identificacion Receptor no es soportado.");

            RuleFor(x => x.IdPagador)
                .NotEmpty().WithMessage("El campo Numero de Identificacion Receptor es requerido.")
                .NotNull().WithMessage("El campo Numero de Identificacion Receptor es requerido.")
                .Length(3, 20).WithMessage("El campo Numero de Identificacion Receptor debe contener de 3 a 20 caracteres.")
                .Matches(@"^[0-9]+$").WithMessage("El campo Numero de Identificacion Receptor tiene caracteres no validos.");

            RuleFor(x => x.FechaInicio).Cascade(CascadeMode.Stop)
                .Must(x => isValidInitialDate(x)).WithMessage("La Fecha Desde no es una fecha valida.");

            RuleFor(x => x.FechaFin).Cascade(CascadeMode.Stop)
                .Must(x => isValidInitialDate(x)).WithMessage("La Fecha Hasta no es una fecha valida.");
        }

        public static bool isValidInitialDate(string initialDate)
        {
            try
            {
                DateTime time = DateTime.Parse(initialDate);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
