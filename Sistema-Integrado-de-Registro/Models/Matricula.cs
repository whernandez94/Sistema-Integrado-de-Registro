using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; }

        [Required]
        public int SeccionId { get; set; }
        public Seccion Seccion { get; set; }

        [Required]
        public int AnioEscolarId { get; set; }
        public AnioEscolar AnioEscolar { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Matrícula")]
        public DateTime FechaMatricula { get; set; } = DateTime.Today;

        [Display(Name = "Número de Expediente")]
        public string NumeroExpediente { get; set; }

        public bool Activa { get; set; } = true;

        [MaxLength(500)]
        public string Observaciones { get; set; }
    }
}
