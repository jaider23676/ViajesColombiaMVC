using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Necesario para IFormFile

namespace ViajesColombiaMVC.Models
{
    [Table("paquetes_turisticos")]
    public class PaqueteTuristico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        public int Cupo { get; set; }

        [Column("destino_id")]
        public int? DestinoId { get; set; }
        public Destino Destino { get; set; }

        public DateTime? CreadoEn { get; set; }

        // Nombre del archivo que se guarda en la base de datos
        [Column("imagen")]
        public string? Imagen { get; set; }

        // Campo NO mapeado: permite subir la imagen
        [NotMapped]
        public IFormFile ImagenFile { get; set; }

        public ICollection<Itinerario> Itinerarios { get; set; }
        public ICollection<TransportePaquete> Transporte { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
