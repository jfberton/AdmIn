using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Reparacion
    {
        public int Id { get; set; }
        public ReparacionCategoria Categoria { get; set; } // Categoría de la reparación (Ej: Plomería, Electricidad, etc.)

        // Fecha en la que se solicita la reparación
        public DateTime FechaSolicitud { get; set; }

        // Fecha en la que inicia la reparación (puede ser null hasta que se asigne)
        public DateTime? FechaInicio { get; set; }

        // Fecha de finalización (puede ser null si aún no finalizó)
        public DateTime? FechaFin { get; set; }

        public string Descripcion { get; set; }
        public decimal? CostoEstimado { get; set; }
        public decimal? CostoFinal { get; set; }

        // Relación con el inmueble donde se realiza la reparación
        public Inmueble Inmueble { get; set; }
        public int InmuebleId { get; set; }

        // Estado de la reparación (Pendiente, En proceso, Finalizado)
        public ReparacionEstado Estado { get; set; }
        public int EstadoId { get; set; }

        // Empleado responsable de la reparación
        public Empleado Empleado { get; set; }
        public int EmpleadoId { get; set; }

        // Detalles de la reparación (Materiales, Mano de obra, etc.)
        public List<ReparacionDetalle> Detalles { get; set; } = new();
    }


    public class ReparacionCategoria
    {
        public int Id { get; set; }
        public string Categoria { get; set; } // Ej: Plomería, Electricidad, etc.
    }

    public class ReparacionEstado
    {
        public int Id { get; set; }
        public string Estado { get; set; } // Ej: Pendiente, En proceso, Finalizado
    }

    public class ReparacionDetalle
    {
        public int Id { get; set; }

        // Referencia a la reparación principal
        public Reparacion Reparacion { get; set; }

        // Descripción del trabajo realizado
        public string Descripcion { get; set; } = string.Empty;

        // Costo del trabajo realizado
        public decimal Costo { get; set; }

        // Fecha en que se realizó esta parte de la reparación
        public DateTime Fecha { get; set; }
    }


}
