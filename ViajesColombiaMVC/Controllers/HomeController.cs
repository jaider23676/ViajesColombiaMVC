using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // PÁGINA INICIAL (REDIRECCIÓN SEGÚN ROL)
        // =========================================================
        public IActionResult Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            return rolId switch
            {
                1 => RedirectToAction("Admin"),
                2 => RedirectToAction("Cliente"),
                _ => RedirectToAction("Login", "Acceso")
            };
        }

        // =======================
        // DASHBOARD CLIENTE
        // =======================
        public async Task<IActionResult> Cliente()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId != 2)
                return RedirectToAction("Admin");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            var reservas = await _context.Reservas
                .Include(r => r.Paquete)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            var accesos = await _context.Accesos
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaAcceso)
                .Take(5)
                .ToListAsync();

            ClienteDashboardViewModel vm = new()
            {
                Usuario = usuario,
                Reservas = reservas,
                TotalReservas = reservas.Count,
                UltimosAccesos = accesos
            };

            return View(vm);
        }

        // =========================================================
        // ADMIN DASHBOARD
        // =========================================================
        public async Task<IActionResult> Admin()
        {
            int? rolId = HttpContext.Session.GetInt32("RolId");
            if (rolId != 1) return RedirectToAction("Login", "Acceso");

            AdminDashboardViewModel vm = new()
            {
                TotalUsuarios = await _context.Usuarios.CountAsync(),
                TotalPaquetes = await _context.PaquetesTuristicos.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),

                Pendientes = await _context.Reservas.CountAsync(r => r.Estado == "Pendiente"),
                Confirmadas = await _context.Reservas.CountAsync(r => r.Estado == "Confirmada"),
                Canceladas = await _context.Reservas.CountAsync(r => r.Estado == "Cancelada"),

                TotalProveedores = await _context.Proveedores.CountAsync(),
                TotalConductores = await _context.Conductores.CountAsync(),

                TotalPagos = await _context.Pagos.SumAsync(p => p.Monto)
            };

            return View(vm);
        }

        // =======================
        // LOGOUT
        // =======================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Acceso");
        }
    }
}