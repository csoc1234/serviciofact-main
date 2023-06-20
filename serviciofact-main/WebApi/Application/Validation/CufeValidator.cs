using FluentValidation;

namespace WebApi.Application.Validation
{
    public class CufeValidator : AbstractValidator<string>
    {
        public CufeValidator()
        {
            RuleFor(x => x).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El CUFE es requerido")
                .NotEmpty().WithMessage("El CUFE es requerido")
                .Matches(@"^([a-zA-Z0-9]{96})$").WithMessage("El formato del CUFE es incorrecto");
        }
    }
}
