using Contributors.Application.Dto;
using Contributors.Application.Interface;
using Contributors.Application.Validator;
using Contributors.Domain.Interface;
using Contributors.Infraestructure.Logging;
using Contributors.Infraestructure.Logging.Interface;
using Contributors.Models.Response;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Contributors.Application
{
    public class EnableDianCreate : IEnableDianCreate
    {
        private readonly IConfiguration _configuration;
        private readonly ILogAzure log;
        private readonly IStartEnableDianDomain _startEnableDianDomain;

        public EnableDianCreate(IConfiguration configuration, ILogAzure logAzure, IStartEnableDianDomain startEnableDianDomain)
        {
            log = logAzure;
            _configuration = configuration;
            _startEnableDianDomain = startEnableDianDomain;
        }

        public ResponseBase Register(TaxPayersDto request, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            log.Setup(logRequest.Context, logRequest.Method, logRequest.Application, logRequest.Api);

            if (request == null)
            {
                var result = new ResponseBase() { Code = 400, Message = "Bad Request" };
                timeT.Stop();
                log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                return result;
            }

            //Check Fluent Validation            
            var validator = new EnableDianValidator();

            ValidationResult resultValidation = validator.Validate(request);

            if (!resultValidation.IsValid)
            {
                var resultMessage = string.Join("; ", resultValidation.Errors.Select(x => x.ErrorMessage));

                ResponseBase result = new ResponseBase
                {
                    Code = 400,
                    Message = resultMessage
                };

                log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                return result;
            }

            //Domain
            ResponseBase response = _startEnableDianDomain.Register(request.CompanyId, request.TestSetId);

            timeT.Stop();
            log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

            return response;
        }
    }
}
