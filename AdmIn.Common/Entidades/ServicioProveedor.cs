using System;
using System.ComponentModel.DataAnnotations;

namespace AdmIn.Common.Entidades
{
    public class ServicioProveedor
    {
        public int Id { get; set; }
        
        [Required]
        public int ProveedorId { get; set; }
        
        [Required]
        public int ServicioId { get; set; } // Mantener nombre original por compatibilidad, pero mapear a TipoServicioId en repositorio
        
        // Campos adicionales para funcionalidad completa
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
        public int? UsuarioCreadorId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? UsuarioModificadorId { get; set; }
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        #region Propiedades de navegación
        
        /// <summary>
        /// Proveedor asociado
        /// </summary>
        public Proveedor? Proveedor { get; set; }
        
        /// <summary>
        /// Tipo de servicio asociado
        /// </summary>
        public TipoServicio? TipoServicio { get; set; }
        
        #endregion
    }
}
