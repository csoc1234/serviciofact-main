using DocumentBuildCO.DocumentClass.UBL2._1;
using FeCoEventos.Domain.Core;
using FeCoEventos.Models.Requests;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace FeCoEventos.Application.Validation
{
    public class BuildApplicationResponseValidator : AbstractValidator<EventsBuildRequest>
    {
        private readonly IConfiguration _configuration;
        public BuildApplicationResponseValidator(IConfiguration configuration)
        {
            _configuration = configuration;

            RuleFor(x => x.XmlBase64).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El Archivo XML Base64 es Requerido")
                .NotEmpty().WithMessage("El Archivo XML Base64 es Requerido")
                .Must(x => DocumentBuild.SerializeAttachedDocument(x, _configuration["ProfileExecutionID"])).WithMessage(_configuration["ProfileExecutionID"] == "1" ? "Se está intentando generar un evento en el ambiente de Produccion con un documento electronico o aceptacion DIAN de pruebas." : "Se está intentando generar un evento en el ambiente de Pruebas con un documento electronico o aceptacion DIAN de Produccion.");

            RuleFor(x => x.EventCode).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("El Codigo de Evento es Requerido")
                .NotEmpty().WithMessage("El Codigo de Evento es Requerido")
                .Matches("(030|031|032|033|034|035|036|037|038|039|040|041|042|043|044|045|046)$").WithMessage("Codigo de evento no soportado")
                .MaximumLength(3).WithMessage("Longitud No Válida para Código Evento")
                .MinimumLength(3).WithMessage("Longitud No Válida para Código Evento");

            RuleFor(x => x.CorrelativeNumber).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Número Correlativo es Requerido")
                .NotEmpty().WithMessage("Número Correlativo es Requerido");

            RuleFor(x => x.RejectedCode).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Código de Rechazo es Requerido")
                .NotEmpty().WithMessage("Código de Rechazo es Requerido")
                    .When(x => (!string.IsNullOrEmpty(x.EventCode) ? x.EventCode.Trim() : "") == "031")
                .Matches("(01|02|03|04)$").WithMessage("Código de Rechazo no soportado, solo se admite uno de los siguientes valores 01, 02, 03 ó 04 ")
                .When(x => (!string.IsNullOrEmpty(x.EventCode) ? x.EventCode.Trim() : "") == "031");

            RuleFor(x => x.IssuerParty).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Ejecutante del Evento es Requerido")
                    .When(x => new List<string> { "030", "032" }.Contains(x.EventCode))
                .SetValidator(new IssuerPartyValidator())
                    .When(x => new List<string> { "030", "032" }.Contains(x.EventCode));
            //.SetValidator(new IssuerPartyPartyTaxSchemeValidator())
            //     .When(x => new List<string> { "037", "038", "039", "040" }.Contains(x.EventCode));

            /* RuleFor(x => x.EmailAddress).Cascade(CascadeMode.Stop)
                 .MaximumLength(100).WithMessage("Longitud no valida para el correo electronico")
                 .Matches(@"^(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,8}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*$").WithMessage("No corresponde a un formato válido de Correo Electrónica")
                 .When(x => !String.IsNullOrEmpty(x.EmailAddress));*/

            //RuleForEach(x => x.Note).Cascade(CascadeMode.Stop)
            //    .NotNull().WithMessage("Debe incluir las notas del evento")
            //        .When(x => new List<string> { "034", "035", "036" }.Contains(x.EventCode));

            // RuleFor(x => x.SenderParty).Cascade(CascadeMode.Stop)
            //     .NotNull().WithMessage("La informacion del comprador del evento es requerido")
            //        .When(x => new List<string> { "035", "036" }.Contains(x.EventCode));
            //.SetValidator(new PartyLegalEntityValidator());

            //RuleFor(x => x.CustomTagGeneral).Cascade(CascadeMode.Stop)
            //   .NotNull().WithMessage("La informacion adicional del evento es requerido")
            //       .When(x => new List<string> { "035", "036", "037", "038", "039" }.Contains(x.EventCode));

            //RuleFor(x => x.ReceiverParty).Cascade(CascadeMode.Stop)
            //    .NotNull().WithMessage("La informacion del comprador del evento es requerido")
            //        .When(x => new List<string> { "037" }.Contains(x.EventCode));
            //.SetValidator(new PartyLegalEntityValidator());

            RuleFor(x => x.Environment).Cascade(CascadeMode.Stop)
                .InclusiveBetween(0, 2).WithMessage("Ambiente no permitido");

            RuleFor(x => x.EffectiveDate).Cascade(CascadeMode.Stop)
                .Matches(@"^(^20)([0-9]{2})-([0-1][0-9])-([0-3][0-9])$").WithMessage("EffectiveDate no es valido el formato")
                .When(x => !String.IsNullOrEmpty(x.EffectiveDate));

            RuleFor(x => x.CreatedBy).Cascade(CascadeMode.Stop)
                //.NotNull().WithMessage("El codigo de producto que genera el evento no puede estar vacio")
                .InclusiveBetween(1, 2).WithMessage("El codigo de producto que genera el evento no es permitido");
        }
    }

    public class IssuerPartyValidator : AbstractValidator<IssuerParty>
    {
        public IssuerPartyValidator()
        {
            RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Nombre es Requerido")
                .NotEmpty().WithMessage("Nombre es Requerido")
                .MaximumLength(100).WithMessage("Longitud No Válida para Nombre");

            RuleFor(x => x.FamilyName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Apellido es Requerido")
                .NotEmpty().WithMessage("Apellido es Requerido")
                .MaximumLength(100).WithMessage("Longitud  No Válida para Apellido");

            RuleFor(x => x.ID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Numero de Identificación es Requerido")
                .NotEmpty().WithMessage("Numero de Identificación es Requerido")
                .MaximumLength(20).WithMessage("Longitud No Válida para Numero de Identificación")
                .Matches(@"[a-zA-Z0-9]+$").WithMessage("Valor No Válido para Numero de Identificación");

            RuleFor(x => x.IDSchemeName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Tipo Identificación es Requerido")
                .NotEmpty().WithMessage("Tipo Identificación es Requerido")
                .MaximumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .MinimumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .Matches("(11|12|13|21|22|31|41|42|91)$").WithMessage("Tipo Identificación No Soportado; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");

            RuleFor(x => x.IDSchemeID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Dígito Verificador es Requerido")
                .NotEmpty().WithMessage("Dígito Verificador es Requerido")
                    .When(x => (!string.IsNullOrEmpty(x.IDSchemeName) ? x.IDSchemeName : "") == "31")
                .MaximumLength(1).WithMessage("Longitud No Válida para Dígito Verificador")
                    .When(x => (!string.IsNullOrEmpty(x.IDSchemeName) ? x.IDSchemeName : "") == "31")
                .Matches(@"[0-9]+$").WithMessage("Valor No Válido para Dígito Verificador")
                    .When(x => (!string.IsNullOrEmpty(x.IDSchemeName) ? x.IDSchemeName : "") == "31");

            RuleFor(x => new { x.IDSchemeID, x.ID })
                .Must(x => RigthDV(x.IDSchemeID, x.ID)).WithMessage("Valor del Dígito Verificador no corresponde al Número de Identificación")
                    .When(x => (!string.IsNullOrEmpty(x.IDSchemeName) ? x.IDSchemeName : "") == "31" && !string.IsNullOrEmpty(x.IDSchemeID));

            RuleFor(x => x.JobTitle.Trim())
                .MaximumLength(100).WithMessage("Longitud No Válida para Cargo")
                    .When(x => !String.IsNullOrEmpty(x.JobTitle.Trim())).When(x => x.JobTitle != null);

            RuleFor(x => x.OrganizationDepartment.Trim())
                .MaximumLength(100).WithMessage("Longitud No Válida para Departamento")
                    .When(x => !String.IsNullOrEmpty(x.OrganizationDepartment.Trim())).When(x => x.OrganizationDepartment != null);
        }

        public bool RigthDV(string sdv, string sidentification)
        {
            int iadv; Int64 iidentification = 0;
            bool arenumeric = (int.TryParse(sdv, out iadv) && Int64.TryParse(sidentification, out iidentification));
            if (arenumeric)
            {
                int dv = 0, lengthnumer = sidentification.Trim().Length, intcount = 0;
                string seq = "716759534743413729231917130703".Substring(31 - lengthnumer * 2 - 1, lengthnumer * 2);
                int temp1 = 0, temp2 = 0;
                while ((intcount < lengthnumer) && arenumeric)
                {
                    arenumeric = int.TryParse(seq.Substring(0, 2), out temp1) &&
                                 int.TryParse(sidentification.Substring(intcount, 1), out temp2);
                    if (arenumeric)
                    {
                        dv = dv + temp1 * temp2;
                        intcount++;
                        seq = seq.Substring(2, (lengthnumer - intcount) * 2);
                    }
                }
                if (arenumeric)
                {

                    dv = 11 - (dv - (int)Math.Floor((decimal)(dv / 11)) * 11);
                    dv = (dv > 9) ? 11 - dv : dv;

                    arenumeric = (dv == iadv);
                }

            }
            return arenumeric;
        }
    }

    public class IssuerPartyPartyTaxSchemeValidator : AbstractValidator<IssuerParty>
    {
        public IssuerPartyPartyTaxSchemeValidator()
        {
            RuleFor(x => x.PartyTaxScheme).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("Nombre es Requerido");
            //.SetValidator(new PartyTaxSchemeValidator());
        }
    }

    public class PartyLegalEntityValidator : AbstractValidator<PartyLegalEntityAR>
    {
        public PartyLegalEntityValidator()
        {
            RuleFor(x => x.RegistrationName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Nombre o Razón Social del endosatario es Requerido")
                .NotEmpty().WithMessage("Nombre o Razón Social del endosatario es Requerido")
                .MinimumLength(5).WithMessage("Nombre o Razón Social del endosatario no cumple con la longitud minima permitida")
                .MaximumLength(450).WithMessage("Nombre o Razón Social del endosatario no cumple con la longitud maxima permitida");

            RuleFor(x => x.CompanyID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Numero de Identificación es Requerido")
                .NotEmpty().WithMessage("Numero de Identificación es Requerido")
                .MaximumLength(20).WithMessage("Longitud No Válida para Numero de Identificación")
                .Matches(@"[a-zA-Z0-9]+$").WithMessage("Valor No Válido para Numero de Identificación");

            RuleFor(x => x.CompanyIDSchemeName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Tipo Identificación es Requerido")
                .NotEmpty().WithMessage("Tipo Identificación es Requerido")
                .MaximumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .MinimumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .Matches("(11|12|13|21|22|31|41|42|91)$").WithMessage("Tipo Identificación No Soportado; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");

            RuleFor(x => x.CompanyIDSchemeID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Dígito Verificador es Requerido")
                .NotEmpty().WithMessage("Dígito Verificador es Requerido")
                    .When(x => x.CompanyIDSchemeName == "31")
                .MaximumLength(1).WithMessage("Longitud No Válida para Dígito Verificador")
                    .When(x => x.CompanyIDSchemeName == "31")
                .Matches(@"[0-9]+$").WithMessage("Valor No Válido para Dígito Verificador")
                    .When(x => x.CompanyIDSchemeName == "31");

            RuleFor(x => x.CompanyIDSchemeVersionID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Tipo Identificación es Requerido")
                .NotEmpty().WithMessage("Tipo Identificación es Requerido")
                .Matches("(1|2)$").WithMessage("Tipo Identificación No Soportado; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");


        }

    }

    public class PartyTaxSchemeValidator : AbstractValidator<PartyTaxSchemeAR>
    {
        public PartyTaxSchemeValidator()
        {
            RuleFor(x => x.RegistrationName).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("Nombre o Razón Social del endosatario es Requerido")
               .NotEmpty().WithMessage("Nombre o Razón Social del endosatario es Requerido")
               .MinimumLength(5).WithMessage("Nombre o Razón Social del endosatario no cumple con la longitud minima permitida")
               .MaximumLength(450).WithMessage("Nombre o Razón Social del endosatario no cumple con la longitud maxima permitida");

            RuleFor(x => x.CompanyID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Numero de Identificación es Requerido")
                .NotEmpty().WithMessage("Numero de Identificación es Requerido")
                .MaximumLength(20).WithMessage("Longitud No Válida para Numero de Identificación")
                .Matches(@"[a-zA-Z0-9]+$").WithMessage("Valor No Válido para Numero de Identificación");

            RuleFor(x => x.CompanyIDSchemeName).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Tipo Identificación es Requerido")
                .NotEmpty().WithMessage("Tipo Identificación es Requerido")
                .MaximumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .MinimumLength(2).WithMessage("Longitud No Válida para IDSchemeName")
                .Matches("(11|12|13|21|22|31|41|42|91)$").WithMessage("Tipo Identificación No Soportado; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");

            RuleFor(x => x.CompanyIDSchemeID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Dígito Verificador es Requerido")
                .NotEmpty().WithMessage("Dígito Verificador es Requerido")
                    .When(x => x.CompanyIDSchemeName == "31")
                .MaximumLength(1).WithMessage("Longitud No Válida para Dígito Verificador")
                    .When(x => x.CompanyIDSchemeName == "31")
                .Matches(@"[0-9]+$").WithMessage("Valor No Válido para Dígito Verificador")
                    .When(x => x.CompanyIDSchemeName == "31");

            RuleFor(x => x.CompanyIDSchemeVersionID).Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Tipo Identificación es Requerido")
                .NotEmpty().WithMessage("Tipo Identificación es Requerido")
                .Matches("(1|2)$").WithMessage("Tipo Identificación No Soportado; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");

            RuleFor(x => x.TaxSchemeID).Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("Codigo es Requerido")
               .NotEmpty().WithMessage("Codigo es Requerido");

            RuleFor(x => x.TaxSchemeName).Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Descripcion del codigo es Requerido")
            .NotEmpty().WithMessage("Descripcion del codigo es Requerido");

        }
    }

    public class CustomTagGeneralValidator : AbstractValidator<CustomTagGeneral>
    {
        public CustomTagGeneralValidator()
        {

        }
    }

}
