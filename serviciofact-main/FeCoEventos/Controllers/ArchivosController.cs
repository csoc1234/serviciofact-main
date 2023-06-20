using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Models;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace FeCoEventos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ArchivosController : ControllerBase
    {
        private readonly IDownloadFile _downloadFile;

        public ArchivosController(IDownloadFile downloadFile)
        {
            _downloadFile = downloadFile;
        }

        /// <summary>
        /// Permite la descarga de archivos Xml asociados a un evento
        /// </summary>
        /// <param name="eventId">Numero de Evento</param>
        /// <param name="trackId">Tracking ID</param>
        /// <param name="fileType">Tipos de Archivos</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/descargar/{eventId}/{trackId}/{fileType}")]
        public async Task<ActionResult<FileXmlResponse>> GetEvent([FromRoute] string eventId, string trackId, int fileType)
        {
            //Se obtiene el String Token de Autenticacion y se transforma a la clase Token
            var contextstr = HttpContext.User.FindFirst("context").Value;
            var context = CustomJwtTokenContext.FromJson(contextstr);

            //Lectura del Token JWT            
            String tokenJwt = HttpContext.Request.Headers[HeaderNames.Authorization];

            try
            {
                LogRequest log = new LogRequest
                {
                    Context = context,
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                DownloadFileDto request = new DownloadFileDto
                {
                    EventId = eventId,
                    TrackId = trackId,
                    FileType = fileType
                };

                var response = _downloadFile.GetFile(request, tokenJwt, log);

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
                return UnprocessableEntity(new FactoringEventResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Permite la descarga de archivos XML asociados a un evento externo.
        /// </summary>
        /// <param name="uuid">cufe o cude</param>
        /// <param name="DocumentId">Identificador de la factura</param>
        /// <param name="EventType">Tipo de evento</param>
        /// <param name="fileType">Tipo de archivo</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/descargarEventoExterno/{uuid}/{DocumentId}/{EventType}/{fileType}")]
        public async Task<ActionResult<FileXmlResponse>> GetExternalEvent([FromRoute] string uuid, string DocumentId, string EventType, int fileType)
        {
            var stringContext = HttpContext.User.FindFirst("context").Value;
            var context = CustomJwtTokenContext.FromJson(stringContext);

            String tokenJwt = HttpContext.Request.Headers[HeaderNames.Authorization];

            try
            {
                LogRequest log = new LogRequest
                {
                    Context = context,
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                DownloadFileExternalDto request = new DownloadFileExternalDto
                {
                    uuid = uuid,
                    DocumentId = DocumentId,
                    EventType = EventType,
                    FileType = fileType
                };

                var response = _downloadFile.GetFileExternal(request, tokenJwt, log);

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
                return UnprocessableEntity(new FactoringEventResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }
    }
}
