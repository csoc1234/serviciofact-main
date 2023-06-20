using FluentValidation;

namespace FeCoEventos.Application.Validation
{
    public class FileTypeValidator : AbstractValidator<int>
    {
        public FileTypeValidator()
        {
            RuleFor(x => x)
                 .InclusiveBetween(1, 3).WithMessage("El codigo del tipo de archivo no es permitido");
        }
    }
}
