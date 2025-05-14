using Sistema_Integrado_de_Registro.Models;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IEstudianteService
    {
        Task<IEnumerable<Estudiante>> GetAllEstudiantesAsync();
        Task<Estudiante?> GetEstudianteByIdAsync(int id);
        Task<ServiceResult> SaveEstudianteAsync(Estudiante estudiante);
        Task<ServiceResult> DeleteEstudianteAsync(int id);
    }

}
