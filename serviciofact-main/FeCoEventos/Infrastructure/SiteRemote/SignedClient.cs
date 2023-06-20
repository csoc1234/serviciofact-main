using FeCoEventos.Clients.Signed;
using FeCoEventos.Domain.Entity;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class SignedClient : ISignedClient
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRestClient _apiRestClient;

        public SignedClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;
            _apiRestClient = apiRestClient;
        }

        public SignedInternalResponse SignedXml(string xml, Certificate certificate, ILogAzure log)
        {
            SignedResponse data = new SignedResponse();
            SignedInternalResponse response = new SignedInternalResponse();

            var uuidSession = log.GetSession();
            var requestToSend = new SignedRequest
            {
                nitCertificado = certificate.Identification,
                xml = xml,
                uuid = uuidSession,
                typeName = certificate.TypeName,
                nameFile = certificate.NameFile
            };

            //Cliente HTTP Rest
            ResponseHttp<SignedResponse> result = _apiRestClient.Post<SignedResponse>(
                 _configuration["url:Signed.url"],
                 _configuration["url:Signed.api"],
                 requestToSend,
                 String.Empty,
                 log
                 );

            if (result.Code == 200)
            {
                //Si fue 200, hubo respuesta positiva o negativa el API
                data = result.Result;

                if (data != null)
                {

                    if (data.code == "200")
                    {
                        if (uuidSession == data.uuid)
                        {
                            if (!string.IsNullOrEmpty(data.xml))
                            {
                                response = new SignedInternalResponse { Code = 200, File = data.xml, Message = data.message };

                                log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

                                return response;
                            }
                            else
                            {
                                response = new SignedInternalResponse { Code = 103, Message = "Error, no se genero el xml firmado" };
                            }
                        }
                        else
                        {
                            response = new SignedInternalResponse { Code = 4, Message = "Se presentó un error servicio de firma - Session de Firmado Erronea" };
                        }
                    }
                    else
                    {
                        response = new SignedInternalResponse { Code = 2, Message = String.Format("Se presentó un error servicio de firma, se retorna el siguiente mensaje {0}", data.message) };
                    }
                }
                else
                {
                    response = new SignedInternalResponse { Code = 2, Message = "Se presentó un error servicio de firma" };
                }
            }
            else
            {
                response = new SignedInternalResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Error);

            return response;
        }
    }
}