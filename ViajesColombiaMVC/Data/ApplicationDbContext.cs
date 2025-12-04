using Microsoft.EntityFrameworkCore;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // =======================
        // TABLAS PRINCIPALES
        // =======================
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolesPermisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<PaqueteTuristico> PaquetesTuristicos { get; set; }
        public DbSet<Itinerario> Itinerarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Acceso> Accesos { get; set; }
       

        // =======================
        // CRM / Conductores
        // =======================
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<AsignacionConductor> AsignacionesConductores { get; set; }
        public DbSet<CalificacionConductor> CalificacionesConductores { get; set; }

        // =======================
        // Proveedores
        // =======================
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Contrato> Contratos { get; set; }

        // =======================
        // Transporte
        // =======================
        public DbSet<TransportePaquete> TransportePaquetes { get; set; }

        // =======================
        // Pagos
        // =======================
        public DbSet<FormaPago> FormasPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================
            // ROLES - PERMISOS
            // =============================
            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Rol)
                .WithMany(r => r.RolesPermisos)
                .HasForeignKey(rp => rp.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Permiso)
                .WithMany()
                .HasForeignKey(rp => rp.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);

            // =============================
            // USUARIOS → ROLES
            // =============================
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            // =============================
            // PAQUETES → DESTINOS
            // =============================
            modelBuilder.Entity<PaqueteTuristico>()
                .HasOne(p => p.Destino)
                .WithMany(d => d.Paquetes)
                .HasForeignKey(p => p.DestinoId);

            // =============================
            // ITINERARIOS → PAQUETES
            // =============================
            modelBuilder.Entity<Itinerario>(entity =>
            {
                entity.ToTable("itinerarios");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.PaqueteId).HasColumnName("paquete_id");
                entity.Property(i => i.Dia).HasColumnName("dia");
                entity.Property(i => i.Titulo).HasColumnName("titulo");
                entity.Property(i => i.Descripcion).HasColumnName("descripcion");
                entity.Property(i => i.HoraInicio).HasColumnName("hora_inicio");
                entity.Property(i => i.HoraFin).HasColumnName("hora_fin");

                entity.HasOne(i => i.Paquete)
                      .WithMany(p => p.Itinerarios)
                      .HasForeignKey(i => i.PaqueteId);
            });

            // =============================
            // RESERVAS → USUARIOS & PAQUETES
            // =============================
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioId);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Paquete)
                .WithMany(p => p.Reservas) // si quieres mostrar itinerarios en paquete
                .HasForeignKey(r => r.PaqueteId);

            // =============================
            // PAGOS → RESERVAS & FORMAS DE PAGO
            // =============================
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Reserva)
                .WithMany(r => r.Pagos)
                .HasForeignKey(p => p.ReservaId);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.FormaPago)
                .WithMany(f => f.Pagos)
                .HasForeignKey(p => p.FormaPagoId);

            // =============================
            // COMPROBANTES → PAGOS
            // =============================
            modelBuilder.Entity<Comprobante>()
                .HasOne(c => c.Pago)
                .WithMany(p => p.Comprobantes)
                .HasForeignKey(c => c.PagoId)
                .OnDelete(DeleteBehavior.Cascade);

            // =============================
            // CONDUCTORES
            // =============================
            modelBuilder.Entity<Conductor>(entity =>
            {
                entity.ToTable("conductores");
                entity.HasKey(e => e.ConductorId);
                entity.Property(e => e.Nombre).IsRequired();
                entity.Property(e => e.Licencia);
                entity.Property(e => e.Zona);
                entity.Property(e => e.Especialidad);
                entity.Property(e => e.Telefono);
                entity.Property(e => e.Correo);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // =============================
            // ASIGNACIONES → CONDUCTORES & USUARIOS
            // =============================
            modelBuilder.Entity<AsignacionConductor>(entity =>
            {
                entity.ToTable("asignaciones_conductores");
                entity.HasOne(a => a.Conductor)
                      .WithMany(c => c.Asignaciones)
                      .HasForeignKey(a => a.ConductorId);

                entity.HasOne(a => a.Usuario)
                      .WithMany(u => u.Asignaciones)
                      .HasForeignKey(a => a.UsuarioId);
            });

            // =============================
            // CALIFICACIONES → CONDUCTORES & USUARIOS
            // =============================
            modelBuilder.Entity<CalificacionConductor>(entity =>
            {
                entity.ToTable("calificaciones_conductores");
                entity.HasOne(c => c.Conductor)
                      .WithMany(co => co.Calificaciones)
                      .HasForeignKey(c => c.ConductorId);

                entity.HasOne(c => c.Usuario)
                      .WithMany(u => u.Calificaciones)
                      .HasForeignKey(c => c.UsuarioId);
            });

            // =============================
            // PROVEEDORES → CONTRATOS
            // =============================
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Proveedor)
                .WithMany(p => p.Contratos)
                .HasForeignKey(c => c.ProveedorId);

            // =============================
            // TRANSPORTE PAQUETE → PAQUETE & CONDUCTOR
            // =============================
            modelBuilder.Entity<TransportePaquete>()
                .HasOne(tp => tp.Paquete)
                .WithMany(p => p.Transporte)
                .HasForeignKey(tp => tp.PaqueteId);

            modelBuilder.Entity<TransportePaquete>()
                .HasOne(tp => tp.Conductor)
                .WithMany(c => c.Transporte)
                .HasForeignKey(tp => tp.ConductorId);
        }
    }
}
