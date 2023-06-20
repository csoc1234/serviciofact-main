using APIAttachedDocument.Application.Dto;
using APIAttachedDocument.Application.Interface;
using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace APIAttachedDocument.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AttachedDocumentController : ControllerBase
    {
        private readonly ICreateDocument _createDocument;

        public AttachedDocumentController(ICreateDocument createDocument)
        {
            _createDocument = createDocument;
        }

        /// <summary>
        /// Genera Attached Document
        /// </summary>
        /// <param name="request">Datos del Attached Document</param>
        /// <returns></returns>
        [HttpPost("/api/AttachedDocument")]
        public async Task<ActionResult<AttachedDocumentDto>> PostAttachedDocument([FromBody] FilesXmlDto request)
        {
            try
            {
                //Lectura del Token JWT            
                String tokenJwt = HttpContext.Request.Headers[HeaderNames.Authorization];

                LogRequest log = new LogRequest
                {
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = this.ControllerContext.RouteData.Values["action"].ToString(),
                    Application = LogAzure.ApplicationType.Integracion
                };

                EnterpriseCredential enterpriseCredential = new EnterpriseCredential
                {
                    Type = 1,
                    TokenJwt = tokenJwt
                };

                var response = _createDocument.Generate(request, enterpriseCredential, log);

                if (response.Code == 500)
                {
                    return UnprocessableEntity(response);
                }
                if (response.Code == 400)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(new AttachedDocumentDto()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Genera Attached Document
        /// </summary>
        /// <param name="request">Datos del Attached Document</param>
        /// <returns></returns>
        [HttpPost("/api/AttachedDocument/{idEnterprise}/{identificacion}")]
        [AllowAnonymous]
        public async Task<ActionResult<AttachedDocumentDto>> PostAttachedDocument([FromBody] FilesXmlDto request, string idEnterprise, string identificacion)
        {
            try
            {
                //Lectura del Token JWT            
                String tokenJwt = HttpContext.Request.Headers[HeaderNames.Authorization];

                LogRequest log = new LogRequest
                {
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = this.ControllerContext.RouteData.Values["action"].ToString(),
                    Application = LogAzure.ApplicationType.Integracion
                };

                EnterpriseCredential enterpriseCredential = new EnterpriseCredential
                {
                    Type = 2,
                    IdEnterprise = idEnterprise,
                    Identification = identificacion,
                    TokenJwt = string.Empty
                };

                var response = _createDocument.Generate(request, enterpriseCredential, log);

                if (response.Code == 500)
                {
                    return UnprocessableEntity(response);
                }
                if (response.Code == 400)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(new AttachedDocumentDto()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }
    }
}
