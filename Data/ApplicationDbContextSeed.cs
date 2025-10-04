using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Models;

namespace Ep_Linares.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            await context.Database.MigrateAsync();

            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso { Codigo = "INF101", Nombre = "Introducción a la Informática", Creditos = 3, CupoMaximo = 30, HorarioInicio = new TimeSpan(8,0,0), HorarioFin = new TimeSpan(10,0,0), Activo = true },
                    new Curso { Codigo = "MAT201", Nombre = "Cálculo II", Creditos = 4, CupoMaximo = 25, HorarioInicio = new TimeSpan(10,0,0), HorarioFin = new TimeSpan(12,0,0), Activo = true },
                    new Curso { Codigo = "ADM301", Nombre = "Administración General", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14,0,0), HorarioFin = new TimeSpan(16,0,0), Activo = true }
                );
                await context.SaveChangesAsync();
            }

            var email = "coordinador@portal.com";
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                await userManager.CreateAsync(user, "Coordinador123!");
            }
        }
    }
}
