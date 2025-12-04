using System.Collections.Generic;
using ViajesColombiaMVC.Models;

namespace ViajesColombiaMVC.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int TotalReservas { get; set; }
        public int TotalPaquetes { get; set; }
        public int TotalConductores { get; set; }
        public int TotalProveedores { get; set; }

        public int Pendientes { get; set; }
        public int Confirmadas { get; set; }
        public int Canceladas { get; set; }

        public decimal TotalPagos { get; set; }  // ← ahora es decimal

        public List<Reserva> ReservasRecientes { get; set; }
    }
}

