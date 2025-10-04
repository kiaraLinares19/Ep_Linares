using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Data;
using Ep_Linares.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ep_Linares.Controllers
{
    [Authorize] // Solo usuarios autenticados pueden matricularse
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribirse(int cursoId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Debes iniciar sesión para inscribirte.";
                return RedirectToAction("Login", "Account");
            }

            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null || !curso.Activo)
            {
                TempData["Error"] = "El curso no existe o no está disponible.";
                return RedirectToAction("Index", "Cursos");
            }

            //  Validar si ya está matriculado
            bool yaMatriculado = await _context.Matriculas.AnyAsync(m => m.UsuarioId == user.Id && m.CursoId == cursoId);
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            //  Validar cupo máximo
            int inscritos = await _context.Matriculas.CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Error"] = "No hay cupos disponibles para este curso.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Validar solapamiento de horarios
            var cursosMatriculados = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id && m.Estado != EstadoMatricula.Cancelada)
                .Select(m => m.Curso)
                .ToListAsync();

            bool solapa = cursosMatriculados.Any(c =>
                (curso.HorarioInicio < c.HorarioFin) && (curso.HorarioFin > c.HorarioInicio)
            );

            if (solapa)
            {
                TempData["Error"] = "No puedes inscribirte: el horario se solapa con otro curso matriculado.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Crear la matrícula
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = user.Id,
                Estado = EstadoMatricula.Pendiente,
                FechaRegistro = DateTime.Now
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Inscripción registrada correctamente en estado Pendiente.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }
    }
}
