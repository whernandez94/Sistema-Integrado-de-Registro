using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MatriculaService> _logger;

        public MatriculaService(AppDbContext context, ILogger<MatriculaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Matricula>> GetAllMatriculasAsync()
        {
            return await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.Seccion)
                .Include(m => m.AnioEscolar)
                .OrderByDescending(m => m.FechaMatricula)
                .ToListAsync();
        }

        public async Task<Matricula?> GetMatriculaByIdAsync(int id)
        {
            return await _context.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.Seccion)
                .Include(m => m.AnioEscolar)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ServiceResult> SaveMatriculaAsync(Matricula matricula)
        {
            try
            {
                var existe = await _context.Matriculas
                    .AnyAsync(m => m.EstudianteId == matricula.EstudianteId &&
                                   m.SeccionId == matricula.SeccionId &&
                                   m.AnioEscolarId == matricula.AnioEscolarId &&
                                   m.Id != matricula.Id);

                if (existe)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "El estudiante ya está matriculado en esta sección para el año escolar seleccionado"
                    };
                }

                if (string.IsNullOrWhiteSpace(matricula.NumeroExpediente))
                {
                    matricula.NumeroExpediente = await GenerarNumeroExpedienteAsync();
                }

                if (matricula.Id == 0)
                {
                    await _context.Matriculas.AddAsync(matricula);
                }
                else
                {
                    _context.Matriculas.Update(matricula);
                }

                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Matrícula guardada correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar matrícula");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar la matrícula: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteMatriculaAsync(int id)
        {
            try
            {
                var matricula = await _context.Matriculas.FindAsync(id);
                if (matricula == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Matrícula no encontrada"
                    };
                }

                _context.Matriculas.Remove(matricula);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Matrícula eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar matrícula con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar la matrícula: {ex.Message}"
                };
            }
        }

        public async Task<List<Estudiante>> GetEstudiantesDisponiblesAsync()
        {
            return await _context.Estudiantes
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<List<Seccion>> GetSeccionesDisponiblesAsync()
        {
            return await _context.Secciones
                .Include(s => s.AnioEscolar)
                .Where(s => s.AnioEscolar.Finalizado == false)
                .OrderBy(s => s.AnioEscolar.Anio)
                .ThenBy(s => s.Grado)
                .ThenBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<List<AnioEscolar>> GetAniosEscolaresDisponiblesAsync()
        {
            return await _context.AniosEscolares
                .Where(a => a.Finalizado == false)
                .OrderByDescending(a => a.Anio)
                .ToListAsync();
        }

        public async Task<string> GenerarNumeroExpedienteAsync()
        {
            var ultimoNumero = await _context.Matriculas
                .OrderByDescending(m => m.Id)
                .Select(m => m.NumeroExpediente)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(ultimoNumero))
            {
                return "EXP-000001";
            }

            var numeroParte = ultimoNumero.Split('-').LastOrDefault();

            if (!int.TryParse(numeroParte, out var numero))
            {
                return "EXP-000001";
            }

            numero++;
            return $"EXP-{numero.ToString().PadLeft(6, '0')}";
        }
    }

}
