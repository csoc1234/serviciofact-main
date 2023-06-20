using FeCoEventos.Application.Dto;
using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class EventUpdateValidator : AbstractValidator<EventDeliveryAsyncDto>
    {
        public EventUpdateValidator()
        {
            RuleFor(x => x.TrackIdDian).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("El trackid es requerido")
                    .When(x => x.Status == 204)
               .NotEmpty().WithMessage("El trackid es requerido")
                    .When(x => x.Status == 204)
               .Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$").WithMessage("Estructura del Track ID de la DIAN no valida")
                    .When(x => !string.IsNullOrEmpty(x.TrackIdDian));

            RuleFor(x => x.Status)
                .NotNull().WithMessage("El estatus de evento es requerido")
                .GreaterThan(0).WithMessage("Longitud del estatus de evento no valida")
                .LessThan(999).WithMessage("Longitud del estatus de evento no valida");
        }
    }
}
