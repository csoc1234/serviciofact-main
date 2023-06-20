using DocumentBuildCO.DocumentClass.UBL2._1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApi.Application.Dto;
using WebApi.Application.Interface;
using WebApi.Domain.Core;
using WebApi.Domain.Entity;
using WebApi.Domain.Interface;
using WebApi.Infrastructure.ComunicationDian;
using WebApi.Infrastructure.Data.Context;
using WebApi.Infrastructure.Data.Repository;
using TFHKA.LogsMongo;
using WebApi.Models.Response;
using static WebApi.Models.Response.InvoicesPerTaxpayerResponse;

namespace WebApi.Application.Invoice
{
    public class GestionInvoice : IGestionInvoice
    {
        private readonly FactoringDbContext _context;
        private readonly EmisionDbContext _contextE21;
        private IConfiguration _configuration;
        private readonly IInvoiceDomain _invoiceDomain;
        private readonly IDianStatusRestClient _dianStatusRestClient;

        public GestionInvoice(EmisionDbContext contextE21, FactoringDbContext context, IInvoiceDomain invoiceDomain, IConfiguration configuration, IDianStatusRestClient dianStatusRestClient)
        {
            _context = context;
            _contextE21 = contextE21;
            _invoiceDomain = invoiceDomain;
            _configuration = configuration;
            _dianStatusRestClient = dianStatusRestClient;
        }
        //HU o Caso de Uso
        public ResponseDto GetInfoInvoice(InvoiceDto invoiceDto, ILogMongo log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            ResponseDto responseDto = null;

            try
            {
                //LLamo a Infraestructura y Dominio

                IInvoiceRepository repository = new InvoiceRepository(_contextE21);

                Invoice21Domain invoice = repository.GetEmi21ById(invoiceDto.IdTable);

                if (invoice != null) // Si existe en DB
                {
                    if (invoice.Invoice.Id > 0) // Si existe en DB
                    {
                        //Validamos si esa Factura tiene Docreferenciados asociados
                        repository = new InvoiceRepository(_context);
                        int pin = repository.ValidateInvoiceWhitReferenceDoc(invoice.Invoice.Id);
                        if (pin == 0)
                        {


                            //Validaciones del XML DocumentBuilCo
                            TFHKA.Storage.Fileshare.Client.Models.StorageFileResponse xml = _invoiceDomain.FindXMLFileShare(invoice.Invoice.PathFile, invoice.Invoice.DocumentId + "_AttachmentDocument.xml", log);

                            if (xml.Code == 200)
                            {
                                ValidateXmlResponse resultValidate = invoice.ValidateDocument(xml.File);
                                string message = resultValidate.Message;
                                //Validar  tambien Estatus en La DIAN
                                if (resultValidate.Valid)
                                {
                                    BaseDocument21 doc = resultValidate.Document;

                                    DianStatusResponse resultStatus = _dianStatusRestClient.GetDianStatus(invoice.Invoice.Cufe, log);
                                    if (resultStatus.Code == 200 && resultStatus.statusCode == "00")
                                    {
                                        responseDto = new ResponseDto
                                        {
                                            Code = 200,
                                            Message = "Factura Negociable para Facturing",
                                            IsValid = true,
                                            Invoice = new InvoiceDto
                                            {
                                                IdTable = invoice.Invoice.Id,
                                                DocumentId = doc.ID,
                                                InvoiceUuid = doc.UUID,
                                                InvoiceUuidType = doc.UUID_schemeName,

                                                EnterpriseId = invoice.Invoice.IdEnterprise,
                                                CustomerIdentification = doc.AccountingCustomerParty?.TaxScheme?.CompanyID,
                                                CustomerTypeIdentification = doc.AccountingCustomerParty?.TaxScheme?.SchemeName,
                                                SupplierIdentification = doc.AcountingSupplierParty?.TaxScheme?.CompanyID,
                                                SupplierTypeIdentification = doc.AcountingSupplierParty?.TaxScheme?.SchemeName,

                                                PayableAmount = doc.LegalMonetaryTotal.PayableAmount,

                                                PaymentMeansID = doc.PaymentMeans.FirstOrDefault().ID,
                                                PaymentMeansCode = doc.PaymentMeans.FirstOrDefault().PaymentMeansCode,
                                                PaymentDueDate = doc.PaymentMeans.FirstOrDefault().PaymentDueDate,

                                                PathFileXml = invoice.Invoice.PathFile,
                                                NameFileXml = $"{doc.ID}.xml",
                                                InvoiceIssuedate = DateTime.Parse($"{doc.IssueDate.ToShortDateString()} {doc.IssueTime.ToShortTimeString()}"),
                                                ValidateDianDatetime = resultValidate.ValidateDianDatetime
                                            }
                                        };
                                    }
                                    else
                                    {
                                        responseDto = new ResponseDto
                                        {
                                            Code = 104,
                                            Message = "Factura No es válida en la DIAN",
                                            IsValid = false
                                        };
                                    }
                                }
                                else
                                {

                                    responseDto = new ResponseDto
                                    {
                                        Code = 200,
                                        Message = $"Factura No cumple con los requerimientos para Facturing: {message}",
                                        IsValid = false
                                    };
                                }
                            }
                            else
                            {
                                responseDto = new ResponseDto
                                {
                                    Code = 103,
                                    Message = "No existe XML File en Azure Storage del InvoiceId = " + invoiceDto.IdTable,
                                    IsValid = false
                                };
                            }
                        }
                        else
                        {
                            responseDto = new ResponseDto
                            {
                                Code = 103,
                                Message = "No existe el documento referenciado = " + invoice.Invoice.Id,
                                IsValid = false
                            };
                        }
                    }
                    else
                    {
                        responseDto = new ResponseDto
                        {
                            Code = 103,
                            Message = "No existe Factura en Emision con InvoiceId = " + invoiceDto.IdTable,
                            IsValid = false
                        };
                    }
                }
                else
                {
                    responseDto = new ResponseDto
                    {
                        Code = 103,
                        Message = "No existe Factura en Emision con InvoiceId = " + invoiceDto.IdTable,
                        IsValid = false
                    };
                }

                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto = new ResponseDto
                {
                    Code = 500,
                    Message = ex.Message,
                    IsValid = false
                };

                return responseDto;
            }
        }

        //HU o Caso de Uso de listar
        public InvoicesPerTaxpayerResponse GetInvoicesPerTaxpayerList(InvoicesPerTaxpayerDto invoicesPerTaxpayerDto, ILogMongo log)
        {
            InvoicesPerTaxpayerResponse response = new InvoicesPerTaxpayerResponse();

            try
            {
                string dateRegex = @"^(?:(?:[0-9]{4})[-](?:(?:0[1-9])|(?:1[0-2]))[-](?:(?:0[1-9])|(?:1[0-9])|(?:2[0-9])|(?:3[0-1])))$";
                string dateTimeRegex = @"^(?:(?:[0-9]{4})[-](?:(?:0[1-9])|(?:1[0-2]))[-](?:(?:0[1-9])|(?:1[0-9])|(?:2[0-9])|(?:3[0-1])))T(?:(?:[0-1][0-9])|(?:2[0-3])):[0-5][0-9]:[0-5][0-9]$";

                DateTime dateFrom = DateTime.Now;
                DateTime dateTo = DateTime.Now;

                if (Regex.IsMatch(invoicesPerTaxpayerDto.DateFrom, dateRegex))
                {
                    dateFrom = Convert.ToDateTime(invoicesPerTaxpayerDto.DateFrom + " 00:00:00");
                }
                else if (Regex.IsMatch(invoicesPerTaxpayerDto.DateFrom, dateTimeRegex))
                {
                    dateFrom = Convert.ToDateTime(invoicesPerTaxpayerDto.DateFrom);
                }
                else
                {
                    //Formato invalido 400
                    return new InvoicesPerTaxpayerResponse
                    {
                        Code = 400,
                        Message = "Formato de fecha errado"
                    };
                }

                if (Regex.IsMatch(invoicesPerTaxpayerDto.DateTo, dateRegex))
                {
                    dateTo = Convert.ToDateTime(invoicesPerTaxpayerDto.DateTo + " 23:59:59");
                }
                else if (Regex.IsMatch(invoicesPerTaxpayerDto.DateTo, dateTimeRegex))
                {
                    dateTo = Convert.ToDateTime(invoicesPerTaxpayerDto.DateTo);
                }
                else
                {
                    //Formato invalido 400
                    return new InvoicesPerTaxpayerResponse
                    {
                        Code = 400,
                        Message = "Formato de fecha errado"
                    };
                }

                IInvoiceRepository repository = new InvoiceRepository(_contextE21);
                List<Invoice21Domain> invoicesPerTaxpayerList = repository.GetInvoicesPerTaxpayer(invoicesPerTaxpayerDto.IdEnterprise, dateFrom, dateTo);
                if (invoicesPerTaxpayerList.Count > 0)
                {
                    response.Code = 200;
                    response.Message = "Se retorna la lista de facturas por contribuyente";
                    response.InvoicesList = new List<InvoicesPerTaxpayerList>();
                    foreach (Invoice21Domain item in invoicesPerTaxpayerList)
                    {
                        InvoicesPerTaxpayerList row = new InvoicesPerTaxpayerList
                        {
                            Id = item.Invoice.Id,
                            DocumentId = item.Invoice.DocumentId,
                            UUID = item.Invoice.Cufe,
                            trackId = item.Invoice.TrackId,
                            pathFile = item.Invoice.PathFile,
                            nameFile = item.Invoice.NameFile
                        };

                        response.InvoicesList.Add(row);
                    }
                }
                else
                {
                    response.Code = 100;
                    response.Message = "No se encontro ninguna Factura para la empresa con id = " + invoicesPerTaxpayerDto.IdEnterprise + " activa, con status 200 en el rango dado.";
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        //HU o Caso de Uso de Persistencia a InvoicesFactoring
        public ResponseDto AddInfoInvoice(InvoiceDto invoiceDto, ILogMongo log)
        {
            //Persite en BD la Info de una Factura Negociable en el Catalogo
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            ResponseDto responseDto = null;

            try
            {
                //LLamo a Infraestructura y Dominio
                IRepository<InvoiceFactoring> repository = new RepositoryBase<InvoiceFactoring>(_context);
                //Mapeo de DTO a EntityDomain
                InvoiceFactoring invoice = new InvoiceFactoring
                {
                    DocumentId = invoiceDto.DocumentId,
                    PathFileXml = invoiceDto.PathFileXml,
                    InvoiceUuid = invoiceDto.InvoiceUuid,
                    EnterpriseId = invoiceDto.EnterpriseId,
                    InvoiceIssuedate = invoiceDto.InvoiceIssuedate,
                    InvoiceId = DateTime.Now.Millisecond,
                    CustomerIdentification = invoiceDto.CustomerIdentification,
                    CustomerTypeIdentification = invoiceDto.CustomerTypeIdentification,
                    SupplierIdentification = invoiceDto.SupplierIdentification,
                    SupplierTypeIdentification = invoiceDto.SupplierTypeIdentification,
                    InvoiceUuidType = invoiceDto.InvoiceUuidType,
                    InvoiceAuthorization = "-",
                    InvoiceNumber = 0,
                    PayableAmount = invoiceDto.PayableAmount,
                    PaymentDate = invoiceDto.PaymentDueDate.Value
                };

                //Valido logica de Negocio en Dominio
                //Validaciones del XML DocumentBuilCo
                TFHKA.Storage.Fileshare.Client.Models.StorageFileResponse xml = _invoiceDomain.FindXMLFileShare(invoice.PathFileXml, invoice.DocumentId + "_AttachmentDocument.xml", log);

                if (xml.Code == 200)
                {
                    BaseDocument21 doc = null;
                    string message = string.Empty;
                    ValidateXmlResponse resultValidate = _invoiceDomain.ValidateDocument(xml.File);

                    if (resultValidate.Valid)
                    {
                        //Valido la situacion de la Factura en los Registros de la DIAN
                        DianStatusResponse resultStatus = _dianStatusRestClient.GetDianStatus(invoice.InvoiceUuid, log);

                        bool valid = (resultStatus.Code == 200 && resultStatus.statusCode == "00");

                        if (valid)
                        {
                            responseDto = new ResponseDto
                            {
                                Code = 200,
                                Message = "Factura Negociable para Facturing",
                                IsValid = true,
                                Invoice = invoiceDto
                            };
                        }
                        else
                        {
                            responseDto = new ResponseDto
                            {
                                Code = 500,
                                Message = resultStatus.Message,
                                IsValid = false,
                                Invoice = null
                            };
                        }
                    }
                    else
                    {
                        responseDto = new ResponseDto
                        {
                            Code = 500,
                            Message = "Factura No cumple con los requerimientos para Facturing",
                            IsValid = false
                        };

                        return responseDto;

                    }
                }
                else
                {

                    responseDto = new ResponseDto
                    {
                        Code = 103,
                        Message = "No existe XML File en Azure Storage del Consecutivo = " + invoice.DocumentId,
                        IsValid = false
                    };

                    return responseDto;
                }

                //Llamo a Infraestructura para persistir informacion
                repository.Add(invoice);
                //_unitOfWork.Commit();

                responseDto = new ResponseDto
                {
                    Code = 200,
                    Message = "Informacion de Factura Guardada Exitosamente",
                    IsValid = true,
                    Invoice = invoiceDto
                };

                return responseDto;

            }
            catch (Exception ex)
            {
                responseDto = new ResponseDto
                {
                    Code = 500,
                    Message = ex.Message,
                    IsValid = false
                };

                return responseDto;
            }
        }

        public InvoicesPerTaxpayerResponse GetInvoicesInHabilitation(string nit, ILogMongo log)
        {
            InvoicesPerTaxpayerResponse response = new InvoicesPerTaxpayerResponse();

            try
            {
                IInvoiceRepository repository = new InvoiceRepository(_contextE21);
                List<Invoice21Domain> invoicesPerTaxpayerList = repository.GetInvoicesInHab(nit);
                if (invoicesPerTaxpayerList.Count > 0)
                {
                    response.Code = 200;
                    response.Message = "Se retorna la lista de facturas de habilitacion del contribuyente";
                    response.InvoicesList = new List<InvoicesPerTaxpayerList>();
                    foreach (Invoice21Domain item in invoicesPerTaxpayerList)
                    {
                        InvoicesPerTaxpayerList row = new InvoicesPerTaxpayerList
                        {
                            Id = item.Invoice.Id,
                            DocumentId = item.Invoice.DocumentId,
                            UUID = item.Invoice.Cufe,
                            trackId = item.Invoice.TrackId,
                            pathFile = item.Invoice.PathFile,
                            nameFile = item.Invoice.NameFile
                        };

                        response.InvoicesList.Add(row);
                    }
                }
                else
                {
                    response.Code = 100;
                    response.Message = "No se encontro ninguna Factura para la empresa con nit = " + nit + " activas en habilitacion.";
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}