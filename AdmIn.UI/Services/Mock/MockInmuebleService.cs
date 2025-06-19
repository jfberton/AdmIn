using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockInmuebleService : IInmuebleService 
    {
        private readonly List<Inmueble> _inmuebles;
        private readonly List<EstadoInmueble> _estadosInmueble;
        private static readonly Random random = new Random();

        public MockInmuebleService()
        {
            _estadosInmueble = GenerarEstadosInmueble().ToList();
            _inmuebles = GenerarInmuebles().ToList();
        }

        private IEnumerable<EstadoInmueble> GenerarEstadosInmueble() => new List<EstadoInmueble>
        {
            new() { Id = 1, Estado = "Disponible" },
            new() { Id = 2, Estado = "Ocupado" },
            new() { Id = 3, Estado = "Reservado" },
            new() { Id = 4, Estado = "En reparación" }
        };

        private IEnumerable<Inmueble> GenerarInmuebles()
        {
            for (int i = 1; i <= 15; i++)
            {
                List<Imagen> imagenes = GenerarImagenesInmueble(i).ToList();
                var estadoDisponible = _estadosInmueble.First(e => e.Estado == "Disponible");

                yield return new Inmueble
                {
                    Id = i,
                    Descripcion = $"Descripción inmueble #{i}",
                    Comentario = "Hermoso inmueble en excelente ubicación.",
                    Valor = random.Next(50000, 500000),
                    Superficie = random.Next(50, 500),
                    Construido = random.Next(30, 450),
                    Telefono = new Telefono { Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}" },
                    Estado = estadoDisponible, 
                    Direccion = new Direccion
                    {
                        DireccionId = i, // Assuming DireccionId can be same as InmuebleId for mock
                        CalleNumero = $"Calle {random.Next(1, 200)} #{random.Next(1, 9999)}",
                        Colonia = "Centro",
                        Ciudad = "Ciudad Ejemplo",
                        Estado = "Estado Ejemplo",
                        CodigoPostal = "12345",
                        Pais = "México"
                    },
                    Caracteristicas = new List<CaracteristicaInmueble>
                    {
                        new() { Id = 1, Nombre = "Habitaciones", Valor = random.Next(1, 5).ToString() },
                        new() { Id = 2, Nombre = "Baños", Valor = random.Next(1, 3).ToString() },
                        new() { Id = 3, Nombre = "Estacionamiento", Valor = random.Next(0, 2).ToString() }
                    },
                    Imagenes = imagenes,
                    ImagenPrincipalId = imagenes.Any() ? imagenes.First().Id : (Guid?)null,
                    Contratos = new List<Contrato>(),
                    Inquilinos = new List<Inquilino>(),
                    Pagos = new List<Pago>(),
                    Reparaciones = new List<Reparacion>()
                };
            }
        }

        private IEnumerable<Imagen> GenerarImagenesInmueble(int inmuebleId)
        {
            int cantidadImagenes = random.Next(3, 10);
            for (int j = 1; j <= cantidadImagenes; j++)
            {
                yield return new Imagen
                {
                    Id = Guid.NewGuid(),
                    Descripcion = $"Imagen {j} del inmueble {inmuebleId}",
                    Url = $"https://picsum.photos/800/600?random={random.Next(1, 1000)}",
                    ThumbnailUrl = $"https://picsum.photos/200/150?random={random.Next(1, 1000)}"
                };
            }
        }

        public async Task<IEnumerable<Inmueble>> ObtenerInmuebles() => await Task.FromResult(_inmuebles);

        public async Task<Inmueble?> ObtenerInmueblePorId(int id)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == id);
            return await Task.FromResult(inmueble);
        }

        public async Task<int> CrearInmueble(Inmueble inmueble)
        {
            inmueble.Id = _inmuebles.Any() ? _inmuebles.Max(i => i.Id) + 1 : 1;
            _inmuebles.Add(inmueble);
            return await Task.FromResult(inmueble.Id);
        }

        public async Task ActualizarInmueble(Inmueble inmueble)
        {
            var index = _inmuebles.FindIndex(i => i.Id == inmueble.Id);
            if (index != -1)
            {
                _inmuebles[index] = inmueble;
            }
            await Task.CompletedTask;
        }

        public async Task EliminarInmueble(int id)
        {
            _inmuebles.RemoveAll(i => i.Id == id);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<EstadoInmueble>> ObtenerEstadosInmueble() => await Task.FromResult(_estadosInmueble);

        public async Task<EstadoInmueble?> ObtenerEstadoInmueblePorId(int id)
        {
            return await Task.FromResult(_estadosInmueble.FirstOrDefault(e => e.Id == id));
        }

        public async Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                if (inmueble.Caracteristicas.Any())
                {
                    caracteristica.Id = inmueble.Caracteristicas.Max(c => c.Id) + 1;
                }
                else
                {
                    caracteristica.Id = 1;
                }
                inmueble.Caracteristicas.Add(caracteristica);
            }
            await Task.CompletedTask;
        }

        public async Task ActualizarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                var index = inmueble.Caracteristicas.FindIndex(c => c.Id == caracteristica.Id);
                if (index != -1)
                {
                    inmueble.Caracteristicas[index] = caracteristica;
                }
            }
            await Task.CompletedTask;
        }

        public async Task EliminarCaracteristica(int inmuebleId, int caracteristicaId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                inmueble.Caracteristicas.RemoveAll(c => c.Id == caracteristicaId);
            }
            await Task.CompletedTask;
        }

        public async Task AgregarImagen(int inmuebleId, Imagen imagen)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                imagen.Id = Guid.NewGuid();
                inmueble.Imagenes.Add(imagen);
            }
            await Task.CompletedTask;
        }

        public async Task EliminarImagen(int inmuebleId, Guid imagenId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                inmueble.Imagenes.RemoveAll(img => img.Id == imagenId);
            }
            await Task.CompletedTask;
        }

        public async Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null && inmueble.Imagenes.Any(img => img.Id == imagenId))
            {
                inmueble.ImagenPrincipalId = imagenId;
            }
            await Task.CompletedTask;
        }
    }
}
