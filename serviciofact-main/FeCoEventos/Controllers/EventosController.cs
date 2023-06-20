using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Models;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace FeCoEventos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EventosController : ControllerBase
    {
        private readonly IEventList _eventList;
        private readonly IEventCreate _eventCreate;
        private readonly IEventStatus _eventStatus;
        private readonly IEventUpdate _eventUpdate;

        public EventosController(
          IEventList eventList,
          IEventCreate eventCreate,
          IEventStatus eventStatus,
          IEventUpdate eventUpdate)
        {
            _eventList = eventList;
            _eventCreate = eventCreate;
            _eventStatus = eventStatus;
            _eventUpdate = eventUpdate;
        }

        /// <summary>
        /// Establece un Nuevo Evento de Factura
        /// </summary>
        /// <param name="request">Datos del Evento</param>
        /// <returns></returns>
        [HttpPost("/api/BuildApplicationResponse")]
        public async Task<ActionResult<EventsBuildResponse>> PostBuildApplicationResponse([FromBody] EventsBuildRequest request)
        {
            try
            {
                //Se obtiene el String Token de Autenticacion y se transforma a la clase Token
                var contextstr = HttpContext.User.FindFirst("context").Value;

                //Lectura del Token JWT            
                String tokenJwt = HttpContext.Request.Headers[HeaderNames.Authorization];

                LogRequest log = new LogRequest
                {
                    Context = CustomJwtTokenContext.FromJson(contextstr),
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                EventsBuildResponse response = _eventCreate.Generate(request, log, tokenJwt);

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
                return UnprocessableEntity(new EventsPendingResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una lista de Eventos de Factura de un Status determinado
        /// </summary>
        /// <param name="status">Status</param>
        /// <param name="dateFrom">Fecha Inicial</param>
        /// <param name="dateTo">Fecha Final</param>
        /// <param name="eventCode">Codigo de Evento</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/status/{status}/{dateFrom}/{dateTo}/{eventCode}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventsPendingResponse>> GetAnysEvents([FromRoute] int status, string dateFrom, string dateTo, string eventCode)
        {
            try
            {
                LogRequest log = new LogRequest
                {
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                EventStatusDto request = new EventStatusDto
                {
                    Status = status,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    EventCode = eventCode
                };

                EventsPendingResponse response = _eventList.GetList(request, log);

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
                return UnprocessableEntity(new EventsPendingResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza Un Evento de Factura
        /// </summary>
        /// <param name="eventId">Identidicador del Evento</param>
        /// <param name="trackId">Tracking ID del Evento</param>
        /// <param name="request">Datos del Evento</param>
        /// <returns></returns>
        [HttpPut("/api/eventos/pendientes/{eventId}/{trackId}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventUpdatingResponse>> PutEventsPending([FromRoute] string eventId, string trackId, [FromBody] EventUpdatingRequest request)
        {
            try
            {
                LogRequest log = new LogRequest
                {
                    Context = new Models.CustomJwtTokenContext { User = new Models.CustomJwtTokenContext.UserClass { EnterpriseToken = "Task", EnterpriseNit = "xxxx" } },
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                EventDto eventDto = new EventDto { TrackId = trackId, EventId = eventId };

                var response = _eventUpdate.UpdateDianResult(eventDto, request, log);

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
        /// Retorna el estatus del evento
        /// </summary>
        /// <param name="eventId">ID del evento</param>
        /// <param name="trackId">Tracking ID para seguimiento</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/{eventId}/{trackId}")]
        public async Task<ActionResult<FactoringEventResponse>> GetEvent([FromRoute] string eventId, string trackId)
        {
            //Se obtiene el String Token de Autenticacion y se transforma a la clase Token
            var contextstr = HttpContext.User.FindFirst("context").Value;
            var context = CustomJwtTokenContext.FromJson(contextstr);

            try
            {
                LogRequest log = new LogRequest
                {
                    Context = context,
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                EventDto eventDto = new EventDto { TrackId = trackId, EventId = eventId };

                FactoringEventResponse response = _eventStatus.GetStatus(eventDto, log);

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
        /// Retorna la información de un evento determinado.
        /// </summary>
        /// <param name="eventId">Identificador del Evento</param>
        /// <param name="eventUuid">UUID del Evento</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/status/{eventId}/{eventUuid}")]
        [AllowAnonymous]
        public async Task<ActionResult<FactoringEventResponse>> GetStatusByEventUuid([FromRoute] string eventId, string eventUuid)
        {
            try
            {
                LogRequest log = new LogRequest
                {
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    Method = (string)this.ControllerContext.RouteData.Values["action"],
                    Application = LogAzure.ApplicationType.Integracion
                };

                EventUuidDto eventUuidDto = new EventUuidDto { EventId = eventId, EventUuid = eventUuid };

                FactoringEventResponse response = _eventStatus.GetFactoringEvent(eventUuidDto, log);

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
                return UnprocessableEntity(new EventsPendingResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

    }
}