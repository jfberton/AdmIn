using AdmIn.Business.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace AdmIn.UI.Services
{
    public interface IServ_Mock
    {
        Task<IEnumerable<Inmueble>> ObtenerInmuebles();

        Task<Inmueble> ObtenerInmueblePorId(int id);
    }

    public class MockData : IServ_Mock
    {
        private readonly IEnumerable<Inmueble> _inmuebles;
        private static readonly Random random = new Random();

        public MockData()
        {
            if (_inmuebles == null)
            {
                _inmuebles = GenerarInmuebles();
            }
        }

        private IEnumerable<Inmueble> GenerarInmuebles()
        {
            List<Inmueble> inmuebles = new();
            for (int i = 1; i <= 15; i++)
            {
                var inmueble = new Inmueble
                {
                    Id = i,
                    Descripcion = $"Inmueble #{i}",
                    Comentario = "Hermoso inmueble en excelente ubicación.",
                    Valor = random.Next(50000, 500000),
                    Superficie = random.Next(50, 500),
                    Construido = random.Next(30, 450),
                    Telefono = new Telefono { Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}" },
                    Estado = new EstadoInmueble { Id = i, Estado = random.Next(1, 100) <= 60 ? "Ocupado" : "Disponible" },
                    Direccion = new Direccion
                    {
                        DireccionId = i,
                        CalleNumero = $"Calle {random.Next(1, 200)} #{random.Next(1, 9999)}",
                        Colonia = "Centro",
                        Ciudad = "Ciudad Ejemplo",
                        Estado = "Estado Ejemplo",
                        CodigoPostal = "12345",
                        Pais = "México"
                    },
                    Caracteristicas = new List<CaracteristicaInmueble>
            {
                new CaracteristicaInmueble { Nombre = "Habitaciones", Valor = random.Next(1, 5).ToString() },
                new CaracteristicaInmueble { Nombre = "Baños", Valor = random.Next(1, 3).ToString() },
                new CaracteristicaInmueble { Nombre = "Estacionamiento", Valor = random.Next(0, 2).ToString() }
            },
                    Imagenes = GenerarImagenes(random.Next(2, 10))
                };

                // Asignar una imagen principal
                if (inmueble.Imagenes.Any())
                {
                    inmueble.ImagenPrincipalId = inmueble.Imagenes.First().Id;
                }

                inmuebles.Add(inmueble);
            }
            return inmuebles;
        }

        private List<Imagen> GenerarImagenes(int inmuebleId)
        {
            var imagenes = new List<Imagen>();
            for (int j = 1; j <= 3; j++) // Generar 3 imágenes por inmueble
            {
                var imagen = new Imagen
                {
                    Descripcion = $"Imagen {j} del inmueble {inmuebleId}",
                    Url = $"https://picsum.photos/800/600?random={random.Next(1, 1000)}",
                    ThumbnailUrl = $"https://picsum.photos/200/150?random={random.Next(1, 1000)}" // Thumbnail
                };
                imagenes.Add(imagen);
            }
            return imagenes;
        }

        private List<Reparacion> GenerarReparaciones(int inmuebleId)
        {
            return new List<Reparacion>
        {
            new Reparacion
            {
                Id = 1,
                FechaSolicitud = DateTime.Now.AddDays(-30),
                FechaInicio = DateTime.Now.AddDays(-25),
                FechaFin = DateTime.Now.AddDays(-20),
                Descripcion = "Pintura de interiores.",
                CostoEstimado = 5000,
                CostoFinal = 4800,
                Estado = new ReparacionEstado { Estado = "Finalizado" },
                Empleado = new Empleado { Nombre = "Juan Pérez" }
            }
        };
        }

        private List<Inquilino> GenerarInquilinos(int inmuebleId)
        {
            return new List<Inquilino>
        {
            new Inquilino
            {
                Id = 1,
                Nombre = "Ana López",
                Email = "ana.lopez@example.com",
                Telefonos = new List<Telefono>
                {
                    new Telefono { Numero = "555-9876-543", Tipo = new TipoTelefono { Descripcion = "Móvil" } }
                }
            }
        };
        }

        private List<Pago> GenerarPagos(int inmuebleId)
        {
            return new List<Pago>
        {
            new Pago
            {
                Id = 1,
                FechaVencimiento = DateTime.Now.AddDays(10),
                //Monto = 15000,
                Estado = new PagoEstado { Estado = "Pendiente" },
                Descripcion = "Renta mensual octubre 2023"
            }
        };
        }

        private List<Contrato> GenerarContratos(int inmuebleId)
        {
            return new List<Contrato>
        {
            new Contrato
            {
                Id = 1,
                FechaInicio = DateTime.Now.AddMonths(-1),
                FechaFin = DateTime.Now.AddMonths(11),
                MontoMensual = 15000,
                Estado = new ContratoEstado { Descripcion = "Vigente" }
            }
        };
        }

        public async Task<IEnumerable<Inmueble>> ObtenerInmuebles()
        {
            return await Task.FromResult(_inmuebles);
        }

        public async Task<Inmueble?> ObtenerInmueblePorId(int id)
        {
            return await Task.FromResult(_inmuebles.FirstOrDefault(i => i.Id == id));
        }
    }
}