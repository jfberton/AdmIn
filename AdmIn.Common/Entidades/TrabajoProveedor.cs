using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class TrabajoProveedor
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public int? ProveedorId { get; set; } // made nullable
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public decimal Costo { get; set; }
        public int? Contratoid { get; set; }
        public string? FacturaURL { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreador { get; set; }
        public DateTime? FechaInicio { get; set; }
        public decimal? CostoAproximado { get; set; }
        public List<Imagen> Imagenes { get; set; } = new();
    }
}
