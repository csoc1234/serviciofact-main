using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class CertificatesClient : ICertificatesClient
    {
        private readonly IConfiguration _configuration;        
        private readonly IApiRestClient _apiRestClient;

        public CertificatesClient(IConfiguration configuration, IApiRestClient apiRestClient)
        {
            _configuration = configuration;            
            _apiRestClient = apiRestClient;
        }

        public CertificatesResponse GetCertificate(string tokenJwt, ILogAzure log)
        {
            CertificatesResponse response = new CertificatesResponse();

            //Cliente HTTP Rest
            ResponseHttp<CertificatesResponse> result = _apiRestClient.Get<CertificatesResponse>(
                 _configuration["url:CertificateUrl"],
                 _configuration["url:CertificateGet"],
                 tokenJwt,
                 log
                 );

            if (result.Code == 200)
            {
                //Si fue 200, hubo respuesta positiva o negativa el API
                response = result.Result;

                //Resultado Exitoso
                if (response.Code == 200)
                {
                    if (response.Certificate != null)
                    {
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

                        //Carga de los datos del response en el certificado
                        return response;
                    }
                    else
                    {
                        response = new CertificatesResponse { Code = 103, Message = "El certificado no se logro obtener" };
                    }
                }
                else
                {
                    response = new CertificatesResponse { Code = response.Code, Message = response.Message };
                }
            }
            else
            {
                response = new CertificatesResponse { Code = result.Code, Message = result.Message };
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);

            return response;
        }
    }
}
