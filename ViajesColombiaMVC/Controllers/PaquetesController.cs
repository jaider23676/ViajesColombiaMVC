using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace ViajesColombiaMVC.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PaquetesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Paquetes
        public async Task<IActionResult> Index()
        {
            var paquetes = await _context.PaquetesTuristicos
                .Include(p => p.Destino)
                .ToListAsync();
            return View(paquetes);
        }

        // GET: /Paquetes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var paquete = await _context.PaquetesTuristicos
                .Include(p => p.Destino)
                .Include(p => p.Itinerarios)
                .Include(p => p.Transporte)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paquete == null)
                return NotFound();

            return View(paquete);
        }

        // GET: /Paquetes/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Destinos"] = new SelectList(await _context.Destinos.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: /Paquetes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaqueteTuristico paquete)
        {
            if (ModelState.IsValid)
            {
                // GUARDAR IMAGEN
                if (paquete.ImagenFile != null)
                {
                    string carpeta = Path.Combine(_env.WebRootPath, "imagenes/paquetes");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(paquete.ImagenFile.FileName);
                    string path = Path.Combine(carpeta, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await paquete.ImagenFile.CopyToAsync(stream);
                    }

                    paquete.Imagen = fileName;
                }

                _context.PaquetesTuristicos.Add(paquete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Destinos"] = new SelectList(await _context.Destinos.ToListAsync(), "Id", "Nombre", paquete.DestinoId);
            return View(paquete);
        }

        // GET: /Paquetes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var paquete = await _context.PaquetesTuristicos.FindAsync(id);
            if (paquete == null) return NotFound();

            ViewData["Destinos"] = new SelectList(await _context.Destinos.ToListAsync(), "Id", "Nombre", paquete.DestinoId);
            return View(paquete);
        }

        // POST: /Paquetes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaqueteTuristico paquete)
        {
            if (id != paquete.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // OBTENER PAQUETE ORIGINAL
                var paqueteDB = await _context.PaquetesTuristicos.FindAsync(id);

                if (paqueteDB == null)
                    return NotFound();

                // ACTUALIZAR CAMPOS
                paqueteDB.Nombre = paquete.Nombre;
                paqueteDB.Descripcion = paquete.Descripcion;
                paqueteDB.Precio = paquete.Precio;
                paqueteDB.FechaInicio = paquete.FechaInicio;
                paqueteDB.FechaFin = paquete.FechaFin;
                paqueteDB.Cupo = paquete.Cupo;
                paqueteDB.DestinoId = paquete.DestinoId;

                // SI SUBEN NUEVA IMAGEN
                if (paquete.ImagenFile != null)
                {
                    string carpeta = Path.Combine(_env.WebRootPath, "imagenes/paquetes");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    // ELIMINAR IMAGEN ANTERIOR
                    if (!string.IsNullOrEmpty(paqueteDB.Imagen))
                    {
                        string rutaAnterior = Path.Combine(carpeta, paqueteDB.Imagen);
                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    // GUARDAR NUEVA
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(paquete.ImagenFile.FileName);
                    string path = Path.Combine(carpeta, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await paquete.ImagenFile.CopyToAsync(stream);
                    }

                    paqueteDB.Imagen = fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Destinos"] = new SelectList(await _context.Destinos.ToListAsync(), "Id", "Nombre", paquete.DestinoId);
            return View(paquete);
        }

        // GET: /Paquetes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var paquete = await _context.PaquetesTuristicos
                .Include(p => p.Destino)
                .FirstOrDefaultAsync(p => p.Id == id);

            return paquete == null ? NotFound() : View(paquete);
        }

        // POST: /Paquetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paquete = await _context.PaquetesTuristicos.FindAsync(id);
            if (paquete != null)
            {
                // BORRAR IMAGEN DEL SERVIDOR
                if (!string.IsNullOrEmpty(paquete.Imagen))
                {
                    string carpeta = Path.Combine(_env.WebRootPath, "imagenes/paquetes");
                    string path = Path.Combine(carpeta, paquete.Imagen);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.PaquetesTuristicos.Remove(paquete);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

