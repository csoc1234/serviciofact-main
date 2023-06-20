using Contributors.Application.Dto;
using Contributors.Application.Dto.Response;
using Contributors.Application.Interface;
using Contributors.Application.Validator;
using Contributors.Domain.Interface;
using Contributors.Infraestructure.Logging;
using Contributors.Infraestructure.Logging.Interface;
using FluentValidation.Results;
using System.Diagnostics;
using System.Linq;

namespace Contributors.Application
{
    public class TaxpayerListStatus : ITaxpayerListStatus
    {
        private readonly ILogAzure log;
        private readonly ITaxpayersListDomain _taxpayersListDomain;

        public TaxpayerListStatus(ILogAzure logAzure, ITaxpayersListDomain taxpayersListDomain)
        {
            log = logAzure;
            _taxpayersListDomain = taxpayersListDomain;
        }

        public TaxPayerListStatusResponse GetList(int status, LogRequest logRequest)
        {
            //Log
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            log.Setup(logRequest.Context, logRequest.Method, logRequest.Application, logRequest.Api);


            //Check Fluent Validation  
            StatusDto request = new StatusDto { Status = status };
                                  
            var validator = new StatusValidator();

            ValidationResult resultValidation = validator.Validate(request);

            if (!resultValidation.IsValid)
            {
                var resultMessage = string.Join("; ", resultValidation.Errors.Select(x => x.ErrorMessage));

                TaxPayerListStatusResponse result = new TaxPayerListStatusResponse
                {
                    Code = 400,
                    Message = resultMessage
                };

                log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                return result;
            }

            //Domain
            TaxPayerListStatusResponse response = _taxpayersListDomain.GetList(status);

            //result
            return response;
        }
    }
}
