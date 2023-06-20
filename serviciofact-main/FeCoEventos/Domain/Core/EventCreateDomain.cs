using DocumentBuildCO.DocumentClass.UBL2._1;
using DocumentBuildCO.Response;
using FeCoEventos.Clients.Signed;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Domain.Core
{
    public class EventCreateDomain : IEventCreateDomain
    {
        private readonly IConfiguration _configuration;
        private readonly ISignedClient _signedClient;
        private readonly IStorageFiles _storageFiles;
        private readonly IApplicationDbContext _dbContext;
        private readonly IEmailClient _emailClient;
        private readonly IDocumentBuild _documentBuild;
        private readonly ICertificatesClient _certificatesClient;

        public EventCreateDomain(IConfiguration configuration, ISignedClient signedClient, IStorageFiles storageFiles, IApplicationDbContext dbContext, IEmailClient emailClient, IDocumentBuild documentBuild, ICertificatesClient certificatesClient)
        {
            _configuration = configuration;
            _signedClient = signedClient;
            _storageFiles = storageFiles;
            _dbContext = dbContext;
            _emailClient = emailClient;
            _documentBuild = documentBuild;
            _certificatesClient = certificatesClient;
        }

        public EventsBuildResponse Create(EventsBuildRequest request, CustomJwtTokenContext context, string tokenJWT, ILogAzure log)
        {
            try
            {
                string env = _configuration["Environment"].Trim().ToLower();
                BuildResponse response = new BuildResponse();

                BaseDocument21 xmlInvoice;

                try
                {
                    request.XmlBase64 = _documentBuild.BOMFomartXml(request.XmlBase64);

                    if (string.IsNullOrEmpty(request.XmlBase64))
                    {
                        return new EventsBuildResponse { Code = 422, Message = "El archivo xml no pudo ser leido con exito, es posible que tenga errores de codificacion" };
                    }

                    //request.XmlBase64 = DocumentBuildCO.Common.Utils.AttachmentDocument_RepairXMLStructure(request.XmlBase64); //Intento reparar problemas en el XML del AttachedDocument

                    string xmlInvoicePlain = _documentBuild.GetInvoiceXml(env, request.XmlBase64);

                    string xmlEncode = DocumentBuildCO.Common.Utils.EncodeToBase64(xmlInvoicePlain);

                    xmlInvoicePlain = DocumentBuildCO.Common.Utils.DecodeFromBase64(xmlEncode);

                    xmlInvoice = _documentBuild.SerializeDocument(env, xmlInvoicePlain);

                    //Si el email viene en el xml Invoice asigno ese email
                    //Si no usar el email del request
                    if (_configuration["Email:NotificationEmailFromInvoiceXml"] == "1")
                    {
                        if (xmlInvoice != null)
                        {
                            request.EmailAddress = !string.IsNullOrEmpty(xmlInvoice.AcountingSupplierParty?.ASContact?.ElectronicMail) ? xmlInvoice.AcountingSupplierParty.ASContact.ElectronicMail : request.EmailAddress;
                        }
                    }

                }
                catch (Exception ex)
                {
                    return new EventsBuildResponse()
                    {
                        Code = 500,
                        Message = ex.Message
                    };
                }

                //Se obtiene el certificado y se escribe en la VM
                CertificatesResponse? resultCertificate = _certificatesClient.GetCertificate(tokenJWT, log);

                if (resultCertificate.Code != 200)
                {
                    return new EventsBuildResponse
                    {
                        Code = resultCertificate.Code,
                        Message = resultCertificate.Message
                    };
                }

                response = _documentBuild.BuildApplicationResponse(env, request);

                response.code = (response.code == 100 ? 200 : 103);
                if (response.code == 200)
                {
                    ValidateXMLDocumentResponse? responseXSD = _documentBuild.ValidateXSD(env, response.xml);

                    if (responseXSD.Code != 0)
                    {
                        return new EventsBuildResponse() { Code = 103, Message = "Archivo generado no válido; " + responseXSD.Message };
                    }

                    //Firma Xml
                    SignedInternalResponse responseSigned = _signedClient.SignedXml(response.xml, resultCertificate.Certificate, log);

                    if (responseSigned.Code == 200)
                    {
                        responseXSD = _documentBuild.ValidateXSD(env, responseSigned.File);
                        if (responseXSD.Code != 0)
                        {
                            return new EventsBuildResponse() { Code = 103, Message = "Archivo firmado generado no válido; " + responseXSD.Message };
                        }

                        //Se asigna el xml firmado para el response
                        response.xml = responseSigned.File;
                    }
                    else
                    {
                        response.code = 103;
                        response.message = responseSigned.Message;
                    }

                    if (response.code == 200)
                    {
                        request.EmailAddress = string.IsNullOrEmpty(request.EmailAddress) ? "" : request.EmailAddress.Trim().ToLower();

                        //Una implementación del Estandard Official: RFC 5322
                        string pattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
                        bool isMatch = Regex.IsMatch(request.EmailAddress.ToLower(), pattern, RegexOptions.IgnoreCase);

                        //Persisto en DB la Informacion del Evento de Factura
                        //Mapeo los datos desde el Request a la Entidad de Datos
                        InvoiceEventTable? invoiceEventEntity = new InvoiceEventTable
                        {
                            active = true,
                            id_enterprise = Int32.Parse(context.User.EnterpiseId),
                            event_type = request.EventCode,
                            event_id = request.CorrelativeNumber.ToString(),
                            event_uuid = response.uuid,
                            event_uuid_type = "CUDE-SHA384",
                            track_id = Guid.NewGuid().ToString(),
                            invoice_uuid = xmlInvoice.UUID,
                            invoice_uuid_type = xmlInvoice.UUID_schemeName,
                            invoice_id = DateTime.Now.Minute + 15010,//TODO Ubicar id
                            invoice_issuedate = Convert.ToDateTime($"{xmlInvoice.IssueDate.ToString("yyyy-MM-dd")} {xmlInvoice.IssueTime.ToString("HH:mm:ss")}"),
                            status = 201,
                            dian_status = 0,
                            dian_response_datetime = DateTime.Now,
                            dian_message = "",
                            document_id = xmlInvoice.ID,
                            supplier_identification = xmlInvoice.AcountingSupplierParty.TaxScheme.CompanyID,
                            supplier_type_identification = xmlInvoice.AcountingSupplierParty.TaxScheme.SchemeName,
                            customer_identification = xmlInvoice.AccountingCustomerParty.TaxScheme.CompanyID,
                            customer_type_identification = xmlInvoice.AccountingCustomerParty.TaxScheme.SchemeName,
                            namefile = Guid.NewGuid().ToString() + ".xml",
                            session_log = log.GetSession(),
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow,
                            email = isMatch ? request.EmailAddress : "",
                            environment = request.Environment.Value,
                            created_by = request.CreatedBy != null ? (short?)request.CreatedBy : 1

                        };

                        invoiceEventEntity.path_file = $"Factoring/Eventos/{invoiceEventEntity.invoice_issuedate.Year}/{invoiceEventEntity.invoice_issuedate.Month}/{invoiceEventEntity.invoice_issuedate.Day}/{invoiceEventEntity.document_id}/{invoiceEventEntity.event_id}";

                        if (_dbContext.SaveUpdateEventInvoice(invoiceEventEntity, 0, _configuration, log) == 0)
                        {
                            return new EventsBuildResponse()
                            {
                                Code = 500,
                                Message = "Error al registrar el evento"
                            };
                        }

                        //Guardar en Storage Factoring
                        byte[] xmlEventByte = Convert.FromBase64String(response.xml);
                        TFHKA.Storage.Fileshare.Client.Models.ResponseBaseStorage? resultStorage = _storageFiles.SaveFile(xmlEventByte, invoiceEventEntity.path_file, invoiceEventEntity.namefile, StorageConfiguration.FactoringFileShare, log);

                        if (resultStorage.Code == 200)
                        {
                            return new EventsBuildResponse()
                            {
                                Code = invoiceEventEntity.status,
                                Message = "Se ha generado un evento, pendiente por enviar a la DIAN",
                                Uuid = response.uuid,
                                Xml = response.xml,
                                TrackId = invoiceEventEntity.track_id,
                                EventId = request.CorrelativeNumber.ToString(),
                                NameFile = invoiceEventEntity.namefile,
                                PathFile = invoiceEventEntity.path_file,
                                Size = xmlEventByte.Length,
                                EventHash = StringUtilies.GetSHA1(response.xml)
                            };
                        }
                        else
                        {
                            return new EventsBuildResponse()
                            {
                                Code = resultStorage.Code,
                                Message = resultStorage.Message,
                            };
                        }
                    }
                    else
                    {
                        return new EventsBuildResponse()
                        {
                            Code = response.code,
                            Message = response.message,
                            Uuid = response.uuid,
                            Xml = response.xml
                        };
                    }
                }
                else
                {
                    return new EventsBuildResponse()
                    {
                        Code = response.code,
                        Message = response.message,
                        Uuid = response.uuid,
                        Xml = response.xml
                    };
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                return new EventsBuildResponse()
                {
                    Code = 500,
                    Message = ex.Message
                };
            }
        }
    }
}