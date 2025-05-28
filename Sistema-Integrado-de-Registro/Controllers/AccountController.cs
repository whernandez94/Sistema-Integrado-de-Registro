using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_Integrado_de_Registro.DTO;
using Sistema_Integrado_de_Registro.Services;
using System.Security.Claims;

namespace Sistema_Integrado_de_Registro.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
            {
                ViewBag.Error = "Código o contraseña inválidos";
                return View(dto);
            }

            // Crear identidad con Claims (basado en el token generado o directamente desde los datos del docente)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, result.Nombre),
            new Claim(ClaimTypes.Role, result.Rol),
            new Claim("Codigo", dto.Codigo)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }

}
