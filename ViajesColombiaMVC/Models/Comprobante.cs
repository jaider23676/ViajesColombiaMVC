using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
    
{
    [Table("comprobantes")]
    public class Comprobante
    {
        public int Id { get; set; }
        [Column("pago_id")]
        public int PagoId { get; set; }
        public Pago Pago { get; set; }

        public string Archivo { get; set; }
        [Column("creado_en")]
        public DateTime CreadoEn { get; set; }
    }
}
