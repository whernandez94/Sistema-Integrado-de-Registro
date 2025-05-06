using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Institution
    {

        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string NombreDirector { get; set; }

        [Required, StringLength(20)]
        public string IdentificacionDirector { get; set; }

        [Required, StringLength(200)]
        public string Direccion { get; set; }

        [Required, Phone]
        public string Telefono { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

    }
}
