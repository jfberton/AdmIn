using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class PolizaSeguro
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string NumPoliza { get; set; } = string.Empty;
        public string Aseguradora { get; set; } = string.Empty;
        public DateTime VigenciaInicio { get; set; } = DateTime.Now;
        public DateTime VigenciaFin { get; set; } = DateTime.Now;
        public decimal MontoAsegurado { get; set; } = 0;
        public string ArchivoPolizaUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
    }
}
