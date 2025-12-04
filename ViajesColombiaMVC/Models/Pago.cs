using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("pagos")]
    public class Pago
    {
        public int Id { get; set; }

        [Column("reserva_id")]
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        [Column("forma_pago_id")]
        public int? FormaPagoId { get; set; }
        public FormaPago FormaPago { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("transaccion_id")]
        public string? TransaccionId { get; set; }

        public ICollection<Comprobante> Comprobantes { get; set; }
    }
}

