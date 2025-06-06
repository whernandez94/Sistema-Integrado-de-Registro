using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/grados")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GradoController : Controller
    {
        private readonly IGradoService _gradoService;

        public GradoController(IGradoService gradoService)
        {
            _gradoService = gradoService;
        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("obtener-todos")]
        public IActionResult ObtenerTodos()
        {
            var grados = _gradoService.ObtenerTodos();
            return Json(grados);
        }

        [HttpGet("obtener/{id:int}")]
        public IActionResult Obtener(int id)
        {
            var grado = _gradoService.ObtenerPorId(id);
            return Json(grado);
        }

        [HttpPost("guardar")]
        public IActionResult Guardar([FromBody] Grado grado)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            _gradoService.Guardar(grado);
            return Json(new { success = true, message = "Grado guardado correctamente" });
        }

        [HttpDelete("eliminar/{id:int}")]
        public IActionResult Eliminar(int id)
        {
            var result = _gradoService.Eliminar(id);
            return Json(new { success = result, message = result ? "Grado eliminado" : "No encontrado" });
        }
    }

}
