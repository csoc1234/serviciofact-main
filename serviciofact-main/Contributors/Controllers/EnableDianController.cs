using Contributors.Application.Dto;
using Contributors.Application.Interface;
using Contributors.Infraestructure.Logging;
using Contributors.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Contributors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnableDianController : ControllerBase
    {
        private readonly IEnableDianCreate _enableDianCreate;
        private readonly ITaxpayerListStatus _taxpayerListStatus;

        public EnableDianController(
            IEnableDianCreate enableDianCreate,
            ITaxpayerListStatus taxpayerListStatus
            )
        {
            _enableDianCreate = enableDianCreate;
            _taxpayerListStatus = taxpayerListStatus;
        }

        [HttpPost("/api/Issuers/IniciarHabilitacion")]
        public async Task<ActionResult> StartEnableDian([FromBody] TaxPayersDto request)
        {
            LogRequest log = new LogRequest
            {
                Context = new CustomJwtTokenContext { User = new CustomJwtTokenContext.UserClass { EnterpriseToken = "Process", EnterpriseNit = request.CompanyId } },
                Api = ControllerContext.HttpContext.Request.Path.Value,
                Method = (string)this.ControllerContext.RouteData.Values["action"],
                Application = LogAzure.ApplicationType.Integracion
            };

            var response = _enableDianCreate.Register(request, log);

            if (response.Code == 200)
            {
                return Ok(response);
            }
            else
            {
                return UnprocessableEntity(response);
            }
        }

        [HttpGet("/api/Issuers/Habilitacion/{status}")]
        public async Task<ActionResult> GetList([FromRoute] int status)
        {
            LogRequest log = new LogRequest
            {
                Context = new CustomJwtTokenContext { User = new CustomJwtTokenContext.UserClass { EnterpriseToken = "Process", EnterpriseNit = "xxxx" } },
                Api = ControllerContext.HttpContext.Request.Path.Value,
                Method = (string)this.ControllerContext.RouteData.Values["action"],
                Application = LogAzure.ApplicationType.Integracion
            };

            var response = _taxpayerListStatus.GetList(status, log);

            if (response.Code == 200)
            {
                return Ok(response);
            }
            else
            {
                return UnprocessableEntity(response);
            }
        }
    }
}
