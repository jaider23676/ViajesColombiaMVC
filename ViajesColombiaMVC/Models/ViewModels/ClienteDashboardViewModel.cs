namespace ViajesColombiaMVC.Models.ViewModels
{
    public class ClienteDashboardViewModel
    {
        public Usuario Usuario { get; set; }

        public int TotalReservas { get; set; }
        public List<Reserva> Reservas { get; set; }
        public List<AsignacionConductor> Asignaciones { get; set; }
        public int TotalConductores { get; set; }
        public List<Acceso> UltimosAccesos { get; set; }




        public List<AsignacionConductor> ConductoresAsignados { get; set; }
    }
}
