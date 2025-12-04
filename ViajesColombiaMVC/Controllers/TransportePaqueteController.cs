// Controllers/TransportePaqueteController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class TransportePaqueteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransportePaqueteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TransportePaquete/Index
        public async Task<IActionResult> Index(int? paqueteId)
        {
            IQueryable<TransportePaquete> query = _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .AsQueryable();

            if (paqueteId.HasValue)
            {
                query = query.Where(t => t.PaqueteId == paqueteId.Value);
                ViewBag.PaqueteId = paqueteId.Value;

                // Obtener nombre del paquete
                var paquete = await _context.PaquetesTuristicos
                    .FirstOrDefaultAsync(p => p.Id == paqueteId.Value);
                if (paquete != null)
                {
                    ViewBag.PaqueteNombre = paquete.Nombre;
                }
            }

            var lista = await query
                .OrderByDescending(t => t.Fecha)
                .ThenBy(t => t.Conductor.Nombre)
                .ToListAsync();

            // Cargar listas para filtros
            await CargarListasParaFiltros();

            return View(lista);
        }

        // GET: TransportePaquete/Create
        public IActionResult Create(int paqueteId)
        {
            ViewBag.PaqueteId = paqueteId;

            var paquete = _context.PaquetesTuristicos
                .FirstOrDefault(p => p.Id == paqueteId);

            if (paquete != null)
            {
                ViewBag.PaqueteNombre = paquete.Nombre;
            }

            CargarListasParaFormulario();

            return View(new TransportePaquete
            {
                PaqueteId = paqueteId,
                Fecha = DateTime.Now
            });
        }

        // POST: TransportePaquete/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransportePaquete transportePaquete)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que no haya conflicto de fecha con el mismo conductor
                    bool conductorOcupado = await _context.TransportePaquetes
                        .AnyAsync(t => t.ConductorId == transportePaquete.ConductorId
                                    && t.Fecha.Date == transportePaquete.Fecha.Date);

                    if (conductorOcupado)
                    {
                        ModelState.AddModelError("ConductorId",
                            "El conductor ya tiene una asignación para esta fecha");
                        CargarListasParaFormulario();
                        return View(transportePaquete);
                    }

                    _context.TransportePaquetes.Add(transportePaquete);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Transporte asignado exitosamente";
                    return RedirectToAction("Index", new { paqueteId = transportePaquete.PaqueteId });
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            CargarListasParaFormulario();
            return View(transportePaquete);
        }

        // GET: TransportePaquete/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transportePaquete = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transportePaquete == null) return NotFound();

            CargarListasParaFormulario();
            return View(transportePaquete);
        }

        // POST: TransportePaquete/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransportePaquete transportePaquete)
        {
            if (id != transportePaquete.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que no haya conflicto (excluyendo el registro actual)
                    bool conductorOcupado = await _context.TransportePaquetes
                        .AnyAsync(t => t.ConductorId == transportePaquete.ConductorId
                                    && t.Fecha.Date == transportePaquete.Fecha.Date
                                    && t.Id != id);

                    if (conductorOcupado)
                    {
                        ModelState.AddModelError("ConductorId",
                            "El conductor ya tiene una asignación para esta fecha");
                        CargarListasParaFormulario();
                        return View(transportePaquete);
                    }

                    _context.TransportePaquetes.Update(transportePaquete);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Transporte actualizado exitosamente";
                    return RedirectToAction("Index", new { paqueteId = transportePaquete.PaqueteId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransportePaqueteExists(transportePaquete.Id)) return NotFound();
                    else throw;
                }
            }

            CargarListasParaFormulario();
            return View(transportePaquete);
        }

        // GET: TransportePaquete/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transportePaquete = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transportePaquete == null) return NotFound();

            return View(transportePaquete);
        }

        // POST: TransportePaquete/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transportePaquete = await _context.TransportePaquetes.FindAsync(id);
            if (transportePaquete != null)
            {
                int paqueteId = transportePaquete.PaqueteId;
                _context.TransportePaquetes.Remove(transportePaquete);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transporte eliminado exitosamente";
                return RedirectToAction("Index", new { paqueteId = paqueteId });
            }

            return NotFound();
        }

        // GET: TransportePaquete/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var transportePaquete = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transportePaquete == null) return NotFound();

            return View(transportePaquete);
        }

        // ============ NUEVOS MÉTODOS ============

        // GET: TransportePaquete/Calendario
        public async Task<IActionResult> Calendario(int? month, int? year)
        {
            var currentDate = DateTime.Now;
            var viewMonth = month ?? currentDate.Month;
            var viewYear = year ?? currentDate.Year;

            var startDate = new DateTime(viewYear, viewMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transportes = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .Where(t => t.Fecha.Date >= startDate.Date && t.Fecha.Date <= endDate.Date)
                .OrderBy(t => t.Fecha)
                .ToListAsync();

            ViewBag.CurrentMonth = startDate.ToString("MMMM yyyy");
            ViewBag.PreviousMonth = startDate.AddMonths(-1);
            ViewBag.NextMonth = startDate.AddMonths(1);

            return View(transportes);
        }

        // GET: TransportePaquete/Reporte
        public async Task<IActionResult> Reporte(DateTime? fechaInicio, DateTime? fechaFin)
        {
            fechaInicio ??= DateTime.Now.AddMonths(-1);
            fechaFin ??= DateTime.Now;

            var transportes = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Include(t => t.Conductor)
                .Where(t => t.Fecha.Date >= fechaInicio.Value.Date && t.Fecha.Date <= fechaFin.Value.Date)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            ViewBag.FechaInicio = fechaInicio.Value.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalTransportes = transportes.Count;

            // Estadísticas por conductor
            var estadisticas = transportes
                .GroupBy(t => t.Conductor)
                .Select(g => new ReporteConductor
                {
                    Conductor = g.Key?.Nombre ?? "Sin conductor",
                    Licencia = g.Key?.Licencia ?? "N/A",
                    Zona = g.Key?.Zona ?? "N/A",
                    TotalAsignaciones = g.Count(),
                    PrimeraAsignacion = g.Min(t => t.Fecha),
                    UltimaAsignacion = g.Max(t => t.Fecha)
                })
                .OrderByDescending(x => x.TotalAsignaciones)
                .ToList();

            ViewBag.Estadisticas = estadisticas;

            return View(transportes);
        }

        // GET: TransportePaquete/AsignacionRapida
        public async Task<IActionResult> AsignacionRapida()
        {
            ViewBag.Conductores = new SelectList(
                await _context.Conductores.OrderBy(c => c.Nombre).ToListAsync(),
                "ConductorId", "Nombre");

            ViewBag.Paquetes = new SelectList(
                await _context.PaquetesTuristicos.OrderBy(p => p.Nombre).ToListAsync(),
                "Id", "Nombre");

            return View();
        }

        // POST: TransportePaquete/AsignacionRapida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignacionRapida(int conductorId, int paqueteId, DateTime fecha, string actividad)
        {
            if (ModelState.IsValid)
            {
                // Validar que no haya conflicto de fecha con el mismo conductor
                bool conductorOcupado = await _context.TransportePaquetes
                    .AnyAsync(t => t.ConductorId == conductorId
                                && t.Fecha.Date == fecha.Date);

                if (conductorOcupado)
                {
                    TempData["ErrorMessage"] = "El conductor ya tiene una asignación para esta fecha";
                    return RedirectToAction(nameof(AsignacionRapida));
                }

                var transporte = new TransportePaquete
                {
                    ConductorId = conductorId,
                    PaqueteId = paqueteId,
                    Fecha = fecha,
                    Actividad = actividad
                };

                _context.TransportePaquetes.Add(transporte);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Transporte asignado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: TransportePaquete/TransportePorConductor
        public async Task<IActionResult> TransportePorConductor(int conductorId)
        {
            var conductor = await _context.Conductores.FindAsync(conductorId);
            if (conductor == null)
            {
                return NotFound();
            }

            var transportes = await _context.TransportePaquetes
                .Include(t => t.Paquete)
                .Where(t => t.ConductorId == conductorId)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            ViewBag.Conductor = conductor;
            return View(transportes);
        }

        // ============ MÉTODOS PRIVADOS ============

        private bool TransportePaqueteExists(int id)
        {
            return _context.TransportePaquetes.Any(e => e.Id == id);
        }

        private void CargarListasParaFormulario()
        {
            ViewBag.ConductoresList = _context.Conductores
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.ConductorId.ToString(),
                    Text = $"{c.Nombre} - {c.Licencia} ({c.Zona})"
                })
                .ToList();

            ViewBag.PaquetesList = _context.PaquetesTuristicos
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToList();
        }

        private async Task CargarListasParaFiltros()
        {
            ViewBag.Conductores = await _context.Conductores
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.Paquetes = await _context.PaquetesTuristicos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }

    // Clase para el reporte de conductores
    public class ReporteConductor
    {
        public string Conductor { get; set; }
        public string Licencia { get; set; }
        public string Zona { get; set; }
        public int TotalAsignaciones { get; set; }
        public DateTime PrimeraAsignacion { get; set; }
        public DateTime UltimaAsignacion { get; set; }
    }
}