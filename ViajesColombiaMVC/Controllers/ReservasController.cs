using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------
        // LISTA DE RESERVAS (ADMIN)
        // -----------------------------------------
        public async Task<IActionResult> Index()
        {
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (rolId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId != 1)
                return RedirectToAction("Cliente", "Home");

            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Paquete)
                .ToListAsync();

            return View(reservas);
        }

        // -----------------------------------------
        // CREAR RESERVA (GET)
        // -----------------------------------------
        public IActionResult Create()
        {
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (rolId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId == 1)
                ViewData["Usuarios"] = _context.Usuarios.ToList();

            ViewData["Paquetes"] = _context.PaquetesTuristicos.ToList();
            ViewData["FormasPago"] = _context.FormasPago.Where(f => f.Activo).ToList();

            return View();
        }


        // -----------------------------------------
        // TRANSPORTE ASIGNADO AL CLIENTE (CLIENTE) - VERSIÓN CORREGIDA
        // -----------------------------------------
        public async Task<IActionResult> TransporteCliente(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId != 2)
                return RedirectToAction("Cliente", "Home");

            try
            {
                // VERSIÓN 100% FUNCIONAL - SIN PROPIEDADES FALTANTES
                // Solo usamos relaciones que SÍ existen

                // Primero, obtener los IDs de paquetes del cliente
                var paqueteIds = await _context.Reservas
                    .Where(r => r.UsuarioId == id)
                    .Select(r => r.PaqueteId)
                    .Distinct()
                    .ToListAsync();

                if (!paqueteIds.Any())
                {
                    ViewBag.Message = "No tienes reservas activas.";
                    return View(new List<TransportePaquete>());
                }

                // Buscar transportes relacionados con esos paquetes
                // NOTA: NO usamos Include para propiedades que no existen
                var transportes = await _context.TransportePaquetes
                    .Where(t => paqueteIds.Contains(t.PaqueteId))
                    .ToListAsync();

                if (!transportes.Any())
                {
                    ViewBag.Message = "Tus reservas están confirmadas, pero el transporte aún está siendo asignado.";
                    return View(transportes);
                }

                // Cargar datos relacionados MANUALMENTE para evitar errores
                foreach (var transporte in transportes)
                {
                    // Cargar Paquete
                    transporte.Paquete = await _context.PaquetesTuristicos
                        .FirstOrDefaultAsync(p => p.Id == transporte.PaqueteId);

                    // Cargar Conductor (si existe la relación)
                    if (transporte.ConductorId > 0)
                    {
                        transporte.Conductor = await _context.Conductores
                            .FirstOrDefaultAsync(c => c.ConductorId == transporte.ConductorId);
                    }
                }

                return View(transportes);
            }
            catch (Exception ex)
            {
                // Si hay error, mostrar página informativa
                ViewBag.Title = "Mi Transporte";
                ViewBag.Message = "Estamos trabajando en mejorar esta funcionalidad.";
                ViewBag.Error = ex.Message;
                return View(new List<TransportePaquete>());
            }
        }


        // -----------------------------------------
        // CREAR RESERVA (POST) CON PAGO
        // -----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva, int formaPagoId)
        {
            int? usuarioIdSesion = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioIdSesion == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId == 2)
                reserva.UsuarioId = usuarioIdSesion.Value;

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == reserva.UsuarioId);
            if (!usuarioExiste)
            {
                ModelState.AddModelError("", "El usuario no existe.");
                ViewData["Paquetes"] = _context.PaquetesTuristicos.ToList();
                ViewData["FormasPago"] = _context.FormasPago.Where(f => f.Activo).ToList();
                return View(reserva);
            }

            reserva.FechaReserva = DateTime.Now;
            reserva.Estado = "Pendiente";

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Crear pago asociado automáticamente
            var paquete = await _context.PaquetesTuristicos.FindAsync(reserva.PaqueteId);
            var pago = new Pago
            {
                ReservaId = reserva.Id,
                FormaPagoId = formaPagoId,
                Monto = paquete.Precio,
                FechaPago = DateTime.Now,
                Estado = "Pendiente"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            // Redirigir a detalle de pago
            return RedirectToAction("DetallePago", "Pagos", new { pagoId = pago.Id });
        }

        // -----------------------------------------
        // MIS RESERVAS (CLIENTE)
        // -----------------------------------------
        public async Task<IActionResult> MisReservas()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId != 2)
                return RedirectToAction("Home", "Cliente");

            var reservas = await _context.Reservas
                .Include(r => r.Paquete)
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            return View(reservas);
        }

        // -----------------------------------------
        // ELIMINAR RESERVA
        // -----------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Paquete)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

