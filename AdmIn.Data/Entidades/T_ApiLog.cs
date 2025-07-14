using System;

namespace AdmIn.Data.Entidades
{
    public class T_ApiLog
    {
        public int ApiLogID { get; set; }
        public int? UsuarioID { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}