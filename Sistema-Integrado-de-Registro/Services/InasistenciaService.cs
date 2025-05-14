using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class InasistenciaService : IInasistenciaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InasistenciaService> _logger;

        public InasistenciaService(AppDbContext context, ILogger<InasistenciaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<InasistenciaResumenDto>> GetInasistenciasConResumenAsync(int anioEscolarId, int? asignaturaId)
        {
            var query = _context.Inasistencias
                .Include(i => i.Estudiante)
                .Include(i => i.Asignatura)
                .Include(i => i.AnioEscolar)
                .Where(i => i.AnioEscolarId == anioEscolarId);

            if (asignaturaId.HasValue)
            {
                query = query.Where(i => i.AsignaturaId == asignaturaId.Value);
            }

            var inasistencias = await query.ToListAsync();

            var resultados = inasistencias
                .GroupBy(i => new { i.EstudianteId, i.AsignaturaId })
                .Select(g => new InasistenciaResumenDto
                {
                    EstudianteId = g.Key.EstudianteId,
                    EstudianteNombre = $"{g.First().Estudiante.Nombre} {g.First().Estudiante.Apellido}",
                    AsignaturaId = g.Key.AsignaturaId,
                    AsignaturaNombre = g.First().Asignatura.Nombre,
                    PorcentajePermitido = g.First().Asignatura.PorcentajeInasistencia,
                    Lapso1 = g.FirstOrDefault(i => i.Lapso == 1)?.Porcentaje,
                    Lapso2 = g.FirstOrDefault(i => i.Lapso == 2)?.Porcentaje,
                    Lapso3 = g.FirstOrDefault(i => i.Lapso == 3)?.Porcentaje,
                    Alerta = g.Any(i => i.Porcentaje > i.Asignatura.PorcentajeInasistencia)
                })
                .OrderBy(r => r.EstudianteNombre)
                .ToList();

            return resultados;
        }

        public async Task<Inasistencia?> GetInasistenciaAsync(int id)
        {
            return await _context.Inasistencias.FindAsync(id);
        }

        public async Task<ServiceResult> SaveInasistenciaAsync(Inasistencia inasistencia)
        {
            try
            {
                // Validar que no exista ya un registro para el mismo estudiante, asignatura, año y lapso
                var existe = await _context.Inasistencias
                    .AnyAsync(i => i.EstudianteId == inasistencia.EstudianteId &&
                                  i.AsignaturaId == inasistencia.AsignaturaId &&
                                  i.AnioEscolarId == inasistencia.AnioEscolarId &&
                                  i.Lapso == inasistencia.Lapso &&
                                  i.Id != inasistencia.Id);

                if (existe)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Ya existe un registro de inasistencia para este estudiante, asignatura, año escolar y lapso"
                    };
                }

                // Validar porcentaje máximo
                var asignatura = await _context.Asignaturas.FindAsync(inasistencia.AsignaturaId);
                if (asignatura == null)
                {
                    return new ServiceResult { Success = false, Message = "Asignatura no encontrada" };
                }

                if (inasistencia.Id == 0)
                {
                    await _context.Inasistencias.AddAsync(inasistencia);
                }
                else
                {
                    _context.Inasistencias.Update(inasistencia);
                }

                await _context.SaveChangesAsync();

                // Verificar si se supera el porcentaje permitido
                if (inasistencia.Porcentaje > asignatura.PorcentajeInasistencia)
                {
                    return new ServiceResult
                    {
                        Success = true,
                        Message = "Inasistencia guardada (ALERTA: Porcentaje supera el límite permitido)",
                        Data = {} // Indica que hay alerta
                    };
                }

                return new ServiceResult
                {
                    Success = true,
                    Message = "Inasistencia guardada correctamente",
                    Data = {} // No hay alerta
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar inasistencia");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar la inasistencia: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteInasistenciaAsync(int id)
        {
            try
            {
                var inasistencia = await _context.Inasistencias.FindAsync(id);
                if (inasistencia == null)
                {
                    return new ServiceResult { Success = false, Message = "Inasistencia no encontrada" };
                }

                _context.Inasistencias.Remove(inasistencia);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Inasistencia eliminada correctamente" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar inasistencia con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar la inasistencia: {ex.Message}"
                };
            }
        }

        public async Task<List<Asignatura>> GetAsignaturasDisponiblesAsync()
        {
            return await _context.Asignaturas
                .Where(a => a.Activa)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync()
        {
            return await _context.AniosEscolares
                .OrderByDescending(a => a.Anio)
                .ToListAsync();
        }

        public async Task<List<Estudiante>> GetEstudiantesConInasistenciasAsync(int anioEscolarId)
        {
            return await _context.Inasistencias
                .Where(i => i.AnioEscolarId == anioEscolarId)
                .Select(i => i.Estudiante)
                .Distinct()
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }
    }
}
