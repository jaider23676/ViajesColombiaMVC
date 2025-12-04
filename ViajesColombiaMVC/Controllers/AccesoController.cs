using Microsoft.AspNetCore.Mvc;
using ViajesColombiaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ViajesColombiaMVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Contrasena == contrasena);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }
            

            HttpContext.Session.SetString("Usuario", usuario.Nombre);
            HttpContext.Session.SetInt32("RolId", usuario.RolId);
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);

            if (usuario.RolId == 1)
                return RedirectToAction("Admin", "Home");
            else
                return RedirectToAction("Cliente", "Home");
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            usuario.FechaRegistro = DateTime.Now;
            usuario.RolId = 2;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar(string correo)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario == null)
            {
                ViewBag.Error = "No se encontró una cuenta con ese correo.";
                return View();
            }

            ViewBag.Mensaje = "Se envió un enlace de recuperación al correo.";
            return View();
        }

        // 🔹 Acción Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}