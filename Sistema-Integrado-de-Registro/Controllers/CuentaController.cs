namespace Sistema_Integrado_de_Registro.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using Sistema_Integrado_de_Registro.Data;

    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CuentaController> _logger;

        public CuentaController(AppDbContext context, ILogger<CuentaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string codigo, string contrasena)
        {
            _logger.LogInformation("Inicio de sesión intentado para el código: {Codigo}", codigo);

            string hashed = BCrypt.Net.BCrypt.HashPassword("123456");
            _logger.LogWarning("Hash generado: {Hashed}", hashed);

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(contrasena))
            {
                _logger.LogWarning("Intento de inicio de sesión con datos incompletos.");
                ModelState.AddModelError("", "Debe ingresar todos los datos.");
                return View();
            }

            var docente = _context.Docentes.FirstOrDefault(d => d.Codigo == codigo);
            
            if (docente == null || !BCrypt.Net.BCrypt.Verify(contrasena, docente.Contrasena))
            {
                _logger.LogWarning("Intento de inicio de sesión fallido. Código: {Codigo}", codigo);
                ModelState.AddModelError("", "Código o contraseña incorrectos.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, docente.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{docente.Nombre} {docente.Apellido}"),
                new Claim(ClaimTypes.Role, docente.Rol),
                new Claim("Codigo", docente.Codigo)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation("Usuario {Codigo} ha iniciado sesión correctamente.", codigo);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }

}
