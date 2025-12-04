using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("transporte_paquete")]
    public class TransportePaquete
    {
        public int Id { get; set; }
        [Column("paquete_id")]
        public int PaqueteId { get; set; }
        public PaqueteTuristico Paquete { get; set; }
        [Column("conductor_id")]
        public int ConductorId { get; set; }
        public Conductor Conductor { get; set; }

        public DateTime Fecha { get; set; }
        public string Actividad { get; set; }
       
    }
}
