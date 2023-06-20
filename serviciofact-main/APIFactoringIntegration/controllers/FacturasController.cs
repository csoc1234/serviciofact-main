using APIFactoringIntegration.Application.Dto;
using APIFactoringIntegration.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIFactoringIntegration.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly IDocument _document;

        public FacturasController(IDocument document)
        {
            _document = document;
        }

        [HttpPost("/api/Facturas")]
        [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DataDto>> PostFacturas([FromBody] DocumentRequestDto request)
        {
            try
            {
                ResponseDto response = await _document.ValidDocument(request);

                if (response.Codigo == 200)
                {
                    DataDto dto = new DataDto
                    {
                        Data = response.Facturas
                    };

                    return Ok(dto);
                }
                else
                {
                    return StatusCode(response.Codigo, null);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, null);
            }
        }
    }
}
