using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/anios")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AnioEscolarController : Controller
    {
        private readonly IAnioEscolarService _service;

        public AnioEscolarController(IAnioEscolarService service)
        {
            _service = service;
        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("obtener-todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var anios = await _service.GetAllAniosEscolaresAsync();
            return Json(anios);
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] AnioEscolar anioEscolar)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveAnioEscolarAsync(anioEscolar);
            return Json(result);
        }

        [HttpGet("obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var anio = await _service.GetAnioEscolarByIdAsync(id);
            return anio != null ? Json(anio) : NotFound();
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteAnioEscolarAsync(id);
            return Json(result);
        }

        [HttpPost("finalizar/{id:int}")]
        public async Task<IActionResult> Finalizar(int id)
        {
            var result = await _service.FinalizarAnioEscolarAsync(id);
            return Json(result);
        }

        [HttpGet("form/{id:int?}")]
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
