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
    }
}
