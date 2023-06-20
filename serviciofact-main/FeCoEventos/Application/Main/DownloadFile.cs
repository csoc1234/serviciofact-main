using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FeCoEventos.Application.Main
{
    public class DownloadFile : IDownloadFile
    {
        private readonly ILogAzure _log;
        private readonly IDownloadFileDomain _downloadFileDomain;

        public DownloadFile(ILogAzure logAzure, IDownloadFileDomain downloadFileDomain)
        {
            _log = logAzure;
            _downloadFileDomain = downloadFileDomain;
        }

        public FileDto GetFile(DownloadFileDto request, string tokenJwt, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            var response = new FileDto();

            _log.SetUp(logRequest.Context, logRequest);

            try
            {
                //Validaciones Fluent Validation

                //Validation Event
                var validator = new EventFindValidator();
                EventDto ev = new EventDto { EventId = request.EventId, TrackId = request.TrackId };
                ValidationResult validationEvent = validator.Validate(ev);

                ResponseValidation resultValidation = MainValidator.Check(validationEvent, _log);

                //Validation File
                var validatorFile = new FileTypeValidator();

                ValidationResult validationFile = validatorFile.Validate(request.FileType);

                resultValidation = MainValidator.Check(validationFile, _log, resultValidation.Message);

                //Validation Result
                if (!resultValidation.IsValid)
                {
                    var resultDto = new FileDto() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultDto;
                }

                //Consuming Domain
                var result = _downloadFileDomain.GetFile(request.EventId, request.TrackId, request.FileType, tokenJwt, logRequest.Context.User.EnterpriseNit, _log);

                if (result.Code == 200)
                {
                    response = new FileDto()
                    {
                        Code = result.Code,
                        Message = result.Message,
                        FileXml = new FileXml
                        {
                            File = result.FileXml.File,
                            Name = result.FileXml.Name
                        }
                    };
                }
                else
                {
                    response = new FileDto()
                    {
                        Code = result.Code,
                        Message = result.Message
                    };
                }

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response = new FileDto()
                {
                    Code = 500,
                    Message = "Error al intentar obetener el Evento de Factura"
                };

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }

        public FileDto GetFileExternal(DownloadFileExternalDto request, string tokenJwt, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            var response = new FileDto();

            _log.SetUp(logRequest.Context, logRequest);

            try
            {
                //Validaciones Fluent Validation
                var validator = new FileTypeExternalValidator();
                ValidationResult resultvbr = validator.Validate(request);

                ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

                //Validation Result
                if (!resultValidation.IsValid)
                {
                    var resultDto = new FileDto() { Code = resultValidation.Code, Message = resultValidation.Message };

                    _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                    return resultDto;
                }

                var result = _downloadFileDomain.GetFileExternal(request.uuid, request.DocumentId, request.EventType, request.FileType, logRequest.Context.User.EnterpriseNit, tokenJwt, _log);

                if (result.Code == 200)
                {
                    response = new FileDto()
                    {
                        Code = result.Code,
                        Message = result.Message,
                        FileXml = new FileXml
                        {
                            File = result.FileXml.File,
                            Name = result.FileXml.Name
                        }
                    };
                }
                else
                {
                    response = new FileDto() { Code = result.Code, Message = result.Message };
                }

                _log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response = new FileDto()
                {
                    Code = 500,
                    Message = "Error al intentar obetener el Evento de Factura"
                };

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Error);

                return response;
            }
        }
    }
}
