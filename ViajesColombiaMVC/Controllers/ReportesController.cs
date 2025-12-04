using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // DASHBOARD DE REPORTES
        // ================================
        public IActionResult Index()
        {
            return View();
        }

        // ================================
        // REPORTE DE RESERVAS
        // ================================
        public async Task<IActionResult> Reservas()
        {
            var data = await _context.Reservas
                .Include(r => r.Paquete)
                .GroupBy(r => r.Paquete.Nombre)
                .Select(g => new ReporteReservasViewModel
                {
                    Paquete = g.Key,
                    TotalReservas = g.Count(),
                    PersonasTotales = g.Sum(x => x.CantidadPersonas),
                    VentasTotales = g.Sum(x => x.Total)
                })
                .ToListAsync();

            return View(data);
        }

        // ================================
        // REPORTE DE VENTAS / PAGOS
        // ================================
        public async Task<IActionResult> Ventas()
        {
            var data = await _context.Pagos
                .Include(p => p.FormaPago)
                .GroupBy(p => p.FormaPago.Nombre)
                .Select(g => new ReporteVentasViewModel
                {
                    FormaPago = g.Key,
                    TotalMonto = g.Sum(x => x.Monto),
                    CantidadPagos = g.Count()
                })
                .ToListAsync();

            return View(data);
        }

        // ================================
        // REPORTE DE CONDUCTORES
        // ================================
        public async Task<IActionResult> Conductores()
        {
            var data = await _context.Conductores
                .Select(c => new ReporteConductoresViewModel
                {
                    Conductor = c.Nombre,
                    ViajesAsignados = c.Asignaciones.Count(a => a.Activo),
                    PromedioCalificacion = c.Calificaciones.Any()
                        ? c.Calificaciones.Average(x => x.Puntaje)
                        : 0
                })
                .ToListAsync();

            return View(data);
        }

        // ================================
        // REPORTE DE CLIENTES
        // ================================
        public async Task<IActionResult> Clientes()
        {
            var data = await _context.Usuarios
                .Where(u => u.RolId == 2) // clientes
                .Select(u => new ReporteClientesViewModel
                {
                    Cliente = u.Nombre,
                    ReservasTotales = u.Reservas.Count(),
                    MontoTotalGastado = u.Reservas.Sum(r => r.Total)
                })
                .ToListAsync();

            return View(data);
        }
    }
}
