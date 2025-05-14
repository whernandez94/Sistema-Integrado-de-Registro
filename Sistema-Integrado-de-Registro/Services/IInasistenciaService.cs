using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IInasistenciaService
    {
        Task<List<InasistenciaResumenDto>> GetInasistenciasConResumenAsync(int anioEscolarId, int? asignaturaId);
        Task<Inasistencia?> GetInasistenciaAsync(int id);
        Task<ServiceResult> SaveInasistenciaAsync(Inasistencia inasistencia);
        Task<ServiceResult> DeleteInasistenciaAsync(int id);
        Task<List<Asignatura>> GetAsignaturasDisponiblesAsync();
        Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync();
        Task<List<Estudiante>> GetEstudiantesConInasistenciasAsync(int anioEscolarId);
    }
}
