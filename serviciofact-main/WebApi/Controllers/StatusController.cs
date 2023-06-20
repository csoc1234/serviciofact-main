using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TFHKA.LogsMongo;
using WebApi.Application.Dto;
using WebApi.Application.Interface;
using WebApi.Domain.Entity;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IInvoiceEventsStatus _statusEvent;

        public StatusController(IInvoiceEventsStatus statusEvent)
        {
            _statusEvent = statusEvent;
        }

        /// <summary>
        /// Resumen del estado de una factura en la DIAN
        /// </summary>
        /// <param name="supplierIdentification"></param>
        /// <param name="cufe"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet("/api/invoice/statusdian/{cufe}/{supplierIdentification}/{documentId}")]
        [AllowAnonymous]
        public async Task<ActionResult<InvoiceStatusDianDto>> LastInvoiceEventStatus([FromRoute] string cufe, string supplierIdentification, string documentId)
        {
            try
            {
                LogRequest log = new LogRequest
                {
                    Api = ControllerContext.HttpContext.Request.Path.Value,
                    NameMethod = this.ControllerContext.RouteData.Values["action"].ToString(),
                    Application = ApplicationType.Integracion,
                    DocumentId = documentId,
                    Identification = supplierIdentification,
                    PartitionKey = cufe
                };
                InvoiceStatusDianDto response = _statusEvent.GetLastStatus(supplierIdentification, documentId, cufe, log);

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
                return UnprocessableEntity(new InvoiceEventsStatusDian()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }
    }
}
