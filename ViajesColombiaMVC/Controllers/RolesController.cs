using Microsoft.AspNetCore.Mvc;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar todos los roles
        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        // Crear nuevo rol
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Rol rol)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(rol);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rol);
        }

        // Editar rol
        public IActionResult Edit(int id)
        {
            var rol = _context.Roles.Find(id);
            if (rol == null) return NotFound();
            return View(rol);
        }

        [HttpPost]
        public IActionResult Edit(Rol rol)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Update(rol);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Edit", rol);
        }

        // Eliminar rol
        public IActionResult Eliminar(int id)
        {
            var rol = _context.Roles.Find(id);
            if (rol == null) return NotFound();

            _context.Roles.Remove(rol);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
