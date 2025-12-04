using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("roles_permisos")]
    public class RolPermiso
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        [Column("permiso_id")]
        public int PermisoId { get; set; }

        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; }
    }
}
