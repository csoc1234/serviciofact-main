using DocumentBuildCO.DocumentClass.UBL2._1;
using System;
using System.Linq;
using TFHKA.Storage.Fileshare.Client.Models;
using WebApi.Domain.Entity;
using WebApi.Domain.Interface;
using WebApi.Infrastructure.AzureStorage;
using WebApi.Infrastructure.AzureStorage.Interface;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Domain.Core
{
    public class Invoice21Domain : IInvoiceDomain
    {
        public Invoice Invoice { get; set; }

        private static IStorageFiles _storageFiles;

        public Invoice21Domain()
        {

        }

        public Invoice21Domain(IStorageFiles storageFiles)
        {
            _storageFiles = storageFiles;
        }

        public ValidateXmlResponse ValidateDocument(string xmlAttachment)
        {
            ValidateXmlResponse validateXml = new ValidateXmlResponse();

            //Decodificamos el Xml base64 a Texto
            xmlAttachment = Base64Decode(xmlAttachment);

            //Detecta si es AttachedDocument
            var attachedDocumentUbl = DocumentBuildCO.Serialize.SerializeUBL21.AttachedDocument(xmlAttachment);

            if (attachedDocumentUbl != null)
            {
                //Valida si posee un Xml anidado
                string xmlInvoice = attachedDocumentUbl.Attachment?.ExternalReference?.Description?.FirstOrDefault()?.Value;

                string xmlApplicationResponse = attachedDocumentUbl.ParentDocumentLineReference?.FirstOrDefault()?.DocumentReference?.Attachment?.ExternalReference?.Description?.FirstOrDefault()?.Value;

                if (xmlInvoice != null)
                {
                    //Implementamos DocumentBuilCO del UBL del XML
                    //DocumenntBuildCO - Metodo serialize 
                    BaseDocument21 doc = DocumentBuildCO.SerializeXmlUBL21.Invoice(xmlInvoice);
                    //Validacion 1 - que sea factura de venta 
                    //Val 1: Factura de venta:
                    if (doc.InvoiceTypeCode != "01")
                    {
                        return new ValidateXmlResponse
                        {
                            Message = "El documento no es una factura de venta",
                            Valid = false
                        };
                    }
                    if (doc.PaymentMeans == null)
                    {
                        return new ValidateXmlResponse
                        {
                            Message = "La factura no posee información de pago",
                            Valid = false
                        };
                    }

                    if (doc.PaymentMeans.Count > 1 || doc.PaymentMeans.Count < 1)
                    {
                        return new ValidateXmlResponse
                        {
                            Message = "La factura contiene múltiples pagos",
                            Valid = false
                        };
                    }
                    var paymentMeans = doc.PaymentMeans.FirstOrDefault();

                    if (paymentMeans.ID != "2")
                    {
                        return new ValidateXmlResponse
                        {
                            Message = "No cumple y no es de crédito",
                            Valid = false
                        };
                    }

                    if (paymentMeans.PaymentDueDate.HasValue)
                    {
                        if (paymentMeans.PaymentDueDate <= DateTime.Today.AddDays(2))
                        {
                            return new ValidateXmlResponse
                            {
                                Message = "La fecha de pago no cumple con el rango permitido",
                                Valid = false
                            };
                        }
                    }

                    if (doc.DueDate.HasValue)
                    {
                        if (doc.DueDate <= DateTime.Today.AddDays(2))
                        {
                            return new ValidateXmlResponse
                            {
                                Message = "La fecha de vencimiento no cumple con el rango permitido",
                                Valid = false
                            };
                        }
                    }

                    //Revision de Evento
                    var applicationResponse = DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(xmlApplicationResponse);

                    if (applicationResponse != null)
                    {
                        DateTime fechaAceptacion = DateTime.Parse($"{applicationResponse.IssueDate.Value.ToShortDateString()} {applicationResponse.IssueTime.Value.ToShortTimeString()}");

                        return new ValidateXmlResponse
                        {
                            ValidateDianDatetime = fechaAceptacion,
                            Message = "Factura Negociable",
                            Valid = true,
                            Document = doc
                        };
                    }
                    else
                    {
                        return new ValidateXmlResponse
                        {
                            Message = "El evento de aceptacion DIAN no es legible",
                            Valid = false
                        };
                    }
                }

                else
                {
                    return new ValidateXmlResponse
                    {
                        Message = "El documento electronico no es legible",
                        Valid = false
                    };
                }
            }
            else
            {
                return new ValidateXmlResponse
                {
                    Message = "El documento electronico no es legible",
                    Valid = false
                };
            }
        }

        public StorageFileResponse FindXMLFileShare(string pathFile, string nameFile, ILogMongo log)
        {
            return _storageFiles.GetFile(pathFile, nameFile, StorageConfiguration.EmisionFileShare, log);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes;
            try
            {
                base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            }
            catch (Exception)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = new byte[1];
            try
            {
                plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            }
            catch (Exception)
            {
            }
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}