using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Services;
using Sistema_Integrado_de_Registro.Utils;

namespace Sistema_Integrado_de_Registro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // Servicios (DI)
            builder.Services.AddScoped<IInstitutionService, InstitutionService>();
            builder.Services.AddScoped<IDocenteService, DocenteService>();
            builder.Services.AddScoped<IEstudianteService, EstudianteService>();
            builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();
            builder.Services.AddScoped<IAnioEscolarService, AnioEscolarService>();
            builder.Services.AddScoped<INotaService, NotaService>();
            builder.Services.AddScoped<IInasistenciaService, InasistenciaService>();
            builder.Services.AddScoped<ISeccionService, SeccionService>();
            builder.Services.AddScoped<IMatriculaService, MatriculaService>();
            builder.Services.AddScoped<IGradoService, GradoService>();

            // Base de datos
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Autenticación con cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Cuenta/Login";
                    options.LogoutPath = "/Cuenta/Logout";
                });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    DataSeeder.Initialize(services);
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Datos agregados a la tabla asignatura y docente");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al sembrar la base de datos");
                }
            }

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                    DataSeeder.Initialize(scope.ServiceProvider);
                }
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapControllerRoute(
                name: "login",
                pattern: "iniciar-sesion",
                defaults: new { controller = "Cuenta", action = "Login" });

            app.MapControllerRoute(
                name: "logout",
                pattern: "cerrar-sesion",
                defaults: new { controller = "Cuenta", action = "Logout" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Cuenta}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
