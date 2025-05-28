using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    public class EstudianteController : Controller
    {
        private readonly IEstudianteService _service;

        public EstudianteController(IEstudianteService service)
        {
            _service = service;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var estudiantes = await _service.GetAllEstudiantesAsync();
            return Json(estudiantes);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var estudiante = await _service.GetEstudianteByIdAsync(id);
            return estudiante != null
                ? Json(estudiante)
                : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Estudiante estudiante)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveEstudianteAsync(estudiante);

            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteEstudianteAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }
    }

}
