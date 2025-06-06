namespace Sistema_Integrado_de_Registro.DTO
{
    public class MatriculaDto
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int SeccionId { get; set; }
        public int AnioEscolarId { get; set; }
        public DateTime FechaMatricula { get; set; }
        public string? NumeroExpediente { get; set; }
        public string? Observaciones { get; set; }
    }

    public class MatriculaDetailsDto
    {
        public int Id { get; set; }
        public string NombreEstudiante { get; set; }
        public string Seccion { get; set; }
        public string AnioEscolar { get; set; }
        public string NumeroExpediente { get; set; }
        public string Fecha { get; set; }
        public string Observaciones { get; set; }
        public bool Activa { get; set; } = true;
    }

    public class MatriculaEditDto
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string NombreEstudiante { get; set; }
        public int SeccionId { get; set; }
        public string NombreSeccion { get; set; }
        public int AnioEscolarId { get; set; }
        public string NombreAnioEscolar { get; set; }
        public string NumeroExpediente { get; set; }
        public DateTime FechaMatricula { get; set; }
        public bool Activa { get; set; }
        public string? Observaciones { get; set; }
    }


}
