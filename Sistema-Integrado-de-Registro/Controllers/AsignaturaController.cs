using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    [Route("gestion-escolar/asignaturas")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AsignaturaController : Controller
    {
        private readonly IAsignaturaService _service;

        public AsignaturaController(IAsignaturaService service)
        {
            _service = service;
        }

        [HttpGet("")]
        [HttpGet("listado")]
        public IActionResult Index() => View();

        [HttpGet("obtener-todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var asignaturas = await _service.GetAllAsignaturasAsync();
            return Json(asignaturas);
        }

        [HttpGet("obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var asignatura = await _service.GetAsignaturaByIdAsync(id);
            return asignatura != null
                ? Json(asignatura)
                : NotFound();
        }

        [HttpPost("guardar")]
        public async Task<IActionResult> Guardar([FromBody] AsignaturaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveAsignaturaAsync(dto);

            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteAsignaturaAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpPost("cambiar-estado/{id:int}")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var result = await _service.ToggleAsignaturaStatusAsync(id);
            return result.Success
                ? Ok(new { result.Message, result.Data })
                : BadRequest(result.Message);
        }
    }
}
