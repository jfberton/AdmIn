using System;

namespace AdmIn.Data.Entidades
{
    public class T_Auditoria_Inmueble
    {
        public int AuditoriaInmuebleID { get; set; }
        public int? InmuebleID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public int? UsuarioEditorID { get; set; }
    }
}