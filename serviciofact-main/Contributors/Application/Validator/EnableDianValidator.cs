using Contributors.Application.Dto;
using FluentValidation;

namespace Contributors.Application.Validator
{
    public class EnableDianValidator : AbstractValidator<TaxPayersDto>
    {
        public EnableDianValidator()
        {
            RuleFor(x => x.CompanyId).Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("El numero de identificacion es requerido")
                 .NotEmpty().WithMessage("El numero de identificacion es requerido")
                 .Matches(@"^([0-9]{3,20})$").WithMessage("El numero de evento tiene un formato invalido");

            RuleFor(x => x.TestSetId).Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("El TestSetId es requerido")
                 .NotEmpty().WithMessage("El TestSetId es requerido")
                 .Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$").WithMessage("El TestSetId tiene un formato invalido")                 ;
        }
    }
}
