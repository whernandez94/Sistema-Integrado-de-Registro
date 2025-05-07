using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Integrado_de_Registro.Controllers
{
    public class AnioEscolarController : Controller
    {
        private readonly AppDbContext _context;

        public AnioEscolarController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var anios = await _context.AniosEscolares.ToListAsync();
            return Json(anios);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] AnioEscolar anio)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            // Verificar duplicado (excluyendo el actual en edición)
            var existe = await _context.AniosEscolares
                .AnyAsync(a => a.Anio == anio.Anio && a.Id != anio.Id);

            if (existe)
                return BadRequest("Ese año escolar ya existe.");

            if (anio.Id == 0)
            {
                _context.AniosEscolares.Add(anio);
            }
            else
            {
                _context.AniosEscolares.Update(anio);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var anio = await _context.AniosEscolares.FindAsync(id);
            if (anio == null)
                return NotFound();
            return Json(anio);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var anio = await _context.AniosEscolares.FindAsync(id);
            if (anio == null)
                return NotFound();

            _context.AniosEscolares.Remove(anio);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Finalizar(int id)
        {
            var anio = await _context.AniosEscolares.FindAsync(id);
            if (anio == null)
                return NotFound();

            anio.Finalizado = true;
            _context.AniosEscolares.Update(anio);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            AnioEscolar modelo;

            if (id == null || id == 0)
            {
                modelo = new AnioEscolar();
            }
            else
            {
                modelo = await _context.AniosEscolares.FindAsync(id);
                if (modelo == null)
                    return NotFound();
            }

            return PartialView("_Form", modelo);
        }

    }
}
