namespace Sistema_Integrado_de_Registro.Models
{
    public class DocenteAsignatura
    {
        public int DocenteId { get; set; }
        public Docente Docente { get; set; }

        public int AsignaturaId { get; set; }
        public Asignatura Asignatura { get; set; }
    }

}
