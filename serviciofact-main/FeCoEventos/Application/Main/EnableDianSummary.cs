using FeCoEventos.Application.Interface;
using FeCoEventos.Application.Validation;
using FeCoEventos.Domain.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using System.Diagnostics;

namespace FeCoEventos.Application.Main
{
    public class EnableDianSummary : IEnableDianSummary
    {
        private readonly ILogAzure _log;
        private readonly IEnableSummaryDomain _enableSummaryDomain;

        public EnableDianSummary(ILogAzure logAzure, IEnableSummaryDomain senableSummaryDomain)
        {
            _log = logAzure;
            _enableSummaryDomain = senableSummaryDomain;
        }

        public EventsSummaryResponse GetList(string nit, LogRequest logRequest)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            _log.SetUp(new Models.CustomJwtTokenContext { User = new Models.CustomJwtTokenContext.UserClass { EnterpriseToken = "xxxxx", EnterpriseNit = "xxxx" } }, logRequest);

            //Validaciones Fluent Validation

            //Validation NIT
            var validator = new IdentificationValidator();

            ValidationResult resultvbr = validator.Validate(nit);

            ResponseValidation resultValidation = MainValidator.Check(resultvbr, _log);

            //Validation Result
            if (!resultValidation.IsValid)
            {
                var resultbr = new EventsSummaryResponse() { Code = resultValidation.Code, Message = resultValidation.Message };

                _log.SaveLog(resultValidation.Code, resultValidation.Message, ref timeT, LevelMsn.Error);

                return resultbr;
            }

            EventsSummaryResponse response = _enableSummaryDomain.GetList(nit, _log);

            _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

            return response;
        }
    }
}
