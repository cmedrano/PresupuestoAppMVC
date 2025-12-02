using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Models.Entities;

namespace PresupuestoMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<RubroType> RubroType { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
    }
}
