using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class DocenteService : IDocenteService
    {
        private readonly AppDbContext _context;

        public DocenteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocenteDto>> GetAllDocentesAsync()
        {
            var docentes = await _context.Docentes
                .Include(d => d.DocenteAsignaturas)
                    .ThenInclude(da => da.Asignatura)
                .ToListAsync();

            return docentes.Select(d => new DocenteDto
            {
                Id = d.Id,
                Cedula = d.Cedula,
                NombreCompleto = $"{d.Nombre} {d.Apellido}",
                Telefono = d.Telefono,
                Correo = d.Correo,
                CargaHoras = d.CargaHoras,
                Asignaturas = string.Join(", ", d.DocenteAsignaturas.Select(a => a.Asignatura.Nombre))
            });
        }

        public async Task<DocenteDetailDto?> GetDocenteByIdAsync(int id)
        {
            var docente = await _context.Docentes
                .Include(d => d.DocenteAsignaturas)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null) return null;

            return new DocenteDetailDto
            {
                Id = docente.Id,
                Cedula = docente.Cedula,
                Nombre = docente.Nombre,
                Apellido = docente.Apellido,
                Telefono = docente.Telefono,
                Correo = docente.Correo,
                CargaHoras = docente.CargaHoras,
                Asignaturas = docente.DocenteAsignaturas.Select(da => da.AsignaturaId).ToList()
            };
        }

        public async Task<ServiceResult> SaveDocenteAsync(DocenteSaveDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Docente docente;

                if (dto.Id == 0)
                {
                    docente = new Docente
                    {
                        Cedula = dto.Cedula,
                        Nombre = dto.Nombre,
                        Apellido = dto.Apellido,
                        Telefono = dto.Telefono,
                        Correo = dto.Correo,
                        CargaHoras = dto.CargaHoras,
                        Codigo = dto.Codigo, // debes agregarlo al DTO
                        Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena), // también agregar al DTO
                        Rol = "Evaluador", // o "Administrador"
                    };

                    await _context.Docentes.AddAsync(docente);
                }
                else
                {
                    docente = await _context.Docentes
                        .Include(d => d.DocenteAsignaturas)
                        .FirstOrDefaultAsync(d => d.Id == dto.Id);

                    if (docente == null)
                        return new ServiceResult { Success = false, Message = "Docente no encontrado" };

                    docente.Cedula = dto.Cedula;
                    docente.Nombre = dto.Nombre;
                    docente.Apellido = dto.Apellido;
                    docente.Telefono = dto.Telefono;
                    docente.Correo = dto.Correo;
                    docente.CargaHoras = dto.CargaHoras;

                    // Eliminar relaciones existentes
                    _context.DocenteAsignaturas.RemoveRange(docente.DocenteAsignaturas);
                }

                await _context.SaveChangesAsync();

                // Agregar nuevas relaciones
                foreach (var asignaturaId in dto.Asignaturas)
                {
                    docente.DocenteAsignaturas.Add(new DocenteAsignatura
                    {
                        DocenteId = docente.Id,
                        AsignaturaId = asignaturaId
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Docente guardado correctamente",
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar docente: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteDocenteAsync(int id)
        {
            try
            {
                var docente = await _context.Docentes
                    .Include(d => d.DocenteAsignaturas)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (docente == null)
                    return new ServiceResult { Success = false, Message = "Docente no encontrado" };

                _context.DocenteAsignaturas.RemoveRange(docente.DocenteAsignaturas);
                _context.Docentes.Remove(docente);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Docente eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar docente: {ex.Message}"
                };
            }
        }
    }
}
