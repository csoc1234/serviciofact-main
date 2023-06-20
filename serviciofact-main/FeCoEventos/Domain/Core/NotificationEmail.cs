using DocumentBuildCO.DocumentClass;
using FeCoEventos.Clients.SendEmail;
using FeCoEventos.Domain.Entity;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Core
{
    public class NotificationEmail : INotificationEmail
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;
        private readonly IStorageFiles _storageFiles;
        private readonly IEmailClient _emailClient;
        private readonly IDocumentBuild _documentBuild;

        public NotificationEmail(IConfiguration configuration, IApplicationDbContext context, IStorageFiles storageFiles, IEmailClient emailClient, IDocumentBuild documentBuild)
        {
            _configuration = configuration;
            _context = context;
            _storageFiles = storageFiles;
            _emailClient = emailClient;
            _documentBuild = documentBuild;
        }

        public ResponseBase Send(string eventId, string trackId, AttachedDocumentResponse attachedDocument, StorageFileResponse storageFile, Event eventDB, ILogAzure log)
        {
            try
            {
                if (eventDB != null)
                {
                    if (!string.IsNullOrEmpty(eventDB.Email))
                    {

                        DocumentBuildCO.ClassXSD.ApplicationResponseType? applicationResponseObj = DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(StringUtilies.Base64Decode(storageFile.File));

                        ParamSendEmail paramSendEmail = new ParamSendEmail(
                            new BaseDocumentClass
                            {
                                ID = eventDB.DocumentId,
                                AccountingCustomerParty = new AccountingCustomerParty
                                {
                                    RegistrationName = applicationResponseObj.ReceiverParty.PartyTaxScheme[0].RegistrationName.Value
                                },
                                AccountingSupplierParty = new AccountingSupplierParty
                                {
                                    RegistrationName = applicationResponseObj.SenderParty.PartyTaxScheme[0].RegistrationName.Value,
                                    PartyIdentificationID = applicationResponseObj.SenderParty.PartyTaxScheme[0].CompanyID.Value,
                                    PartyContactElectronicMail = eventDB.Email
                                }
                            },
                            _configuration["Email:TemplateDefault"],
                            _configuration["url:Portal.url"],
                            _documentBuild.GetEventName(eventDB.EventType),
                            eventDB.EventId,
                            eventDB.EventType,
                            eventDB.Email
                        );

                        SendEmailInternalResponse responseEmail = _emailClient.Send(paramSendEmail, attachedDocument, _documentBuild.GetEnvironmentDocumentBuild(_configuration["Environment"]), log);

                        if (responseEmail.Code == 200)
                        {
                            return new ResponseBase
                            {
                                Code = 200,
                                Message = responseEmail.Message
                            };
                        }
                        else
                        {
                            Exception exception = new Exception("Error al momento de enviar el correo electronico");
                            throw exception;
                        }
                    }
                    else
                    {
                        Exception exception = new Exception("El email destinatario es vacio");
                        throw exception;
                    }
                }
                else
                {
                    Exception exception = new Exception("Error al momento de consultar el evento");
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                return new ResponseBase
                {
                    Code = 103,
                    Message = ex.Message
                };
            }
        }
    }
}
