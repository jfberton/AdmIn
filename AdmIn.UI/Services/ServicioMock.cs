using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_Mock
    {
        Task<IEnumerable<Inmueble>> ObtenerInmuebles();
        Task<Inmueble?> ObtenerInmueblePorId(int id);

        Task CrearInmueble(Inmueble inmueble);
        Task ActualizarInmueble(Inmueble inmueble);
        Task EliminarInmueble(int id);

        Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task ActualizarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task EliminarCaracteristica(int inmuebleId, int caracteristicaId);
        Task AgregarImagen(int inmuebleId, Imagen imagen);
        Task EliminarImagen(int inmuebleId, Guid imagenId);
        Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId);


        Task<IEnumerable<Contrato>> ObtenerContratos();
        Task<Contrato?> ObtenerContratoPorId(int id);

        Task<IEnumerable<Inquilino>> ObtenerInquilinos();
        Task<Inquilino?> ObtenerInquilinoPorId(int id);

        Task<IEnumerable<Pago>> ObtenerPagos();
        Task<Pago?> ObtenerPagoPorId(int id);

        Task<IEnumerable<Reparacion>> ObtenerReparaciones();
        Task<Reparacion?> ObtenerReparacionPorId(int id);

        Task<IEnumerable<Empleado>> ObtenerEmpleados();
        Task<Empleado?> ObtenerEmpleadoPorId(int id);

    }

    public class MockData : IServ_Mock
    {
        private readonly List<Inmueble> _inmuebles;
        private readonly List<Contrato> _contratos;
        private readonly List<Inquilino> _inquilinos;
        private readonly List<Pago> _pagos;
        private readonly List<Reparacion> _reparaciones;
        private readonly List<Empleado> _empleados;

        private static readonly Random random = new Random();

        public MockData()
        {
            // Generar listas en el orden correcto
            _inmuebles = GenerarInmuebles().ToList();
            _inquilinos = GenerarInquilinos().ToList();
            _empleados = GenerarEmpleados().ToList();
            _contratos = GenerarContratos().ToList();
            _pagos = GenerarPagos().ToList();
            _reparaciones = GenerarReparaciones().ToList();

            // Relacionar datos después de que todas las listas estén inicializadas
            RelacionarDatos();
        }

        private IEnumerable<Inmueble> GenerarInmuebles()
        {
            for (int i = 1; i <= 15; i++)
            {
                // Asignar estado según la probabilidad
                string estado;
                int probabilidad = random.Next(1, 101); // Número aleatorio entre 1 y 100

                if (probabilidad <= 10) // 10% de probabilidad
                {
                    estado = "En reparación";
                }
                else if (probabilidad <= 60) // 50% de probabilidad (10% + 50%)
                {
                    estado = "Ocupado";
                }
                else // 40% de probabilidad (restante)
                {
                    estado = "Disponible";
                }

                List<Imagen> imagenes = GenerarImagenes(i).ToList();

                yield return new Inmueble
                {
                    Id = i,
                    Descripcion = $"Descripción inmueble #{i}",
                    Comentario = "Hermoso inmueble en excelente ubicación.",
                    Valor = random.Next(50000, 500000),
                    Superficie = random.Next(50, 500),
                    Construido = random.Next(30, 450),
                    Telefono = new Telefono { Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}" },
                    Estado = new EstadoInmueble { Id = i, Estado = estado },
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
                        new CaracteristicaInmueble { Id = 1, Nombre = "Habitaciones", Valor = random.Next(1, 5).ToString() },
                        new CaracteristicaInmueble { Id = 2, Nombre = "Baños", Valor = random.Next(1, 3).ToString() },
                        new CaracteristicaInmueble { Id = 3, Nombre = "Estacionamiento", Valor = random.Next(0, 2).ToString() }
                    },
                    Imagenes = imagenes,
                    ImagenPrincipalId = imagenes.First().Id
                };
            }
        }

        private void RelacionarDatos()
        {
            // Relacionar Inmuebles con Contratos, Inquilinos, Pagos y Reparaciones
            foreach (var inmueble in _inmuebles)
            {
                inmueble.Contratos = _contratos.Where(c => c.Inmueble.Id == inmueble.Id).ToList();
                inmueble.Pagos = _pagos.Where(p => p.Contrato.Inmueble.Id == inmueble.Id).ToList();
                inmueble.Reparaciones = _reparaciones.Where(r => r.Inmueble.Id == inmueble.Id).ToList();

                // Asignar inquilinos solo si el inmueble está "Ocupado"
                if (inmueble.Estado.Estado == "Ocupado")
                {
                    inmueble.Inquilinos = _inquilinos.Where(i => i.Inmueble.Id == inmueble.Id).ToList();
                }
                else
                {
                    inmueble.Inquilinos = new List<Inquilino>(); // No hay inquilinos si no está ocupado
                }
            }

            // Relacionar Contratos con Pagos
            foreach (var contrato in _contratos)
            {
                contrato.Pagos = _pagos.Where(p => p.Contrato.Id == contrato.Id).ToList();
            }

            // Relacionar Reparaciones con Empleados
            foreach (var reparacion in _reparaciones)
            {
                reparacion.Empleado = _empleados.FirstOrDefault(e => e.EmpleadoId == reparacion.Empleado.EmpleadoId);
            }
        }

        private IEnumerable<Contrato> GenerarContratos()
        {
            for (int i = 1; i <= 10; i++)
            {
                yield return new Contrato
                {
                    Id = i,
                    Inmueble = _inmuebles[random.Next(0, _inmuebles.Count)],
                    Inquilino = _inquilinos[random.Next(0, _inquilinos.Count)],
                    Administrador = new Administrador { Nombre = "Admin Ejemplo" },
                    FechaInicio = DateTime.Now.AddMonths(-random.Next(1, 12)),
                    FechaFin = DateTime.Now.AddMonths(random.Next(1, 12)),
                    MontoMensual = random.Next(1000, 5000),
                    Estado = new ContratoEstado { Descripcion = "Vigente" }
                };
            }
        }

        private IEnumerable<Inquilino> GenerarInquilinos()
        {
            // Lista de tipos de teléfono
            var tiposTelefono = new List<TipoTelefono>
            {
                new TipoTelefono { TipoTelefonoId = 1, Descripcion = "Móvil" },
                new TipoTelefono { TipoTelefonoId = 2, Descripcion = "Fijo" },
                new TipoTelefono { TipoTelefonoId = 3, Descripcion = "Trabajo" }
            };

            for (int i = 1; i <= 10; i++)
            {
                yield return new Inquilino
                {
                    Id = i,
                    Nombre = $"Inquilino #{i}",
                    Email = $"inquilino{i}@example.com",
                    Telefonos = new List<Telefono>
            {
                new Telefono
                {
                    Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}",
                    Tipo = tiposTelefono[random.Next(0, tiposTelefono.Count)] // Asignar un tipo aleatorio
                }
            },
                    Inmueble = _inmuebles[random.Next(0, _inmuebles.Count)]
                };
            }
        }

        private IEnumerable<Pago> GenerarPagos()
        {
            for (int i = 1; i <= 20; i++)
            {
                var pago = new Pago
                {
                    Id = i,
                    Contrato = _contratos[random.Next(0, _contratos.Count)],
                    FechaVencimiento = DateTime.Now.AddDays(random.Next(1, 30)),
                    Estado = new PagoEstado { Estado = "Pendiente" },
                    Descripcion = $"Pago #{i}",
                    DetallesPago = new List<DetallePago>
                    {
                        new DetallePago { Descripcion = "Cuota mensual", Monto = random.Next(1000, 5000) },
                        new DetallePago { Descripcion = "Depósito de garantía", Monto = random.Next(500, 1000) }
                    }
                };

                yield return pago;
            }
        }

        private IEnumerable<Reparacion> GenerarReparaciones()
        {
            for (int i = 1; i <= 10; i++)
            {
                yield return new Reparacion
                {
                    Id = i,
                    Inmueble = _inmuebles[random.Next(0, _inmuebles.Count)],
                    FechaSolicitud = DateTime.Now.AddDays(-random.Next(1, 30)),
                    FechaInicio = DateTime.Now.AddDays(-random.Next(1, 15)),
                    FechaFin = DateTime.Now.AddDays(random.Next(1, 15)),
                    Descripcion = $"Reparación #{i}",
                    CostoEstimado = random.Next(1000, 5000),
                    CostoFinal = random.Next(1000, 5000),
                    Estado = new ReparacionEstado { Estado = "Finalizado" },
                    Empleado = _empleados[random.Next(0, _empleados.Count)]
                };
            }
        }

        private IEnumerable<Empleado> GenerarEmpleados()
        {
            for (int i = 1; i <= 5; i++)
            {
                yield return new Empleado
                {
                    EmpleadoId = i,
                    Nombre = $"Empleado #{i}",
                    Especialidad = new EmpleadoEspecialidad { Especialidad = $"Especialidad #{i}" },
                    Agenda = new List<EmpleadoAgenda>()
                };
            }
        }

        private IEnumerable<Imagen> GenerarImagenes(int inmuebleId)
        {
            int imagenes = random.Next(3, 10);
            for (int j = 1; j <= imagenes; j++)
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

        public async Task CrearInmueble(Inmueble inmueble)
        {
            inmueble.Id = _inmuebles.Max(i => i.Id) + 1;
            _inmuebles.Add(inmueble);
            await Task.CompletedTask;
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
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == id);
            if (inmueble != null)
            {
                _inmuebles.Remove(inmueble);
            }
            await Task.CompletedTask;
        }

        public async Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                caracteristica.Id = inmueble.Caracteristicas.Max(c => c.Id) + 1;
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
                var caracteristica = inmueble.Caracteristicas.FirstOrDefault(c => c.Id == caracteristicaId);
                if (caracteristica != null)
                {
                    inmueble.Caracteristicas.Remove(caracteristica);
                }
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
                var imagen = inmueble.Imagenes.FirstOrDefault(i => i.Id == imagenId);
                if (imagen != null)
                {
                    inmueble.Imagenes.Remove(imagen);
                }
            }
            await Task.CompletedTask;
        }

        public async Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                inmueble.ImagenPrincipalId = imagenId;
            }
            await Task.CompletedTask;
        }

        // Implementación de los métodos de la interfaz
        public async Task<IEnumerable<Inmueble>> ObtenerInmuebles() => await Task.FromResult(_inmuebles);
        public async Task<Inmueble> ObtenerInmueblePorId(int id)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == id);
            if (inmueble != null)
            {
                return new Inmueble
                {
                    Id = inmueble.Id,
                    Descripcion = inmueble.Descripcion,
                    Comentario = inmueble.Comentario,
                    Valor = inmueble.Valor,
                    Superficie = inmueble.Superficie,
                    Construido = inmueble.Construido,
                    Telefono = inmueble.Telefono,
                    Estado = inmueble.Estado,
                    Direccion = inmueble.Direccion,
                    Caracteristicas = new List<CaracteristicaInmueble>(inmueble.Caracteristicas),
                    Imagenes = new List<Imagen>(inmueble.Imagenes),
                    ImagenPrincipalId = inmueble.ImagenPrincipalId
                };
            }
            return null;
        }

        public async Task<IEnumerable<Contrato>> ObtenerContratos() => await Task.FromResult(_contratos);
        public async Task<Contrato?> ObtenerContratoPorId(int id) => await Task.FromResult(_contratos.FirstOrDefault(c => c.Id == id));

        public async Task<IEnumerable<Inquilino>> ObtenerInquilinos() => await Task.FromResult(_inquilinos);
        public async Task<Inquilino?> ObtenerInquilinoPorId(int id) => await Task.FromResult(_inquilinos.FirstOrDefault(i => i.Id == id));

        public async Task<IEnumerable<Pago>> ObtenerPagos() => await Task.FromResult(_pagos);
        public async Task<Pago?> ObtenerPagoPorId(int id) => await Task.FromResult(_pagos.FirstOrDefault(p => p.Id == id));

        public async Task<IEnumerable<Reparacion>> ObtenerReparaciones() => await Task.FromResult(_reparaciones);
        public async Task<Reparacion?> ObtenerReparacionPorId(int id) => await Task.FromResult(_reparaciones.FirstOrDefault(r => r.Id == id));

        public async Task<IEnumerable<Empleado>> ObtenerEmpleados() => await Task.FromResult(_empleados);
        public async Task<Empleado?> ObtenerEmpleadoPorId(int id) => await Task.FromResult(_empleados.FirstOrDefault(e => e.EmpleadoId == id));
    }
}