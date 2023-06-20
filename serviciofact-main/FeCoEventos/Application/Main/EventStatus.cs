using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using System;
using System.Diagnostics;

namespace FeCoEventos.Application.Main
{
    public class EventStatus : IEventStatus
    {
        private readonly ILogAzure _log;
        private readonly IEventStatusDomain _eventStatusDomain;

        public EventStatus(ILogAzure logAzure, IEventStatusDomain eventStatusDomain)
        {
            _log = logAzure;
            _eventStatusDomain = eventStatusDomain;
        }

        public FactoringEventResponse GetStatus(EventDto eventDto, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(logRequest.Context, logRequest);

            FactoringEventResponse response = new FactoringEventResponse();

            try
            {
                //Validation Event
                EventFindValidator? validator = new EventFindValidator();

                ValidationResult resultvbr = validator.Validate(eventDto);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                if (!resultValidation.IsValid)
                {
                    FactoringEventResponse? resultbr = new FactoringEventResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                eventDto.EventId = eventDto.EventId.Trim();
                eventDto.TrackId = eventDto.TrackId.Trim();

                response = _eventStatusDomain.GetStatus(eventDto.EventId, eventDto.TrackId, _log);

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception e)
            {
                response.Code = 500;
                response.Message = "Error al intentar obetener el Evento de Factura";
                _log.SaveLog(response.Code, e.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }

        public FactoringEventResponse GetFactoringEvent(EventUuidDto eventUuidDto, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(new Models.CustomJwtTokenContext { User = new Models.CustomJwtTokenContext.UserClass { EnterpiseId = "0", EnterpriseToken = "Task", EnterpriseNit = "0" } }, logRequest);

            FactoringEventResponse response = new FactoringEventResponse();

            try
            {
                EventUuidValidator? validator = new EventUuidValidator();

                ValidationResult resultvbr = validator.Validate(eventUuidDto);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                if (!resultValidation.IsValid)
                {
                    FactoringEventResponse? resultbr = new FactoringEventResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                response = _eventStatusDomain.GetStatusByUuid(eventUuidDto.EventId.Trim(), eventUuidDto.EventUuid.Trim(), _log);

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception e)
            {
                response.Code = 500;
                response.Message = "Error al intentar obetener el Evento de Factura";
                _log.SaveLog(response.Code, e.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }

    }
}
