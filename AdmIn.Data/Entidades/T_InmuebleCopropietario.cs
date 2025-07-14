using System;

namespace AdmIn.Data.Entidades
{
    public class T_InmuebleCopropietario
    {
        public int InmuebleCopropietarioID { get; set; }
        public int InmuebleID { get; set; }
        public int? PersonaID { get; set; }
        public int? EmpresaID { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorID { get; set; }
    }
}