using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class PagoRenta
    {
        public int Id { get; set; }
        public int ContratoRentaId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public bool EsParcial { get; set; } = false;
        public bool Atraso { get; set; } = false;
        public decimal Penalizacion { get; set; } = 0.0m;
        public string ComprobanteURL { get; set; } = string.Empty;
        public int MonedaId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioModificadorId { get; set; }
    }
}
