using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class ValidationXMLClient : IValidationXMLClient
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRestClient _apiRestClient;

        public ValidationXMLClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;
            _apiRestClient = apiRestClient;
        }

        public ValidationXMLResponse GetValidationXML(string cufe, ILogAzure log)
        {
            ValidationXMLResponse response = new ValidationXMLResponse();

            //Cliente HTTP Rest
            ResponseHttp<ValidationXMLResponse> result = _apiRestClient.Get<ValidationXMLResponse>(
                 _configuration["url:ComunicacionesDianUrl"],
                 _configuration["url:ComunicacionesDianConsultar"] + "/" + cufe,
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
                        if (response.ApplicationResponse != null)
                        {
                            return response;
                        }
                        else
                        {
                            response = new ValidationXMLResponse { Code = 103, Message = "No se logro obtener el XML de validacion" };
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(response.Message) || response.Message=="Error")
                        {
                            response.Message = response.EstatusDescripcion;
                        }
                        response = new ValidationXMLResponse { Code = response.Code, Message = response.Message };
                    }

                }
                else
                {
                    response = new ValidationXMLResponse { Code = 103, Message = "Error al realizar la obtencion del XMl de validacion" };
                }
            }
            else
            {
                response = new ValidationXMLResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);
            return response;
        }
    }
}
