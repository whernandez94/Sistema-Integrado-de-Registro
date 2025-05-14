using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Nota
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
        [Range(0, 20, ErrorMessage = "La nota debe estar entre 0 y 20")]
        public decimal Valor { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    public class NotaPromedioDto
    {
        public int EstudianteId { get; set; }
        public string EstudianteNombre { get; set; }
        public int AsignaturaId { get; set; }
        public string AsignaturaNombre { get; set; }
        public int AnioEscolarId { get; set; }
        public string AnioEscolar { get; set; }
        public decimal? Lapso1 { get; set; }
        public decimal? Lapso2 { get; set; }
        public decimal? Lapso3 { get; set; }
        public decimal? PromedioFinal { get; set; }
    }
}
