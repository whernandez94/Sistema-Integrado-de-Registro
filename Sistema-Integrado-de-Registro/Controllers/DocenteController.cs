using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Services;

[Authorize]
[Route("gestion-escolar/docentes")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DocenteController : Controller
{
    private readonly IDocenteService _service;

    public DocenteController(IDocenteService service)
    {
        _service = service;
    }

    [HttpGet("")]
    [HttpGet("listado")]
    public IActionResult Index() => View();

    [HttpGet("obtener-todos")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var docentes = await _service.GetAllDocentesAsync();
        return Json(docentes);
    }

    [HttpGet("obtener/{id:int}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var docente = await _service.GetDocenteByIdAsync(id);
        return docente != null
            ? Json(docente)
            : NotFound();
    }

    [HttpPost("guardar")]
    public async Task<IActionResult> Guardar([FromBody] DocenteSaveDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.SaveDocenteAsync(dto);

        return result.Success
            ? Ok(new { result.Message, Data = result.Data })
            : BadRequest(result.Message);
    }

    [HttpDelete("eliminar/{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _service.DeleteDocenteAsync(id);
        return result.Success
            ? Ok(new { result.Message })
            : BadRequest(result.Message);
    }
}
