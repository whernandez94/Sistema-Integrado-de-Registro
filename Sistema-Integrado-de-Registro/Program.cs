using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Services;

namespace Sistema_Integrado_de_Registro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add services
            builder.Services.AddScoped<IInstitutionService, InstitutionService>();
            builder.Services.AddScoped<IDocenteService, DocenteService>();
            builder.Services.AddScoped<IEstudianteService, EstudianteService>();
            builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();
            builder.Services.AddScoped<IAnioEscolarService, AnioEscolarService>();
            builder.Services.AddScoped<INotaService, NotaService>();
            builder.Services.AddScoped<IInasistenciaService, InasistenciaService>();
            builder.Services.AddScoped<ISeccionService, SeccionService>();
            builder.Services.AddScoped<IMatriculaService, MatriculaService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService <AppDbContext>();
            //    db.Database.Migrate();
            //}

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "estudiantes",
                    pattern: "estudiantes/{action=Index}/{id?}",
                    defaults: new { controller = "Estudiante" });

                endpoints.MapControllerRoute(
                    name: "docentes",
                    pattern: "docentes/{action=Index}/{id?}",
                    defaults: new { controller = "Docente" });

                endpoints.MapControllerRoute(
                    name: "asignaturas",
                    pattern: "asignaturas/{action=Index}/{id?}",
                    defaults: new { controller = "Asignatura" });

                endpoints.MapControllerRoute(
                    name: "asignacion-docente",
                    pattern: "asignacion-docente/{action=Index}/{id?}",
                    defaults: new { controller = "Asignacion" });

                endpoints.MapControllerRoute(
                    name: "anios-escolares",
                    pattern: "anios-escolares/{action=Index}/{id?}",
                    defaults: new { controller = "SchoolYear" });
            });

            app.Run();
        }
    }
}
