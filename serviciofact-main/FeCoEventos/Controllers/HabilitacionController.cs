using FeCoEventos.Application.Dto;
using FeCoEventos.Application.Interface;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FeCoEventos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabilitacionController : ControllerBase
    {
        private readonly IEnableDianSummary _dianSummary;
        private readonly IEventUpdate _eventUpdate;

        public HabilitacionController(IEnableDianSummary dianSummary, IEventUpdate eventUpdate)
        {
            _dianSummary = dianSummary;
            _eventUpdate = eventUpdate;
        }

        /// <summary>
        /// Permite actualizar el estatus en la entrega async
        /// </summary>
        /// <param name="eventId">Numero de Evento</param>
        /// <param name="trackId">Tracking ID para seguimiento</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("/api/eventos/EntregaAsync/{eventId}/{trackId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase>> PutDeliveryAsyncSet([FromRoute] string eventId, string trackId, [FromBody] EventDeliveryAsyncDto request)
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

                var response = _eventUpdate.DeliveryAsyncDian(eventDto, request, log);

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
        /// Resumen de los eventos enviados en habilitacion de un contribyente
        /// </summary>
        /// <param name="nit">Numero de Identificacion del Contribuyente</param>
        /// <returns></returns>
        [HttpGet("/api/eventos/resumenHabilitacion/{nit}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventsSummaryResponse>> GetListEvents([FromRoute] string nit)
        {
            LogRequest log = new LogRequest
            {
                Api = ControllerContext.HttpContext.Request.Path.Value,
                Method = (string)this.ControllerContext.RouteData.Values["action"],
                Application = LogAzure.ApplicationType.Integracion
            };

            EventsSummaryResponse response = _dianSummary.GetList(nit, log);

            if (response.Code == 500)
            {
                return UnprocessableEntity(response);
            }
            else
            {
                return Ok(response);
            }
        }
    }
}
