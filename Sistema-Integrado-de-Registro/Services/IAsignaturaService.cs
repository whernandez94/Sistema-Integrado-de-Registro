using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IAsignaturaService
    {
        Task<IEnumerable<AsignaturaDto>> GetAllAsignaturasAsync();
        Task<AsignaturaDto?> GetAsignaturaByIdAsync(int id);
        Task<ServiceResult> SaveAsignaturaAsync(AsignaturaDto dto);
        Task<ServiceResult> DeleteAsignaturaAsync(int id);
        Task<ServiceResult> ToggleAsignaturaStatusAsync(int id);
    }

    public class AsignaturaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de inasistencias debe estar entre 0 y 100")]
        public int PorcentajeInasistencia { get; set; }

        public bool Activa { get; set; } = true;
    }
}
