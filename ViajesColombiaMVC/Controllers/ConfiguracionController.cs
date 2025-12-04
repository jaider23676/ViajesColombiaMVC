using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesColombiaMVC.Configuraciones;

namespace ViajesColombiaMVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguration _configuration;

        public ConfiguracionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            try
            {
                // Cargar configuración directamente desde appsettings.json
                var config = _configuration.GetSection("ViajesColombiaConfig").Get<ViajesColombiaConfig>();

                // Si no existe en appsettings.json, crear nueva
                if (config == null)
                {
                    config = new ViajesColombiaConfig();
                    ViewBag.Mensaje = "Usando configuración por defecto";
                }

                // Asegurar que ninguna propiedad sea null
                config.Empresa ??= new EmpresaConfig();
                config.Reservas ??= new ReservasConfig();
                config.Pagos ??= new PagosConfig();
                config.Notificaciones ??= new NotificacionesConfig();
                config.Seguridad ??= new SeguridadConfig();
                config.Email ??= new EmailConfig();

                return View(config);
            }
            catch (Exception ex)
            {
                // Si hay error, usar configuración por defecto
                var defaultConfig = new ViajesColombiaConfig
                {
                    Empresa = new EmpresaConfig(),
                    Reservas = new ReservasConfig(),
                    Pagos = new PagosConfig(),
                    Notificaciones = new NotificacionesConfig(),
                    Seguridad = new SeguridadConfig(),
                    Email = new EmailConfig()
                };

                ViewBag.Error = $"Error cargando configuración: {ex.Message}";
                return View(defaultConfig);
            }
        }

        [HttpPost]
        public IActionResult Guardar(string section)
        {
            TempData["Mensaje"] = "Para cambios permanentes, edita el archivo appsettings.json";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Restablecer(string section)
        {
            TempData["Mensaje"] = "Los valores por defecto se encuentran en el código fuente";
            return RedirectToAction("Index");
        }
    }
}