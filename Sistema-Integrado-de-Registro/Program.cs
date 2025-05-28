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
            builder.Services.AddScoped<IGradoService, GradoService>();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Cuenta/Login";
                options.LogoutPath = "/Cuenta/Logout";
            });


            var app = builder.Build();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "",
                defaults: new { controller = "Cuenta", action = "Login" });

            app.MapControllerRoute(
                name: "login",
                pattern: "iniciar-sesion",
                defaults: new { controller = "Cuenta", action = "Login" });


            app.MapControllerRoute(
                name: "logout",
                pattern: "cerrar-sesion",
                defaults: new { controller = "Cuenta", action = "Logout" });

            app.MapControllerRoute(
                name: "inicio",
                pattern: "panel",
                defaults: new { controller = "Home", action = "Index" });

            /*** rutas anios escolares  **/
            app.MapControllerRoute(
                name: "anios",
                pattern: "anios-escolares/{action=Index}",
                defaults: new { controller = "AnioEscolar" });

            /** Asignaturas **/
            app.MapControllerRoute(
                name: "asignaturas",
                pattern: "asignaturas/{action=Index}",
                defaults: new { controller = "Asignatura" });

            /*** Docentes **/
            app.MapControllerRoute(
                name: "docentes",
                pattern: "docentes/{action=Index}",
                defaults: new { controller = "Docente" });

            /*** Estudiantes **/
            app.MapControllerRoute(
                name: "estudiantes",
                pattern: "estudiantes/{action=Index}",
                defaults: new { controller = "Estudiante" });

            /*** Grados **/
            app.MapControllerRoute(
                name: "grados",
                pattern: "grados/{action=Index}",
                defaults: new { controller = "Grado" });

            /*** Inasistencias **/
            app.MapControllerRoute(
                name: "inasistencias",
                pattern: "inasistencias/{action=Index}",
                defaults: new { controller = "Inasistencia" });

            /*** Institucion **/
            app.MapControllerRoute(
                name: "institucion",
                pattern: "institucion/{action=Index}",
                defaults: new { controller = "Institution" });

            /*** Matricula **/
            app.MapControllerRoute(
                name: "matricula",
                pattern: "matricula-de-alumnos/{action=Index}",
                defaults: new { controller = "Matricula" });

            /*** Notas **/
            app.MapControllerRoute(
                name: "notas",
                pattern: "notas/{action=Index}",
                defaults: new { controller = "Nota" });

            /*** Notas **/
            app.MapControllerRoute(
                name: "secciones",
                pattern: "secciones/{action=Index}",
                defaults: new { controller = "Seccion" });

            app.Run();

        }
    }
}
