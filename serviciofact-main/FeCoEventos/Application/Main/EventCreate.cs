using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FeCoEventos.Application.Main
{
    public class EventCreate : IEventCreate
    {
        private readonly IConfiguration _configuration;
        private readonly ILogAzure _log;
        private static IEventCreateDomain _eventCreateDomain;

        public EventCreate(IEventCreateDomain eventCreateDomain, ILogAzure logAzure, IConfiguration configuration)
        {
            _log = logAzure;
            _eventCreateDomain = eventCreateDomain;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="logRequest"></param>
        public EventsBuildResponse Generate(EventsBuildRequest request, LogRequest logRequest, string tokenJWT)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(logRequest.Context, logRequest);


            try
            {
                request.Environment = request.Environment == null ? (_configuration["Environment"] == "prod" ? 1 : 0) : request.Environment;

                if (request == null)
                {
                    EventsBuildResponse? result = new EventsBuildResponse() { Code = 400, Message = "Bad Request" };
                    timeT.Stop();
                    _log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                    return result;
                }

                //AutoGeneracion de Numero
                if (request.Environment == 1 || request.Environment == 0)
                {
                    Random random = new Random();
                    request.CorrelativeNumber = DateTime.Now.ToString("yyddffff") + random.Next(1000, 9999);
                }
                else if (request.Environment == 2)
                {
                    //Si el correlativo es vacio, le asignamos una
                    if (string.IsNullOrEmpty(request.CorrelativeNumber))
                    {
                        Random random = new Random();
                        request.CorrelativeNumber = DateTime.Now.ToString("yyddffff") + random.Next(1000, 9999);
                    }
                }

                //Validaciones Fluent Validation
                BuildApplicationResponseValidator? validator = new BuildApplicationResponseValidator(_configuration);

                ValidationResult resultvbr = validator.Validate(request);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                //Validation Result
                if (!resultValidation.IsValid)
                {
                    EventsBuildResponse? resultbr = new EventsBuildResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                EventsBuildResponse? resultT = _eventCreateDomain.Create(request, logRequest.Context, tokenJWT, _log);

                _log.SaveLog(resultT.Code, resultT.Message, ref timeT, LevelMsn.Info);

                return resultT;
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                EventsBuildResponse? response = new EventsBuildResponse()
                {
                    Code = 500,
                    Message = "Error al intentar generar un evento"
                };

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }
    }
}
