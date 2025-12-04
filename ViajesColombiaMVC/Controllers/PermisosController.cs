using Microsoft.AspNetCore.Mvc;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class PermisosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermisosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📋 Listar permisos
        public IActionResult Index()
        {
            var permisos = _context.Permisos.ToList();
            return View(permisos);
        }

        // ➕ Crear permiso (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // ➕ Crear permiso (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                _context.Permisos.Add(permiso);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // ✏️ Editar permiso (GET)
        public IActionResult Editar(int id)
        {
            var permiso = _context.Permisos.Find(id);
            if (permiso == null)
                return NotFound();

            return View("Editar", permiso);
        }

        // ✏️ Editar permiso (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                _context.Permisos.Update(permiso);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // 🗑️ Eliminar permiso
        public IActionResult Eliminar(int id)
        {
            var permiso = _context.Permisos.Find(id);
            if (permiso == null)
                return NotFound();

            _context.Permisos.Remove(permiso);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}

