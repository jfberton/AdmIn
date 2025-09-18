using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class ContratoRenta
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public int InquilinoId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? MontoRenta { get; set; }
        public string Condiciones { get; set; } = string.Empty;
        public decimal? Deposito { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string DocFirmadoUrl { get; set; } = string.Empty;
        public int? PolizaRentaSeguraId { get; set; }
        public int MonedaId { get; set; } = 1;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioModificadorId { get; set; }

        #region Propiedades de navegación
        public Usuario? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }
        public PolizaSeguro? PolizaRentaSegura { get; set; }
        public Moneda? Moneda { get; set; }
        public List<PagoRenta> Pagos { get; set; }

        #endregion

    }
}
