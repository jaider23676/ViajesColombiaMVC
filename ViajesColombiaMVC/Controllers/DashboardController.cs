using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Evita error cuando la tabla 'pagos' está vacía
            decimal totalPagos = 0;

            if (await _context.Pagos.AnyAsync())
            {
                totalPagos = await _context.Pagos.SumAsync(p => p.Monto);
            }

            var model = new AdminDashboardViewModel
            {
                TotalUsuarios = await _context.Usuarios.CountAsync(),
                TotalPaquetes = await _context.PaquetesTuristicos.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),
                Pendientes = await _context.Reservas.CountAsync(r => r.Estado == "Pendiente"),
                Confirmadas = await _context.Reservas.CountAsync(r => r.Estado == "Confirmada"),
                Canceladas = await _context.Reservas.CountAsync(r => r.Estado == "Cancelada"),
                TotalProveedores = await _context.Proveedores.CountAsync(),
                TotalConductores = await _context.Conductores.CountAsync(),
                TotalPagos = totalPagos
            };

            return View(model);
        }
    }
}
