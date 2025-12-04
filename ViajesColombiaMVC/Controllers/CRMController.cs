using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class CRMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CRMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================================================
        // DASHBOARD DEL CLIENTE
        // ===================================================================
        public async Task<IActionResult> Cliente(int? id)
        {
            // -------------------------------------------
            // Seguridad
            // -------------------------------------------
            int? usuarioIdSesion = HttpContext.Session.GetInt32("UsuarioId");
            int? rolIdSesion = HttpContext.Session.GetInt32("RolId");

            if (usuarioIdSesion == null)
                return RedirectToAction("Login", "Acceso");

            // Si el admin entra puede ver cualquier cliente
            int usuarioId = (rolIdSesion == 1)
                ? id ?? usuarioIdSesion.Value
                : usuarioIdSesion.Value;

            var usuario = await _context.Usuarios
                .Include(u => u.Asignaciones)
                    .ThenInclude(a => a.Conductor)
                .Include(u => u.Reservas)
                    .ThenInclude(r => r.Paquete)
                .Include(u => u.Reservas)
                    .ThenInclude(r => r.Pagos)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // ===================================================================
        // ASIGNAR CONDUCTOR
        // ===================================================================
        public IActionResult AsignarConductor(int usuarioId)
        {
            ViewBag.UsuarioId = usuarioId;
            ViewBag.Conductores = _context.Conductores.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AsignarConductor(AsignacionConductor obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Conductores = _context.Conductores.ToList();
                return View(obj);
            }

            obj.FechaAsignacion = DateTime.Now;
            obj.Activo = true;

            _context.AsignacionesConductores.Add(obj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cliente", new { id = obj.UsuarioId });
        }

        // ===================================================================
        // CALIFICAR CONDUCTOR
        // ===================================================================
        public async Task<IActionResult> Calificar(int conductorId, int usuarioId)
        {
            ViewBag.Conductor = await _context.Conductores.FindAsync(conductorId);
            ViewBag.UsuarioId = usuarioId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Calificar(CalificacionConductor obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            obj.Fecha = DateTime.Now;

            _context.CalificacionesConductores.Add(obj);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cliente", new { id = obj.UsuarioId });
        }
    }
}
