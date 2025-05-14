using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Docente
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Cedula { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, MaxLength(100)]
        public string Apellido { get; set; }

        [MaxLength(15)]
        public string Telefono { get; set; }

        [EmailAddress]
        public string Correo { get; set; }

        [Range(1, 1000)]
        public int CargaHoras { get; set; }

        public bool Activo { get; set; } = true;

        // Relación con asignaturas (muchos a muchos)
        public List<DocenteAsignatura> DocenteAsignaturas { get; set; } = new();

        public List<int> Asignaturas { get; set; } = new();

    }

}
