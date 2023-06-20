using APIValidateEvents.Application.Dto;
using APIValidateEvents.Application.Interface;
using Microsoft.AspNetCore.Mvc;
namespace APIValidateEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceValidationController : ControllerBase
    {
        private readonly IInvoiceCheck _invoiceCheck;

        public InvoiceValidationController(IInvoiceCheck invoiceCheck)
        {
            _invoiceCheck = invoiceCheck;
        }

        /// <summary>
        /// Busca en la DIAN los eventos de una factura y determira si es valido o no para ser candidata para negociacion en factoring
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/validationEvents")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto>> PostValidationEvents([FromBody] InvoiceDto request)
        {
            try
            {

                ResponseDto response = await _invoiceCheck.ValidateAsync(request);

                if (response.Code == 200)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(response.Code, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Code = 500, Message = ex.Message });
            }
        }

    }
}
