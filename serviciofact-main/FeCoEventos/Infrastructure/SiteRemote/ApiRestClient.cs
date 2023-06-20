using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace FeCoEventos.Infrastructure.SiteRemote
{
    public class ApiRestClient : IApiRestClient
    {
        private readonly IRestClient _restClient;

        public ApiRestClient(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public ResponseHttp<T> Get<T>(string url, string api, string tokenJwt, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            var response = new ResponseHttp<T>();

            try
            {
                //url API
                _restClient.BaseUrl = new Uri(url);

                //Timeout
                _restClient.Timeout = -1;

                //Path
                var request = new RestRequest(api, Method.GET);

                request.AddHeader("Content-type", "application/json");

                //Carga del Token JWT para autenticacion
                if (!string.IsNullOrEmpty(tokenJwt))
                {
                    request.AddHeader("Authorization", $"{tokenJwt}");
                }

                var responseWS = _restClient.Execute<T>(request);

                timeT.Stop();

                response = HandlerResponse<T>(responseWS, timeT.ElapsedMilliseconds, log);

                return response;
            }
            catch (Exception ex)
            {
                timeT.Stop();

                response = new ResponseHttp<T> { Code = 500, Message = "Error al momento de consumir el API" };

                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return response;
            }
        }

        public ResponseHttp<T> Post<T>(string url, string api, object body, string tokenJwt, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            var response = new ResponseHttp<T>();

            try
            {
                //url API
                _restClient.BaseUrl = new Uri(url);

                //Timeout
                _restClient.Timeout = -1;

                //Path
                var request = new RestRequest(api, Method.POST);

                request.AddHeader("Content-type", "application/json");

                //Carga del Token JWT para autenticacion
                if (!string.IsNullOrEmpty(tokenJwt))
                {
                    request.AddHeader("Authorization", $"{tokenJwt}");
                }

                //Se carga el Body
                request.AddJsonBody(body);	 

                var responseWS = _restClient.Execute<T>(request);

                timeT.Stop();

                response = HandlerResponse<T>(responseWS, timeT.ElapsedMilliseconds, log);

                return response;
            }
            catch (Exception ex)
            {
                timeT.Stop();

                response = new ResponseHttp<T> { Code = 500, Message = "Error al momento de consumir el API" };

                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return response;
                //
            }
        }


        public ResponseHttp<T> HandlerResponse<T>(IRestResponse restResponse, long elapsedMilliseconds, ILogAzure log)
        {
            var response = new ResponseHttp<T>();

            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.OK:

                    var result = JsonConvert.DeserializeObject<T>(restResponse.Content);

                    if (result != null)
                    {
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, "Consumo Api Exitosa", LevelMsn.Info, elapsedMilliseconds);

                        return new ResponseHttp<T>
                        {
                            Code = 200,
                            Message = "Ok",
                            Result = result
                        };
                    }
                    else
                    {
                        response = new ResponseHttp<T> { Code = 103, Message = "No se ha obtenido respuesta del Servicio" };
                    }

                    break;

                case HttpStatusCode.Unauthorized:

                    response = new ResponseHttp<T> { Code = (int)HttpStatusCode.Unauthorized, Message = "Servicio no autorizado" };

                    break;

                case HttpStatusCode.NotFound:

                    response = new ResponseHttp<T> { Code = (int)HttpStatusCode.NotFound, Message = "Servicio no encontrado" };

                    break;

                case HttpStatusCode.InternalServerError:

                    response = new ResponseHttp<T> { Code = (int)HttpStatusCode.InternalServerError, Message = "Error Interno" };

                    break;

                default:

                    response = new ResponseHttp<T> { Code = 500, Message = "No se ha obtenido respuesta del Servicio" };

                    break;
            }

            log.WriteComment(MethodBase.GetCurrentMethod().Name, response.Message, LevelMsn.Warning, elapsedMilliseconds);

            return response;
        }
    }
}
