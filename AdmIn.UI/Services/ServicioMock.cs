using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_Mock
    {
        #region Persona Base
        Task<List<PersonaBase>> BuscarPersonasAsync(string apellido, string rfc);
        Task<PersonaBase> CrearPersonaAsync(PersonaBase persona);
        Task<PersonaBase> ActualizarPersonaAsync(PersonaBase persona);
        Task EliminarPersonaAsync(int personaId);
        Task<PersonaBase?> ObtenerPersonaPorIdAsync(int personaId);

        #endregion

        #region Servicio Inmuebles
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

        #endregion

        #region Inquilinos contratos y  pagos
        Task<IEnumerable<Contrato>> ObtenerContratos();
        Task<Contrato?> ObtenerContratoPorId(int id);

        Task<IEnumerable<Inquilino>> ObtenerInquilinos();
        Task<Inquilino?> ObtenerInquilinoPorId(int id);

        Task<IEnumerable<Pago>> ObtenerPagos();
        Task<Pago?> ObtenerPagoPorId(int id);

        Task<Reserva> GuardarReservaAsync(Reserva reserva);
        Task<Reserva?> ObtenerReservaPorInmuebleIdAsync(int inmuebleId);

        #endregion

        #region Reparaciones
        Task<IEnumerable<Reparacion>> ObtenerReparaciones();
        Task<Reparacion?> ObtenerReparacionPorId(int id);
        Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion();
        Task<ReparacionEstado> ObtenerEstadoReparacion(int id);
        Task<IEnumerable<ReparacionCategoria>> ObtenerCategoriasReparacion();
        Task<IEnumerable<Empleado>> ObtenerEmpleados();
        Task<Empleado?> ObtenerEmpleadoPorId(int id);
        Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId);

        //VER!
        Task ActualizarReparacion(Reparacion reparacion);
        Task EliminarReparacion(int reparacionId);

        #region Nueva reparacion

        Task AgregarReparacion(int inmuebleId, Reparacion reparacion);
        Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId);

        #endregion 

        #region Pendiente
        Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio);
        Task<bool> RechazarReparacion(int reparacionId, int empleadoId);
        #endregion

        #region En proceso
        Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle);
        Task<bool> DisputarDetalle(int reparacionId, int detalleId);
        Task<bool> ResolverDisputa(int reparacionId, int detalleId, bool aceptarDisputa);
        #endregion

        #region Finalizacion de reparacion
        Task<bool> FinalizarReparacion(int reparacionId);
        Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion);
        Task<bool> DesaprobarReparacion(int reparacionId);
        #endregion

        #region Pendiente sin asignar
        Task<bool> CancelarReparacion(int reparacionId, int adminId, string motivoCancelacion);
        Task<bool> AsignarEmpleado(int reparacionId, int empleadoId);
        #endregion

        #endregion

    }

    public class MockData : IServ_Mock
    {
        private readonly List<EstadoInmueble> _estadosInmueble;
        private readonly List<ReparacionEstado> _reparacionEstados;
        private readonly List<ReparacionCategoria> _reparacionCategorias;
        private readonly List<ContratoEstado> _contratoEstados;
        private readonly List<PagoEstado> _pagoEstados;

        private readonly List<PersonaBase> _personas;
        private readonly List<Inmueble> _inmuebles;
        private readonly List<Empleado> _empleados;
        
        private readonly List<EmpleadoCalificacion> _calificaciones = new();
        private readonly List<Contrato> _contratos = new();
        private readonly List<Reserva> _reservas = new();
        private readonly List<Inquilino> _inquilinos = new();
        private readonly List<Pago> _pagos = new();
        private readonly List<Reparacion> _reparaciones;

        private static readonly Random random = new Random();

        public MockData()
        {
            // Inicializar listas de estados y categorías
            _estadosInmueble = GenerarEstadosInmueble();
            _reparacionEstados = GenerarReparacionEstados();
            _reparacionCategorias = GenerarCategoriasReparacion();
            _contratoEstados = GenerarContratoEstados();
            _pagoEstados = GenerarPagoEstados();

            // Inicializar Personas, inmuebles, empleados
            _personas = GenerarPersonas().ToList();
            _inmuebles = GenerarInmuebles().ToList();
            _empleados = GenerarEmpleados().ToList();

            GenerarContratosYPagos();
            _reparaciones = GenerarReparaciones().ToList();
        }

        #region Generacion de datos de prueba

        #region Listas de estados y categorias
        private List<EstadoInmueble> GenerarEstadosInmueble() => new()
            {
                new() { Id = 1, Estado = "Disponible" },
                new() { Id = 2, Estado = "Ocupado" },
                new() { Id = 3, Estado = "Reservado" },
                new() { Id = 4, Estado = "En reparación" }
            };

        private List<ReparacionEstado> GenerarReparacionEstados() => new()
            {
                new() { Id = 1, Estado = "Pendiente", Descripcion = "Pendiente de aceptar" },
                new() { Id = 2, Estado = "Pendiente sin asignar", Descripcion = "Rechazada por el empleado" },
                new() { Id = 3, Estado = "En proceso", Descripcion = "Aceptada y en proceso" },
                new() { Id = 4, Estado = "En disputa", Descripcion = "Detalle en disputa" },
                new() { Id = 5, Estado = "Finalizado por aprobar", Descripcion = "Finalizada, pendiente de aprobación" },
                new() { Id = 6, Estado = "Finalizado", Descripcion = "Finalizada y aprobada" },
                new() { Id = 7, Estado = "Cancelado", Descripcion = "Cancelada" }
            };

        private List<ReparacionCategoria> GenerarCategoriasReparacion() => new()
            {
                new() { Id = 1, Categoria = "Sin clasificar" },
                new() { Id = 2, Categoria = "Plomería" },
                new() { Id = 3, Categoria = "Herreria" },
                new() { Id = 4, Categoria = "Electricidad" },
                new() { Id = 5, Categoria = "Pintura" },
                new() { Id = 6, Categoria = "Albañilería" },
                new() { Id = 7, Categoria = "Carpintería" }
            };

        private List<ContratoEstado> GenerarContratoEstados() => new()
            {
                new() { Id = 1, Descripcion = "Vigente" },
                new() { Id = 2, Descripcion = "Finalizado normal" },
                new() { Id = 3, Descripcion = "Finalizado antes de tiempo" }
            };

        private List<PagoEstado> GenerarPagoEstados() => new()
            {
                new() { Id = 1, Estado = "Pendiente" },
                new() { Id = 2, Estado = "Informado" },
                new() { Id = 3, Estado = "Vencido" },
                new() { Id = 4, Estado = "Impago" },
                new() { Id = 5, Estado = "Pagado" }
            };

        #endregion

        #region Generar Personas, Inmuebles y Empleados
        private IEnumerable<PersonaBase> GenerarPersonas()
        {
            var nombres = new[] { "Juan", "Ana", "Carlos", "María", "Pedro", "Lucía", "Javier", "Sofía", "Miguel", "Laura" };
            var apellidosP = new[] { "Pérez", "García", "López", "Martínez", "Rodríguez", "Fernández", "Sánchez", "Ramírez", "Torres", "Flores" };
            var apellidosM = new[] { "Gómez", "Díaz", "Morales", "Vargas", "Silva", "Castro", "Ramos", "Molina", "Ortega", "Suárez" };
            var nacionalidades = new[] { "Argentina", "Uruguaya", "Chilena", "Paraguaya", "Boliviana", "Peruana", "Colombiana", "Venezolana", "Mexicana", "Española" };

            for (int i = 1; i <= 20; i++)
            {
                var nombre = nombres[random.Next(nombres.Length)];
                var apellidoP = apellidosP[random.Next(apellidosP.Length)];
                var apellidoM = apellidosM[random.Next(apellidosM.Length)];
                var rfc = $"{nombre.Substring(0, 2).ToUpper()}{apellidoP.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}";
                var email = $"{nombre.ToLower()}.{apellidoP.ToLower()}{i}@mail.com";
                var nacionalidad = nacionalidades[random.Next(nacionalidades.Length)];

                yield return new PersonaBase
                {
                    PersonaId = i,
                    Nombre = nombre,
                    ApellidoPaterno = apellidoP,
                    ApellidoMaterno = apellidoM,
                    Rfc = rfc,
                    Email = email,
                    Nacionalidad = nacionalidad,
                    EsPersonaFisica = true,
                    EsTitular = random.Next(2) == 0,
                    Direcciones = new List<Direccion>
            {
                new Direccion
                {
                    DireccionId = i,
                    CalleNumero = $"Calle {random.Next(1, 200)} #{random.Next(1, 9999)}",
                    Colonia = "Centro",
                    Ciudad = "Ciudad Ejemplo",
                    Estado = "Estado Ejemplo",
                    CodigoPostal = $"{random.Next(10000, 99999)}",
                    Pais = "Argentina"
                }
            },
                    Telefonos = new List<Telefono>
            {
                new Telefono
                {
                    TelefonoId = i,
                    Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}",
                    Tipo = new TipoTelefono { TipoTelefonoId = 1, Descripcion = "Móvil" }
                }
            }
                };
            }
        }

        private IEnumerable<Inmueble> GenerarInmuebles()
        {
            for (int i = 1; i <= 15; i++)
            {
                List<Imagen> imagenes = GenerarImagenesInmueble(i).ToList();

                yield return new Inmueble
                {
                    Id = i,
                    Descripcion = $"Descripción inmueble #{i}",
                    Comentario = "Hermoso inmueble en excelente ubicación.",
                    Valor = random.Next(50000, 500000),
                    Superficie = random.Next(50, 500),
                    Construido = random.Next(30, 450),
                    Telefono = new Telefono { Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}" },
                    Estado = _estadosInmueble.First(e => e.Estado == "Disponible"),
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
                        new() { Id = 1, Nombre = "Habitaciones", Valor = random.Next(1, 5).ToString() },
                        new() { Id = 2, Nombre = "Baños", Valor = random.Next(1, 3).ToString() },
                        new() { Id = 3, Nombre = "Estacionamiento", Valor = random.Next(0, 2).ToString() }
                    },
                    Imagenes = imagenes,
                    ImagenPrincipalId = imagenes.First().Id
                };
            }
        }

        private IEnumerable<Empleado> GenerarEmpleados()
        {
            for (int i = 1; i <= 5; i++)
            {
                var persona = _personas[i - 1];
                yield return new Empleado
                {
                    EmpleadoId = i,
                    PersonaId = persona.PersonaId,
                    Nombre = persona.Nombre,
                    ApellidoPaterno = persona.ApellidoPaterno,
                    ApellidoMaterno = persona.ApellidoMaterno,
                    Rfc = persona.Rfc,
                    Email = persona.Email,
                    Nacionalidad = persona.Nacionalidad,
                    EsPersonaFisica = persona.EsPersonaFisica,
                    EsTitular = persona.EsTitular,
                    Direcciones = persona.Direcciones,
                    Telefonos = persona.Telefonos,
                    Especialidad = new EmpleadoEspecialidad { Especialidad = $"Especialidad #{i}" },
                    Agenda = new List<EmpleadoAgenda>(),
                    EsContratistaExterno = i % 2 == 0
                };
            }
        }

        private IEnumerable<Imagen> GenerarImagenesInmueble(int inmuebleId)
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

        #endregion

        private void GenerarContratosYPagos()
        {
            var personasDisponibles = _personas.ToList();
            var inmueblesDisponibles = _inmuebles.Where(i => i.Estado.Estado == "Disponible").ToList();
            int contratosACrear = Math.Min(personasDisponibles.Count, inmueblesDisponibles.Count);

            for (int i = 0; i < contratosACrear; i++)
            {
                var persona = personasDisponibles[i];
                var inmueble = inmueblesDisponibles[i];

                // Estado del contrato aleatorio
                var estadoContrato = _contratoEstados[random.Next(_contratoEstados.Count)];
                bool vigente = estadoContrato.Descripcion == "Vigente";

                // Crear inquilino a partir de persona
                var inquilino = new Inquilino
                {
                    Id = i + 1,
                    PersonaId = persona.PersonaId,
                    Nombre = persona.Nombre,
                    ApellidoPaterno = persona.ApellidoPaterno,
                    ApellidoMaterno = persona.ApellidoMaterno,
                    Rfc = persona.Rfc,
                    Email = persona.Email,
                    Nacionalidad = persona.Nacionalidad,
                    EsPersonaFisica = persona.EsPersonaFisica,
                    EsTitular = persona.EsTitular,
                    Direcciones = persona.Direcciones,
                    Telefonos = persona.Telefonos,
                    Inmueble = inmueble
                };

                _inquilinos.Add(inquilino);

                // Marcar inmueble como ocupado si el contrato está vigente
                inmueble.Estado = vigente
                    ? _estadosInmueble.First(e => e.Estado == "Ocupado")
                    : _estadosInmueble.First(e => e.Estado == "Disponible");

                if(vigente)
                {
                    inmueble.Inquilinos.Add(inquilino);
                }

                // Crear contrato
                var contrato = new Contrato
                {
                    Id = i + 1,
                    Inquilino = inquilino,
                    Inmueble = inmueble,
                    Administrador = new Administrador { Nombre = "Admin Ejemplo" },
                    FechaInicio = DateTime.Now.AddMonths(-random.Next(1, 12)),
                    FechaFin = DateTime.Now.AddMonths(random.Next(1, 12)),
                    MontoMensual = random.Next(1000, 5000),
                    Estado = estadoContrato,
                    Pagos = new List<Pago>()
                };
                _contratos.Add(contrato);

                // Generar pagos para el contrato
                int pagosACrear = vigente ? 12 : random.Next(1, 12);
                for (int j = 0; j < pagosACrear; j++)
                {
                    var estadoPago = contrato.Estado.Descripcion.StartsWith("Finalizado")
                        ? _pagoEstados.First(e => e.Estado == "Pagado")
                        : (j == 0 ? _pagoEstados.First(e => e.Estado == "Pagado") : _pagoEstados.First(e => e.Estado == "Pendiente"));

                    var pago = new Pago
                    {
                        Id = _pagos.Count + 1,
                        Contrato = contrato,
                        FechaVencimiento = contrato.FechaInicio.AddMonths(j),
                        Estado = estadoPago,
                        Descripcion = $"Cuota {j + 1} de {pagosACrear}",
                        DetallesPago = new List<DetallePago>
                {
                    new DetallePago { Descripcion = "Cuota mensual", Monto = contrato.MontoMensual }
                }
                    };
                    _pagos.Add(pago);
                    contrato.Pagos.Add(pago);
                }

                inmueble.Contratos.Add(contrato);
                if(vigente)
                {
                    inmueble.Pagos.AddRange(contrato.Pagos);
                }
            }
        }

        private IEnumerable<Reparacion> GenerarReparaciones()
        {
            for (int i = 1; i <= 10; i++)
            {
                var fechaSolicitud = DateTime.Now.AddDays(-random.Next(1, 30));
                var fechaInicio = fechaSolicitud.AddDays(random.Next(1, 10));
                var fechaFin = fechaInicio.AddDays(random.Next(1, 15));

                // Decidir si la reparación tiene empleado asignado
                bool tieneEmpleado = random.Next(1, 101) <= 70; // 70% sí, 30% no
                Empleado? empleado = tieneEmpleado ? _empleados[random.Next(0, _empleados.Count)] : null;
                int empleadoId = empleado?.EmpleadoId ?? 0;

                // Decidir si la reparación tiene detalles (solo si tiene empleado)
                List<ReparacionDetalle> detalles = new();
                bool tieneDetalles = tieneEmpleado && random.Next(0, 101) <= 60;

                if (tieneDetalles)
                {
                    detalles = new List<ReparacionDetalle>
                        {
                            new ReparacionDetalle
                            {
                                Id = 1,
                                Descripcion = "Descripcion trabajo realizado",
                                Costo = random.Next(500, 2000),
                                ACargoDeInquilino = true,
                                Fecha = fechaInicio.AddDays(1)
                            },
                            new ReparacionDetalle
                            {
                                Id = 2,
                                Descripcion = "Otra tarea realizada para la reparacion",
                                Costo = random.Next(300, 1500),
                                ACargoDeInquilino = true,
                                Fecha = fechaInicio.AddDays(2)
                            }
                        };
                }

                // Seleccionar inmueble
                int inmuebleIndex = random.Next(0, _inmuebles.Count);
                var inmueble = _inmuebles[inmuebleIndex];
                //Actualizar estado inmueble
                inmueble.Estado = _estadosInmueble.First(e => e.Estado == "En reparación");

                // Determinar estado y estadoId según reglas
                int estadoId;
                if (!tieneEmpleado)
                {
                    estadoId = 2; // "Pendiente sin asignar"
                }
                else if (tieneDetalles)
                {
                    estadoId = 3; // "En proceso"
                }
                else
                {
                    estadoId = 1; // "Pendiente"
                }

                // Control para FechaInicio y CostoEstimado según estado
                DateTime? fechaInicioFinal = (estadoId == 1 || estadoId == 2) ? null : fechaInicio;
                int? costoEstimado = (estadoId == 1 || estadoId == 2) ? null : random.Next(1000, 5000);

                yield return new Reparacion
                {
                    Id = i,
                    FechaSolicitud = fechaSolicitud,
                    FechaInicio = fechaInicioFinal,
                    FechaFin = tieneDetalles ? fechaFin : null,
                    Categoria = _reparacionCategorias[random.Next(0, _reparacionCategorias.Count)],
                    Descripcion = $"Reparación #{i}",
                    CostoEstimado = costoEstimado,
                    CostoFinal = tieneDetalles ? random.Next(1000, 5000) : null,
                    Inmueble = _inmuebles[inmuebleIndex],
                    InmuebleId = _inmuebles[inmuebleIndex].Id,
                    Estado = ObtenerEstadoReparacion(estadoId).Result,
                    EstadoId = estadoId,
                    Empleado = empleado,
                    EmpleadoId = empleadoId,
                    Detalles = detalles
                };
            }
        }

        #endregion

        #region Inmueble


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

        public async Task<IEnumerable<Inmueble>> ObtenerInmuebles() => await Task.FromResult(_inmuebles);
        public async Task<Inmueble> ObtenerInmueblePorId(int id)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == id);
            inmueble.Reparaciones = _reparaciones.Where(r=>r.InmuebleId == id).ToList();
            return inmueble;
        }

        #endregion

        #region Persona Base

        // Buscar personas por apellido o RFC
        public async Task<List<PersonaBase>> BuscarPersonasAsync(string apellido, string rfc)
        {
            var query = _personas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(apellido))
                query = query.Where(p => (p.ApellidoPaterno ?? "").Contains(apellido, StringComparison.OrdinalIgnoreCase)
                                      || (p.ApellidoMaterno ?? "").Contains(apellido, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(rfc))
                query = query.Where(p => p.Rfc.Contains(rfc, StringComparison.OrdinalIgnoreCase));

            return await Task.FromResult(query.ToList());
        }

        // Crear una nueva persona
        public async Task<PersonaBase> CrearPersonaAsync(PersonaBase persona)
        {
            persona.PersonaId = _personas.Any() ? _personas.Max(p => p.PersonaId) + 1 : 1;
            _personas.Add(persona);
            return await Task.FromResult(persona);
        }

        // Actualizar persona existente
        public async Task<PersonaBase> ActualizarPersonaAsync(PersonaBase persona)
        {
            var existente = _personas.FirstOrDefault(p => p.PersonaId == persona.PersonaId);
            if (existente != null)
            {
                existente.Nombre = persona.Nombre;
                existente.ApellidoPaterno = persona.ApellidoPaterno;
                existente.ApellidoMaterno = persona.ApellidoMaterno;
                existente.Rfc = persona.Rfc;
                existente.Email = persona.Email;
                existente.Nacionalidad = persona.Nacionalidad;
                existente.EsPersonaFisica = persona.EsPersonaFisica;
                existente.EsTitular = persona.EsTitular;
                existente.Direcciones = persona.Direcciones;
                existente.Telefonos = persona.Telefonos;
            }
            return await Task.FromResult(existente ?? persona);
        }

        // Eliminar persona por ID
        public async Task EliminarPersonaAsync(int personaId)
        {
            var persona = _personas.FirstOrDefault(p => p.PersonaId == personaId);
            if (persona != null)
                _personas.Remove(persona);
            await Task.CompletedTask;
        }

        // Obtener persona por ID
        public async Task<PersonaBase?> ObtenerPersonaPorIdAsync(int personaId)
        {
            var persona = _personas.FirstOrDefault(p => p.PersonaId == personaId);
            return await Task.FromResult(persona);
        }


        #endregion

        #region Reparacion

        public async Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion()
        {
            return await Task.FromResult(_reparacionEstados);
        }

        public async Task<ReparacionEstado> ObtenerEstadoReparacion(int estadoId)
        {
            return await Task.FromResult(_reparacionEstados.FirstOrDefault(e => e.Id == estadoId));
        }

        public async Task<IEnumerable<Reparacion>> ObtenerReparaciones() => await Task.FromResult(_reparaciones);
        public async Task<Reparacion?> ObtenerReparacionPorId(int id) => await Task.FromResult(_reparaciones.FirstOrDefault(r => r.Id == id));

        public async Task<IEnumerable<Empleado>> ObtenerEmpleados() => await Task.FromResult(_empleados);
        public async Task<Empleado?> ObtenerEmpleadoPorId(int id) => await Task.FromResult(_empleados.FirstOrDefault(e => e.EmpleadoId == id));

        public async Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId)
        {
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == inmuebleId);
            if (inmueble != null)
            {
                return await Task.FromResult(inmueble.Reparaciones);
            }
            return new List<Reparacion>();
        }

        private async Task ActualizarEstadoReparacion(int nuevoEstadoId, string observacion, Reparacion reparacion)
        {
            var nuevoEstado = await ObtenerEstadoReparacion(nuevoEstadoId);

            // Agregar al historial
            reparacion.HistorialEstados.Add(new ReparacionEstadoHistorial
            {
                ReparacionId = reparacion.Id,
                EstadoId = nuevoEstado.Id,
                Estado = nuevoEstado,
                FechaCambio = DateTime.Now,
                Observacion = observacion
            });

            // Actualizar estado
            reparacion.EstadoId = nuevoEstado.Id;
            reparacion.Estado = nuevoEstado;
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

                reparacion.Empleado = _empleados.FirstOrDefault(e => e.EmpleadoId == reparacion.EmpleadoId);

                await ActualizarEstadoReparacion(1, "Alta reparación", reparacion); //pendiente

                // Agregar la reparación a la lista global
                _reparaciones.Add(reparacion);

                // Agregar la reparación al inmueble
                inmueble.Reparaciones.Add(reparacion);
            }
            await Task.CompletedTask;
        }

        public async Task<bool> RechazarReparacion(int reparacionId, int empleadoId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;
            Empleado empleado = _empleados.First(e => e.EmpleadoId == empleadoId);

            await ActualizarEstadoReparacion(2, $"{empleado.TipoEmpleado} rechazó solicitud", reparacion); //Pendiente sin asignar
            reparacion.EmpleadoId = 0;
            reparacion.Empleado = null;

            return true;
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
                reparacionExistente.Empleado = await ObtenerEmpleadoPorId(reparacion.EmpleadoId);
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

        public async Task<IEnumerable<ReparacionCategoria>> ObtenerCategoriasReparacion()
        {
            return await Task.FromResult(_reparacionCategorias);
        }

        public async Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            reparacion.EmpleadoId = empleadoId;
            reparacion.Empleado = _empleados.First(e => e.EmpleadoId == empleadoId);
            reparacion.CostoEstimado = costoEstimado;
            reparacion.FechaInicio = fechaInicio;

            await ActualizarEstadoReparacion(3, $"{reparacion.Empleado.TipoEmpleado} aceptó la solicitud", reparacion); //en proceso

            return true;
        }

        public async Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            detalle.Id = reparacion.Detalles.Any() ? reparacion.Detalles.Max(d => d.Id) + 1 : 1;
            detalle.Reparacion = reparacion;
            detalle.Fecha = DateTime.Now;
            detalle.ACargoDeInquilino = reparacion.Inmueble.Inquilinos.Any(); //si tiene inquilinos activos
            detalle.ACargoDePropietario = !detalle.ACargoDeInquilino;
            reparacion.Detalles.Add(detalle);

            // Actualizar estado si es la primera vez que se agrega un detalle
            await ActualizarEstadoReparacion(3, "Se agregó un detalle de reparación", reparacion); //en proceso

            return true;
        }

        public async Task<bool> DisputarDetalle(int reparacionId, int detalleId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            var detalle = reparacion.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle == null) return false;

            detalle.ACargoDePropietario = true;
            detalle.ACargoDeInquilino = false;
            detalle.Disputada = true;

            await ActualizarEstadoReparacion(4, "El inquilino disputo detalle", reparacion);//en disputa

            return true;
        }

        public async Task<bool> ResolverDisputa(int reparacionId, int detalleId, bool aceptarDisputa)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            var detalle = reparacion.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle == null) return false;

            detalle.Disputada = false; // Marcar como no disputada
            detalle.ACargoDePropietario = aceptarDisputa;
            detalle.ACargoDeInquilino = !aceptarDisputa;

            // Volver al estado anterior (En proceso) si no existe otro detalle con disputa
            if (reparacion.Detalles.All(d => !d.Disputada))
            {
                await ActualizarEstadoReparacion(3, "Administrador resolvio disputa de un detalle", reparacion); //en proceso
            }
            else
            {
                await ActualizarEstadoReparacion(4, "Administrador resolvio disputa de un detalle", reparacion); //en disputa
            }

            return true;
        }

        public async Task<bool> FinalizarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            Empleado empleado = _empleados.First(e => e.EmpleadoId == reparacion.EmpleadoId);
            await ActualizarEstadoReparacion(5, $"{empleado.TipoEmpleado} indica que terminó reparación", reparacion); // finalizado por aprobar
            reparacion.FechaFin = DateTime.Now;
            reparacion.CostoFinal = reparacion.Detalles.Sum(d => d.Costo);

            return true;
        }

        public async Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            await ActualizarEstadoReparacion(6, "Se aprobo la reparación realizada por el profesional", reparacion); //finalizado

            calificacion.Id = _calificaciones.Any() ? _calificaciones.Max(c => c.Id) + 1 : 1;
            calificacion.ReparacionId = reparacionId;
            calificacion.EmpleadoId = reparacion.EmpleadoId;
            _calificaciones.Add(calificacion);

            return true;
        }

        public async Task<bool> DesaprobarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;

            await ActualizarEstadoReparacion(3, "Se rechazó la reparación realizada por el profesional", reparacion); //en proceso

            return true;
        }

        public async Task<bool> CancelarReparacion(int reparacionId, int adminId, string motivoCancelacion)
        {
            var reparacion = _reparaciones.FirstOrDefault(r =>
                                                            r.Id == reparacionId &&
                                                            r.EstadoId == 2 // Solo se puede cancelar si está pendiente sin asignar
                        );
            if (reparacion == null) return false;

            await ActualizarEstadoReparacion(7, "Administrador cancela la reparación", reparacion); //Cancelado
            reparacion.MotivoCancelacion = motivoCancelacion;
            reparacion.FechaCancelacion = DateTime.Now;
            reparacion.CanceladoPorId = adminId;
            return true;
        }

        public async Task<bool> AsignarEmpleado(int reparacionId, int empleadoId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null) return false;
            reparacion.EmpleadoId = empleadoId;
            reparacion.Empleado = _empleados.First(e => e.EmpleadoId == empleadoId);
            await ActualizarEstadoReparacion(1, "Administrador asigna un profesional", reparacion); //Pendiente
            return true;
        }

        public async Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId)
        {
            return await Task.FromResult(_calificaciones.Where(c => c.EmpleadoId == empleadoId));
        }


        #endregion

        #region Alquileres y reservas

        public async Task<IEnumerable<Contrato>> ObtenerContratos() => await Task.FromResult(_contratos);
        public async Task<Contrato?> ObtenerContratoPorId(int id) => await Task.FromResult(_contratos.FirstOrDefault(c => c.Id == id));

        public async Task<IEnumerable<Inquilino>> ObtenerInquilinos() => await Task.FromResult(_inquilinos);
        public async Task<Inquilino?> ObtenerInquilinoPorId(int id) => await Task.FromResult(_inquilinos.FirstOrDefault(i => i.Id == id));

        public async Task<IEnumerable<Pago>> ObtenerPagos() => await Task.FromResult(_pagos);
        public async Task<Pago?> ObtenerPagoPorId(int id) => await Task.FromResult(_pagos.FirstOrDefault(p => p.Id == id));

        public async Task<Reserva> GuardarReservaAsync(Reserva reserva)
        {
            reserva.Id = _reservas.Any() ? _reservas.Max(r => r.Id) + 1 : 1;
            reserva.FechaCreacion = DateTime.Now;
            _reservas.Add(reserva);

            // Asociar la reserva al inmueble
            var inmueble = _inmuebles.FirstOrDefault(i => i.Id == reserva.Inmueble.Id);
            if (inmueble != null)
            {
                inmueble.Reserva = reserva;
                inmueble.Estado = _estadosInmueble.First(e => e.Estado == "Reservado");
            }

            return await Task.FromResult(reserva);
        }

        public async Task<Reserva?> ObtenerReservaPorInmuebleIdAsync(int inmuebleId)
        {
            return await Task.FromResult(_reservas.FirstOrDefault(r => r.Inmueble.Id == inmuebleId));
        }

        #endregion
    }
}