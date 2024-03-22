using Microsoft.EntityFrameworkCore;
using SistemaTurnos.Models;

namespace SistemaTurnos.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 23)));
        }

        public DbSet<Asesor> Asesores { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<TurnosAtendidos> TurnosAtendidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TurnosAtendidos>()
                .HasOne(ta => ta.Asesor) 
                .WithMany() 
                .HasForeignKey(ta => ta.IdAsesor) 
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<TurnosAtendidos>()
                .HasOne(ta => ta.Turno) 
                .WithMany()  
                .HasForeignKey(ta => ta.IdTurno) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

