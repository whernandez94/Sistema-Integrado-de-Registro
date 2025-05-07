using Microsoft.EntityFrameworkCore;
using Sistema_Integrado_de_Registro.Models;

namespace Sistema_Integrado_de_Registro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Institution> Instituciones { get; set; }
        public DbSet<AnioEscolar> AniosEscolares { get; set; }

    }
}
