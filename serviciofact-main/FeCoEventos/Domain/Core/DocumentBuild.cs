using DocumentBuildCO;
using DocumentBuildCO.Builder.UBL2._1;
using DocumentBuildCO.ClassXSD;
using DocumentBuildCO.Common;
using DocumentBuildCO.DocumentClass;
using DocumentBuildCO.DocumentClass.UBL2._1;
using DocumentBuildCO.Models.Request;
using DocumentBuildCO.Reader;
using DocumentBuildCO.Request;
using DocumentBuildCO.Response;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AccountingCustomerParty = DocumentBuildCO.DocumentClass.UBL2._1.AccountingCustomerParty;

namespace FeCoEventos.Domain.Core
{
    public class DocumentBuild : IDocumentBuild
    {
        private readonly IConfiguration _configuration;

        public DocumentBuild(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationResponseType SerializeApplicationResponse(string xml)
        {
            return DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(xml);
        }

        public static bool SerializeAttachedDocument(string xmlBase64, string env)
        {
            xmlBase64 = StringUtilies.BOMStringXml(xmlBase64);

            string xml = StringUtilies.Base64Decode(xmlBase64);
            if (string.IsNullOrEmpty(xml))
            {
                System.Exception exception = new System.Exception("Se genero un error decodificando el base64 del xml");
                throw exception;
            }

            xml = Utils.AttachmentDocument_RepairXMLStructure(xml); //Intento reparar problemas en el XML del AttachedDocument

            AttachedDocumentType? document = DocumentBuildCO.Serialize.SerializeUBL21.AttachedDocument(xml);

            if (document != null)
            {
                //Validacion Ambiente
                bool resultInvoice = SerializeInvoice(document.Attachment?.ExternalReference?.Description[0]?.Value, env);

                if (resultInvoice)
                {
                    //Extraigo el Application Response
                    string? xmlApplicationResponse = document.ParentDocumentLineReference[0]?.DocumentReference?.Attachment?.ExternalReference?.Description[0]?.Value;

                    bool resultApplicationResponse = ValidateEnvironmentApplicationResponse(xmlApplicationResponse, env);

                    return resultApplicationResponse;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool SerializeInvoice(string xmlBase64, string env)
        {
            if (string.IsNullOrEmpty(xmlBase64))
            {
                System.Exception exception = new System.Exception("Se genero un error decodificando el base64 del xml");
                throw exception;
            }

            string xmlEncode = DocumentBuildCO.Common.Utils.EncodeToBase64(xmlBase64);

            xmlBase64 = DocumentBuildCO.Common.Utils.DecodeFromBase64(xmlEncode);



            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlBase64);

            XmlNodeList isInvoice = xDoc.GetElementsByTagName("Invoice");
            if (isInvoice.Count == 0)
            {
                isInvoice = xDoc.GetElementsByTagName("fe:Invoice");
            }

            XmlNodeList isCreditNote = xDoc.GetElementsByTagName("CreditNote");
            if (isCreditNote.Count == 0)
            {
                isCreditNote = xDoc.GetElementsByTagName("fe:CreditNote");
            }

            XmlNodeList isDebitNote = xDoc.GetElementsByTagName("DebitNote");
            if (isDebitNote.Count == 0)
            {
                isDebitNote = xDoc.GetElementsByTagName("fe:DebitNote");
            }

            string profileExecutionID = string.Empty;

            if (isInvoice.Count > 0)
            {
                InvoiceType? document = DocumentBuildCO.Serialize.SerializeUBL21.Invoice(xmlBase64);
                profileExecutionID = document.ProfileExecutionID.Value;
            }
            else if (isCreditNote.Count > 0)
            {
                CreditNoteType? document = DocumentBuildCO.Serialize.SerializeUBL21.CreditNote(xmlBase64);
                profileExecutionID = document.ProfileExecutionID.Value;
            }
            else if (isDebitNote.Count > 0)
            {
                DebitNoteType? document = DocumentBuildCO.Serialize.SerializeUBL21.DebitNote(xmlBase64);
                profileExecutionID = document.ProfileExecutionID.Value;
            }

            return profileExecutionID == env;
        }

        public static bool ValidateEnvironmentApplicationResponse(string xmlBase64, string env)
        {
            if (string.IsNullOrEmpty(xmlBase64))
            {
                System.Exception exception = new System.Exception("Se genero un error decodificando el base64 del xml");
                throw exception;
            }

            string xmlEncode = DocumentBuildCO.Common.Utils.EncodeToBase64(xmlBase64);

            xmlBase64 = DocumentBuildCO.Common.Utils.DecodeFromBase64(xmlEncode);

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlBase64);

            XmlNodeList isApplicationResponse = xDoc.GetElementsByTagName("ApplicationResponse");

            if (isApplicationResponse.Count > 0)
            {
                ApplicationResponseType? document = DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(xmlBase64);

                return document.ProfileExecutionID.Value == env;
            }
            else
            {
                return false;
            }
        }

        public string GetInvoiceXml(string env, string xmlBase64)
        {
            xmlBase64 = StringUtilies.BOMStringXml(xmlBase64);

            ValidateXMLDocumentResponse? responseXSDXML = ValidateXSD(env, xmlBase64);
            if (responseXSDXML.Code != 0)
            {
                System.Exception exception = new System.Exception("Archivo XML Base no pudo ser procesado; " + responseXSDXML.Message);
                throw exception;
            }

            //Serializa el XML
            string xmlFile = StringUtilies.Base64Decode(xmlBase64);
            if (string.IsNullOrEmpty(xmlFile))
            {
                System.Exception exception = new System.Exception("Se genero un error decodificando el base64 del xml");
                throw exception;
            }

            xmlFile = Utils.AttachmentDocument_RepairXMLStructure(xmlFile); //Intento reparar problemas en el XML del AttachedDocument

            AttachedDocumentType? xmlAttachedDocument = DocumentBuildCO.Serialize.SerializeUBL21.AttachedDocument(xmlFile);

            string? xmlInvoicePlain = xmlAttachedDocument.Attachment?.ExternalReference?.Description?.FirstOrDefault()?.Value;

            return xmlInvoicePlain;
        }

        public BaseDocument21 SerializeDocument(string env, string xmlInvoicePlain)
        {
            if (!string.IsNullOrEmpty(xmlInvoicePlain))
            {
                //Validar XSD de factura
                ValidateXMLDocumentResponse? resultXsdInvoice = ValidateXSD(env, StringUtilies.Base64Encode(xmlInvoicePlain));
                if (resultXsdInvoice.Code != 0)
                {
                    System.Exception exception = new System.Exception("Archivo XML Base no pudo ser procesado; " + resultXsdInvoice.Message);
                    throw exception;
                }

                BaseDocument21 xmlInvoice = DocumentBuildCO.SerializeXmlUBL21.SerializeDocumentForEvents(xmlInvoicePlain);

                if (xmlInvoice != null)
                {
                    return xmlInvoice;
                }
                else
                {
                    System.Exception exception = new System.Exception("Error leyendo el documento electronico");
                    throw exception;
                }
            }
            else
            {
                System.Exception exception = new System.Exception("El Contenedor AttachedDocument no contiene un documento electronico");
                throw exception;
            }
        }

        public ValidateXMLDocumentResponse ValidateXSD(string env, string xml)
        {
            ValidateDocument requestXSD = new ValidateDocument
            {
                Xml = xml,
                Enviroment = GetEnvironmentDocumentBuild(env),
                Version = DocumentBuildCO.Common.UBL_Version.UBL2_1,
                PathXSD = _configuration["DocumentBuilCO:PathXSD"]
            };

            return DocumentBuildCO.ValidateXml.Document(requestXSD);
        }

        public DocumentBuildCO.Response.BuildResponse BuildApplicationResponse(string env, EventsBuildRequest eventRequest)
        {
            ApplicationResponseReception buildXml = new ApplicationResponseReception();

            ApplicationResponseInformationRequest buildRequest = new ApplicationResponseInformationRequest
            {
                CodeEvent = eventRequest.EventCode,
                Id = eventRequest.CorrelativeNumber.ToString(),
                SoftwarePin = _configuration["CredencialesDIAN:SoftwarePin"],
                SoftwareID = _configuration["CredencialesDIAN:SoftwareId"],
                Url = env == "prod" ? _configuration["CredencialesDIAN:UrlProduccion"] : _configuration["CredencialesDIAN:UrlHabilitacion"],
                IsCustomer = true,
                Env = GetEnvironmentDocumentBuild(env),
                Note = eventRequest.Note
            };

            if (!string.IsNullOrEmpty(eventRequest.EffectiveDate))
            {
                DateTime fecha;

                DateTime.TryParse(eventRequest.EffectiveDate, out fecha);
                if (fecha.Year >= 2000)
                {
                    buildRequest.EffectiveDate = fecha;
                }
            }

            switch (buildRequest.CodeEvent)
            {
                case "030":
                case "032":
                case "033":
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.CustomizationID = "1";
                    break;

                case "031":
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomizationID = "1";
                    break;

                case "034":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = "1";
                    break;

                case "035":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = "035";
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    break;

                case "036":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    break;

                case "037":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    break;

                case "038":
                    buildRequest.CustomizationID = "038";
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    break;

                case "039":
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.CustomizationID = "039";
                    break;

                case "040":
                case "041":
                case "042":
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;

                    break;

                case "043":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.CustomizationIDSchemeID = eventRequest.CustomizationIDSchemeID;
                    buildRequest.DocumentResponseMandato = eventRequest.DocumentResponseMandato;
                    buildRequest.LineResponse = eventRequest.LineResponse;
                    break;

                case "044":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;

                    break;

                case "045":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = eventRequest.CustomizationID;
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    break;

                case "046":
                    buildRequest.IsCustomer = false;
                    buildRequest.CustomizationID = "046";
                    buildRequest.RejectedCode = eventRequest.RejectedCode;
                    buildRequest.CustomTagGeneral = eventRequest.CustomTagGeneral;
                    buildRequest.SenderParty = eventRequest.SenderParty;
                    buildRequest.ReceiverParty = eventRequest.ReceiverParty;
                    buildRequest.IssuerParty = eventRequest.IssuerParty;
                    buildRequest.DocumentReference = eventRequest.DocumentReference;
                    break;

                default:

                    break;
            }

            BuildResponse response = BuildXML(eventRequest.XmlBase64, buildRequest);

            return response;
        }

        public DocumentBuildCO.Request.Ambiente GetEnvironmentDocumentBuild(string env)
        {
            return env.Trim().ToLower() switch
            {
                "dev" => Ambiente.DEV,
                "test" => Ambiente.TEST,
                "demo" => Ambiente.DEMO,
                "prod" => Ambiente.PRODUCCION,
                _ => Ambiente.DEV,
            };
        }

        public string GetEventName(string eventCode)
        {
            string result = eventCode switch
            {
                "030" => "Acuse de recibo de Factura Electrónica de Venta",
                "031" => "Reclamo de la Factura Electrónica de Venta",
                "032" => "Recibo del bien y/o prestación del servicio",
                "033" => "Aceptación expresa",
                "034" => "Aceptación Tácita",
                "035" => "Aval",
                "036" => "Inscripción de la factura electrónica de venta como título valor - RADIAN",
                "037" => "Endoso en propiedad",
                "038" => "Endoso en garantía",
                "039" => "Endoso en procuración",
                "040" => "Cancelación de endoso",
                "041" => "Limitaciones a la circulación de la factura electrónica de venta como título",
                "042" => "Terminación de las limitaciones a la circulación de la factura electrónica de venta como título",
                "043" => "Mandato",
                "044" => "Terminacion del Mandato",
                "045" => "Pago de la factura electrónica de venta como título valor",
                "046" => "Informe para el pago",
                _ => "",
            };
            return result;
        }

        public string BOMFomartXml(string xml)
        {
            XmlDocument doc = new XmlDocument
            {
                PreserveWhitespace = true
            };

            xml = StringUtilies.Base64Decode(xml);

            try
            {
                doc.LoadXml(xml);

                return StringUtilies.Base64Encode(xml);
            }
            catch
            {
                try
                {
                    string BOMMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (xml.StartsWith(BOMMarkUtf8))
                    {
                        xml = xml.Remove(0, BOMMarkUtf8.Length);
                    }
                    doc.LoadXml(xml);
                    return StringUtilies.Base64Encode(xml);
                }
                catch (Exception en)
                {
                    try
                    {
                        doc.LoadXml(xml.Substring(xml.IndexOf(Environment.NewLine)));
                        return StringUtilies.Base64Encode(xml.Substring(xml.IndexOf(Environment.NewLine)));
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }



        public BuildResponse BuildXML(String xml, ApplicationResponseInformationRequest request)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                String Xmldec = Utils.DecodeFromBase64(xml);

                try
                {
                    doc.LoadXml(Xmldec);
                }
                catch
                {
                    try
                    {
                        string BOMMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (Xmldec.StartsWith(BOMMarkUtf8))
                        {
                            Xmldec = Xmldec.Remove(0, BOMMarkUtf8.Length);
                            xml = Utils.EncodeToBase64(Xmldec);
                        }
                        doc.LoadXml(Xmldec);
                    }
                    catch (Exception en)
                    {

                        try
                        {
                            doc.LoadXml(Xmldec.Substring(Xmldec.IndexOf(Environment.NewLine)));
                            xml = Utils.EncodeToBase64(Xmldec.Substring(Xmldec.IndexOf(Environment.NewLine)));
                        }
                        catch (Exception en1)
                        {
                            return new BuildResponse { code = 402, message = String.Format("No se pudo  procesar archivo XML {0}", en1.Message) };
                        }
                    }
                }

                XmlNodeList xmlValue = doc.GetElementsByTagName("cbc:UBLVersionID");

                if (xmlValue.Count == 0)
                {
                    xmlValue = doc.GetElementsByTagName("UBLVersionID");
                }

                if (xmlValue.Count > 0)
                {
                    XmlNode xmlValueNode = xmlValue.Item(0);
                    XmlElement dataXml = (XmlElement)xmlValueNode;
                    string stringnode = dataXml.InnerXml;
                    stringnode = Regex.Replace(stringnode, @"\s+", " ");

                    return BuildXML21(xml, request); /* De este modo si cumple con el XSD pasa, caso contrario dará error */

                }
                else
                {
                    BuildResponse? response = new BuildResponse { code = 103, message = "No se pudo determinar la versión de UBL" };
                    return response;
                }
            }
            catch (Exception)
            {
                BuildResponse response = new BuildResponse();
                response.code = 101;
                response.message = "Se produjo un error interno al intentar generar el Application Response";
                response.uuid = "";
                return response;
            }
        }

        public BuildResponse BuildXML21(String xml, ApplicationResponseInformationRequest request)
        {

            try
            {
                xml = Utils.DecodeFromBase64(xml);
                TypeDocument DocumentType = Node.LoadNodeDocumentType(xml);
                if (DocumentType == TypeDocument.AttachedDocument)
                {
                    xml = ValidateXml.getXMLBaseAttachedDocument(xml);
                }

                string xmlEncode = DocumentBuildCO.Common.Utils.EncodeToBase64(xml);

                xml = DocumentBuildCO.Common.Utils.DecodeFromBase64(xmlEncode);

                BaseDocument21 dataAux = new BaseDocument21();
                BaseDocumentClass document = new BaseDocumentClass();
                BaseDocument21 InvoiceSerialize = SerializeXmlUBL21.SerializeDocumentForEvents(xml);
                string traza = JsonConvert.SerializeObject(InvoiceSerialize);
                dataAux = JsonConvert.DeserializeObject<BaseDocument21>(traza);

                //Concatenar texto

                dataAux.AccountingCustomerParty.TaxScheme.RegistrationName = StringUtilies.FillWithSpace(dataAux.AccountingCustomerParty.TaxScheme.RegistrationName, 5);

                dataAux.AcountingSupplierParty.TaxScheme.RegistrationName = StringUtilies.FillWithSpace(dataAux.AcountingSupplierParty.TaxScheme.RegistrationName, 5);


                if (request.CodeEvent == "034")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Unidad Administrativa Especial Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };

                    request.Note = new List<string>
                    {
                        "Nombre del Mandatario \"OBRANDO EN NOMBRE Y REPRESENTACION DE\" Nombre del mandante",
                        "Debe existir una Nota cuando este evento sea trasmitido sin mandatario responderá el siguiente mensaje: \"Manifiesto bajo la gravedad de juramento que transcurridos 3 días hábiles siguientes a la fecha de recepción de la mercancía o del servicio en la referida factura de este evento, el adquirente [Razón social] identificado con NIT [XXXX] no manifestó expresamente la aceptación o rechazo de la referida factura, ni reclamó en contra de su contenido.",
                        "1. Debe existir una Nota  cuando un mandatario sea quien transmita este evento a la DIAN responderá el siguiente mensaje: [Razón social / Nombre del mandatario] identificado con NIT / cédula de ciudadanía No. [XXXXX], actuando en nombre y representación de [Razón Social] con Nit [XXXXX], manifiesto bajo la gravedad de juramento que transcurridos 3 días hábiles siguientes a la fecha de recepción de la mercancía o del servicio en la referida factura de este evento, el adquirente [Razón social] identificado con NIT [XXXX] no manifestó expresamente la aceptación o rechazo de la referida factura, ni reclamó en contra de su contenido.",
                        "2. Debe existir una Nota  por mandatos sea quien transmita este evento a la DIAN responderá el siguiente mensaje: [razón social / nombre del mandatario] identificado con Nit / cédula de ciudadanía No. [XXXX] obrando en nombre y representación de [nombre de persona natural comerciante] identificado con cédula de ciudadanía No. [XXXXX], con Nit [XXXXX], manifiesta bajo la gravedad de juramento que transcurridos 3 días hábiles siguientes a la fecha de recepción de la mercancía o del servicio en la referida factura de este evento, el adquirente [nombre de persona natural comerciante] identificado con cédula de ciudadanía No. [XXXXX], con Nit [XXXXX] no manifestó expresamente la aceptación o rechazo de la referida factura, ni reclamó en contra de su contenido."
                    };
                }

                if (request.CodeEvent == "036")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };
                }

                if (request.CodeEvent == "035")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };

                    dataAux.AcountingSupplierParty = new AcountingSupplierParty
                    {
                        AdditionalAccountID = 1
                    };

                    if (request.SenderParty.PartyTaxScheme != null)
                    {
                        PartyTaxSchemeAR? taxScheme = request.SenderParty.PartyTaxScheme[0];

                        dataAux.AcountingSupplierParty.TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = taxScheme.RegistrationName,
                            CompanyID = taxScheme.CompanyID,
                            SchemeID = taxScheme.CompanyIDSchemeID,
                            SchemeName = taxScheme.CompanyIDSchemeName,
                            SchemeVersionID = taxScheme.CompanyIDSchemeVersionID,
                            TaxSchemeID = taxScheme.TaxSchemeID,
                            TaxSchemeName = taxScheme.TaxSchemeName
                        };
                    }
                }

                if (request.CodeEvent == "043")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };
                }

                if (request.CodeEvent == "044")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };
                }

                if (request.CodeEvent == "045")
                {
                    dataAux.AccountingCustomerParty = new AccountingCustomerParty
                    {
                        AdditionalAccountID = 1,
                        TaxScheme = new PartyTaxScheme
                        {
                            RegistrationName = "Dirección de Impuestos y Aduanas Nacionales",
                            CompanyID = "800197268",
                            SchemeID = "4",
                            SchemeName = "31",
                            SchemeAgencyName = "",
                            SchemeVersionID = "1",
                            TaxSchemeID = "01",
                            TaxSchemeName = "IVA",
                            SchemeAgencyID = "",
                        }
                    };
                }

                if (request.CodeEvent == "046")
                {

                }

                request.IssueDate = DateTime.UtcNow.AddHours(-5);
                string SQRcode = "";

                request.SQRcode = SQRcode;

                ApplicationResponseReception application = new ApplicationResponseReception();

                BuildResponse? response = application.BuildXML(dataAux, request);
                response.code = response.code == 0 ? 100 : response.code;

                return response;
            }
            catch (Exception ex)
            {
                BuildResponse response = new BuildResponse();
                response.code = 101;
                response.message = "Se produjo un error interno al intentar generar el Application Response";
                response.uuid = "";
                return response;
            }
        }
    }
}
