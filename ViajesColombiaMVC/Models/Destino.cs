using Microsoft.AspNetCore.Http; // Para IFormFile
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    public class Destino
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public string Imagen { get; set; }

        [NotMapped]
        public IFormFile ImagenFile { get; set; }

        // ⬅ ESTA PROPIEDAD ERA OBLIGATORIA
        public ICollection<PaqueteTuristico> Paquetes { get; set; } = new List<PaqueteTuristico>();

    }
}
