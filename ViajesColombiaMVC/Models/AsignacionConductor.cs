using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("asignaciones_conductores")]
    public class AsignacionConductor
    {
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Column("ConductorId")]
        public int ConductorId { get; set; }
        public Conductor Conductor { get; set; }

        [Column("fecha_asignacion")]
        public DateTime FechaAsignacion { get; set; }

        public string Motivo { get; set; }

        public bool Activo { get; set; } = true;
    }
}
