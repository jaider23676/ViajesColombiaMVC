using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("accesos")]
    public class Acceso
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Column("fecha_acceso")]
        public DateTime FechaAcceso { get; set; } = DateTime.Now;

        [Column("ip")]
        public string Ip { get; set; }

        [Column("exito")]
        public bool Exito { get; set; }
    }
}

