using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------
        // LISTA DE USUARIOS
        // -----------------------------------------
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();

            return View(usuarios);
        }

        // -----------------------------------------
        // MI PERFIL
        // -----------------------------------------
        public async Task<IActionResult> MiPerfil()
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null) return RedirectToAction("Login", "Acceso");

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // -----------------------------------------
        // CREAR USUARIO
        // -----------------------------------------
        public IActionResult Create()
        {
            ViewData["Roles"] = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Roles"] = _context.Roles.ToList();
            return View(usuario);
        }

        // -----------------------------------------
        // EDITAR USUARIO (GET)
        // -----------------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            ViewData["Roles"] = _context.Roles.ToList();
            return View(usuario);
        }

        // -----------------------------------------
        // EDITAR USUARIO (POST)
        // -----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Roles"] = _context.Roles.ToList();
            return View(usuario);
        }

        // -----------------------------------------
        // CAMBIAR CONTRASEÑA (GET)
        // -----------------------------------------
        public async Task<IActionResult> CambiarPassword(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // -----------------------------------------
        // CAMBIAR CONTRASEÑA (POST)
        // -----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(int id, string NuevaPassword)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Contrasena = NuevaPassword;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------
        // ELIMINAR USUARIO (GET)
        // -----------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // -----------------------------------------
        // ELIMINAR USUARIO (POST)
        // -----------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
