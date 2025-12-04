using System;
using System.Collections.Generic;

namespace ViajesColombiaMVC.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public DateTime CreadoEn { get; set; }

        // Lista de contratos asociados
        public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
