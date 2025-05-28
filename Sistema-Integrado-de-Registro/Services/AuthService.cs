using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.DTO;
using Sistema_Integrado_de_Registro.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sistema_Integrado_de_Registro.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Docentes.FirstOrDefaultAsync(d =>
                d.Codigo == dto.Codigo && d.Activo);

            if (user == null || !VerifyPassword(user.Contrasena, dto.Contrasena))
                return null;

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Rol = user.Rol,
                Nombre = $"{user.Nombre} {user.Apellido}"
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hash, string input)
        {
            return BCrypt.Net.BCrypt.Verify(input, hash);
        }

        private string GenerateJwtToken(Docente user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Rol),
            new Claim("Nombre", $"{user.Nombre} {user.Apellido}")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
