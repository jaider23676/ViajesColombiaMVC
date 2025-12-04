namespace ViajesColombiaMVC.Configuraciones
{
    public class ViajesColombiaConfig
    {
        public EmpresaConfig Empresa { get; set; } = new();
        public ReservasConfig Reservas { get; set; } = new();
        public PagosConfig Pagos { get; set; } = new();
        public NotificacionesConfig Notificaciones { get; set; } = new();
        public SeguridadConfig Seguridad { get; set; } = new();
        public EmailConfig Email { get; set; } = new();
    }

    public class EmpresaConfig
    {
        public string Nombre { get; set; } = "Viajes Colombia";
        public string Slogan { get; set; } = "Tu aventura comienza aquí";
        public string Email { get; set; } = "info@viajescolombia.com";
        public string Telefono { get; set; } = "+57 1 1234567";
        public string Direccion { get; set; } = "Calle 123 #45-67, Bogotá";
        public string Moneda { get; set; } = "COP";
        public string Idioma { get; set; } = "es";
        public string ZonaHoraria { get; set; } = "America/Bogota";
    }

    public class ReservasConfig
    {
        public int AnticipacionMinimaHoras { get; set; } = 24;
        public bool CancelacionGratuita { get; set; } = true;
        public int LimiteReservasPorUsuario { get; set; } = 5;
        public int DiasCancelacion { get; set; } = 3;
        public bool ConfirmacionAutomatica { get; set; } = false;
    }

    public class PagosConfig
    {
        public int PorcentajeDeposito { get; set; } = 30;
        public int DiasPagoCompleto { get; set; } = 7;
        public string MetodosPago { get; set; } = "Transferencia,Tarjeta,Efectivo";
        public int Iva { get; set; } = 19;
        public int ComisionReserva { get; set; } = 5;
    }

    public class NotificacionesConfig
    {
        public bool EmailReservas { get; set; } = true;
        public bool EmailMarketing { get; set; } = true;
        public bool EmailContacto { get; set; } = true;
        public bool Whatsapp { get; set; } = true;
        public bool Push { get; set; } = false;
    }

    public class SeguridadConfig
    {
        public int IntentosLogin { get; set; } = 3;
        public int BloqueoTemporalMinutos { get; set; } = 15;
        public bool RequerirVerificacionEmail { get; set; } = false;
        public bool HttpsObligatorio { get; set; } = true;
    }

    public class EmailConfig
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPuerto { get; set; } = 587;
        public string SmtpUsuario { get; set; } = "notificaciones@viajescolombia.com";
        public bool SmtpSsl { get; set; } = true;
        public string RemitenteNombre { get; set; } = "Viajes Colombia";
    }
}