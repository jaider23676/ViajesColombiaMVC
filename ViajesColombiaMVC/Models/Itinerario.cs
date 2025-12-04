using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("itinerarios")]
    public class Itinerario
    {
        public int Id { get; set; }

        [Required]
        public int PaqueteId { get; set; }
        public PaqueteTuristico? Paquete { get; set; }

        [Required]
        public int Dia { get; set; }

        [Required]
        public string? Titulo { get; set; }

        [Required]
        public string? Descripcion { get; set; }

        [Column("hora_inicio")]
        public TimeSpan? HoraInicio { get; set; }

        [Column("hora_fin")]
        public TimeSpan? HoraFin { get; set; }
    }
}
