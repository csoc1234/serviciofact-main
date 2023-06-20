using FeCoEventos.Util.TableLog;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace FeCoEventos.Application.Validation
{
    public class MainValidator
    {
        public static ResponseValidation Check(ValidationResult result, ILogAzure _log, string? resultConcat = null)
        {
            ResponseValidation response = new ResponseValidation();

            try
            {
                if (!result.IsValid)
                {
                    string errormessages = GetMessage(result);

                    response = new ResponseValidation() { Code = 400, Message = errormessages };

                    if (!string.IsNullOrEmpty(resultConcat))
                    {
                        response.Message = string.Format("{0}; {1}", response.Message, resultConcat);
                    }
                }
                else if (!string.IsNullOrEmpty(resultConcat))
                {
                    response = new ResponseValidation() { Code = 400, Message = resultConcat };
                }
                else
                {
                    //Ha pasado las validaciones
                    response = new ResponseValidation() { Code = 200, Message = "", IsValid = true };
                }

                _log.WriteComment(MethodBase.GetCurrentMethod().Name, "Validaciones", LevelMsn.Info);

            }
            catch (Exception ex)
            {
                _log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error);

                response = new ResponseValidation { Code = 400, Message = "Error al momento de validar los datos de entrada" };
            }

            return response;
        }

        public static string GetMessage(ValidationResult result)
        {
            return string.Join("; ", result.Errors.Select(x => x.ErrorMessage));
        }
    }
}