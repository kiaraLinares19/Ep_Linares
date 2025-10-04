using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Data;
using Ep_Linares.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ep_Linares.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // ---------------------
        // LISTADO DE CURSOS
        // ---------------------
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "CursosActivos";
            List<Curso> cursos;

            // Buscar en caché
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                cursos = JsonSerializer.Deserialize<List<Curso>>(cachedData);
            }
            else
            {
                cursos = await _context.Cursos.Where(c => c.Activo).ToListAsync();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                };

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cursos), options);
            }

            return View(cursos);
        }

        // ---------------------
        // DETALLES DEL CURSO
        // ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound();

            // Guardar en sesión el último curso
            HttpContext.Session.SetString("UltimoCurso", curso.Nombre);
            HttpContext.Session.SetInt32("UltimoCursoId", curso.Id);

            return View(curso);
        }

        // ---------------------
        // CREAR CURSO
        // ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();

                // Invalidar caché
                await _cache.RemoveAsync("CursosActivos");

                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // ---------------------
        // EDITAR CURSO
        // ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();

                // Invalidar caché
                await _cache.RemoveAsync("CursosActivos");

                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }
    }
}
