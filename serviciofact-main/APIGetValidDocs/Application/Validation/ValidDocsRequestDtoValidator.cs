using APIGetValidDocs.Application.Dto;
using FluentValidation;
using System.Xml;

namespace APIGetValidDocs.Application.Validation
{
    public class ValidDocsRequestDtoValidator : AbstractValidator<ValidDocsRequestDto>
    {
        public ValidDocsRequestDtoValidator()
        {
            RuleFor(x => x.SupplierIdentificationType)
                .NotEmpty().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Tipo de Identificacion Emisor es requerido.")
                .Matches("(11|12|13|21|22|31|41|42|47|48|50|50|91)$").WithMessage("El campo Tipo de Identificacion Emisor no es soportado.");

            RuleFor(x => x.SupplierIdentification)
                .NotEmpty().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .NotNull().WithMessage("El campo Numero de Identificacion Emisor es requerido.")
                .Length(3, 20).WithMessage("El campo Numero de Identificacion Emisor debe contener de 3 a 20 caracteres.")
                .Matches(@"^[0-9]+$").WithMessage("El campo Numero de Identificacion Emisor tiene caracteres no validos.");
            RuleFor(x => x.CustomerIdentificationType)
                .NotEmpty().WithMessage("El campo Tipo de Identificacion Receptor es requerido.")
                .NotNull().WithMessage("El campo Tipo de Identificacion Receptor es requerido.")
                .Matches("(11|12|13|21|22|31|41|42|47|48|50|50|91)$").WithMessage("El campo Tipo de Identificacion Receptor no es soportado.");

            RuleFor(x => x.CustomerIdentification)
                .NotEmpty().WithMessage("El campo Numero de Identificacion Receptor es requerido.")
                .NotNull().WithMessage("El campo Numero de Identificacion Receptor es requerido.")
                .Length(3, 20).WithMessage("El campo Numero de Identificacion Receptor debe contener de 3 a 20 caracteres.")
                .Matches(@"^[0-9]+$").WithMessage("El campo Numero de Identificacion Receptor tiene caracteres no validos.");

            RuleFor(x => x.DateFrom).Cascade(CascadeMode.Stop)
                .Must(x => isValidInitialDate(x)).WithMessage("La Fecha Desde no es una fecha valida.");

            RuleFor(x => x.DateTo).Cascade(CascadeMode.Stop)
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
