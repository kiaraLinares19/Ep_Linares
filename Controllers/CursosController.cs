using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Data;
using Ep_Linares.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ep_Linares.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index(string nombre, int? creditosMin, int? creditosMax, TimeSpan? horaInicio, TimeSpan? horaFin)
        {
            var cursos = _context.Cursos.Where(c => c.Activo);

            // 🔹 Filtro por nombre
            if (!string.IsNullOrEmpty(nombre))
                cursos = cursos.Where(c => c.Nombre.Contains(nombre));

            // 🔹 Filtro por rango de créditos
            if (creditosMin.HasValue)
                cursos = cursos.Where(c => c.Creditos >= creditosMin.Value);
            if (creditosMax.HasValue)
                cursos = cursos.Where(c => c.Creditos <= creditosMax.Value);

            // 🔹 Filtro por horario
            if (horaInicio.HasValue)
                cursos = cursos.Where(c => c.HorarioInicio >= horaInicio.Value);
            if (horaFin.HasValue)
                cursos = cursos.Where(c => c.HorarioFin <= horaFin.Value);

            return View(await cursos.ToListAsync());
        }

        // GET: Cursos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            return View(curso);
        }
    }
}
