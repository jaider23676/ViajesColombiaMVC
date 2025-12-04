using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsuarios = await _context.Usuarios.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),
                TotalPaquetes = await _context.PaquetesTuristicos.CountAsync(),
                TotalConductores = await _context.Conductores.CountAsync(),

                ReservasRecientes = await _context.Reservas
                    .Include(r => r.Usuario)
                    .Include(r => r.Paquete)
                    .OrderByDescending(r => r.FechaReserva)
                    .Take(8)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
