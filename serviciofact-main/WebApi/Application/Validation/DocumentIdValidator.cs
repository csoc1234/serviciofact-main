using FluentValidation;

namespace WebApi.Application.Validation
{
    public class DocumentIdValidator : AbstractValidator<string>
    {
        public DocumentIdValidator()
        {
            RuleFor(x => x).Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("El numero de documento es requerido")
                  .NotEmpty().WithMessage("El numero de documento es requerido")
                  .Matches(@"^([a-zA-Z0-9-]{0,10})([0-9]{1,20})$").WithMessage("El numero de documento tiene un formato invalido")
                  .MinimumLength(1).WithMessage("Longitud no valida para el numero de documento")
                  .MaximumLength(50).WithMessage("Longitud no valida para el numero de documento");
        }
    }
}
