using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class AsignaturaService : IAsignaturaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AsignaturaService> _logger;

        public AsignaturaService(AppDbContext context, ILogger<AsignaturaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AsignaturaDto>> GetAllAsignaturasAsync()
        {
            try
            {
                return await _context.Asignaturas
                    .Where(a => a.Activa) // Solo asignaturas activas por defecto
                    .Select(a => new AsignaturaDto
                    {
                        Id = a.Id,
                        Nombre = a.Nombre,
                        PorcentajeInasistencia = a.PorcentajeInasistencia,
                        Activa = a.Activa
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las asignaturas");
                throw;
            }
        }

        public async Task<AsignaturaDto?> GetAsignaturaByIdAsync(int id)
        {
            try
            {
                var asignatura = await _context.Asignaturas.FindAsync(id);

                if (asignatura == null)
                    return null;

                return new AsignaturaDto
                {
                    Id = asignatura.Id,
                    Nombre = asignatura.Nombre,
                    PorcentajeInasistencia = asignatura.PorcentajeInasistencia,
                    Activa = asignatura.Activa
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener asignatura con ID {id}");
                throw;
            }
        }

        public async Task<ServiceResult> SaveAsignaturaAsync(AsignaturaDto dto)
        {
            try
            {
                if (dto.Id == 0)
                {
                    // Crear nueva asignatura
                    var nuevaAsignatura = new Asignatura
                    {
                        Nombre = dto.Nombre,
                        PorcentajeInasistencia = dto.PorcentajeInasistencia,
                        Activa = dto.Activa
                    };

                    await _context.Asignaturas.AddAsync(nuevaAsignatura);
                }
                else
                {
                    // Actualizar asignatura existente
                    var existente = await _context.Asignaturas.FindAsync(dto.Id);
                    if (existente == null)
                        return new ServiceResult { Success = false, Message = "Asignatura no encontrada" };

                    existente.Nombre = dto.Nombre;
                    existente.PorcentajeInasistencia = dto.PorcentajeInasistencia;
                    // Nota: No actualizamos Activa aquí para usar el método específico Toggle
                }

                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Asignatura guardada correctamente"
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar asignatura en la base de datos");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al guardar la asignatura en la base de datos"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar asignatura");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error inesperado al guardar la asignatura"
                };
            }
        }

        public async Task<ServiceResult> DeleteAsignaturaAsync(int id)
        {
            try
            {
                var asignatura = await _context.Asignaturas.FindAsync(id);
                if (asignatura == null)
                    return new ServiceResult { Success = false, Message = "Asignatura no encontrada" };

                // Verificar si la asignatura está siendo usada
                var tieneRelaciones = await _context.DocenteAsignaturas
                    .AnyAsync(da => da.AsignaturaId == id);

                if (tieneRelaciones)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "No se puede eliminar, la asignatura está asignada a docentes"
                    };

                _context.Asignaturas.Remove(asignatura);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Asignatura eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar asignatura con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al eliminar la asignatura"
                };
            }
        }

        public async Task<ServiceResult> ToggleAsignaturaStatusAsync(int id)
        {
            try
            {
                var asignatura = await _context.Asignaturas.FindAsync(id);
                if (asignatura == null)
                    return new ServiceResult { Success = false, Message = "Asignatura no encontrada" };

                asignatura.Activa = !asignatura.Activa;
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = $"Asignatura {(asignatura.Activa ? "activada" : "desactivada")} correctamente",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cambiar estado de asignatura con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al cambiar el estado de la asignatura"
                };
            }
        }
    }
}
