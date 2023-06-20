using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Contributors.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Contributors.Application.Interface;
using Contributors.Infraestructure.Logging;
using Contributors.Application.Dto;
using Contributors.Models.Response;

namespace Contributors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IssuersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna una lista de todos los Contribuyentes Emisores afiliados a Factoring
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<EnterpriseTable>> Get()
        {
            try
            {
                return Ok(_context.Enterprise_Factoring.ToList());

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un Contribuyente Emisor afiliados a Factoring por su Nit
        /// </summary>
        /// <param name="nit">Nit del Contribuyente Emisor</param>
        /// <returns></returns>
        [HttpGet("{nit}")]
        public ActionResult<EnterpriseTable> Get(string nit)
        {
            try
            {
                var enterprise = _context.Enterprise_Factoring.Where(e => e.company_id == nit).FirstOrDefault();

                return Ok(enterprise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }

        /// <summary>
        /// Establece un Nuevo Contribuyente Emisor
        /// </summary>
        /// <param name="enterprise">>Datos del Nuevo Contribuyente Emisor</param>
        [HttpPost]
        public ActionResult Post([FromBody] EnterpriseTable enterprise)
        {
            try
            {
                //Validar atributos requeridos del Contribuyente
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (enterprise == null)
                    return BadRequest("Argumento invalido");

                enterprise.created_at = DateTime.Now;
                enterprise.updated_at = DateTime.Now;

                _context.Add(enterprise);
                _context.SaveChanges();

                return CreatedAtAction("Get", new { id = enterprise.id }, enterprise);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un Contribuyente Emisor
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <param name="enterprise">Datos del Contribuyente Emisor</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] EnterpriseTable enterprise)
        {
            try
            {
                //Validar atributos requeridos del Contribuyente
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (enterprise == null)
                    return BadRequest("Argumento invalido");

                if (id != enterprise.id)
                {
                    return BadRequest("Identificador no concuerda con el del objeto");
                }

                enterprise.updated_at = DateTime.Now;

                _context.Entry(enterprise).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok("Actualizacion exitosa");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina un Contribuyente Emisor por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var enterprise = _context.Enterprise_Factoring.Find(id);

                if (enterprise == null)
                {
                    return NotFound("No existe el contribuyente indicado");
                }

                _context.Enterprise_Factoring.Remove(enterprise);

                _context.SaveChanges();

                return Ok("Contribuyente eliminado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se ha producido un error. " + ex.Message);
            }
        }        
    }
}