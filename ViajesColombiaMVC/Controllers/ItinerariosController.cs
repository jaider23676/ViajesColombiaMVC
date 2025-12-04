using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ItinerariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItinerariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR ITINERARIOS POR PAQUETE
        public async Task<IActionResult> Index(int paqueteId)
        {
            var lista = await _context.Itinerarios
                .Where(i => i.PaqueteId == paqueteId)
                .OrderBy(i => i.Dia)
                .ToListAsync();

            ViewBag.PaqueteId = paqueteId;
            return View(lista);
        }

        // CREAR ITINERARIO - GET
        public IActionResult Create(int paqueteId)
        {
            var modelo = new Itinerario
            {
                PaqueteId = paqueteId
            };

            ViewBag.PaqueteId = paqueteId;
            return View(modelo);
        }

        // CREAR ITINERARIO - POST
        [HttpPost]
        public async Task<IActionResult> Create(Itinerario modelo)
        {
            // Validación manual opcional
            if (modelo.PaqueteId == 0)
            {
                ModelState.AddModelError("PaqueteId", "El paquete es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PaqueteId = modelo.PaqueteId; // ← Muy importante
                return View(modelo);
            }

            _context.Itinerarios.Add(modelo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { paqueteId = modelo.PaqueteId });
        }

        // ELIMINAR ITINERARIO
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Itinerarios.FindAsync(id);
            if (item == null) return NotFound();

            int paqueteId = item.PaqueteId;
            _context.Itinerarios.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { paqueteId });
        }
    }
}

