using DocumentBuildCO.Request;
using FeCoEventos.Clients.SendEmail;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class EmailClient : IEmailClient
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRestClient _apiRestClient;

        public EmailClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;
            _apiRestClient = apiRestClient;
        }

        public SendEmailInternalResponse Send(ParamSendEmail inputSendEmail, AttachedDocumentResponse attachedDocument, Ambiente enviroment, ILogAzure log)
        {
            SendEmailResponse data = new SendEmailResponse();
            SendEmailInternalResponse response = new SendEmailInternalResponse();

            string fromEmail = _configuration["Counts:FromEmail"];

            if (String.IsNullOrEmpty(inputSendEmail.Email))
            {
                return new SendEmailInternalResponse() { Code = 104, Message = "No se pudo generar correo de notificación de evento, puesto que no se tiene información de dirección de correo del destinatario" };
            }

            SendEmailRequest requestSendEmail = new SendEmailRequest(inputSendEmail.NameTemplate,
                inputSendEmail.NameSupplier, inputSendEmail.SupplierId, inputSendEmail.NameCustomer,
                inputSendEmail.DocumentId, inputSendEmail.EventName, inputSendEmail.EventId, inputSendEmail.EventType, inputSendEmail.UrlWeb,
                inputSendEmail.Email, fromEmail, inputSendEmail.BusinessLine, attachedDocument,
                Convert.ToBoolean(_configuration["Email:CompressAttachments"]), enviroment);

            ResponseHttp<SendEmailResponse> result = _apiRestClient.Post<SendEmailResponse>(
                 _configuration["url:SendEmail.url"],
                 _configuration["url:SendEmail.api"],
                 requestSendEmail,
                 String.Empty,
                 log
                 );

            if (result.Code == 200)
            {
                //Si fue 200, hubo respuesta positiva o negativa el API
                data = result.Result;

                if (data != null)
                {
                    if (data.Code == 200)
                    {
                        response = new SendEmailInternalResponse { Code = data.Code, Message = data.Message, EmailRequestId = data.EmailRequestId };

                        log.WriteComment(MethodBase.GetCurrentMethod().Name, data.Message, LevelMsn.Info);

                        return response;
                    }
                    else
                    {
                        response = new SendEmailInternalResponse { Code = data.Code, Message = data.Message };
                    }
                }
                else
                {
                    response = new SendEmailInternalResponse { Code = 102, Message = "Error en Consumo de Servicio de Envío de Correo" };
                }
            }
            else
            {
                response = new SendEmailInternalResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Error);

            return response;
        }
    }
}
