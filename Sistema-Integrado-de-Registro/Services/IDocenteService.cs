namespace Sistema_Integrado_de_Registro.Services
{
    public interface IDocenteService
    {
        Task<IEnumerable<DocenteDto>> GetAllDocentesAsync();
        Task<DocenteDetailDto?> GetDocenteByIdAsync(int id);
        Task<ServiceResult> SaveDocenteAsync(DocenteSaveDto dto);
        Task<ServiceResult> DeleteDocenteAsync(int id);
    }

    public class DocenteDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int CargaHoras { get; set; }
        public string Asignaturas { get; set; }
    }

    public class DocenteDetailDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int CargaHoras { get; set; }
        public List<int> Asignaturas { get; set; } = new();
    }

    public class DocenteSaveDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int CargaHoras { get; set; }
        public List<int> Asignaturas { get; set; } = new();
    }
}
