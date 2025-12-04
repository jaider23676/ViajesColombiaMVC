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

        [Column("vehiculo_id")]
        public int? VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }

        [Column("reserva_id")]
        public int? ReservaId { get; set; }
        public Reserva Reserva { get; set; }

        public DateTime Fecha { get; set; }
        public string Actividad { get; set; }
       
    }
}
