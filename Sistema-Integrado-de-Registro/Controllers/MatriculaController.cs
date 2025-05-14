using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Route("[controller]/[action]")]
    public class MatriculaController : Controller
    {
        private readonly IMatriculaService _service;

        public MatriculaController(IMatriculaService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var matriculas = await _service.GetAllMatriculasAsync();
            return Json(matriculas);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var matricula = await _service.GetMatriculaByIdAsync(id);
            return matricula != null ? Json(matricula) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Matricula model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveMatriculaAsync(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteMatriculaAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.Estudiantes = await _service.GetEstudiantesDisponiblesAsync();
            ViewBag.Secciones = await _service.GetSeccionesDisponiblesAsync();
            ViewBag.AniosEscolares = await _service.GetAniosEscolaresDisponiblesAsync();

            Matricula modelo;
            if (id == null || id == 0)
            {
                modelo = new Matricula
                {
                    FechaMatricula = DateTime.Today,
                    NumeroExpediente = await _service.GenerarNumeroExpedienteAsync()
                };
            }
            else
            {
                modelo = await _service.GetMatriculaByIdAsync(id.Value) ?? new Matricula();
            }

            return PartialView("_Form", modelo);
        }
    }
}
