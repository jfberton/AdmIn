using AdmIn.Data.Entidades;
using AdmIn.Data.Entidades.Todos;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Propiedad
    {
        public int Id { get; set; } // ID de la propiedad
        public string Descripcion { get; set; } // Descripción de la propiedad
        public Direccion Direccion { get; set; } // ID de la dirección asociada
        public Telefono? Telefono { get; set; } // ID del teléfono asociado (opcional)
        public string Email { get; set; } // Email de la propiedad
        public int? Unidades { get; set; } // Número de unidades
        public int? UnidadesHabitadas { get; set; } // Número de unidades habitadas
        public string Comentario { get; set; } // Comentario sobre la propiedad
        public decimal Valor { get; set; } // Valor de la propiedad
        public decimal Superficie { get; set; } // Superficie de la propiedad
        public Administrador Administrador { get; set; } // ID del administrador asociado
        public Propietario Propietario { get; set; } // ID del cliente asociado
        public TipoPropiedad Tipo { get; set; } // ID del tipo de propiedad
    }

    public class TipoPropiedad
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
    }

    public class Inmueble
    {
        public int Id { get; set; }
        public Direccion Direccion { get; set; }
        public string Descripcion { get; set; }
        public string Comentario { get; set; }
        public decimal Valor { get; set; }
        public decimal Superficie { get; set; }
        public decimal Construido { get; set; }
        public Telefono Telefono { get; set; }
        public EstadoInmueble Estado { get; set; }

        // Propiedad para indicar cuál es la imagen principal
        public Guid? ImagenPrincipalId { get; set; }

        // Propiedad de navegación para obtener la imagen principal
        public Imagen? ImagenPrincipal => Imagenes.FirstOrDefault(i => i.Id == ImagenPrincipalId);

        // Relaciones
        public List<Imagen> Imagenes { get; set; } = new();

        public List<Reparacion> Reparaciones { get; set; } = new();
        public List<Inquilino> Inquilinos { get; set; } = new();
        public List<Pago> Pagos { get; set; } = new();
        public List<Contrato> Contratos { get; set; } = new();
        public List<CaracteristicaInmueble> Caracteristicas { get; set; } = new();
        public Reserva? Reserva { get; set; }
    }

    //Habitaciones, baños, pileta, garage, etc el valor es indica cantidad o estado
    public class CaracteristicaInmueble
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }

    public class EstadoInmueble
    {
        public int Id { get; set; }
        public string Estado { get; set; }
    }
}
