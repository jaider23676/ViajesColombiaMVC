using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTA DE PROVEEDORES
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Proveedores
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            return View(lista);
        }

        // CREAR PROVEEDOR
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Proveedor obj)
        {
            if (ModelState.IsValid)
            {
                obj.CreadoEn = DateTime.Now;
                _context.Proveedores.Add(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // EDITAR PROVEEDOR
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Proveedores.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Proveedor obj)
        {
            if (ModelState.IsValid)
            {
                _context.Proveedores.Update(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // ELIMINAR PROVEEDOR
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores
                .Include(p => p.Contratos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proveedor == null)
                return NotFound();

            if (proveedor.Contratos.Any())
            {
                TempData["Error"] = "No puedes eliminar este proveedor porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

