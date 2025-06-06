using Microsoft.AspNetCore.Authentication.Cookies;
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

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
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
