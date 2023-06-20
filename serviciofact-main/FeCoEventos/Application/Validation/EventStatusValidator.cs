using FeCoEventos.Application.Dto;
using FeCoEventos.Domain.ValueObjects;
using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class EventStatusValidator : AbstractValidator<EventStatusDto>
    {
        public EventStatusValidator()
        {
            RuleFor(x => x.DateFrom).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("La fecha de inicio es requerida")
                .NotEmpty().WithMessage("La fecha de inicio es requerida")
                .Matches(@"^(^20)([0-9]{2})-([0-1][0-9])-([0-3][0-9])(T([0-1][0-9]|[2][0-3]):([0-5][0-9]):([0-5][0-9]))?$").WithMessage("El formato de la fecha de inicio no es valido")
                .Must(x => DateTimeTools.TryParse(x)).WithMessage("El formato de la fecha de inicio no es valido");

            RuleFor(x => x.DateTo).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("La fecha fin es requerida")
                .NotEmpty().WithMessage("La fecha fin es requerida")
                .Matches(@"^(^20)([0-9]{2})-([0-1][0-9])-([0-3][0-9])(T([0-1][0-9]|[2][0-3]):([0-5][0-9]):([0-5][0-9]))?$").WithMessage("El formato de la fecha fin no es valido")
                .Must(x => DateTimeTools.TryParse(x)).WithMessage("El formato de la fecha fin no es valido");

            RuleFor(x => x.EventCode).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El codigo de evento es requerido")
                .NotEmpty().WithMessage("El codigo de evento es requerido")
                .MaximumLength(3).WithMessage("Longitud no valida para el codigo de evento")
                .Matches("^(0|030|031|032|033|034|035|036|037|038|039|040)$").WithMessage("Codigo de evento no soportado");

            RuleFor(x => x.Status).Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 999).WithMessage("El codigo de estatus esta fuera del rango");
        }
    }
}
