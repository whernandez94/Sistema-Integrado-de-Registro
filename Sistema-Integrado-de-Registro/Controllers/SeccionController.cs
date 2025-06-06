using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/secciones")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SeccionController : Controller
    {
        private readonly ISeccionService _service;
        private readonly AppDbContext _context;

        public SeccionController(ISeccionService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index()
        {
            ViewBag.Grados = _context.Grados.ToList();

            ViewBag.Docentes = _context.Docentes
                .Include(s => s.DocenteAsignaturas)
                .ToList();

            ViewBag.AniosEscolares = _context.AniosEscolares.Where(a => !a.Finalizado).ToList();

            return View();
        }

        [HttpGet("obtener-todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var secciones = await _service.GetAllSeccionesAsync();
            return Json(secciones);
        }

        [HttpGet("obtener-detalle/{id:int}")]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var seccion = await _service.GetSeccionWithEstudiantesAsync(id);
                return Json(seccion);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("imprimir-listado/{id:int}")]
        public async Task<IActionResult> ImprimirListado(int id)
        {
            try
            {
                var seccion = await _service.GetSeccionWithEstudiantesAsync(id);
                return View("_ListadoImpresion", seccion);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] SeccionGuardarDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var seccion = new Seccion
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Grado = dto.Grado,
                AnioEscolarId = dto.AnioEscolarId,
                DocenteId = dto.DocenteId
            };

            var result = await _service.SaveSeccionAsync(seccion);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteSeccionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("form/{id:int}")]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.AniosEscolares = await _service.GetAniosEscolaresDisponiblesAsync();
            ViewBag.Docentes = await _service.GetDocentesDisponiblesAsync();
            ViewBag.Grados = Enumerable.Range(1, 6).ToList();

            Seccion modelo;
            if (id == null || id == 0)
            {
                modelo = new Seccion();
            }
            else
            {
                var secciones = await _service.GetAllSeccionesAsync();
                modelo = await _service.GetSeccionByIdAsync(id.Value) ?? new Seccion();

            }

            return PartialView("_Form", modelo);
        }
    }
}
