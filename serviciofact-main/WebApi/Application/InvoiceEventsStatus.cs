using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TFHKA.LogsMongo;
using WebApi.Application.Dto;
using WebApi.Application.Interface;
using WebApi.Application.Validation;
using WebApi.Domain.Interface;

namespace WebApi.Application
{
    public class InvoiceEventsStatus : IInvoiceEventsStatus
    {
        private readonly IConfiguration _configuration;
        private static ILogMongo log;
        private readonly IInvoiceLastStatusDomain _invoiceLastStatusDomain;

        public InvoiceEventsStatus(IConfiguration configuration, ILogMongo LogMongo, IInvoiceLastStatusDomain invoiceLastStatusDomain)
        {
            _configuration = configuration;
            log = LogMongo;
            _invoiceLastStatusDomain = invoiceLastStatusDomain;
        }


        public InvoiceStatusDianDto GetLastStatus(string supplierId, string documentId, string cufe, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            log.Setup(logRequest);

            InvoiceStatusDianDto response = new InvoiceStatusDianDto();

            try
            {
                supplierId = supplierId == null ? "" : supplierId;
                NitValidator nitValidator = new NitValidator();
                ValidationResult nitValidation = nitValidator.Validate(supplierId);

                CufeValidator cufeValidator = new CufeValidator();
                cufe = String.IsNullOrEmpty(cufe) ? "" : cufe.ToLower();
                ValidationResult cufeValidation = cufeValidator.Validate(cufe);

                DocumentIdValidator documentIdValidator = new DocumentIdValidator();
                documentId = documentId == null ? "" : documentId;

                ValidationResult documentIdValidation = documentIdValidator.Validate(documentId);

                string resultMessage = "";

                if (!nitValidation.IsValid)
                {
                    resultMessage = string.Join("; ", nitValidation.Errors.Select(x => x.ErrorMessage));
                }

                if (!cufeValidation.IsValid)
                {
                    resultMessage = resultMessage + string.Join("; ", cufeValidation.Errors.Select(x => x.ErrorMessage));
                }

                if (!documentIdValidation.IsValid)
                {
                    resultMessage = resultMessage + string.Join("; ", documentIdValidation.Errors.Select(x => x.ErrorMessage));
                }

                if (!nitValidation.IsValid || !cufeValidation.IsValid || !documentIdValidation.IsValid)
                {
                    InvoiceStatusDianDto result = new InvoiceStatusDianDto
                    {
                        Code = 400,
                        Message = resultMessage
                    };

                    log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                    return result;
                }
                else
                {
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Validación Cufe OK", LevelMsn.Info);
                }

                Domain.Entity.InvoiceEventsStatusDian invoiceLastStatusResponse = _invoiceLastStatusDomain.GetStatusInvoice(supplierId, documentId, cufe, log);
                if (invoiceLastStatusResponse.Code == 200 || invoiceLastStatusResponse.Code == 201)
                {
                    response = new InvoiceStatusDianDto
                    {
                        Code = invoiceLastStatusResponse.Code,
                        Message = invoiceLastStatusResponse.Message,
                        InvoiceStatusCode = invoiceLastStatusResponse.StatusCode,
                        InvoiceStatusDesc = invoiceLastStatusResponse.StatusMessage,
                        StatusDian = new InvoiceStatus
                        {

                            InvoiceSupplierIdentification = invoiceLastStatusResponse.Invoice.SupplierIdentification,
                            InvoiceSupplierTypeIdentification = invoiceLastStatusResponse.Invoice.SupplierTypeIdentification,
                            InvoiceCustomerIdentification = invoiceLastStatusResponse.Invoice.CustomerIdentification,
                            InvoiceCustomerTypeIdentification = invoiceLastStatusResponse.Invoice.CustomerTypeIdentification,
                            Uuid = invoiceLastStatusResponse.Invoice.InvoiceUuid,
                            DocumentId = invoiceLastStatusResponse.Invoice.DocumentId,
                            CreatedAt = invoiceLastStatusResponse.Invoice.CreatedAt,
                            UpdatedAt = invoiceLastStatusResponse.Invoice.UpdatedAt,
                            IsValid = invoiceLastStatusResponse.Invoice.IsValid,
                            Status = invoiceLastStatusResponse.Invoice.StatusCode
                        },
                        Events = invoiceLastStatusResponse.Events,
                        TieneEventos = invoiceLastStatusResponse.TieneEventos,
                        CantidadEventos = invoiceLastStatusResponse.CantidadEventos,
                        EsRecibida = invoiceLastStatusResponse.EsRecibida,
                        EsReclamada = invoiceLastStatusResponse.EsReclamada,
                        EsBienoServicio = invoiceLastStatusResponse.EsBienoServicio,
                        EsAceptada = invoiceLastStatusResponse.EsAceptada,
                        EsAceptadaTacitamente = invoiceLastStatusResponse.EsAceptadaTacitamente,
                        EsTituloValor = invoiceLastStatusResponse.EsTituloValor,
                        EstaEndosada = invoiceLastStatusResponse.EstaEndosada
                    };
                }
                else
                {
                    response = new InvoiceStatusDianDto
                    {
                        Code = invoiceLastStatusResponse.Code,
                        Message = invoiceLastStatusResponse.Message,
                        InvoiceStatusCode = invoiceLastStatusResponse.StatusCode,
                        InvoiceStatusDesc = invoiceLastStatusResponse.StatusMessage
                    };
                }

                if (response == null)
                {
                    response = new InvoiceStatusDianDto
                    {
                        Code = 500,
                        Message = "Error al consultar el último status asociado al CUFE " + cufe
                    };
                }

            }
            catch (Exception ex)
            {
                response = new InvoiceStatusDianDto
                {
                    Code = 500,
                    Message = "Error al consultar el último status asociado al CUFE " + cufe
                };

                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
            }

            log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

            return response;
        }
    }
}
