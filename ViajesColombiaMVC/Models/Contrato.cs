using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("contratos")]
    public class Contrato
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // FK hacia proveedores
        [Column("proveedor_id")]
        public int ProveedorId { get; set; }

        // No debe ser obligatorio, NO poner [Required]
        public Proveedor? Proveedor { get; set; }

        [Column("fecha_inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("valor")]
        public decimal Valor { get; set; }

        // La tabla tiene este nombre: ArchivoPdf (respetar mayúsculas)
        [Column("ArchivoPdf")]
        public string? ArchivoPdf { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.Now;
    }
}
