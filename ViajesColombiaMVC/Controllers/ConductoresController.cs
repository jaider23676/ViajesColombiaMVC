using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ConductoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConductoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Conductores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Conductores.ToListAsync());
        }

        // GET: Conductores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var conductor = await _context.Conductores
                .FirstOrDefaultAsync(c => c.ConductorId == id);

            if (conductor == null) return NotFound();

            return View(conductor);
        }

        // GET: Conductores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conductores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conductor conductor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conductor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conductor);
        }

        // GET: Conductores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var conductor = await _context.Conductores.FindAsync(id);
            if (conductor == null) return NotFound();

            return View(conductor);
        }

        // POST: Conductores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Conductor conductor)
        {
            if (id != conductor.ConductorId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(conductor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conductor);
        }

        // GET: Conductores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var conductor = await _context.Conductores
                .FirstOrDefaultAsync(c => c.ConductorId == id);

            if (conductor == null) return NotFound();

            return View(conductor);
        }

        // POST: Conductores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conductor = await _context.Conductores.FindAsync(id);
            _context.Conductores.Remove(conductor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===========================================
        // NUEVA ACCIÓN: MiConductor - PARA CLIENTES
        // ===========================================

        // GET: Conductores/MiConductor
        // GET: Conductores/MiConductor - VERSIÓN CORREGIDA
        public IActionResult MiConductor()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rolId = HttpContext.Session.GetInt32("RolId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Acceso");

            if (rolId != 2) // Solo para clientes
                return RedirectToAction("Cliente", "Home");

            try
            {
                // Buscar conductor asignado al cliente VÍA ASIGNACIONES
                var asignacion = _context.AsignacionesConductores
                    .Include(a => a.Conductor)
                    .FirstOrDefault(a => a.UsuarioId == usuarioId);

                if (asignacion?.Conductor != null)
                {
                    return View(asignacion.Conductor);
                }

                // Si no hay en asignaciones, buscar VÍA RESERVAS DEL CLIENTE
                // 1. Obtener paquetes del cliente
                var paqueteIds = _context.Reservas
                    .Where(r => r.UsuarioId == usuarioId)
                    .Select(r => r.PaqueteId)
                    .ToList();

                if (paqueteIds.Any())
                {
                    // 2. Buscar transportes de esos paquetes
                    var transporte = _context.TransportePaquetes
                        .Include(t => t.Conductor)
                        .Where(t => paqueteIds.Contains(t.PaqueteId))
                        .OrderByDescending(t => t.Fecha)
                        .FirstOrDefault();

                    if (transporte?.Conductor != null)
                    {
                        return View(transporte.Conductor);
                    }
                }

                // Si no encuentra conductor
                ViewBag.Message = "No tienes un conductor asignado actualmente.";
                ViewBag.Sugerencia = "Realiza una reserva para que se te asigne un conductor.";
                return View();
            }
            catch (Exception ex)
            {
                // Si hay error, mostrar vista vacía con mensaje
                ViewBag.Error = "Error al cargar información del conductor";
                ViewBag.Message = "Por favor, intenta más tarde.";
                return View();
            }
        }
    }
        // ===========================================
    
}
