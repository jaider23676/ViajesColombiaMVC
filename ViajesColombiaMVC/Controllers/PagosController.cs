using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;
using ViajesColombiaMVC.Models.ViewModels;

namespace ViajesColombiaMVC.Controllers
{
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================
        // LISTAR PAGOS DEL USUARIO
        // =====================
        public IActionResult Index()
        {
            var usuarioId = int.Parse(HttpContext.Session.GetString("UsuarioId") ?? "0");

            var pagos = _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Paquete)
                .Include(p => p.FormaPago)
                .Include(p => p.Comprobantes)
                .Where(p => p.Reserva.UsuarioId == usuarioId)
                .ToList();

            return View(pagos);
        }

        // =====================
        // CREAR PAGO
        // =====================
        [HttpGet]
        public IActionResult Crear(int reservaId)
        {
            ViewBag.FormasPago = _context.FormasPago
                .Where(f => f.Activo)
                .ToList();

            var pago = new Pago
            {
                ReservaId = reservaId
            };

            return View(pago);
        }

        [HttpPost]
        public IActionResult Crear(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.FechaPago = DateTime.Now;
                pago.Estado = "Pendiente";
                _context.Pagos.Add(pago);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.FormasPago = _context.FormasPago
                .Where(f => f.Activo)
                .ToList();

            return View(pago);
        }

        // =====================
        // DETALLE DE PAGO
        // =====================
        public IActionResult DetallePago(int pagoId)
        {
            var pago = _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Paquete)
                .Include(p => p.FormaPago)
                .Include(p => p.Comprobantes)
                .FirstOrDefault(p => p.Id == pagoId);

            if (pago == null)
                return NotFound();

            return View(pago);
        }

        // =====================
        // SUBIR COMPROBANTE
        // =====================
        [HttpGet]
        public IActionResult SubirComprobante(int pagoId)
        {
            var comprobante = new Comprobante
            {
                PagoId = pagoId
            };
            return View(comprobante);
        }

        [HttpPost]
        public IActionResult SubirComprobante(int pagoId, IFormFile archivo)
        {
            if (archivo != null && archivo.Length > 0)
            {
                var nombreArchivo = Path.GetFileName(archivo.FileName);
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    archivo.CopyTo(stream);
                }

                var comprobante = new Comprobante
                {
                    PagoId = pagoId,
                    Archivo = nombreArchivo,
                    CreadoEn = DateTime.Now
                };

                _context.Comprobantes.Add(comprobante);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Debe seleccionar un archivo.";
            return View(new Comprobante { PagoId = pagoId });
        }

        // =====================
        // VER COMPROBANTES
        // =====================
        public IActionResult VerComprobantes(int pagoId)
        {
            var comprobantes = _context.Comprobantes
                .Where(c => c.PagoId == pagoId)
                .ToList();

            return View(comprobantes);
        }

        // =====================
        // CAMBIAR ESTADO DEL PAGO
        // =====================
        [HttpPost]
        public IActionResult CambiarEstado(int pagoId, string estado)
        {
            var pago = _context.Pagos.Find(pagoId);
            if (pago != null)
            {
                pago.Estado = estado;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // =====================
        // DASHBOARD ADMIN
        // =====================
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new AdminPagosViewModel();

            viewModel.TotalPagos = await _context.Pagos.CountAsync();
            viewModel.PagosCompletados = await _context.Pagos.CountAsync(p => p.Estado == "Completado");
            viewModel.PagosPendientes = await _context.Pagos.CountAsync(p => p.Estado == "Pendiente");
            viewModel.PagosFallidos = await _context.Pagos.CountAsync(p => p.Estado == "Fallido");

            viewModel.UltimosPagos = await _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Usuario)
                .OrderByDescending(p => p.FechaPago)
                .Take(10)
                .ToListAsync();

            viewModel.Meses = new List<string> { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio" };
            viewModel.Completados = new List<int> { 5, 8, 10, 7, 12, 9 };
            viewModel.Pendientes = new List<int> { 2, 1, 3, 0, 1, 2 };
            viewModel.Fallidos = new List<int> { 0, 1, 0, 2, 0, 1 };

            return View(viewModel);
        }
    }
}
