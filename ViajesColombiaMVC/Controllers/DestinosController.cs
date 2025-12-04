using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;

namespace ViajesColombiaMVC.Controllers
{
    public class DestinosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DestinosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===============================
        // 1️⃣ LISTADO DE DESTINOS
        // ===============================
        public async Task<IActionResult> Index()
        {
            var destinos = await _context.Destinos
                .Include(d => d.Paquetes) // ⚡ Incluye los paquetes
                .ToListAsync();
            return View(destinos);
        }

        // ===============================
        // 2️⃣ DETALLES DEL DESTINO
        // ===============================
        public async Task<IActionResult> Details(int id)
        {
            var destino = await _context.Destinos
                .Include(d => d.Paquetes)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (destino == null)
                return NotFound();

            return View(destino);
        }

        // ===============================
        // 3️⃣ CREAR DESTINO
        // ===============================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Destino destino)
        {
            if (destino.ImagenFile != null && destino.ImagenFile.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(destino.ImagenFile.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await destino.ImagenFile.CopyToAsync(stream);
                }

                destino.Imagen = fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Destinos.Add(destino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(destino);
        }

        // ===============================
        // 4️⃣ EDITAR DESTINO
        // ===============================
        public async Task<IActionResult> Edit(int id)
        {
            var destino = await _context.Destinos.FindAsync(id);
            if (destino == null)
                return NotFound();

            return View(destino);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Destino destino)
        {
            if (destino.ImagenFile != null && destino.ImagenFile.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(destino.ImagenFile.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await destino.ImagenFile.CopyToAsync(stream);
                }

                // Eliminar imagen anterior
                if (!string.IsNullOrEmpty(destino.Imagen))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, "images", destino.Imagen);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                destino.Imagen = fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Destinos.Update(destino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(destino);
        }

        // ===============================
        // 5️⃣ ELIMINAR DESTINO
        // ===============================
        public async Task<IActionResult> Delete(int id)
        {
            var destino = await _context.Destinos.FindAsync(id);
            if (destino == null)
                return NotFound();

            return View(destino);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destino = await _context.Destinos.FindAsync(id);

            if (destino != null)
            {
                // Eliminar imagen física si existe
                if (!string.IsNullOrEmpty(destino.Imagen))
                {
                    string path = Path.Combine(_env.WebRootPath, "images", destino.Imagen);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.Destinos.Remove(destino);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // 6️⃣ VER PAQUETES DE UN DESTINO (opcional)
        // ===============================
        public async Task<IActionResult> Paquetes(int id)
        {
            var destino = await _context.Destinos
                .Include(d => d.Paquetes)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (destino == null)
                return NotFound();

            return View(destino);
        }
    }
}
