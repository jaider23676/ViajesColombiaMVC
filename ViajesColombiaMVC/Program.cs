using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ViajesColombiaMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// CONFIGURACIÓN DE SERVICIOS
// =====================================
builder.Services.AddControllersWithViews();

builder.Services.Configure<ViajesColombiaMVC.Configuraciones.ViajesColombiaConfig>(
    builder.Configuration.GetSection("ViajesColombiaConfig"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<ViajesColombiaMVC.Configuraciones.ViajesColombiaConfig>>().Value);

// Sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ⬅️ NECESARIO para evitar: InvalidOperationException: No service for type 'IHttpContextAccessor'
builder.Services.AddHttpContextAccessor();

// **CONFIGURACIÓN DE AUTENTICACIÓN CON COOKIES**
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Ruta a tu página de login
        options.LogoutPath = "/Home/Logout"; // Ruta a tu logout
        options.AccessDeniedPath = "/Home/AccessDenied"; // Ruta cuando deniegas acceso
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Tiempo de expiración
        options.SlidingExpiration = true; // Renovar cookie al usar
    });

// **CONFIGURACIÓN DE AUTORIZACIÓN**
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
        policy.RequireRole("Administrador"));

    options.AddPolicy("Cliente", policy =>
        policy.RequireRole("Cliente"));

    // Si quieres que ciertos endpoints sean accesibles sin autenticación
    options.FallbackPolicy = null; // O puedes definir una política por defecto
});

// Conexion MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 30))
    )
);

// =====================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// =====================================
var app = builder.Build();

// =====================================
// PIPELINE HTTP
// =====================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **IMPORTANTE: UseSession debe ir antes de UseAuthentication**
app.UseSession();

// **ORDEN CRÍTICO: Authentication antes de Authorization**
app.UseAuthentication(); // <-- ¡ESTA LÍNEA ES LA QUE TE FALTABA!
app.UseAuthorization();

// =====================================
// RUTAS PERSONALIZADAS
// =====================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "PermisoSingular",
    pattern: "Permiso/{action=Index}/{id?}",
    defaults: new { controller = "Permisos" }
);

app.MapControllerRoute(
    name: "RolSingular",
    pattern: "Rol/{action=Index}/{id?}",
    defaults: new { controller = "Roles" }
);

app.MapControllerRoute(
    name: "UsuarioSingular",
    pattern: "Usuario/{action=Index}/{id?}",
    defaults: new { controller = "Usuarios" }
);

app.MapControllerRoute(
    name: "PaqueteTuristicoSingular",
    pattern: "PaqueteTuristico/{action=Index}/{id?}",
    defaults: new { controller = "Paquetes" }
);

app.MapControllerRoute(
    name: "ReservaSingular",
    pattern: "Reserva/{action=Index}/{id?}",
    defaults: new { controller = "Reservas" }
);

app.MapControllerRoute(
    name: "admin_dashboard",
    pattern: "Admin/Dashboard",
    defaults: new { controller = "AdminDashboard", action = "Index" }
);

// =====================================
// EJECUTAR
// =====================================
app.Run();
