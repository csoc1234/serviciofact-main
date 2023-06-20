using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FeCoEventos.Application.Main
{
    /// <summary>
    /// Lista los eventos activos en el sistema
    /// </summary>
    public class EventList : IEventList
    {
        private readonly IEventListDomain _eventListDomain;
        private readonly ILogAzure _log;

        public EventList(IEventListDomain eventListDomain, ILogAzure logAzure)
        {
            _eventListDomain = eventListDomain;
            _log = logAzure;
        }

        /// <summary>
        /// Se consulta la lista de eventos activos
        /// </summary>
        /// <param name="request">Rango de fecha, estatus y codigo de evento para la consulta</param>
        /// <param name="logRequest">Valores del Controller para el log</param>
        /// <returns></returns>
        public EventsPendingResponse GetList(EventStatusDto request, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(new Models.CustomJwtTokenContext { User = new Models.CustomJwtTokenContext.UserClass { EnterpriseToken = "xxxxx", EnterpriseNit = "xxxx" } }, logRequest);

            EventsPendingResponse response = new EventsPendingResponse();

            try
            {
                //Validaciones de Entrada
                var validator = new EventStatusValidator();

                ValidationResult validationList = validator.Validate(request);

                ResponseValidation resultValidation = MainValidator.Check(validationList, _log);

                if (!resultValidation.IsValid)
                {
                    EventsPendingResponse result = new EventsPendingResponse { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return result;
                }

                response = _eventListDomain.GetEventsByStatus(request, _log);

                _log.WriteComment(MethodBase.GetCurrentMethod().Name, "Se realiza la consulta", LevelMsn.Info);
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                EventsPendingResponse result = new EventsPendingResponse
                {
                    Code = 500,
                    Message = "Se genero error al momento de realizar la transaccion"
                };

                _log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                return result;
            }

            _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

            return response;
        }
    }
}
