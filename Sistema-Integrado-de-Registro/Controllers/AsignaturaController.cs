using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    public class AsignaturaController : Controller
    {
        private readonly IAsignaturaService _service;

        public AsignaturaController(IAsignaturaService service)
        {
            _service = service;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var asignaturas = await _service.GetAllAsignaturasAsync();
            return Json(asignaturas);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var asignatura = await _service.GetAsignaturaByIdAsync(id);
            return asignatura != null
                ? Json(asignatura)
                : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] AsignaturaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveAsignaturaAsync(dto);

            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteAsignaturaAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var result = await _service.ToggleAsignaturaStatusAsync(id);
            return result.Success
                ? Ok(new { result.Message, result.Data })
                : BadRequest(result.Message);
        }
    }
}
