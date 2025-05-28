using Sistema_Integrado_de_Registro.DTO;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        string HashPassword(string password);
        bool VerifyPassword(string hash, string input);
    }

}
