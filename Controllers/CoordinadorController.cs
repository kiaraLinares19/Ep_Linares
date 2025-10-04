using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ep_Linares.Data;
using Ep_Linares.Models;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoordinadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Panel principal
    public IActionResult Index()
    {
        return View();
    }

    // Listar cursos
    public async Task<IActionResult> Cursos()
    {
        var cursos = await _context.Cursos.ToListAsync();
        return View(cursos);
    }

    // Crear curso
    public IActionResult CrearCurso() => View();

    [HttpPost]
    public async Task<IActionResult> CrearCurso(Curso curso)
    {
        if (ModelState.IsValid)
        {
            _context.Add(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cursos));
        }
        return View(curso);
    }

    // Editar curso
    public async Task<IActionResult> EditarCurso(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();
        return View(curso);
    }

    [HttpPost]
    public async Task<IActionResult> EditarCurso(Curso curso)
    {
        if (ModelState.IsValid)
        {
            _context.Update(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cursos));
        }
        return View(curso);
    }

    // Desactivar curso
    public async Task<IActionResult> DesactivarCurso(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso != null)
        {
            curso.Activo = false;
            _context.Update(curso);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Cursos));
    }

    // Listar matrículas de un curso
    public async Task<IActionResult> Matriculas(int cursoId)
    {
        var matriculas = await _context.Matriculas
            .Include(m => m.Usuario)
            .Where(m => m.CursoId == cursoId)
            .ToListAsync();
        ViewBag.CursoId = cursoId;
        return View(matriculas);
    }

    // Confirmar matrícula
    public async Task<IActionResult> ConfirmarMatricula(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula != null)
        {
            matricula.Estado = EstadoMatricula.Confirmada;
            _context.Update(matricula);
            await _context.SaveChangesAsync();
            return RedirectToAction("Matriculas", new { cursoId = matricula.CursoId });
        }
        return NotFound();
    }

    // Cancelar matrícula
    public async Task<IActionResult> CancelarMatricula(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula != null)
        {
            matricula.Estado = EstadoMatricula.Cancelada;
            _context.Update(matricula);
            await _context.SaveChangesAsync();
            return RedirectToAction("Matriculas", new { cursoId = matricula.CursoId });
        }
        return NotFound();
    }
}
