using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.DTO;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/matricula")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MatriculaController : Controller
    {
        private readonly IMatriculaService _matriculaService;
        private readonly AppDbContext _context;

        public MatriculaController(IMatriculaService matriculaService, AppDbContext context)
        {
            _matriculaService = matriculaService;
            _context = context;

        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index()
        {
            ViewBag.Estudiantes = _context.Estudiantes
         .Select(e => new { e.Id, e.Nombre, e.Apellido, e.Cedula })
         .ToList();

            ViewBag.Secciones = _context.Secciones
                .Include(s => s.AnioEscolar)
                .ToList();

            ViewBag.AniosEscolares = _context.AniosEscolares.ToList();

            return View();
        }

        [HttpGet("obtener-todas")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var matriculas = await _matriculaService.GetAllMatriculasAsync();
            return Json(matriculas);
        }

        [HttpGet("obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var matricula = await _matriculaService.GetMatriculaByIdAsync(id);
            if (matricula == null)
                return NotFound();

            return Json(matricula);
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] MatriculaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matricula = new Matricula
            {
                Id = dto.Id,
                EstudianteId = dto.EstudianteId,
                SeccionId = dto.SeccionId,
                AnioEscolarId = dto.AnioEscolarId,
                FechaMatricula = dto.FechaMatricula,
                NumeroExpediente = dto.NumeroExpediente,
                Observaciones = dto.Observaciones
            };

            var resultado = await _matriculaService.SaveMatriculaAsync(matricula);

            return resultado.Success
                ? Ok(new { resultado.Message })
                : BadRequest(resultado.Message);
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _matriculaService.DeleteMatriculaAsync(id);
            if (!resultado.Success)
                return BadRequest(resultado.Message);

            return Ok(new { resultado.Message });
        }

        [HttpGet("estudiantes-disponibles")]
        public async Task<IActionResult> EstudiantesDisponibles()
        {
            var estudiantes = await _matriculaService.GetEstudiantesDisponiblesAsync();
            return Json(estudiantes);
        }

        [HttpGet("secciones-disponibles")]
        public async Task<IActionResult> SeccionesDisponibles()
        {
            var secciones = await _matriculaService.GetSeccionesDisponiblesAsync();
            return Json(secciones);
        }

        [HttpGet("anios-disponibles")]
        public async Task<IActionResult> AniosEscolaresDisponibles()
        {
            var anios = await _matriculaService.GetAniosEscolaresDisponiblesAsync();
            return Json(anios);
        }

        [HttpGet("generar-numero-expediente")]
        public async Task<IActionResult> GenerarNumeroExpediente()
        {
            var numero = await _matriculaService.GenerarNumeroExpedienteAsync();
            return Ok(numero);
        }
    }
}
