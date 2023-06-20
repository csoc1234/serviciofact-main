using APIGetValidDocs.Application.Dto;
using APIGetValidDocs.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIGetValidDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IValidDocs _validDocs;

        public DocumentsController(IValidDocs pValidDocs)
        {
            _validDocs = pValidDocs;
        }

        /// <summary>
        /// Retorna la lista de facturas candidatas para negociacion en factoring
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/documents/listvalid")]
        public async Task<ActionResult<List<ValidDocsResponseDto>>> GetList([FromBody] ValidDocsRequestDto request)
        {
            try
            {
                ResponseDto response = await _validDocs.GetListValidDocs(request);

                if (response.Code == 200)
                {
                    return Ok(response.List);
                }
                else
                {
                    return StatusCode(response.Code, new ResponseBase
                    {
                        Code = response.Code,
                        Message = response.Message
                    });
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase
                {
                    Code = 500,
                    Message = "Se ha producido un error en la peticion"
                });
            }
        }
    }
}
