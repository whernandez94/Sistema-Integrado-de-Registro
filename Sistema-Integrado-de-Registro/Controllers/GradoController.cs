using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    public class GradoController : Controller
    {
        private readonly IGradoService _gradoService;

        public GradoController(IGradoService gradoService)
        {
            _gradoService = gradoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var grados = _gradoService.ObtenerTodos();
            return Json(grados);
        }

        [HttpGet]
        public IActionResult Obtener(int id)
        {
            var grado = _gradoService.ObtenerPorId(id);
            return Json(grado);
        }

        [HttpPost]
        public IActionResult Guardar([FromBody] Grado grado)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            _gradoService.Guardar(grado);
            return Json(new { success = true, message = "Grado guardado correctamente" });
        }

        [HttpDelete]
        public IActionResult Eliminar(int id)
        {
            var result = _gradoService.Eliminar(id);
            return Json(new { success = result, message = result ? "Grado eliminado" : "No encontrado" });
        }
    }

}
