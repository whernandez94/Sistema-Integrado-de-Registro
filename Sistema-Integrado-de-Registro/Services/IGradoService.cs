using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Services
{
    public interface IGradoService
    {
        IEnumerable<Grado> ObtenerTodos();
        Grado ObtenerPorId(int id);
        void Guardar(Grado grado);
        bool Eliminar(int id);
    }

}
