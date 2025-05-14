using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Asignatura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de inasistencias debe estar entre 0 y 100")]
        public int PorcentajeInasistencia { get; set; }
        public bool Activa { get; set; } = true;
    }
}
