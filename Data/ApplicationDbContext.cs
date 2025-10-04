using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Models;

namespace Ep_Linares.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            builder.Entity<Matricula>()
                .HasIndex(m => new { m.CursoId, m.UsuarioId })
                .IsUnique(); // Un usuario no puede matricularse dos veces

            builder.Entity<Curso>()
                .Property(c => c.Creditos)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");

            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
        }
    }
}
