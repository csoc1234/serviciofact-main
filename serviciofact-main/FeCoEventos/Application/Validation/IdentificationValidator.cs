using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class IdentificationValidator : AbstractValidator<string>
    {
        public IdentificationValidator()
        {
            RuleFor(x => x).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("El Numero de Identificacion es requerido")                    
               .NotEmpty().WithMessage("El Numero de Identificacion es requerido")                    
               .Matches(@"^[0-9]{3,20}$").WithMessage("El Numero de Identificacion no validos");
        }
    }
}
