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

        Task<int> CrearInmueble(Inmueble inmueble);
        Task ActualizarInmueble(Inmueble inmueble);
        Task EliminarInmueble(int id);

        Task<IEnumerable<EstadoInmueble>> ObtenerEstadosInmueble();
        Task<EstadoInmueble?> ObtenerEstadoInmueblePorId(int id);

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

        Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion();
        Task<ReparacionEstado> ObtenerEstadoReparacion(int id);
        Task<IEnumerable<Empleado>> ObtenerEmpleados();
        Task<Empleado?> ObtenerEmpleadoPorId(int id);
        Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId);
        Task AgregarReparacion(int inmuebleId, Reparacion reparacion);
        Task ActualizarReparacion(Reparacion reparacion);
        Task EliminarReparacion(int reparacionId);
        Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio);
        Task<bool> RechazarReparacion(int reparacionId, int empleadoId);
        Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle);
        Task<bool> FinalizarReparacion(int reparacionId);
        Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion);
        Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId);

    }

    public class MockData : IServ_Mock
    {
        private readonly List<Inmueble> _inmuebles;
        private readonly List<Contrato> _contratos;
        private readonly List<Inquilino> _inquilinos;
        private readonly List<Pago> _pagos;
        private readonly List<Reparacion> _reparaciones;
        private readonly List<ReparacionEstado> _reparacionEstados = new();
        private readonly List<ReparacionCategoria> _reparacionCategorias = new();
        private readonly List<Empleado> _empleados;
        private readonly List<EmpleadoCalificacion> _calificaciones = new();
        private readonly List<EstadoInmueble> _estadosInmueble;

        private static readonly Random random = new Random();

        public MockData()
        {
            // Generar listas en el orden correcto
            _estadosInmueble = GenerarEstadosInmueble().ToList();
            _inmuebles = GenerarInmuebles().ToList();
            _inquilinos = GenerarInquilinos().ToList();
            _empleados = GenerarEmpleados().ToList();
            _contratos = GenerarContratos().ToList();
            _pagos = GenerarPagos().ToList();
            _reparacionEstados = GenerarReparacionEstados().ToList();
            _reparacionCategorias = GenerarCategoriasReparacion();
            _reparaciones = GenerarReparaciones().ToList();


            // Relacionar datos después de que todas las listas estén inicializadas
            RelacionarDatos();
        }

        private List<ReparacionCategoria> GenerarCategoriasReparacion()
        {
            return new List<ReparacionCategoria>
            {
                new ReparacionCategoria { Id = 1, Categoria = "Plomería" },
                new ReparacionCategoria { Id = 2, Categoria = "Electricidad" },
                new ReparacionCategoria { Id = 3, Categoria = "Pintura" },
                new ReparacionCategoria { Id = 4, Categoria = "Albañilería" },
                new ReparacionCategoria { Id = 5, Categoria = "Carpintería" }
            };
        }

        private IEnumerable<EstadoInmueble> GenerarEstadosInmueble()
        {
            return new List<EstadoInmueble>
            {
                new EstadoInmueble { Id = 1, Estado = "Disponible" },
                new EstadoInmueble { Id = 2, Estado = "Ocupado" },
                new EstadoInmueble { Id = 3, Estado = "En reparación" }
            };
        }

        // Método para obtener todos los estados de inmueble
        public async Task<IEnumerable<EstadoInmueble>> ObtenerEstadosInmueble()
        {
            return await Task.FromResult(_estadosInmueble);
        }

        // Método para obtener un estado de inmueble por su ID
        public async Task<EstadoInmueble?> ObtenerEstadoInmueblePorId(int id)
        {
            return await Task.FromResult(_estadosInmueble.FirstOrDefault(e => e.Id == id));
        }

        private IEnumerable<Inmueble> GenerarInmuebles()
        {
            for (int i = 1; i <= 15; i++)
            {
                // Asignar estado según la probabilidad
                int probabilidad = random.Next(1, 101); // Número aleatorio entre 1 y 100
                EstadoInmueble estado;

                if (probabilidad <= 10) // 10% de probabilidad
                {
                    estado = _estadosInmueble.First(e => e.Estado == "En reparación");
                }
                else if (probabilidad <= 60) // 50% de probabilidad (10% + 50%)
                {
                    estado = _estadosInmueble.First(e => e.Estado == "Ocupado");
                }
                else // 40% de probabilidad (restante)
                {
                    estado = _estadosInmueble.First(e => e.Estado == "Disponible");
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
                    Estado = estado, // Asignar el estado generado
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
                // Generar fechas aleatorias

                var fechaSolicitud = DateTime.Now.AddDays(-random.Next(1, 30));
                var fechaInicio = fechaSolicitud.AddDays(random.Next(1, 10));
                var fechaFin = fechaInicio.AddDays(random.Next(1, 15));

                // Generar detalles de reparación
                var detalles = new List<ReparacionDetalle>
                                {
                                    new ReparacionDetalle
                                    {
                                        Id = 1,
                                        Descripcion = "Descripcion trabajo realizado",
                                        Costo = random.Next(500, 2000),
                                        Fecha = fechaInicio.AddDays(1)
                                    },
                                    new ReparacionDetalle
                                    {
                                        Id = 2,
                                        Descripcion = "Otra tarea realizada para la reparacion",
                                        Costo = random.Next(300, 1500),
                                        Fecha = fechaInicio.AddDays(2)
                                    }
                                };

                // Obtener un índice aleatorio para Inmueble, Empleado y Estado
                int inmuebleIndex = random.Next(0, _inmuebles.Count);
                int empleadoIndex = random.Next(0, _empleados.Count);
                int estadoIndex = random.Next(1, 3); // Estados: 1, 2, 3

                yield return new Reparacion
                {
                    Id = i,
                    FechaSolicitud = fechaSolicitud,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Categoria = _reparacionCategorias[random.Next(0, _reparacionCategorias.Count)],
                    Descripcion = $"Reparación #{i}",
                    CostoEstimado = random.Next(1000, 5000),
                    CostoFinal = random.Next(1000, 5000),
                    Inmueble = _inmuebles[inmuebleIndex], // Mismo objeto
                    InmuebleId = _inmuebles[inmuebleIndex].Id, // Mismo ID
                    Estado = ObtenerEstadoReparacion(estadoIndex).Result,
                    EstadoId = estadoIndex, // Mismo ID
                    Empleado = _empleados[empleadoIndex], // Mismo objeto
                    EmpleadoId = _empleados[empleadoIndex].EmpleadoId, // Mismo ID
                    Detalles = detalles
                };
            }
        }

        public IEnumerable<ReparacionEstado> GenerarReparacionEstados()
        {
            var estados = new List<ReparacionEstado>
            {
                new ReparacionEstado { Id = 1, Estado = "Pendiente" },
                new ReparacionEstado { Id = 2, Estado = "En proceso" },
                new ReparacionEstado { Id = 3, Estado = "Finalizado" }
            };

            // Devolver la lista como una tarea completada
            return estados.AsEnumerable();
        }

        public async Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion()
        {
            return await Task.FromResult(_reparacionEstados);
        }

        public async Task<ReparacionEstado> ObtenerEstadoReparacion(int estadoId)
        {
            return await Task.FromResult(_reparacionEstados.FirstOrDefault(e => e.Id == estadoId));
        }

        private IEnumerable<Empleado> GenerarEmpleados()
        {
            for (int i = 1; i <= 5; i++)
            {
                yield return new Empleado
                {
                    EmpleadoId = i,
                    PersonaId = i,
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

        public async Task<int> CrearInmueble(Inmueble inmueble)
        {
            // Generar un nuevo ID para el inmueble
            inmueble.Id = _inmuebles.Any() ? _inmuebles.Max(i => i.Id) + 1 : 1;

            // Agregar el inmueble a la lista
            _inmuebles.Add(inmueble);

            // Devolver el ID generado
            return inmueble.Id;
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
                // Verificar si la lista de características está vacía
                if (inmueble.Caracteristicas.Any())
                {
                    // Si hay características, obtener el máximo ID y sumar 1
                    caracteristica.Id = inmueble.Caracteristicas.Max(c => c.Id) + 1;
                }
                else
                {
                    // Si no hay características, asignar un ID inicial (por ejemplo, 1)
                    caracteristica.Id = 1;
                }

                // Agregar la característica al inmueble
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
            return inmueble;
        }

        // Implementación de los nuevos métodos para reparaciones
        public async Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                return await Task.FromResult(inmueble.Reparaciones);
            }
            return new List<Reparacion>();
        }

        public async Task AgregarReparacion(int inmuebleId, Reparacion reparacion)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                // Asignar un nuevo ID a la reparación
                reparacion.Id = _reparaciones.Any() ? _reparaciones.Max(r => r.Id) + 1 : 1;

                // Asignar el inmueble a la reparación
                reparacion.Inmueble = inmueble;
                reparacion.InmuebleId = inmueble.Id;

                // Agregar la reparación a la lista global
                _reparaciones.Add(reparacion);

                // Agregar la reparación al inmueble
                inmueble.Reparaciones.Add(reparacion);
            }
            await Task.CompletedTask;
        }

        public async Task ActualizarReparacion(Reparacion reparacion)
        {
            var reparacionExistente = _reparaciones.FirstOrDefault(r => r.Id == reparacion.Id);
            if (reparacionExistente != null)
            {
                // Actualizar los campos de la reparación
                reparacionExistente.Descripcion = reparacion.Descripcion;
                reparacionExistente.FechaSolicitud = reparacion.FechaSolicitud;
                reparacionExistente.FechaInicio = reparacion.FechaInicio;
                reparacionExistente.FechaFin = reparacion.FechaFin;
                reparacionExistente.CostoEstimado = reparacion.CostoEstimado;
                reparacionExistente.CostoFinal = reparacion.CostoFinal;
                reparacionExistente.Estado = reparacion.Estado;
                reparacionExistente.Empleado = reparacion.Empleado;
                reparacionExistente.EmpleadoId = reparacion.EmpleadoId;
            }
            await Task.CompletedTask;
        }

        public async Task EliminarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion != null)
            {
                // Eliminar la reparación de la lista global
                _reparaciones.Remove(reparacion);

                // Eliminar la reparación del inmueble correspondiente
                var inmueble = _inmuebles.FirstOrDefault(i => i.Reparaciones.Any(r => r.Id == reparacionId));
                if (inmueble != null)
                {
                    inmueble.Reparaciones.Remove(reparacion);
                }
            }
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Reparacion>> ObtenerReparacionesPorEstado(int estadoId)
        {
            return await Task.FromResult(_reparaciones.Where(r => r.EstadoId == estadoId));
        }

        public async Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            reparacion.EstadoId = 2; // Iniciado
            reparacion.Estado = await ObtenerEstadoReparacion(2);
            reparacion.EmpleadoId = empleadoId;
            reparacion.Empleado = _empleados.First(e => e.EmpleadoId == empleadoId);
            reparacion.CostoEstimado = costoEstimado;
            reparacion.FechaInicio = fechaInicio;

            return true;
        }

        public async Task<bool> RechazarReparacion(int reparacionId, int empleadoId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            reparacion.EstadoId = 4; // Rechazado
            reparacion.Estado = await ObtenerEstadoReparacion(4);
            return true;
        }

        public async Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            detalle.Id = reparacion.Detalles.Any() ? reparacion.Detalles.Max(d => d.Id) + 1 : 1;
            detalle.Reparacion = reparacion;
            detalle.Fecha = DateTime.Now;
            reparacion.Detalles.Add(detalle);

            // Actualizar estado si es la primera vez que se agrega un detalle
            if (reparacion.EstadoId == 2) // Si estaba "Iniciado"
            {
                reparacion.EstadoId = 3; // En proceso
                reparacion.Estado = await ObtenerEstadoReparacion(3);
            }

            return true;
        }

        public async Task<bool> FinalizarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            reparacion.EstadoId = 5; // Finalizado por aprobar
            reparacion.Estado = await ObtenerEstadoReparacion(5);
            reparacion.FechaFin = DateTime.Now;
            reparacion.CostoFinal = reparacion.Detalles.Sum(d => d.Costo);

            return true;
        }

        public async Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            reparacion.EstadoId = 6; // Finalizado
            reparacion.Estado = await ObtenerEstadoReparacion(6);

            calificacion.Id = _calificaciones.Any() ? _calificaciones.Max(c => c.Id) + 1 : 1;
            calificacion.ReparacionId = reparacionId;
            calificacion.EmpleadoId = reparacion.EmpleadoId;
            _calificaciones.Add(calificacion);

            return true;
        }

        public async Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId)
        {
            return await Task.FromResult(_calificaciones.Where(c => c.EmpleadoId == empleadoId));
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