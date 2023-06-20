using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class EventXmlClient : IEventXmlClient
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRestClient _apiRestClient;

        public EventXmlClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;
            _apiRestClient = apiRestClient;
        }

        public EventXMLResponse GetEventXML(string cufe, ILogAzure log)
        {
            EventXMLResponse response = new EventXMLResponse();

            //Cliente HTTP Rest
            ResponseHttp<EventXMLResponse> result = _apiRestClient.Get<EventXMLResponse>(
                 _configuration["url:ComunicacionesDianUrl"],
                 _configuration["url:ComunicacionesDianConsultarXML"] + "/" + cufe,
                 String.Empty,
                 log
                 );

            if (result.Code == 200)
            {
                //Si fue 200, hubo respuesta positiva o negativa el API
                response = result.Result;

                if (response != null)
                {
                    if (response.Code == 200)
                    {
                        if (!string.IsNullOrEmpty(response.ApplicationResponse))
                        {
                            return response;
                        }
                        else
                        {
                            response = new EventXMLResponse { Code = 103, Message = "No se logro obtener el XML del Evento" };
                        }
                    }
                    else
                    {
                        response = new EventXMLResponse { Code = response.Code, Message = response.Message };
                    }
                }
                else
                {
                    response = new EventXMLResponse { Code = 103, Message = "Error al momento de realizar la transaccion" };
                }
            }
            else
            {
                response = new EventXMLResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);
            return response;
        }
    }
}
