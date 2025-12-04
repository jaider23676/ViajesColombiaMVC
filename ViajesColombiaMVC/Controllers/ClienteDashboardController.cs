using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class ClienteDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            var usuario = await _context.Usuarios
                .Include(u => u.Asignaciones)
                    .ThenInclude(a => a.Conductor)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            var reservas = await _context.Reservas
                .Include(r => r.Paquete)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            var vm = new ClienteDashboardViewModel
            {
                Usuario = usuario,
                TotalReservas = reservas.Count,
                Reservas = reservas,
                ConductoresAsignados = usuario.Asignaciones.ToList()
            };

            return View(vm);
        }
    }
}
