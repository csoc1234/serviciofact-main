using Contributors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contributors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusTaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatusTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Metodo para cambiar el estatus del proceso de tareas
        /// </summary>
        /// <param name="id"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        [HttpPut("/api/Issuer/{id}/changesTask")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse>> ChangeStatus(int id, [FromBody] StatusTask process)
        {
            bool result = _context.ChangeStatusIssuers(id, process);

            BaseResponse response = result ? new BaseResponse { Code = 200, Message = "Estatus Actualizado" } : new BaseResponse { Code = 422, Message = "Error actualizando el estatus" };

            return response;
        }

        ///<summary>Actualiza el status y ambiente de un contribuyente durante la habilitación.</summary>
        ///<param name="nit">NIT del Contribuyente.</param>
        ///<param name="status">Status del proceso de habilitación.</param>
        ///<param name="environment">Ambiente donde se encuentra ese contribuyente.</param>
        ///<returns></returns>
        [HttpPut("/api/Taxpayer/status/{nit}/{status}/{environment}")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse>> PutTaxpayerStatus(string nit, int status, int environment)
        {
            BaseResponse response = new BaseResponse();
            int result = _context.UpdateTaxpayerStatusFactoring(nit, status, environment);
            if (result < 0)
            {
                response.Code = 500;
                response.Message = "Error actualizando el contrbuyente con NIT " + nit;
                return UnprocessableEntity(response);
            }
            else if (result == 0)
            {
                response.Code = 204;
                response.Message = "No se encontró ningún contribuyente con NIT = " + nit;
                return Ok(response);
            }
            else
            {
                result = _context.UpdateTaxpayerStatusHab(nit, status, environment);
                if (result < 0)
                {
                    response.Code = 500;
                    response.Message = "Error actualizando el contrbuyente con NIT " + nit;
                    return UnprocessableEntity(response);
                }
                else if (result == 0)
                {
                    response.Code = 204;
                    response.Message = "No se encontró ningún contribuyente con NIT = " + nit;
                    return Ok(response);
                }
                else
                {
                    response.Code = 200;
                    response.Message = "Contribuyente actualizado exitosamente";
                    return Ok(response);
                }
            }
        }

        ///<summary>Obtiene la información de un contribuyente en su proceso de habilitación.</summary>
        ///<param name="nit">NIT del Contribuyente.</param>
        ///<returns></returns>
        [HttpGet("/api/Taxpayer/Hab/status/{nit}")]
        public async Task<ActionResult<EnterpriseFactoringHab>> GetTaxpayerInfo(string nit)
        {
            try
            {
                var enterpriseHab = _context.Enterprise_Factoring_Hab.Where(x => x.company_id == nit).FirstOrDefault();
                return Ok(enterpriseHab);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }

    }

}
