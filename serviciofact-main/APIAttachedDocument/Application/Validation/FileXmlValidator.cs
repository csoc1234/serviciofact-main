using APIAttachedDocument.Application.Dto;
using APIAttachedDocument.Domain.Core;
using FluentValidation;

namespace APIAttachedDocument.Application.Validation
{
    public class FileXmlValidator : AbstractValidator<FilesXmlDto>
    {
        private readonly IConfiguration _configuration;

        public FileXmlValidator(IConfiguration configuration)
        {
            _configuration = configuration;

            RuleFor(x => x.Xml).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El Archivo no puede ser vacio")
                .NotEmpty().WithMessage("El Archivo no puede ser vacio")
                .Must(x => BuildDocument.XmlApplicationResponseValid(x)).WithMessage("El archivo no es un Xml permitido")
                .Must(x => BuildDocument.ValidateXSD(x)).WithMessage("El archivo Xml no cumple con la estructura XSD UBL 2.1");

            RuleFor(x => x.XmlDian).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("El Archivo no puede ser vacio")
               .NotEmpty().WithMessage("El Archivo no puede ser vacio")
               .Must(x => BuildDocument.XmlApplicationResponseValid(x)).WithMessage("El archivo no es un Xml permitido");
            //.Must(x => BuildDocument.ValidateXSD(x)).WithMessage("El archivo Xml no cumple con la estructura XSD UBL 2.1");
        }
    }
}
