using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface ISeccionService
    {
        Task<List<SeccionDto>> GetAllSeccionesAsync();
        Task<SeccionDetalleDto> GetSeccionWithEstudiantesAsync(int id);
        Task<ServiceResult> SaveSeccionAsync(Seccion seccion);
        Task<ServiceResult> DeleteSeccionAsync(int id);
        Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync();
        Task<List<Docente>> GetDocentesDisponiblesAsync();
        Task<Seccion?> GetSeccionByIdAsync(int value);
    }

}
