using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly AppDbContext _context;

        public EstudianteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estudiante>> GetAllEstudiantesAsync()
        {
            return await _context.Estudiantes.ToListAsync();
        }

        public async Task<Estudiante?> GetEstudianteByIdAsync(int id)
        {
            return await _context.Estudiantes.FindAsync(id);
        }

        public async Task<ServiceResult> SaveEstudianteAsync(Estudiante estudiante)
        {
            try
            {
                if (estudiante.Id == 0)
                {
                    await _context.Estudiantes.AddAsync(estudiante);
                }
                else
                {
                    var existente = await _context.Estudiantes.FindAsync(estudiante.Id);
                    if (existente == null)
                        return new ServiceResult { Success = false, Message = "Estudiante no encontrado" };

                    _context.Entry(existente).CurrentValues.SetValues(estudiante);
                }

                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Estudiante guardado correctamente"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar estudiante: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error inesperado: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteEstudianteAsync(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null)
                    return new ServiceResult { Success = false, Message = "Estudiante no encontrado" };

                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Estudiante eliminado correctamente"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar estudiante: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error inesperado: {ex.Message}"
                };
            }
        }
    }
}
