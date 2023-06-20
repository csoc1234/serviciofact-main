using APIAttachedDocument.Application.Dto;
using APIAttachedDocument.Application.Interface;
using APIAttachedDocument.Application.Validation;
using APIAttachedDocument.Application.Validation.Result;
using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Domain.Interface;
using APIAttachedDocument.Infrastructure.Logging;
using APIAttachedDocument.Transversal.Model;
using FluentValidation.Results;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace APIAttachedDocument.Application.Main
{
    public class CreateDocument : ICreateDocument
    {
        private readonly ILogAzure _log;
        private readonly IConfiguration _configuration;
        private readonly ICreateDocumentDomain _createDocumentDomain;

        public CreateDocument(ILogAzure log, IConfiguration configuration, ICreateDocumentDomain createDocumentDomain)
        {
            _log = log;
            _configuration = configuration;
            _createDocumentDomain = createDocumentDomain;
        }

        public AttachedDocumentDto Generate(FilesXmlDto request, EnterpriseCredential enterpriseCredential, LogRequest logRequest)
        {
            Stopwatch timeT = new();
            timeT.Start();

            AttachedDocumentDto response = new();

            _log.SetUp(new CustomJwtTokenContext { User = new CustomJwtTokenContext.UserClass { EnterpriseToken = "xxxxx", EnterpriseNit = "xxxx" } },
                            logRequest.Method,
                            _configuration,
                            logRequest.Application,
                            logRequest.Api);
            try
            {
                //Validaciones de Entrada              
                var validator = new FileXmlValidator(_configuration);

                ValidationResult resultValidationXml = validator.Validate(request);

                var resultMessage = "";

                if (!resultValidationXml.IsValid)
                {
                    resultMessage = MessageResult.GetMessage(resultValidationXml);
                }

                if (!resultValidationXml.IsValid)
                {
                    AttachedDocumentDto result = new AttachedDocumentDto
                    {
                        Code = 400,
                        Message = resultMessage
                    };

                    _log.SaveLog(result.Code, result.Message, ref timeT, LevelMsn.Error);

                    return result;
                }
                else
                {
                    _log.WriteComment(MethodBase.GetCurrentMethod().Name, "Validación Cufe OK", LevelMsn.Info);
                }

                //Domain
                var resultDomain = _createDocumentDomain.Generate(request.Xml, request.XmlDian, enterpriseCredential, _log);

                if (resultDomain.Code == 200)
                {
                    response = new AttachedDocumentDto
                    {
                        Code = resultDomain.Code,
                        Message = resultDomain.Message,
                        File = resultDomain.File,
                        Hash = resultDomain.Hash,
                        Namefile = resultDomain.Namefile,
                        Size = resultDomain.Size
                    };
                }
                else
                {
                    response = new AttachedDocumentDto
                    {
                        Code = resultDomain.Code,
                        Message = resultDomain.Message
                    };
                }

                _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

                return response;
            }
            catch (Exception ex)
            {
                response = new AttachedDocumentDto
                {
                    Code = 500,
                    Message = "Se genero un error mientras se consultaba el cufe en la DIAN"
                };

                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);
            }

            _log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);

            return response;
        }
    }
}
