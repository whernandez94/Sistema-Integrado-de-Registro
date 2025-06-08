using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/notas")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NotaController : Controller
    {
        private readonly INotaService _notaService;
        private readonly AppDbContext _context;

        public NotaController(INotaService notaService, AppDbContext context)
        {
            _notaService = notaService;
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index()
        {
            ViewBag.Estudiantes = _context.Estudiantes.ToList();

            ViewBag.Asignaturas = _context.Asignaturas.Where(a => a.Activa).ToList();

            ViewBag.AniosEscolares = _context.AniosEscolares.Where(a => !a.Finalizado).ToList();

            ViewBag.Lapsos = new List<int> { 1, 2, 3 };

            return View();
        }

        [HttpGet("obtener-notas/{anioEscolarId:int}")]
        [HttpGet("obtener-notas/{anioEscolarId:int}/{asignaturaId?}")]
        public async Task<IActionResult> ObtenerNotas(int anioEscolarId, int? asignaturaId)
        {
            var notas = await _notaService.GetNotasConPromediosAsync(anioEscolarId, asignaturaId);
            return Json(notas);
        }

        [HttpGet("obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var nota = await _notaService.GetNotaAsync(id);
            return nota != null ? Json(nota) : NotFound();
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] guardarNotaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           if (!dto.Valor.HasValue)
                return BadRequest("El promedio final es requerido.");

            var nota = new Nota
            {
                EstudianteId = dto.EstudianteId,
                AsignaturaId = dto.AsignaturaId,
                AnioEscolarId = dto.AnioEscolarId,
                Lapso = (int)(dto.Lapso ?? 0), // Verifica también Lapso si es nullable
                Valor = dto.Valor.Value
            };


            var result = await _notaService.SaveNotaAsync(nota);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _notaService.DeleteNotaAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("obtener-filtros")]
        public async Task<IActionResult> ObtenerFiltros()
        {
            var asignaturas = await _notaService.GetAsignaturasDisponiblesAsync();
            var anios = await _notaService.GetAniosEscolaresDisponiblesAsync();

            return Json(new { asignaturas, anios });
        }

        [HttpGet("form/{id:int}")]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.Lapsos = new List<int> { 1, 2, 3 };
            ViewBag.Asignaturas = await _notaService.GetAsignaturasDisponiblesAsync();
            ViewBag.AniosEscolares = await _notaService.GetAniosEscolaresDisponiblesAsync();

            Nota modelo;
            if (id == null || id == 0)
            {
                modelo = new Nota();
            }
            else
            {
                modelo = await _notaService.GetNotaAsync(id.Value) ?? new Nota();
            }

            return PartialView("_Form", modelo);
        }
    }
}
