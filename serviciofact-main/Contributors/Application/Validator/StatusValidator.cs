using Contributors.Application.Dto;
using FluentValidation;

namespace Contributors.Application.Validator
{
    public class StatusValidator : AbstractValidator<StatusDto>
    {
        public StatusValidator()
        {
            RuleFor(x => x.Status).Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 2).WithMessage("El estatus de habilitacion no valido");
        }
    }
}
