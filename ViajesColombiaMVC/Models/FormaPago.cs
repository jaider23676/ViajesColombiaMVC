using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("formas_pago")]
    public class FormaPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Pago> Pagos { get; set; }

    }
}
