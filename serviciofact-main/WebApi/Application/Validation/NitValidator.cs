using FluentValidation;

namespace WebApi.Application.Validation
{
    public class NitValidator : AbstractValidator<string>
    {
        public NitValidator()
        {
            RuleFor(x => x).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("El Numero de Identificacion es requerido")
               .NotEmpty().WithMessage("El Numero de Identificacion es requerido")
               .Matches(@"^[0-9]{3,20}$").WithMessage("El Numero de Identificacion no valido");
        }
    }
}
