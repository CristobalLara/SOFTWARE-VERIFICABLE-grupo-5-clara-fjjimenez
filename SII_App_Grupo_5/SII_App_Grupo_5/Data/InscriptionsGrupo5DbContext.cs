using Microsoft.EntityFrameworkCore;
using SII_App_Grupo_5.Models;
namespace SII_App_Grupo_5.Data
{
    public class InscriptionsGrupo5DbContext : DbContext
    {
        public InscriptionsGrupo5DbContext(DbContextOptions<InscriptionsGrupo5DbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(); // Enable lazy loading
        }
        public DbSet<Enajenante> Enajenantes { get; set; }
        public DbSet<Adquiriente> Adquirientes { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<MultiPropietario> MultiPropietarios { get; set; }
        public DbSet<Comuna> Comunas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enajenante>()
                .HasOne(e => e.Inscripcion)
                .WithMany(i => i.Enajenantes)
                .HasForeignKey(e => e.InscripcionId);
            modelBuilder.Entity<Adquiriente>()
                .HasOne(e => e.Inscripcion)
                .WithMany(i => i.Adquirientes)
                .HasForeignKey(e => e.InscripcionId);
        }
    }
}

