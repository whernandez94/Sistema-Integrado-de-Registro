using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class NotaService : INotaService
    {
        private readonly AppDbContext _context;

        public NotaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotaPromedioDto>> GetNotasConPromediosAsync(int anioEscolarId, int? asignaturaId)
        {
            var query = _context.Notas
                .Include(n => n.Estudiante)
                .Include(n => n.Asignatura)
                .Include(n => n.AnioEscolar)
                .Where(n => n.AnioEscolarId == anioEscolarId);

            if (asignaturaId.HasValue)
            {
                query = query.Where(n => n.AsignaturaId == asignaturaId.Value);
            }

            var notas = await query.ToListAsync();

            var resultados = notas
                .GroupBy(n => new { n.EstudianteId, n.AsignaturaId, n.AnioEscolarId })
                .Select(g => new NotaPromedioDto
                {
                    EstudianteId = g.Key.EstudianteId,
                    EstudianteNombre = $"{g.First().Estudiante.Nombre} {g.First().Estudiante.Apellido}",
                    AsignaturaId = g.Key.AsignaturaId,
                    AsignaturaNombre = g.First().Asignatura.Nombre,
                    AnioEscolarId = g.Key.AnioEscolarId,
                    AnioEscolar = g.First().AnioEscolar.Anio,
                    Lapso1 = g.FirstOrDefault(n => n.Lapso == 1)?.Valor,
                    Lapso2 = g.FirstOrDefault(n => n.Lapso == 2)?.Valor,
                    Lapso3 = g.FirstOrDefault(n => n.Lapso == 3)?.Valor,
                    PromedioFinal = g.Average(n => n.Valor)
                })
                .OrderBy(r => r.EstudianteNombre)
                .ToList();

            return resultados;
        }

        public async Task<Nota?> GetNotaAsync(int id)
        {
            return await _context.Notas.FindAsync(id);
        }

        public async Task<ServiceResult> SaveNotaAsync(Nota nota)
        {
            try
            {
                var existe = await _context.Notas
                    .AnyAsync(n => n.EstudianteId == nota.EstudianteId &&
                                  n.AsignaturaId == nota.AsignaturaId &&
                                  n.AnioEscolarId == nota.AnioEscolarId &&
                                  n.Lapso == nota.Lapso &&
                                  n.Id != nota.Id);

                if (existe)
                    return new ServiceResult { Success = false, Message = "Ya existe una nota registrada para este estudiante, asignatura, año escolar y lapso" };

                if (nota.Id == 0)
                {
                    await _context.Notas.AddAsync(nota);
                }
                else
                {
                    _context.Notas.Update(nota);
                }

                await _context.SaveChangesAsync();
                return new ServiceResult { Success = true, Message = "Nota guardada correctamente" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al guardar la nota: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DeleteNotaAsync(int id)
        {
            try
            {
                var nota = await _context.Notas.FindAsync(id);
                if (nota == null)
                    return new ServiceResult { Success = false, Message = "Nota no encontrada" };

                _context.Notas.Remove(nota);
                await _context.SaveChangesAsync();
                return new ServiceResult { Success = true, Message = "Nota eliminada correctamente" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error al eliminar la nota: {ex.Message}" };
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
    }
}
