using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("vehiculos")]
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Placa { get; set; }

        [MaxLength(50)]
        public string Marca { get; set; }

        [MaxLength(50)]
        public string Modelo { get; set; }

        [MaxLength(50)]
        public string Tipo { get; set; } = "Van";

        [MaxLength(30)]
        public string Color { get; set; }

        public int Capacidad { get; set; } = 12;
        
        public int año { get; set; } = DateTime.Now.Year;
        public string? Foto { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}