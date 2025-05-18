using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Institution> Instituciones { get; set; }
        public DbSet<AnioEscolar> AniosEscolares { get; set; }
        public DbSet<Asignatura> Asignaturas { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<DocenteAsignatura> DocenteAsignaturas { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<Inasistencia> Inasistencias { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Grado> Grados { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocenteAsignatura>()
                .HasKey(da => new { da.DocenteId, da.AsignaturaId });

            modelBuilder.Entity<DocenteAsignatura>()
                .HasOne(da => da.Docente)
                .WithMany(d => d.DocenteAsignaturas)
                .HasForeignKey(da => da.DocenteId);

            modelBuilder.Entity<DocenteAsignatura>()
                .HasOne(da => da.Asignatura)
                .WithMany()
                .HasForeignKey(da => da.AsignaturaId);

            modelBuilder.Entity<Matricula>()
        .HasOne(m => m.Seccion)
        .WithMany(s => s.Matriculas)
        .HasForeignKey(m => m.SeccionId)
        .OnDelete(DeleteBehavior.Restrict); // o .NoAction

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Estudiante)
                .WithMany() // asumiendo que no hay propiedad de navegación inversa
                .HasForeignKey(m => m.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.AnioEscolar)
                .WithMany() // asumiendo lo mismo
                .HasForeignKey(m => m.AnioEscolarId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
