using FeCoEventos.Application.Dto;
using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class FileTypeExternalValidator : AbstractValidator<DownloadFileExternalDto>
    {

        public FileTypeExternalValidator()
        {
            RuleFor(x => x.uuid).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El CUFE es requerido")
                .NotEmpty().WithMessage("El CUFE es requerido")
                .Matches(@"^([a-zA-Z0-9]{96})$").WithMessage("El formato del CUFE es incorrecto");

            RuleFor(x => x.DocumentId).Cascade(CascadeMode.Stop)
                  .NotNull().WithMessage("El numero de documento es requerido")
                  .NotEmpty().WithMessage("El numero de documento es requerido")
                  .Matches(@"^([a-zA-Z0-9]{0,10})([0-9]{1,20})$").WithMessage("El numero de documento tiene un formato invalido");

            RuleFor(x => x.EventType).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El Codigo de Evento es Requerido")
                .NotEmpty().WithMessage("El Codigo de Evento es Requerido")
                .Matches("(030|031|032|033|034|035|036|037|038|039|040)$").WithMessage("Codigo de evento no soportado")
                .MaximumLength(3).WithMessage("Longitud No Válida para Código Evento")
                .MinimumLength(3).WithMessage("Longitud No Válida para Código Evento");

            RuleFor(x => x.FileType)
                 .InclusiveBetween(1, 3).WithMessage("El codigo del tipo de archivo no es permitido");
        }

    }
}
