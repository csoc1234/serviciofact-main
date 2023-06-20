using APIAttachedDocument.Infrastructure.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Reflection;

namespace APIAttachedDocument.Infrastructure
{
    public class CertificateClient : ICertificateClient
    {
        private readonly IConfiguration _configuration;

        public CertificateClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CertificateResponse GetCertificate(string tokenJWT, ILogAzure log)
        {
            CertificateResponse response = new CertificateResponse();

            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                RestClient serviceRest = new RestClient(_configuration["url:CertificateUrl"]);

                var request = new RestRequest(_configuration["url:CertificateGet"], Method.Get);

                request.AddHeader("Content-type", "application/json");
                request.AddHeader("Authorization", $"{tokenJWT}");

                var responseWS = serviceRest.ExecuteAsync<CertificateResponse>(request).Result;

                response = JsonConvert.DeserializeObject<CertificateResponse>(responseWS.Content);

                if (response != null)
                {
                    if (response.Code == 200)
                    {
                        if (response.Certificate != null)
                        {
                            return response;
                        }
                        else
                        {
                            response = new CertificateResponse { Code = 103, Message = "El certificado no se logro obtener" };
                        }
                    }
                    else
                    {
                        response = new CertificateResponse { Code = response.Code, Message = response.Message };
                    }

                }
                else
                {
                    response = new CertificateResponse { Code = 1, Message = "Error al realizar la obtencion del certificado" };
                }
                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);
                return response;
            }
            catch (Exception ex)
            {
                timeT.Stop();
                response = new CertificateResponse { Code = 3, Message = "Error en transaccion" };
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
                return response;
            }
        }

        public CertificateResponse GetCertificateByIdentification(string idEnterprise, string identification, ILogAzure log)
        {
            CertificateResponse response = new CertificateResponse();

            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                RestClient serviceRest = new RestClient(_configuration["url:CertificateUrl"]);

                string api = string.Format("{0}/{1}/{2}", _configuration["url:CertificateGetByIdentification"], idEnterprise, identification);

                var request = new RestRequest(api, Method.Get);

                request.AddHeader("Content-type", "application/json");

                var responseWS = serviceRest.ExecuteAsync<CertificateResponse>(request).Result;

                response = JsonConvert.DeserializeObject<CertificateResponse>(responseWS.Content);

                if (response != null)
                {
                    if (response.Code == 200)
                    {
                        if (response.Certificate != null)
                        {
                            return response;
                        }
                        else
                        {
                            response = new CertificateResponse { Code = 103, Message = "El certificado no se logro obtener" };
                        }
                    }
                    else
                    {
                        response = new CertificateResponse { Code = response.Code, Message = response.Message };
                    }

                }
                else
                {
                    response = new CertificateResponse { Code = 1, Message = "Error al realizar la obtencion del certificado" };
                }
                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Info);
                return response;
            }
            catch (Exception ex)
            {
                timeT.Stop();
                response = new CertificateResponse { Code = 3, Message = "Error en transaccion" };
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
                return response;
            }
        }
    }
}
