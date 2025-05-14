using System.ComponentModel.DataAnnotations;

namespace Sistema_Integrado_de_Registro.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        [Required] public string Cedula { get; set; }
        [Required] public string Nombre { get; set; }
        [Required] public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        // Representante
        public string NombreRepresentante { get; set; }
        public string CedulaRepresentante { get; set; }
        public string TelefonoRepresentante { get; set; }
    }

}
