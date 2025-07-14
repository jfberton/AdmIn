using System;

namespace AdmIn.Data.Entidades
{
    public class T_Persona
    {
        public int PersonaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}