using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }

        public string? Telefono { get; set; }      // Nullable
        public string? Direccion { get; set; }     // Nullable

        [Column("rol_id")]
        public int RolId { get; set; }
        public Rol Rol { get; set; }

        public string? Preferencias { get; set; }  // Nullable
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<Acceso> Accesos { get; set; }
        public ICollection<AsignacionConductor> Asignaciones { get; set; }
        public ICollection<CalificacionConductor> Calificaciones { get; set; }
    }
}
