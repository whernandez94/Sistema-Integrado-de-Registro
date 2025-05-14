using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Seccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la sección es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El grado es requerido")]
        [Range(1, 6, ErrorMessage = "El grado debe estar entre 1 y 6")]
        public int Grado { get; set; }

        [Required(ErrorMessage = "El año escolar es requerido")]
        public int AnioEscolarId { get; set; }
        public AnioEscolar AnioEscolar { get; set; }

        [Required(ErrorMessage = "El docente guía es requerido")]
        public int DocenteId { get; set; }
        public Docente Docente { get; set; }

        // Relación con estudiantes
        public ICollection<Matricula> Matriculas { get; set; }
    }

    public class SeccionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Grado { get; set; }
        public string AnioEscolar { get; set; }
        public string DocenteGuia { get; set; }
        public int CantidadEstudiantes { get; set; }
    }

    public class SeccionDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Grado { get; set; }
        public string AnioEscolar { get; set; }
        public string DocenteGuia { get; set; }
        public List<EstudianteDto> Estudiantes { get; set; } = new();

    }

    public class EstudianteDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
    }
}
