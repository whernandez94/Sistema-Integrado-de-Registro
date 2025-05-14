using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Route("[controller]/[action]")]
    public class InasistenciaController : Controller
    {
        private readonly IInasistenciaService _service;

        public InasistenciaController(IInasistenciaService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerInasistencias(int anioEscolarId, int? asignaturaId)
        {
            var inasistencias = await _service.GetInasistenciasConResumenAsync(anioEscolarId, asignaturaId);
            return Json(inasistencias);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var inasistencia = await _service.GetInasistenciaAsync(id);
            return inasistencia != null ? Json(inasistencia) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Inasistencia model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveInasistenciaAsync(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteInasistenciaAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFiltros()
        {
            var asignaturas = await _service.GetAsignaturasDisponiblesAsync();
            var anios = await _service.GetAniosEscolaresDisponiblesAsync();
            return Json(new { asignaturas, anios });
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> ObtenerEstudiantes(int anioEscolarId)
        {
            var estudiantes = await _service.GetEstudiantesConInasistenciasAsync(anioEscolarId);
            return Json(estudiantes);
        }
    }
}
