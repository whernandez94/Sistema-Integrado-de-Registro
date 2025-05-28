using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    public class AnioEscolarController : Controller
    {
        private readonly IAnioEscolarService _service;

        public AnioEscolarController(IAnioEscolarService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var anios = await _service.GetAllAniosEscolaresAsync();
            return Json(anios);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] AnioEscolar anioEscolar)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveAnioEscolarAsync(anioEscolar);

            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var anio = await _service.GetAnioEscolarByIdAsync(id);
            return anio != null
                ? Json(anio)
                : NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteAnioEscolarAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Finalizar(int id)
        {
            var result = await _service.FinalizarAnioEscolarAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            try
            {
                var modelo = await _service.GetAnioEscolarForFormAsync(id);
                return PartialView("_Form", modelo);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
