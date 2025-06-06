using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/inasistencias")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InasistenciaController : Controller
    {
        private readonly IInasistenciaService _service;
        private readonly AppDbContext _context;

        public InasistenciaController(IInasistenciaService service, AppDbContext context)
        {
            _service = service;
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

        [HttpGet("obtener-todas/{anioEscolarId:int}/{asignaturaId:int}")]
        public async Task<IActionResult> ObtenerInasistencias(int anioEscolarId, int? asignaturaId)
        {
            var inasistencias = await _service.GetInasistenciasConResumenAsync(anioEscolarId, asignaturaId);
            return Json(inasistencias);
        }

        [HttpGet("obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var inasistencia = await _service.GetInasistenciaAsync(id);
            return inasistencia != null ? Json(inasistencia) : NotFound();
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] GuardarInasistenciaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var nota = new Inasistencia
            {
                EstudianteId = dto.EstudianteId,
                AsignaturaId = dto.AsignaturaId,
                AnioEscolarId = dto.AnioEscolarId,
                Lapso = (int)(dto.Lapso ?? 0), // Verifica también Lapso si es nullable
                Observaciones = dto.Observaciones,
                Porcentaje = dto.Porcentaje,
            };


            var result = await _service.SaveInasistenciaAsync(nota);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteInasistenciaAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("obtener-por-filtros")]
        public async Task<IActionResult> ObtenerFiltros()
        {
            var asignaturas = await _service.GetAsignaturasDisponiblesAsync();
            var anios = await _service.GetAniosEscolaresDisponiblesAsync();
            return Json(new { asignaturas, anios });
        }

        [HttpGet("form/{id:int?}")]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.Lapsos = new List<int> { 1, 2, 3 };
            ViewBag.Asignaturas = await _service.GetAsignaturasDisponiblesAsync();
            ViewBag.AniosEscolares = await _service.GetAniosEscolaresDisponiblesAsync();

            Inasistencia modelo;
            if (id == null || id == 0)
            {
                modelo = new Inasistencia();
            }
            else
            {
                modelo = await _service.GetInasistenciaAsync(id.Value) ?? new Inasistencia();
            }

            return PartialView("_Form", modelo);
        }

        [HttpGet("obtener-estudiantes-por-anio/{anioEscolarId:int?}")]
        public async Task<IActionResult> ObtenerEstudiantes(int anioEscolarId)
        {
            var estudiantes = await _service.GetEstudiantesConInasistenciasAsync(anioEscolarId);
            return Json(estudiantes);
        }
    }
}
