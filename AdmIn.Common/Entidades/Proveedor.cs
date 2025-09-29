using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioModificadorId { get; set; }

        // Propiedades calculadas (no almacenadas en la base de datos): Calificación promedio y trabajos realizados
        public decimal CalificacionPromedio { get; set; } = 0m;
        public int TrabajosRealizados { get; set; } = 0;
    }
}
