using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Authorize]
    public class InstitutionController : Controller
    {
        private readonly IInstitutionService _service;

        public InstitutionController(IInstitutionService service)
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
            var instituciones = await _service.GetAllInstitutionsAsync();
            return Json(instituciones);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Institution model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveInstitutionAsync(model);

            return result.Success
                ? Ok(new { result.Message, Data = result.Data })
                : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var institucion = await _service.GetInstitutionByIdAsync(id);
            return institucion != null
                ? Json(institucion)
                : NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteInstitutionAsync(id);
            return result.Success
                ? Ok(new { result.Message })
                : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            Institution modelo;

            if (id == null || id == 0)
            {
                modelo = new Institution();
            }
            else
            {
                modelo = await _service.GetInstitutionByIdAsync(id.Value);
                if (modelo == null)
                    return NotFound();
            }

            return PartialView("_Form", modelo);
        }
    }
}
