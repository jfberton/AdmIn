using System;

namespace AdmIn.Common.Entidades
{
    public class InmuebleCondicion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#6b7280"; // Color para badges/indicadores
        public bool Activo { get; set; } = true;
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioModificadorId { get; set; }

        #region Propiedades de navegación
        public Usuario? UsuarioCreador { get; set; }
        public Usuario? UsuarioModificador { get; set; }
        #endregion
    }
}