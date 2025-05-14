using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{

    public interface IInstitutionService
    {
        Task<IEnumerable<Institution>> GetAllInstitutionsAsync();
        Task<Institution?> GetInstitutionByIdAsync(int id);
        Task<ServiceResult> SaveInstitutionAsync(Institution model);
        Task<ServiceResult> DeleteInstitutionAsync(int id);
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Institution? Data { get; set; }
    }

}
