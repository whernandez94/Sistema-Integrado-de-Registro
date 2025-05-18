using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Route("[controller]/[action]")]
    public class MatriculaController : Controller
    {
        private readonly IMatriculaService _matriculaService;
        private readonly AppDbContext _context;

        public MatriculaController(IMatriculaService matriculaService, AppDbContext context)
        {
            _matriculaService = matriculaService;
            _context = context;

        }

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

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var matriculas = await _matriculaService.GetAllMatriculasAsync();
            return Json(matriculas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var matricula = await _matriculaService.GetMatriculaByIdAsync(id);
            if (matricula == null)
                return NotFound();

            return Json(matricula);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Matricula matricula)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _matriculaService.SaveMatriculaAsync(matricula);
            if (!resultado.Success)
                return BadRequest(resultado.Message);

            return Ok(new { resultado.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _matriculaService.DeleteMatriculaAsync(id);
            if (!resultado.Success)
                return BadRequest(resultado.Message);

            return Ok(new { resultado.Message });
        }

        [HttpGet]
        public async Task<IActionResult> EstudiantesDisponibles()
        {
            var estudiantes = await _matriculaService.GetEstudiantesDisponiblesAsync();
            return Json(estudiantes);
        }

        [HttpGet]
        public async Task<IActionResult> SeccionesDisponibles()
        {
            var secciones = await _matriculaService.GetSeccionesDisponiblesAsync();
            return Json(secciones);
        }

        [HttpGet]
        public async Task<IActionResult> AniosEscolaresDisponibles()
        {
            var anios = await _matriculaService.GetAniosEscolaresDisponiblesAsync();
            return Json(anios);
        }

        [HttpGet]
        public async Task<IActionResult> GenerarNumeroExpediente()
        {
            var numero = await _matriculaService.GenerarNumeroExpedienteAsync();
            return Ok(numero);
        }
    }
}
