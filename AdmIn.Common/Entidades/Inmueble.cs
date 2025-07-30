using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Inmueble
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal Valor { get; set; }
        public decimal ConstruccuionM2 { get; set; }
        public decimal RentaMensual { get; set; }
        public int? AdministradorId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public Guid? ImagenPrincipalId { get; set; }
        public int MonedaId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioModificadorId { get; set; }

        #region Propiedades de navegación
        public Usuario? UsuarioCreador { get; set; }
        public Usuario? UsuarioModificador { get; set; }
        public Usuario? Administrador { get; set; }
        public Moneda? Moneda { get; set; }
        public Imagen? ImagenPrincipal { get; set; }

        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();
        public List<CaracteristicaInmueble> Caracteristicas { get; set; } = new List<CaracteristicaInmueble>();
        public List<ContratoRenta> Contratos { get; set; } = new List<ContratoRenta>();

        #endregion
    }
}
