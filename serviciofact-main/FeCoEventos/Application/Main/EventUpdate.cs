using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FeCoEventos.Application.Main
{
    public class EventUpdate : IEventUpdate
    {
        private readonly IEventUpdateDomain _eventUpdateDomain;
        private readonly ILogAzure _log;

        public EventUpdate(ILogAzure logAzure, IEventUpdateDomain eventUpdateDomain)
        {
            _log = logAzure;
            _eventUpdateDomain = eventUpdateDomain;
        }

        public EventUpdatingResponse UpdateDianResult(EventDto eventDto, EventUpdatingRequest request, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(logRequest.Context, logRequest);

            try
            {
                //Fluent Validation

                //Validation Event
                var validator = new EventFindValidator();

                ValidationResult resultvbr = validator.Validate(eventDto);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                //TODO
                //Validation Update

                //Validation Result
                if (!resultValidation.IsValid)
                {
                    var resultbr = new EventUpdatingResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                //Domain
                var response = _eventUpdateDomain.UpdateDianResult(eventDto.EventId, eventDto.TrackId, request, _log);

                //TODO Crear Dto y asignar en el return


                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                EventUpdatingResponse response = new EventUpdatingResponse
                {
                    Code = 500,
                    Message = "Se genero error al momento de realizar la transaccion"
                };

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }

        public ResponseBase DeliveryAsyncDian(EventDto eventDto, EventDeliveryAsyncDto request, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(logRequest.Context, logRequest);

            try
            {
                //Validation Fluent
                var validator = new EventFindValidator();

                ValidationResult resultvbr = validator.Validate(eventDto);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                if (!resultValidation.IsValid)
                {
                    var resultbr = new FactoringEventResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                request.TrackIdDian = request.TrackIdDian == null ? "" : request.TrackIdDian.Trim();

                var validatorUpdate = new EventUpdateValidator();

                ValidationResult resultUpdate = validatorUpdate.Validate(request);

                if (!resultUpdate.IsValid)
                {
                    string errormessages = string.Join("; ", resultUpdate.Errors.Select(x => x.ErrorMessage));

                    var resultbr = new FactoringEventResponse() { Code = 400, Message = errormessages };
                    _log.SaveLog(resultbr.Code, resultbr.Message, ref timeT, LevelMsn.Error);

                    return resultbr;
                }

                eventDto.EventId = eventDto.EventId.ToLower().Trim();
                eventDto.TrackId = eventDto.TrackId.ToLower().Trim();
                request.TrackIdDian = !string.IsNullOrEmpty(request.TrackIdDian) ? request.TrackIdDian.Trim().ToLower() : "";

                //Update DB
                var response = _eventUpdateDomain.UpdateAsyncDelivery(eventDto, request, _log);

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                ResponseBase response = new ResponseBase
                {
                    Code = 500,
                    Message = "Se genero error al momento de realizar la transaccion"
                };

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }
    }
}
