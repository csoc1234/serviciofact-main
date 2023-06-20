using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Infrastructure.Interface;
using APIAttachedDocument.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Reflection;

namespace APIAttachedDocument.Infrastructure.Signed
{
    public class SignedClient : ISignedClient
    {
        private readonly IConfiguration _configuration;

        public SignedClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SignedInternalResponse SignedXml(string xml, Certificate certificate, ILogAzure log)
        {
            SignedResponse data = new SignedResponse();
            SignedInternalResponse response = new SignedInternalResponse();

            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                RestClient serviceRest = new RestClient(_configuration["url:Signed.url"]);

                //serviceRest.Timeout = -1;

                var request = new RestRequest(_configuration["url:Signed.api"], Method.Post);

                request.AddHeader("Content-type", "application/json");

                var uuidSession = Guid.NewGuid().ToString();
                var requestToSend = new SignedRequest
                {
                    nitCertificado = certificate.Identification,
                    xml = xml,
                    uuid = uuidSession,
                    typeName = certificate.TypeName,
                    nameFile = certificate.NameFile
                };

                request.AddJsonBody(requestToSend);

                var responseWS = serviceRest.ExecuteAsync<SignedResponse>(request).Result;

                data = SignedResponse.FromJson(responseWS.Content);

                timeT.Stop();

                if (data != null)
                {
                    if (data.code == "200")
                    {
                        if (uuidSession == data.uuid)
                        {
                            response = new SignedInternalResponse { Code = 200, File = data.xml, Message = data.message };

                            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info, timeT.ElapsedMilliseconds);

                            return response;
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
                    response = new SignedInternalResponse { Code = 1, Message = "Error en Consumo de Firma" };
                }
            }
            catch (Exception ex)
            {
                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                response = new SignedInternalResponse { Code = 3, Message = "Error en transaccion de firma" };

                return response;
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Error, timeT.ElapsedMilliseconds);

            return response;
        }
    }
}

