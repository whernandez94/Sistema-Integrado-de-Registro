using Sistema_Integrado_de_Registro.Data;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public class GradoService : IGradoService
    {
        private readonly AppDbContext _context;

        public GradoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Grado> ObtenerTodos()
        {
            return _context.Grados.ToList();
        }

        public Grado ObtenerPorId(int id)
        {
            return _context.Grados.Find(id);
        }

        public void Guardar(Grado grado)
        {
            if (grado.Id == 0)
                _context.Grados.Add(grado);
            else
                _context.Grados.Update(grado);

            _context.SaveChanges();
        }

        public bool Eliminar(int id)
        {
            var grado = _context.Grados.Find(id);
            if (grado == null) return false;

            _context.Grados.Remove(grado);
            _context.SaveChanges();
            return true;
        }
    }

}
