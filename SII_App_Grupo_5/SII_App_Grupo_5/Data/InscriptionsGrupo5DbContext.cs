using Microsoft.EntityFrameworkCore;
using SII_App_Grupo_5.Models;
namespace SII_App_Grupo_5.Data
{
    public class InscriptionsGrupo5DbContext : DbContext
    {
        public InscriptionsGrupo5DbContext(DbContextOptions<InscriptionsGrupo5DbContext> options) : base(options)
        {

        }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Persona> Personas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inscripcion>().ToTable(nameof(Inscripcion))
                .HasMany(p => p.Personas)
                .WithMany(i => i.Inscripciones);
        }
    }
}

