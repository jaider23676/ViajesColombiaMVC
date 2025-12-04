using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("conductores")]
    public class Conductor
    {
        [Key]
        [Column("conductor_id")]
        public int ConductorId { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("licencia")]
        public string Licencia { get; set; }

        [Column("zona")]
        public string Zona { get; set; }

        [Column("especialidad")]
        public string Especialidad { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("correo")]
        [EmailAddress]
        public string Correo { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.Now;

        // ====================================
        // NUEVAS PROPIEDADES QUE QUIERES AGREGAR
        // ====================================

        [Column("vehiculo_asignado_id")]
        public int? VehiculoAsignadoId { get; set; }

        [Column("experiencia")]
        public int Experiencia { get; set; } = 0; // años

        [Column("calificacion")]
        [Range(0, 5)]
        public decimal Calificacion { get; set; } = 4.5m;

        [Column("disponible")]
        public bool Disponible { get; set; } = true;

        [Column("foto")]
        public string? Foto { get; set; }

        // ====================================
        // RELACIONES
        // ====================================

        // Relación con Vehículo asignado
        [ForeignKey("VehiculoAsignadoId")]
        public Vehiculo? VehiculoAsignado { get; set; }

        // Relaciones existentes
        public ICollection<AsignacionConductor>? Asignaciones { get; set; }
        public ICollection<CalificacionConductor>? Calificaciones { get; set; }
        public ICollection<TransportePaquete>? Transporte { get; set; }
    }
}