using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("conductores")]
    public class Conductor
    {
        [Column("ConductorId")]
        public int ConductorId { get; set; }
        public string Nombre { get; set; }
        public string Licencia { get; set; }
        public string Zona { get; set; }
        public string Especialidad { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }

        public ICollection<AsignacionConductor> Asignaciones { get; set; }
        public ICollection<CalificacionConductor> Calificaciones { get; set; }
        public ICollection<TransportePaquete> Transporte { get; set; }
    }
}
