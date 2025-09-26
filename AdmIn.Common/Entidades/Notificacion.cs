using System;

namespace AdmIn.Common.Entidades
{
    public class Notificacion
    {
        public int NotificacionId { get; set; }
        public int UsuarioId { get; set; } // destinatario
        public string Tipo { get; set; } = string.Empty; // ejemplo: "TrabajoSeleccionado", "TrabajoFinalizado", "VencimientoContrato"
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Payload { get; set; } // json libre para datos adicionales (ej: TrabajoId)
    }
}
