using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class SeccionService : ISeccionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SeccionService> _logger;

        public SeccionService(AppDbContext context, ILogger<SeccionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SeccionDto>> GetAllSeccionesAsync()
        {
            try
            {
                return await _context.Secciones
                    .Include(s => s.AnioEscolar)
                    .Include(s => s.Docente)
                    .Include(s => s.Matriculas)
                    .OrderBy(s => s.AnioEscolar.Anio)
                    .ThenBy(s => s.Grado)
                    .ThenBy(s => s.Nombre)
                    .Select(s => new SeccionDto
                    {
                        Id = s.Id,
                        Nombre = s.Nombre,
                        Grado = s.Grado,
                        AnioEscolar = s.AnioEscolar.Anio,
                        DocenteGuia = $"{s.Docente.Nombre} {s.Docente.Apellido}",
                        CantidadEstudiantes = s.Matriculas.Count
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las secciones");
                throw;
            }
        }

        public async Task<SeccionDetalleDto> GetSeccionWithEstudiantesAsync(int id)
        {
            var seccion = await _context.Secciones
                .Include(s => s.AnioEscolar)
                .Include(s => s.Docente)
                .Include(s => s.Matriculas)
                    .ThenInclude(m => m.Estudiante)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seccion == null)
                throw new KeyNotFoundException("Sección no encontrada");

            return new SeccionDetalleDto
            {
                Id = seccion.Id,
                Nombre = seccion.Nombre,
                Grado = seccion.Grado,
                AnioEscolar = seccion.AnioEscolar.Anio,
                DocenteGuia = $"{seccion.Docente.Nombre} {seccion.Docente.Apellido}",
                Estudiantes = seccion.Matriculas
                .OrderBy(m => m.Estudiante.Apellido)
                .ThenBy(m => m.Estudiante.Nombre)
                .Select(m => new EstudianteDto
                {
                    Id = m.Estudiante.Id,
                    Cedula = m.Estudiante.Cedula,
                    NombreCompleto = $"{m.Estudiante.Nombre} {m.Estudiante.Apellido}"
                })
                .ToList()

            };
        }

        public async Task<ServiceResult> SaveSeccionAsync(Seccion seccion)
        {
            try
            {
                var existe = await _context.Secciones
                    .AnyAsync(s => s.Nombre == seccion.Nombre &&
                                  s.Grado == seccion.Grado &&
                                  s.AnioEscolarId == seccion.AnioEscolarId &&
                                  s.Id != seccion.Id);

                if (existe)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Ya existe una sección con ese nombre para el mismo grado y año escolar"
                    };

                if (seccion.Id == 0)
                {
                    await _context.Secciones.AddAsync(seccion);
                }
                else
                {
                    _context.Secciones.Update(seccion);
                }

                await _context.SaveChangesAsync();
                return new ServiceResult { Success = true, Message = "Sección guardada correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar sección {seccion.Nombre}");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar la sección: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteSeccionAsync(int id)
        {
            try
            {
                var seccion = await _context.Secciones
                    .Include(s => s.Matriculas)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (seccion == null)
                    return new ServiceResult { Success = false, Message = "Sección no encontrada" };

                if (seccion.Matriculas.Any())
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "No se puede eliminar la sección porque tiene estudiantes matriculados"
                    };

                _context.Secciones.Remove(seccion);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Sección eliminada correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar sección con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar la sección: {ex.Message}"
                };
            }
        }

        public async Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync()
        {
            return await _context.AniosEscolares
                .OrderByDescending(a => a.Anio)
                .ToListAsync();
        }

        public async Task<List<Docente>> GetDocentesDisponiblesAsync()
        {
            return await _context.Docentes
                .Where(d => d.Activo)
                .OrderBy(d => d.Apellido)
                .ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<Seccion?> GetSeccionByIdAsync(int id)
        {
            return await _context.Secciones
                .Include(s => s.AnioEscolar)
                .Include(s => s.Docente)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

    }
}
