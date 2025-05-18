using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Grado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del grado es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nivel es requerido")]
        [StringLength(50, ErrorMessage = "El nivel no puede exceder los 50 caracteres")]
        public string Nivel { get; set; } // Primaria, Secundaria, etc.
    }
}
