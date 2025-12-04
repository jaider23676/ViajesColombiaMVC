using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("reservas")]
    public class Reserva
    {
        public int Id { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        [Column("paquete_id")]
        public int PaqueteId { get; set; }

        // 🔥 ESTA ES LA PROPIEDAD CORRECTA
        public PaqueteTuristico Paquete { get; set; }
        [Column("fecha_reserva")]
        public DateTime FechaReserva { get; set; }
        [Column("cantidad_personas")]
        public int CantidadPersonas { get; set; }
        public string Estado { get; set; }
        [Column("Total")]
        public decimal Total { get; set; }

        public ICollection<Pago> Pagos { get; set; }
    }
}
