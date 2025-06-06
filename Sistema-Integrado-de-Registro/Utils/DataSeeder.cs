using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Utils
{
    public static class DataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            context.Database.EnsureCreated();

            SeedAsignaturas(context);
            SeedDocentes(context);
        }

        private static void SeedAsignaturas(AppDbContext context)
        {
            if (!context.Asignaturas.Any())
            {
                context.Asignaturas.AddRange(
                    new Asignatura
                    {
                        Nombre = "Ingles II",
                        PorcentajeInasistencia = 76,
                        Activa = true
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedDocentes(AppDbContext context)
        {
            if (!context.Docentes.Any())
            {
                string hashed = BCrypt.Net.BCrypt.HashPassword("123456");
                var asignatura = context.Asignaturas.FirstOrDefault();

                context.Docentes.Add(new Docente
                {
                    Cedula = "05060969",
                    Nombre = "Adalberto",
                    Apellido = "Perez",
                    Telefono = "61555490",
                    Codigo = "WH23836",
                    Rol = "Administrador",
                    Correo = "correo@correo.com",
                    CargaHoras = 20,
                    Asignaturas = [asignatura.Id],
                    Contrasena = hashed,
                });

                context.SaveChanges();
            }
        }
    }
}
