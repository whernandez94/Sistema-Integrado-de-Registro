using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface INotaService
    {
        Task<List<NotaPromedioDto>> GetNotasConPromediosAsync(int anioEscolarId, int? asignaturaId);
        Task<Nota?> GetNotaAsync(int id);
        Task<ServiceResult> SaveNotaAsync(Nota nota);
        Task<ServiceResult> DeleteNotaAsync(int id);
        Task<List<Asignatura>> GetAsignaturasDisponiblesAsync();
        Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync();
    }
}
