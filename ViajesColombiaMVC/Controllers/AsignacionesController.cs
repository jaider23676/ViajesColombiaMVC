using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class AsignacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsignacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var asignaciones = await _context.AsignacionesConductores
                .Include(a => a.Usuario)
                .Include(a => a.Conductor)
                .OrderByDescending(a => a.FechaAsignacion)
                .ToListAsync();

            return View(asignaciones);
        }

        // FORMULARIO CREATE
        public async Task<IActionResult> Create()
        {
            ViewBag.Usuarios = await _context.Usuarios.ToListAsync();
            ViewBag.Conductores = await _context.Conductores.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AsignacionConductor asignacion)
        {
            if (ModelState.IsValid)
            {
                asignacion.FechaAsignacion = DateTime.Now;
                asignacion.Activo = true;

                _context.Add(asignacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Usuarios = await _context.Usuarios.ToListAsync();
            ViewBag.Conductores = await _context.Conductores.ToListAsync();
            return View(asignacion);
        }

        // SUGERIR CONDUCTORES
        public async Task<IActionResult> Sugerir(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            string pref = usuario.Preferencias?.ToLower() ?? "";

            var conductores = await _context.Conductores.ToListAsync();

            var sugeridos = conductores
                .Select(c =>
                {
                    int score = 0;

                    if (pref.Contains(c.Especialidad?.ToLower() ?? "")) score += 10;
                    if (pref.Contains(c.Zona?.ToLower() ?? "")) score += 8;
                    if (pref.Contains("vip") && c.Especialidad?.ToLower().Contains("vip") == true) score += 5;

                    return new { c, score };
                })
                .OrderByDescending(x => x.score)
                .Select(x => x.c)
                .Take(10)
                .ToList();

            ViewBag.Usuario = usuario;
            return View(sugeridos);
        }

        [HttpPost]
        public async Task<IActionResult> Asignar(int usuarioId, int conductorId)
        {
            var a = new AsignacionConductor
            {
                UsuarioId = usuarioId,
                ConductorId = conductorId,
                FechaAsignacion = DateTime.Now,
                Activo = true
            };

            _context.AsignacionesConductores.Add(a);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            var asignacion = await _context.AsignacionesConductores.FindAsync(id);
            if (asignacion == null) return NotFound();

            asignacion.Activo = false;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
