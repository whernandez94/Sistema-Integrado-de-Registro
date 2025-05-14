using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Route("[controller]/[action]")]
    public class SeccionController : Controller
    {
        private readonly ISeccionService _service;

        public SeccionController(ISeccionService service)
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
            var secciones = await _service.GetAllSeccionesAsync();
            return Json(secciones);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var seccion = await _service.GetSeccionWithEstudiantesAsync(id);
                return Json(seccion);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ImprimirListado(int id)
        {
            try
            {
                var seccion = await _service.GetSeccionWithEstudiantesAsync(id);
                return View("_ListadoImpresion", seccion);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Seccion seccion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.SaveSeccionAsync(seccion);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _service.DeleteSeccionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.AniosEscolares = await _service.GetAniosEscolaresDisponiblesAsync();
            ViewBag.Docentes = await _service.GetDocentesDisponiblesAsync();
            ViewBag.Grados = Enumerable.Range(1, 6).ToList();

            Seccion modelo;
            if (id == null || id == 0)
            {
                modelo = new Seccion();
            }
            else
            {
                var secciones = await _service.GetAllSeccionesAsync();
                modelo = await _service.GetSeccionByIdAsync(id.Value) ?? new Seccion();

            }

            return PartialView("_Form", modelo);
        }
    }
}
