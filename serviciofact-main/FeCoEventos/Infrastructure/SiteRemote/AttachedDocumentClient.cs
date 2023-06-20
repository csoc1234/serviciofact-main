using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class AttachedDocumentClient : IAttachedDocumentClient
    {
        private readonly IConfiguration _configuration;
        private readonly IApiRestClient _apiRestClient;

        public AttachedDocumentClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;
            _apiRestClient = apiRestClient;
        }

        public AttachedDocumentResponse GenerateXml(AttachedDocumentRequest requestFile, string tokenJwt, ILogAzure log)
        {
            var response = new AttachedDocumentResponse();

            //Cliente HTTP Rest
            ResponseHttp<AttachedDocumentResponse> result = _apiRestClient.Post<AttachedDocumentResponse>(
                 _configuration["url:AttachedDocument.url"],
                 _configuration["url:AttachedDocument.api"],
                 requestFile,
                 tokenJwt,
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
                        if (!string.IsNullOrEmpty(response.File))
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

                            //Se retorna el Xml
                            return response;
                        }
                        else
                        {
                            response = new AttachedDocumentResponse { Code = 104, Message = String.Format("Error al momento de generar el Xml") };
                        }
                    }
                    else
                    {
                        response = new AttachedDocumentResponse { Code = response.Code, Message = response.Message };
                    }
                }
                else
                {
                    response = new AttachedDocumentResponse { Code = 103, Message = "Se presento un error al momento de consumir el servicio" };
                }
            }
            else
            {
                response = new AttachedDocumentResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

            return response;
        }

        public AttachedDocumentResponse GenerateXmlByIdentification(AttachedDocumentRequest requestFile, int idEnterprise, string identification, ILogAzure log)
        {
            var response = new AttachedDocumentResponse();

            //Cliente HTTP Rest
            ResponseHttp<AttachedDocumentResponse> result = _apiRestClient.Post<AttachedDocumentResponse>(
                 _configuration["url:AttachedDocument.url"],
                 string.Format("{0}/{1}/{2}", _configuration["url:AttachedDocument.api"], idEnterprise, identification),
                 requestFile,
                 string.Empty,
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
                        if (!string.IsNullOrEmpty(response.File))
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

                            //Se retorna el Xml
                            return response;
                        }
                        else
                        {
                            response = new AttachedDocumentResponse { Code = 104, Message = String.Format("Error al momento de generar el Xml") };
                        }
                    }
                    else
                    {
                        response = new AttachedDocumentResponse { Code = response.Code, Message = response.Message };
                    }
                }
                else
                {
                    response = new AttachedDocumentResponse { Code = 103, Message = "Se presento un error al momento de consumir el servicio" };
                }
            }
            else
            {
                response = new AttachedDocumentResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

            return response;
        }
    }
}
