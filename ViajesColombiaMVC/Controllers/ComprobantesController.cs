using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Controllers
{
    public class ComprobantesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComprobantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pagoId)
        {
            var lista = await _context.Comprobantes
                .Include(c => c.Pago)
                .Where(c => c.PagoId == pagoId)
                .ToListAsync();

            ViewBag.PagoId = pagoId;
            return View(lista);
        }

        public IActionResult Upload(int pagoId)
        {
            ViewBag.PagoId = pagoId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int pagoId, IFormFile archivo)
        {
            if (archivo != null && archivo.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
                string path = Path.Combine("wwwroot/comprobantes", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                Comprobante c = new()
                {
                    PagoId = pagoId,
                    Archivo = fileName,
                    CreadoEn = DateTime.Now
                };

                _context.Add(c);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { pagoId });
            }

            return View();
        }
    }
}
