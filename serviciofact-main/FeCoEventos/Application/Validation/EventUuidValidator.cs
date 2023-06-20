using FeCoEventos.Application.Dto;
using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class EventUuidValidator : AbstractValidator<EventUuidDto>
    {
        public EventUuidValidator()
        {
            RuleFor(x => x.EventId).Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("El numero de evento es requerido")
                    .NotEmpty().WithMessage("El numero de evento es requerido")
                    .Matches(@"^([a-zA-Z0-9]{0,10})([0-9]{1,20})$").WithMessage("El numero de evento tiene un formato invalido")
                    .MinimumLength(1).WithMessage("Longitud no valida para el numero de evento")
                    .MaximumLength(50).WithMessage("Longitud no valida para el numero de evento");

            RuleFor(x => x.EventUuid).Cascade(CascadeMode.Stop)
                    .NotNull().WithMessage("El CUFE del evento es requerido")
                    .NotEmpty().WithMessage("El CUFE del evento es requerido")
                    .Matches(@"^([a-zA-Z0-9]{96})$").WithMessage("El formato del CUFE del evento es incorrecto");
        }
    }
}
