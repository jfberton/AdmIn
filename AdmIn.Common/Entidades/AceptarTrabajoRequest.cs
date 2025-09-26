using System;

namespace AdmIn.Common.Entidades
{
    public class AceptarTrabajoRequest
    {
        public int TrabajoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public decimal CostoAproximado { get; set; }
        public int UsuarioId { get; set; }
    }
}
