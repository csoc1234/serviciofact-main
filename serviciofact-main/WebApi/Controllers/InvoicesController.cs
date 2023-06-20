using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TFHKA.LogsMongo;
using WebApi.Application.Dto;
using WebApi.Application.Interface;
using WebApi.Models.Response;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IGestionInvoice _gestionInvoice;
        private static ILogMongo log;

        public InvoicesController(IConfiguration configuration, IGestionInvoice gestionInvoice, ILogMongo LogMongo)
        {
            _configuration = configuration;
            _gestionInvoice = gestionInvoice;
            log = LogMongo;
        }

        /// <summary>
        /// Indica si una Factura es apta para negociar como Titulo Valor
        /// </summary>
        /// <param name="id">Id de la Factura en Emision 2.1</param>
        /// <returns></returns>
        [HttpGet("/api/invoices/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto>> GetInvoice([FromRoute] int id)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            LogRequest logRequest = new LogRequest
            {
                Api = ControllerContext.HttpContext.Request.Path.Value,
                NameMethod = this.ControllerContext.RouteData.Values["action"].ToString(),
                Application = ApplicationType.Integracion
            };

            log.Setup(logRequest);

            try
            {
                if (id <= 0)
                {
                    return BadRequest("Argumento invalido");
                }

                ResponseDto response = _gestionInvoice.GetInfoInvoice(new InvoiceDto { IdTable = id }, log);

                if (response.Code == 200)
                {
                    log.SaveLog(response.Code, "Request procesado con Exito", ref timeT, LevelMsn.Info);

                    return Ok(response);
                }
                else
                {
                    log.SaveLog(response.Code, "Request procesado con Observaciones", ref timeT, LevelMsn.Warning);


                    return UnprocessableEntity(response);
                }

            }
            catch (Exception ex)
            {
                log.SaveLog(500, ex.Message, ref timeT, LevelMsn.Error);

                return UnprocessableEntity(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de Facturas de un Emisor determinado en un rango de fechas
        /// </summary>
        /// <param name="idEnterprise">Identidficador del Emisor</param>
        /// <param name="dateFrom">Fecha Inicial</param>
        /// <param name="dateTo">Fecha Final</param>
        /// <returns></returns>
        [HttpGet("/api/invoices/{idEnterprise}/{dateFrom}/{dateTo}")]
        [AllowAnonymous]
        public async Task<ActionResult<InvoicesPerTaxpayerResponse>> GetInvoicesPerTaxpayer([FromRoute] int idEnterprise, string dateFrom, string dateTo)
        {

            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            LogRequest logRequest = new LogRequest
            {
                Api = ControllerContext.HttpContext.Request.Path.Value,
                NameMethod = this.ControllerContext.RouteData.Values["action"].ToString(),
                Application = ApplicationType.Integracion
            };

            log.Setup(logRequest);

            InvoicesPerTaxpayerResponse response = new InvoicesPerTaxpayerResponse();
            try
            {
                response = _gestionInvoice.GetInvoicesPerTaxpayerList(
                    new Application.Dto.InvoicesPerTaxpayerDto
                    {
                        IdEnterprise = idEnterprise,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    },
                    log);

                if (response.Code == 200)
                {
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);
                    return Ok(response);
                }
                else
                {
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                    return UnprocessableEntity(response);
                }
            }
            catch (Exception ex)
            {
                log.SaveLog(500, ex.Message, ref timeT, LevelMsn.Error);
                return UnprocessableEntity(new InvoicesPerTaxpayerResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una lista de Facturas de Habilitacion de un Emisor determinado
        /// </summary>
        /// <param name="nit">NIT del Emisor</param>
        /// <returns></returns>
        [HttpGet("/api/invoices/hab/{nit}")]
        [AllowAnonymous]
        public async Task<ActionResult<InvoicesPerTaxpayerResponse>> GetInvoicesHab([FromRoute] string nit)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            LogRequest logRequest = new LogRequest
            {
                Api = ControllerContext.HttpContext.Request.Path.Value,
                NameMethod = this.ControllerContext.RouteData.Values["action"].ToString(),
                Application = ApplicationType.Integracion
            };

            log.Setup(logRequest);

            InvoicesPerTaxpayerResponse response = new InvoicesPerTaxpayerResponse();

            try
            {
                response = _gestionInvoice.GetInvoicesInHabilitation(nit, log);

                if (response.Code == 200)
                {
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Info);
                    return Ok(response);
                }
                else
                {
                    log.SaveLog(response.Code, response.Message, ref timeT, LevelMsn.Warning);
                    return UnprocessableEntity(response);
                }
            }
            catch (Exception ex)
            {
                log.SaveLog(500, ex.Message, ref timeT, LevelMsn.Error);
                return UnprocessableEntity(new InvoicesPerTaxpayerResponse()
                {
                    Code = 500,
                    Message = ex.Message
                });
            }

        }

        /// <summary>
        /// Registra una Factora Negociable en Factoring
        /// </summary>
        /// <param name="invoiceDto">Datos de la Factura</param>
        /// <returns></returns>
        [HttpPost("/api/invoices")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto>> PostInvoice([FromBody] InvoiceDto invoiceDto)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            LogRequest logRequest = new LogRequest
            {
                Api = ControllerContext.HttpContext.Request.Path.Value,
                NameMethod = this.ControllerContext.RouteData.Values["action"].ToString(),
                Application = ApplicationType.Integracion
            };

            log.Setup(logRequest);

            try
            {
                if (invoiceDto == null)
                {
                    return BadRequest("Argumento invalido");
                }

                ResponseDto response = _gestionInvoice.AddInfoInvoice(invoiceDto, log);

                if (response.Code == 200)
                {
                    log.SaveLog(response.Code, "Request procesado con Exito", ref timeT, LevelMsn.Info);

                    return Ok(response);
                }
                else
                {
                    log.SaveLog(response.Code, "Request procesado con Observaciones", ref timeT, LevelMsn.Warning);

                    return UnprocessableEntity(response);
                }

            }
            catch (Exception ex)
            {
                log.SaveLog(500, ex.Message, ref timeT, LevelMsn.Error);

                return UnprocessableEntity(ex.Message);
            }
        }
    }
}