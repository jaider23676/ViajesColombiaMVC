using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViajesColombiaMVC.Models
{
    [Table("calificaciones_conductores")]
    public class CalificacionConductor
    {
        public int Id { get; set; }

        // ⬇⬇⬇ MAPEADO EXACTO A LA COLUMNA REAL DE MYSQL
        [Column("ConductorId")]
        public int ConductorId { get; set; }
        public Conductor Conductor { get; set; }

        // ⬇ asegúrate de que en la BD se llame igual
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int Puntaje { get; set; }
        public string Comentario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }
    }
}
