using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.Services;

[Route("[controller]/[action]")]
public class DocenteController : Controller
{
    private readonly IDocenteService _service;

    public DocenteController(IDocenteService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var docentes = await _service.GetAllDocentesAsync();
        return Json(docentes);
    }

    [HttpGet]
    public async Task<IActionResult> Obtener(int id)
    {
        var docente = await _service.GetDocenteByIdAsync(id);
        return docente != null
            ? Json(docente)
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] DocenteSaveDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.SaveDocenteAsync(dto);

        return result.Success
            ? Ok(new { result.Message, Data = result.Data })
            : BadRequest(result.Message);
    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int id)
    {
        var result = await _service.DeleteDocenteAsync(id);
        return result.Success
            ? Ok(new { result.Message })
            : BadRequest(result.Message);
    }
}
