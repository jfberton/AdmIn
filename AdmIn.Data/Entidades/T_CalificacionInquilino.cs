using System;

namespace AdmIn.Data.Entidades
{
    public class T_CalificacionInquilino
    {
        public int CalificacionInquilinoID { get; set; }
        public int ContratoID { get; set; }
        public int CalificadorID { get; set; }
        public string TipoCalificador { get; set; } = string.Empty;
        public int? Calificacion { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}