using System.Collections.Generic;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Models.ViewModels
{
    public class AdminPagosViewModel
    {
        public int TotalPagos { get; set; }
        public int PagosCompletados { get; set; }
        public int PagosPendientes { get; set; }
        public int PagosFallidos { get; set; }

        public List<Pago> UltimosPagos { get; set; }

        public List<string> Meses { get; set; }
        public List<int> Completados { get; set; }
        public List<int> Pendientes { get; set; }
        public List<int> Fallidos { get; set; }
    }
}
