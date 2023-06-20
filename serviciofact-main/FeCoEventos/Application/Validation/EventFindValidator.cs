using FeCoEventos.Application.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeCoEventos.Application.Validation
{
    public class EventFindValidator : AbstractValidator<EventDto>
    {
        public EventFindValidator()
        {
            RuleFor(x => x.EventId).Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("El numero de evento es requerido")
                  .NotEmpty().WithMessage("El numero de evento es requerido")
                  .Matches(@"^([a-zA-Z0-9]{0,10})([0-9]{1,20})$").WithMessage("El numero de evento tiene un formato invalido")
                  .MinimumLength(1).WithMessage("Longitud no valida para el numero de evento")
                  .MaximumLength(50).WithMessage("Longitud no valida para el numero de evento");

            RuleFor(x => x.TrackId).Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("El trackid es requerido")
                  .NotEmpty().WithMessage("El trackid es requerido")
                  .Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$").WithMessage("El track id tiene un formato invalido")
                  .MinimumLength(36).WithMessage("Longitud no valida para el trackId")
                  .MaximumLength(36).WithMessage("Longitud no valida para el trackId");
        }
    }
}
