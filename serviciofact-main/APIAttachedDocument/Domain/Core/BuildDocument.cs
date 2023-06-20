using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Transversal;
using DocumentBuildCO.ClassXSD;
using DocumentBuildCO.Common;
using DocumentBuildCO.DocumentClass.UBL2._1;
using DocumentBuildCO.Response;
using DocumentBuildCO.Validate;
using System.Reflection;
using System.Xml;

namespace APIAttachedDocument.Domain.Core
{
    public class BuildDocument
    {
        public static bool ValidateXSD(string xml)
        {
            string pathXSD = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
            TypeDocument typeDocument = TypeDocument.ApplicationReponse;

            string xmlPlain = StringUtilies.Base64Decode(xml);

            UBL_Version ublVersion = UBL_Version.UBL2_1;

            XSDResponse? result = ValidateDocument.validatewithXSD(typeDocument, xmlPlain, pathXSD, ublVersion);
            return result.code == 0;
        }

        public static bool XmlApplicationResponseValid(string xml)
        {
            try
            {
                string xmlPlain = StringUtilies.Base64Decode(xml);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(xmlPlain);

                List<string> listDocument = new List<string>
                {
                    "ApplicationResponse",
                    "Invoice",
                    "CreditNote",
                    "DebitNote"
                };

                if (listDocument.Contains(doc.DocumentElement.Name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string BuildAttachedDocument(string xmlEvent, string xmlDian, DocumentElectronic applicationResponseTypeEvent, DocumentElectronic applicationResponseTypeDian)
        {
            DocumentBuildCO.DocumentClass.UBL2._1.AttachedDocument doc = new DocumentBuildCO.DocumentClass.UBL2._1.AttachedDocument();

            //Header
            doc.UBLVersionID = "UBL 2.1";
            doc.CustomizationID = "Documentos adjuntos";
            doc.ProfileID = "Factura Electrónica de Venta";
            doc.ProfileExecutionID = applicationResponseTypeEvent.ProfileExecutionID;

            Random random = new();
            doc.ID = DateTime.Now.ToString("yyddfffff") + random.Next(100, 999);

            doc.IssueDate = DateTime.UtcNow.AddHours(-5);
            doc.IssueTime = DateTime.UtcNow.AddHours(-5);
            doc.DocumentType = "Contenedor de Factura Electrónica";
            doc.ParentDocumentID = applicationResponseTypeEvent.DocumentId;

            //SenderParty
            doc.SenderRegistrationName = applicationResponseTypeEvent.SenderParty.RegistrationName;
            doc.SenderCompanyID = applicationResponseTypeEvent.SenderParty.CompanyID;
            doc.SenderschemeID = applicationResponseTypeEvent.SenderParty.CompanyIDSchemeID;
            doc.SenderPartyTaxSchemeName = applicationResponseTypeEvent.SenderParty.CompanyIDSchemeName;
            doc.SenderschemeAgencyID = DocumentBuildCO.Common.Catalog.DIAN_ID;
            doc.SenderlistName = "No aplica";
            doc.SenderTaxLevelCode = "R-99-PN";

            doc.SenderTaxSchemeID = applicationResponseTypeEvent.SenderParty.TaxSchemeID;
            doc.SenderTaxSchemeName = applicationResponseTypeEvent.SenderParty.TaxSchemeName;


            //ReceiverParty
            doc.ReceiverRegistrationName = applicationResponseTypeEvent.ReceiverParty.RegistrationName;
            doc.ReceiverCompanyID = applicationResponseTypeEvent.ReceiverParty.CompanyID;
            doc.ReceiverschemeID = applicationResponseTypeEvent.ReceiverParty.CompanyIDSchemeID;
            doc.ReceiverschemeName = applicationResponseTypeEvent.ReceiverParty.CompanyIDSchemeName;
            doc.ReceiverschemeAgencyID = DocumentBuildCO.Common.Catalog.DIAN_ID;
            doc.ReceiverlistName = "No aplica";
            doc.ReceiverTaxLevelCode = "R-99-PN";

            doc.ReceiverTaxSchemeID = applicationResponseTypeEvent.ReceiverParty.TaxSchemeID;
            doc.ReceiverTaxSchemeName = applicationResponseTypeEvent.ReceiverParty.TaxSchemeName;

            //Document
            doc.MimeCode = "text/xml";
            doc.EncodingCode = "UTF-8";
            doc.Description = StringUtilies.Base64Decode(xmlEvent);

            //Line Reference
            doc.ParentDocumentLineReference = new List<ParentDocumentLineReference>();

            ParentDocumentLineReference item = new ParentDocumentLineReference
            {
                LineID = "1",
                DocumentReferenceUUID = applicationResponseTypeEvent.DocumentId,
                DocumentReferenceID = applicationResponseTypeEvent.Uuid,
                DocumentReferenceschemeName = applicationResponseTypeEvent.UuidSchemeName,
                DocumentReferenceIssueDate = applicationResponseTypeEvent.IssueDate,
                DocumentReferenceDocumentType = "ApplicationResponse",
                DocumentReferenceMimeCode = "text/xml",
                DocumentReferenceEncodingCode = "UTF-8",
                DocumentReferenceDescription = StringUtilies.Base64Decode(xmlDian),

                ValidationResultCode = applicationResponseTypeDian.ValidationCode,
                ValidatorID = applicationResponseTypeDian.SenderParty.RegistrationName,
                ValidationDate = applicationResponseTypeDian.IssueDate,
                ValidationTime = applicationResponseTypeDian.IssueTime,
            };

            doc.ParentDocumentLineReference.Add(item);

            BuildResponse result = DocumentBuildCO.BuildAttachedDocument.Document(doc);

            return result.xml;
        }

        public static DocumentElectronic SerializeApplicationResponse(string xml)
        {
            DocumentElectronic document = new DocumentElectronic();

            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(xml);

                switch (doc.DocumentElement.Name)
                {
                    case "Invoice":
                        InvoiceType? invoice = DocumentBuildCO.Serialize.SerializeUBL21.Invoice(xml);

                        if (invoice != null)
                        {
                            document.Uuid = invoice.UUID.Value;
                            document.UuidSchemeName = invoice.UUID.schemeName;
                            document.DocumentId = invoice.ID.Value;
                            document.ProfileExecutionID = invoice.ProfileExecutionID.Value;
                            document.IssueDate = invoice.IssueDate.Value;
                            document.IssueTime = invoice.IssueTime.Value;
                            document.SenderParty = new DocumentParty
                            {
                                CompanyID = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = invoice.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };

                            document.ReceiverParty = new DocumentParty
                            {
                                CompanyID = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = invoice.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };
                        }

                        break;

                    case "CreditNote":
                        CreditNoteType? credit = DocumentBuildCO.Serialize.SerializeUBL21.CreditNote(xml);

                        if (credit != null)
                        {
                            document.Uuid = credit.UUID.Value;
                            document.UuidSchemeName = credit.UUID.schemeName;
                            document.DocumentId = credit.ID.Value;
                            document.ProfileExecutionID = credit.ProfileExecutionID.Value;
                            document.IssueDate = credit.IssueDate.Value;
                            document.IssueTime = credit.IssueTime.Value;

                            document.SenderParty = new DocumentParty
                            {
                                CompanyID = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = credit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };

                            document.ReceiverParty = new DocumentParty
                            {
                                CompanyID = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = credit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };
                        }

                        break;

                    case "DebitNote":
                        DebitNoteType? debit = DocumentBuildCO.Serialize.SerializeUBL21.DebitNote(xml);

                        if (debit != null)
                        {
                            document.Uuid = debit.UUID.Value;
                            document.UuidSchemeName = debit.UUID.schemeName;
                            document.DocumentId = debit.ID.Value;
                            document.ProfileExecutionID = debit.ProfileExecutionID.Value;
                            document.IssueDate = debit.IssueDate.Value;
                            document.IssueTime = debit.IssueTime.Value;

                            document.SenderParty = new DocumentParty
                            {
                                CompanyID = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = debit.AccountingSupplierParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };

                            document.ReceiverParty = new DocumentParty
                            {
                                CompanyID = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = debit.AccountingCustomerParty.Party?.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };
                        }

                        break;

                    case "ApplicationResponse":
                        ApplicationResponseType? application = DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(xml);

                        if (application != null)
                        {
                            document.Uuid = application.UUID.Value;
                            document.UuidSchemeName = application.UUID.schemeName;
                            document.DocumentId = application.ID.Value;
                            document.ProfileExecutionID = application.ProfileExecutionID.Value;
                            document.IssueDate = application.IssueDate.Value;
                            document.IssueTime = application.IssueTime.Value;

                            document.SenderParty = new DocumentParty
                            {
                                CompanyID = application.SenderParty.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = application.SenderParty.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = application.SenderParty.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = application.SenderParty.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = application.SenderParty.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = application.SenderParty.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };

                            document.ReceiverParty = new DocumentParty
                            {
                                CompanyID = application.ReceiverParty.PartyTaxScheme[0]?.CompanyID?.Value,
                                CompanyIDSchemeID = application.ReceiverParty.PartyTaxScheme[0]?.CompanyID?.schemeID,
                                CompanyIDSchemeName = application.ReceiverParty.PartyTaxScheme[0]?.CompanyID?.schemeName,
                                RegistrationName = application.ReceiverParty.PartyTaxScheme[0]?.RegistrationName?.Value,
                                TaxSchemeID = application.ReceiverParty.PartyTaxScheme[0]?.TaxScheme?.ID?.Value,
                                TaxSchemeName = application.ReceiverParty.PartyTaxScheme[0]?.TaxScheme?.Name?.Value
                            };

                            if (application.DocumentResponse != null)
                            {
                                document.ValidationCode = application.DocumentResponse[0]?.Response?.ResponseCode?.Value;
                                document.ReferenceUuid = application.DocumentResponse[0]?.DocumentReference[0]?.UUID?.Value;
                                document.ReferenceId = application.DocumentResponse[0]?.DocumentReference[0]?.ID?.Value;
                            }
                        }

                        break;
                }

                return document;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
