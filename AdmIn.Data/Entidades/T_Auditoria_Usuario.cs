using System;

namespace AdmIn.Data.Entidades
{
    public class T_Auditoria_Usuario
    {
        public int AuditoriaUsuarioID { get; set; }
        public int? UsuarioID { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Accion { get; set; } = string.Empty;
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public int? UsuarioEditorID { get; set; }
    }
}