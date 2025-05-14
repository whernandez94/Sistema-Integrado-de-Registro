using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class AnioEscolarService : IAnioEscolarService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnioEscolarService> _logger;

        public AnioEscolarService(AppDbContext context, ILogger<AnioEscolarService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AnioEscolar>> GetAllAniosEscolaresAsync()
        {
            try
            {
                return await _context.AniosEscolares
                    .OrderByDescending(a => a.Anio)
                    .Select(a => new AnioEscolar
                    {
                        Id = a.Id,
                        Anio = a.Anio,
                        Finalizado = a.Finalizado
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los años escolares");
                throw;
            }
        }

        public async Task<AnioEscolar?> GetAnioEscolarByIdAsync(int id)
        {
            try
            {
                var anio = await _context.AniosEscolares.FindAsync(id);
                if (anio == null)
                    return null;

                return new AnioEscolar
                {
                    Id = anio.Id,
                    Anio = anio.Anio,
                    Finalizado = anio.Finalizado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener año escolar con ID {id}");
                throw;
            }
        }

        public async Task<ServiceResult> SaveAnioEscolarAsync(AnioEscolar dto)
        {
            try
            {
                // Validar duplicados
                var existe = await _context.AniosEscolares
                    .AnyAsync(a => a.Anio == dto.Anio && a.Id != dto.Id);

                if (existe)
                    return new ServiceResult { Success = false, Message = "Ese año escolar ya existe" };

                if (dto.Id == 0)
                {
                    var nuevoAnio = new AnioEscolar
                    {
                        Anio = dto.Anio,
                        Finalizado = dto.Finalizado
                    };

                    await _context.AniosEscolares.AddAsync(nuevoAnio);
                }
                else
                {
                    var existente = await _context.AniosEscolares.FindAsync(dto.Id);
                    if (existente == null)
                        return new ServiceResult { Success = false, Message = "Año escolar no encontrado" };

                    existente.Anio = dto.Anio;
                    // No permitimos cambiar el estado Finalizado desde aquí
                }

                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Año escolar guardado correctamente"
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar año escolar en la base de datos");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al guardar el año escolar en la base de datos"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar año escolar");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error inesperado al guardar el año escolar"
                };
            }
        }

        public async Task<ServiceResult> DeleteAnioEscolarAsync(int id)
        {
            try
            {
                var anio = await _context.AniosEscolares.FindAsync(id);
                if (anio == null)
                    return new ServiceResult { Success = false, Message = "Año escolar no encontrado" };

                // Verificar si hay datos asociados al año escolar
                var tieneDatos = await _context.Matriculas.AnyAsync(m => m.AnioEscolarId == id) ||
                                await _context.Notas.AnyAsync(n => n.AnioEscolarId == id);

                if (tieneDatos)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "No se puede eliminar, hay datos asociados a este año escolar"
                    };

                _context.AniosEscolares.Remove(anio);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Año escolar eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar año escolar con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al eliminar el año escolar"
                };
            }
        }

        public async Task<ServiceResult> FinalizarAnioEscolarAsync(int id)
        {
            try
            {
                var anio = await _context.AniosEscolares.FindAsync(id);
                if (anio == null)
                    return new ServiceResult { Success = false, Message = "Año escolar no encontrado" };

                if (anio.Finalizado)
                    return new ServiceResult { Success = false, Message = "El año escolar ya está finalizado" };

                anio.Finalizado = true;
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Año escolar finalizado correctamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al finalizar año escolar con ID {id}");
                return new ServiceResult
                {
                    Success = false,
                    Message = "Error al finalizar el año escolar"
                };
            }
        }

        public async Task<AnioEscolar> GetAnioEscolarForFormAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return new AnioEscolar();
            }

            var anio = await _context.AniosEscolares.FindAsync(id.Value);
            if (anio == null)
                throw new KeyNotFoundException("Año escolar no encontrado");

            return new AnioEscolar
            {
                Id = anio.Id,
                Anio = anio.Anio,
                Finalizado = anio.Finalizado
            };
        }
    }
}
