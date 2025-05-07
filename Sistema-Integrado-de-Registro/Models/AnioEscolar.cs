using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class AnioEscolar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Año es obligatorio.")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Formato válido: AAAA-AAAA")]
        public string Anio { get; set; }

        public bool Finalizado { get; set; }
    }
}
