using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ContratosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContratosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTA DE CONTRATOS POR PROVEEDOR
        public async Task<IActionResult> Index(int? proveedorId)
        {
            if (proveedorId == null)
                return Content("Debes seleccionar un proveedor primero.");

            var proveedor = await _context.Proveedores.FindAsync(proveedorId);

            if (proveedor == null)
                return Content($"No se encontró el proveedor con Id = {proveedorId}");

            var lista = await _context.Contratos
                .Include(c => c.Proveedor)
                .Where(c => c.ProveedorId == proveedorId)
                .ToListAsync();

            ViewBag.ProveedorId = proveedor.Id;
            ViewBag.ProveedorNombre = proveedor.Nombre;

            return View(lista);
        }

        // CREAR CONTRATO - GET
        public IActionResult Create(int proveedorId)
        {
            ViewBag.ProveedorId = proveedorId;

            var contrato = new Contrato
            {
                ProveedorId = proveedorId
            };

            return View(contrato);
        }

        // CREAR CONTRATO - POST
        [HttpPost]
        public async Task<IActionResult> Create(Contrato obj)
        {
            if (obj.ProveedorId == 0)
            {
                ModelState.AddModelError("ProveedorId", "Debes seleccionar un proveedor.");
            }

            if (ModelState.IsValid)
            {
                _context.Contratos.Add(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { proveedorId = obj.ProveedorId });
            }

            ViewBag.ProveedorId = obj.ProveedorId;
            return View(obj);
        }

        // EDITAR CONTRATO - GET
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Contratos.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // EDITAR CONTRATO - POST
        [HttpPost]
        public async Task<IActionResult> Edit(Contrato obj)
        {
            if (ModelState.IsValid)
            {
                _context.Contratos.Update(obj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { proveedorId = obj.ProveedorId });
            }

            return View(obj);
        }

        // ELIMINAR CONTRATO
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Contratos.FindAsync(id);
            if (item == null) return NotFound();

            int proveedorId = item.ProveedorId;

            _context.Contratos.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { proveedorId });
        }
    }
}

