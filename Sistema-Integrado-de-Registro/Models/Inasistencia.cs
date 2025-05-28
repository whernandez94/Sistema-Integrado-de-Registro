using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Inasistencia
    {
        public int Id { get; set; }

        [Required]
        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; }

        [Required]
        public int AsignaturaId { get; set; }
        public Asignatura Asignatura { get; set; }

        [Required]
        public int AnioEscolarId { get; set; }
        public AnioEscolar AnioEscolar { get; set; }

        [Required]
        public int Lapso { get; set; } // 1, 2 o 3

        [Required]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public int Porcentaje { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }

    public class InasistenciaResumenDto
    {
        public int EstudianteId { get; set; }
        public string EstudianteNombre { get; set; }
        public int AsignaturaId { get; set; }
        public string AsignaturaNombre { get; set; }
        public int PorcentajePermitido { get; set; }
        public int? Lapso1 { get; set; }
        public int? Lapso2 { get; set; }
        public int? Lapso3 { get; set; }
        public bool Alerta { get; set; }
    }

    public class GuardarInasistenciaDto
    {
        public int EstudianteId { get; set; }
        public int AsignaturaId { get; set; }
        public int AnioEscolarId { get; set; }
        public int Porcentaje { get; set; }
        public int? Lapso { get; set; }
        public string Observaciones { get; set; }
    }
}
