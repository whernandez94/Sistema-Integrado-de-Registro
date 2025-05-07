using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Controllers
{
    public class InstitutionController : Controller
    {
        private readonly AppDbContext _context;

        public InstitutionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var instituciones = await _context.Instituciones.ToListAsync();
            return Json(instituciones);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Institution model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Id == 0)
                _context.Instituciones.Add(model);
            else
                _context.Instituciones.Update(model);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Guardado exitosamente" });
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var institucion = await _context.Instituciones.FindAsync(id);
            if (institucion == null)
                return NotFound();

            return Json(institucion);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var institucion = await _context.Instituciones.FindAsync(id);
            if (institucion == null)
                return NotFound();

            _context.Instituciones.Remove(institucion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Eliminado correctamente" });
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
                modelo = await _context.Instituciones.FindAsync(id);
                if (modelo == null)
                    return NotFound();
            }

            return PartialView("_Form", modelo);
        }
    }
}
