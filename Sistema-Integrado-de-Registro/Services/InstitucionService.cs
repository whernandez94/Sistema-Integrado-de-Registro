using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{

    public class InstitutionService : IInstitutionService
    {
        private readonly AppDbContext _context;

        public InstitutionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Institution>> GetAllInstitutionsAsync()
        {
            return await _context.Instituciones.ToListAsync();
        }

        public async Task<Institution?> GetInstitutionByIdAsync(int id)
        {
            return await _context.Instituciones.FindAsync(id);
        }

        public async Task<ServiceResult> SaveInstitutionAsync(Institution model)
        {
            try
            {
                if (model.Id == 0)
                    _context.Instituciones.Add(model);
                else
                    _context.Instituciones.Update(model);

                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Guardado exitosamente",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al guardar: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> DeleteInstitutionAsync(int id)
        {
            try
            {
                var institution = await _context.Instituciones.FindAsync(id);
                if (institution == null)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Institución no encontrada"
                    };

                _context.Instituciones.Remove(institution);
                await _context.SaveChangesAsync();

                return new ServiceResult
                {
                    Success = true,
                    Message = "Eliminado correctamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = $"Error al eliminar: {ex.Message}"
                };
            }
        }
    }

}
