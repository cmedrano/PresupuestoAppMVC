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
        public DbSet<Budget> Budget { get; set; }
        public DbSet<RubroType> RubroType { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Diary> Diary { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Income> Income { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<RubroType>()
                .HasOne(r => r.RubroPadre)
                .WithMany(r => r.SubRubros)
                .HasForeignKey(r => r.RubroPadreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
