using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Infrastructure.ComunicationDian
{
    public class DianStatusRestClient : IDianStatusRestClient
    {
        private readonly IConfiguration _configuration;

        public DianStatusRestClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DianStatusResponse GetDianStatus(string cufe, ILogMongo log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();
            
            DianStatusResponse response = new DianStatusResponse();

            try
            {
                RestClient client = new RestClient(_configuration["url:DianCommunication"]);
                client.Timeout = -1;
                var request = new RestRequest(_configuration["api:DianCommunication.get"] + $"/{cufe}", Method.GET);
                var apiResponse = client.Execute<DianStatusResponse>(request);
                if (apiResponse.IsSuccessful)
                {
                    if (apiResponse.Content != null)
                    {
                        response = JsonConvert.DeserializeObject<DianStatusResponse>(apiResponse.Content);
                        log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);
                    }
                    else
                    {
                        response.Code = 100;
                        response.Message = "No se logro obtener el estatus del documento electrónico en la DIAN para el cufe dado";
                        log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                    }
                }
                else
                {
                    response.Code = 100;
                    response.Message = "No se logro obtener el estatus del documento electrónico en la DIAN para el cufe dado";
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response.Code = 500;
                response.Message = "Error en transaccion";
                log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);
            }
            timeT.Stop();
            return response;
        }

        public DianStatusResponse GetStatusEvent(string cufe, ILogMongo log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            DianStatusResponse response = new DianStatusResponse();

            try
            {
                RestClient client = new RestClient(_configuration["url:DianCommunication"]);
                client.Timeout = -1;
                var request = new RestRequest(_configuration["api:DianCommunication.getEvents"] + $"/{cufe}", Method.GET);
                var apiResponse = client.Execute<DianStatusResponse>(request);
                if (apiResponse.IsSuccessful)
                {
                    if (apiResponse.Content != null)
                    {
                        response = JsonConvert.DeserializeObject<DianStatusResponse>(apiResponse.Content);
                        log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);
                    }
                    else
                    {
                        response.Code = 100;
                        response.Message = "No se logro obtener los eventos del documento electrónico en la DIAN para el cufe dado";
                        log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                    }
                }
                else
                {
                    response.Code = 100;
                    response.Message = "No se logro obtener los eventos del documento electrónico en la DIAN para el cufe dado";
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response.Code = 500;
                response.Message = "Error en transaccion";
                log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);
            }

            timeT.Stop();
            return response;
        }
    }
}
