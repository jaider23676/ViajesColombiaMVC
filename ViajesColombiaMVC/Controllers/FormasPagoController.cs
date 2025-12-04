using Microsoft.AspNetCore.Mvc;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class FormasPagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormasPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.FormasPago.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FormaPago obj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            var item = _context.FormasPago.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FormaPago obj)
        {
            if (ModelState.IsValid)
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.FormasPago.FindAsync(id);
            if (item == null) return NotFound();

            _context.FormasPago.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
