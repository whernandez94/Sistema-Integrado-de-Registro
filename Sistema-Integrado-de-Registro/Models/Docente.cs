using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required, MaxLength(7)]
        [RegularExpression(@"^[a-zA-Z]{2}\d{5}$", ErrorMessage = "El código debe tener 2 letras y 5 números")]
        public string Codigo { get; set; }

        [Required]
        public string Contrasena { get; set; }

        [Required]
        public string Rol { get; set; } // "Administrador" o "Evaluador"

        [NotMapped]
        public List<DocenteAsignatura> DocenteAsignaturas { get; set; } = new();
        [NotMapped]
        public List<int> Asignaturas { get; set; } = new();
    }

}
