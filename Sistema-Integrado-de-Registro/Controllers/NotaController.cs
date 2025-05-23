﻿using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Models;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro.Controllers
{
    [Route("[controller]/[action]")]
    public class NotaController : Controller
    {
        private readonly INotaService _notaService;

        public NotaController(INotaService notaService)
        {
            _notaService = notaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerNotas(int anioEscolarId, int? asignaturaId)
        {
            var notas = await _notaService.GetNotasConPromediosAsync(anioEscolarId, asignaturaId);
            return Json(notas);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int id)
        {
            var nota = await _notaService.GetNotaAsync(id);
            return nota != null ? Json(nota) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] Nota nota)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _notaService.SaveNotaAsync(nota);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var result = await _notaService.DeleteNotaAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFiltros()
        {
            var asignaturas = await _notaService.GetAsignaturasDisponiblesAsync();
            var anios = await _notaService.GetAniosEscolaresDisponiblesAsync();

            return Json(new { asignaturas, anios });
        }

        [HttpGet]
        public async Task<IActionResult> Form(int? id)
        {
            ViewBag.Lapsos = new List<int> { 1, 2, 3 };
            ViewBag.Asignaturas = await _notaService.GetAsignaturasDisponiblesAsync();
            ViewBag.AniosEscolares = await _notaService.GetAniosEscolaresDisponiblesAsync();

            Nota modelo;
            if (id == null || id == 0)
            {
                modelo = new Nota();
            }
            else
            {
                modelo = await _notaService.GetNotaAsync(id.Value) ?? new Nota();
            }

            return PartialView("_Form", modelo);
        }
    }
}
