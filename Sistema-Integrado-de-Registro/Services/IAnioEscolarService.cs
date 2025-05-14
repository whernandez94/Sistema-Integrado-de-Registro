using Sistema_Integrado_de_Registro.Models;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IAnioEscolarService
    {
        Task<IEnumerable<AnioEscolar>> GetAllAniosEscolaresAsync();
        Task<AnioEscolar?> GetAnioEscolarByIdAsync(int id);
        Task<ServiceResult> SaveAnioEscolarAsync(AnioEscolar anioEscolar);
        Task<ServiceResult> DeleteAnioEscolarAsync(int id);
        Task<ServiceResult> FinalizarAnioEscolarAsync(int id);
        Task<AnioEscolar> GetAnioEscolarForFormAsync(int? id);
    }
}
