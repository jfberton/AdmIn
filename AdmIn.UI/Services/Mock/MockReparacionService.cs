using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockReparacionService : IReparacionService
    {
        private readonly List<Reparacion> _reparaciones;
        private readonly List<ReparacionEstado> _reparacionEstados;
        private readonly List<ReparacionCategoria> _reparacionCategorias;

        private readonly IInmuebleService _mockInmuebleService; 
        private readonly IEmpleadoService _mockEmpleadoService;
        private static readonly Random random = new Random();

        public MockReparacionService(IInmuebleService mockInmuebleService, IEmpleadoService mockEmpleadoService) 
        {
            _mockInmuebleService = mockInmuebleService;
            _mockEmpleadoService = mockEmpleadoService;

            _reparacionEstados = GenerarReparacionEstados().ToList();
            _reparacionCategorias = GenerarCategoriasReparacion().ToList();

            var inmuebles = _mockInmuebleService.ObtenerInmuebles().Result.ToList();
            var empleados = _mockEmpleadoService.ObtenerEmpleados().Result.ToList();
            var estadosInmueble = _mockInmuebleService.ObtenerEstadosInmueble().Result.ToList();


            _reparaciones = GenerarReparaciones(inmuebles, empleados, estadosInmueble).ToList();
        }

        private List<ReparacionEstado> GenerarReparacionEstados() => new List<ReparacionEstado>
        {
            new() { Id = 1, Estado = "Pendiente", Descripcion = "Pendiente de aceptar" },
            new() { Id = 2, Estado = "Pendiente sin asignar", Descripcion = "Rechazada por el empleado o sin asignar" },
            new() { Id = 3, Estado = "En proceso", Descripcion = "Aceptada y en proceso" },
            new() { Id = 4, Estado = "En disputa", Descripcion = "Detalle en disputa" },
            new() { Id = 5, Estado = "Finalizado por aprobar", Descripcion = "Finalizada, pendiente de aprobación" },
            new() { Id = 6, Estado = "Finalizado", Descripcion = "Finalizada y aprobada" },
            new() { Id = 7, Estado = "Cancelado", Descripcion = "Cancelada" }
        };

        private List<ReparacionCategoria> GenerarCategoriasReparacion() => new List<ReparacionCategoria>
        {
            new() { Id = 1, Categoria = "Sin clasificar" },
            new() { Id = 2, Categoria = "Plomería" },
            new() { Id = 3, Categoria = "Herreria" },
            new() { Id = 4, Categoria = "Electricidad" },
            new() { Id = 5, Categoria = "Pintura" },
            new() { Id = 6, Categoria = "Albañilería" },
            new() { Id = 7, Categoria = "Carpintería" }
        };

        private IEnumerable<Reparacion> GenerarReparaciones(List<Inmueble> inmuebles, List<Empleado> empleados, List<EstadoInmueble> estadosInmueble)
        {
            if (!inmuebles.Any()) yield break;

            var estadoEnReparacion = estadosInmueble.FirstOrDefault(e => e.Estado == "En reparación");
            if (estadoEnReparacion == null) yield break;


            for (int i = 1; i <= 10; i++)
            {
                var fechaSolicitud = DateTime.Now.AddDays(-random.Next(1, 30));
                var fechaInicio = fechaSolicitud.AddDays(random.Next(1, 10));

                bool tieneEmpleado = empleados.Any() && random.Next(1, 101) <= 70;
                Empleado? empleado = tieneEmpleado ? empleados[random.Next(0, empleados.Count)] : null;

                int estadoId;
                DateTime? fechaInicioFinal;
                int? costoEstimado;

                if (!tieneEmpleado) 
                {
                    estadoId = 2; 
                    fechaInicioFinal = null;
                    costoEstimado = null;
                }
                else 
                {
                    estadoId = 1; 
                    fechaInicioFinal = null; 
                    costoEstimado = null; 
                }

                var estadoActual = _reparacionEstados.First(e => e.Id == estadoId);

                var inmuebleParaReparar = inmuebles[random.Next(0, inmuebles.Count)];
                if(inmuebleParaReparar.Estado.Estado != "En reparación")
                {
                    inmuebleParaReparar.Estado = estadoEnReparacion;
                }


                var reparacion = new Reparacion
                {
                    Id = i,
                    FechaSolicitud = fechaSolicitud,
                    FechaInicio = fechaInicioFinal,
                    Categoria = _reparacionCategorias[random.Next(0, _reparacionCategorias.Count)],
                    Descripcion = $"Reparación #{i} para {inmuebleParaReparar.Descripcion}",
                    CostoEstimado = costoEstimado,
                    Inmueble = inmuebleParaReparar,
                    InmuebleId = inmuebleParaReparar.Id,
                    Estado = estadoActual,
                    EstadoId = estadoActual.Id,
                    Empleado = empleado,
                    EmpleadoId = empleado?.EmpleadoId ?? 0, 
                    Detalles = new List<ReparacionDetalle>(),
                    HistorialEstados = new List<ReparacionEstadoHistorial>()
                };
                reparacion.HistorialEstados.Add(new ReparacionEstadoHistorial{ Estado = estadoActual, EstadoId = estadoActual.Id, FechaCambio = DateTime.Now, Observacion="Creación inicial"});

                if(inmuebleParaReparar.Reparaciones == null) inmuebleParaReparar.Reparaciones = new List<Reparacion>();
                inmuebleParaReparar.Reparaciones.Add(reparacion);

                yield return reparacion;
            }
        }

        private async Task ActualizarEstadoReparacionInterno(Reparacion reparacion, int nuevoEstadoId, string observacion)
        {
            var nuevoEstado = _reparacionEstados.FirstOrDefault(e => e.Id == nuevoEstadoId);
            if (nuevoEstado == null) return;

            reparacion.HistorialEstados.Add(new ReparacionEstadoHistorial
            {
                ReparacionId = reparacion.Id,
                EstadoId = nuevoEstado.Id,
                Estado = nuevoEstado,
                FechaCambio = DateTime.Now,
                Observacion = observacion
            });
            reparacion.EstadoId = nuevoEstado.Id;
            reparacion.Estado = nuevoEstado;
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Reparacion>> ObtenerReparaciones() => await Task.FromResult(_reparaciones);
        public async Task<Reparacion?> ObtenerReparacionPorId(int id) => await Task.FromResult(_reparaciones.FirstOrDefault(r => r.Id == id));
        public async Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion() => await Task.FromResult(_reparacionEstados);
        public async Task<ReparacionEstado?> ObtenerEstadoReparacionPorId(int id) => await Task.FromResult(_reparacionEstados.FirstOrDefault(e => e.Id == id));
        public async Task<IEnumerable<ReparacionCategoria>> ObtenerCategoriasReparacion() => await Task.FromResult(_reparacionCategorias);

        public async Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId)
        {
            var inmueble = await _mockInmuebleService.ObtenerInmueblePorId(inmuebleId);
            if (inmueble != null)
            {
                return await Task.FromResult(_reparaciones.Where(r => r.InmuebleId == inmuebleId));
            }
            return new List<Reparacion>();
        }

        public async Task ActualizarReparacion(Reparacion reparacion)
        {
            var index = _reparaciones.FindIndex(r => r.Id == reparacion.Id);
            if (index != -1)
            {
                _reparaciones[index] = reparacion;
                if (reparacion.EmpleadoId != 0 && reparacion.Empleado == null)
                {
                    reparacion.Empleado = await _mockEmpleadoService.ObtenerEmpleadoPorId(reparacion.EmpleadoId);
                }
            }
            await Task.CompletedTask;
        }

        public async Task EliminarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion != null)
            {
                _reparaciones.Remove(reparacion);
                var inmueble = await _mockInmuebleService.ObtenerInmueblePorId(reparacion.InmuebleId);
                inmueble?.Reparaciones.RemoveAll(r => r.Id == reparacionId);
            }
            await Task.CompletedTask;
        }

        public async Task AgregarReparacion(int inmuebleId, Reparacion reparacion)
        {
            var inmueble = await _mockInmuebleService.ObtenerInmueblePorId(inmuebleId);
            if (inmueble == null) return;

            reparacion.Id = _reparaciones.Any() ? _reparaciones.Max(r => r.Id) + 1 : 1;
            reparacion.Inmueble = inmueble;
            reparacion.InmuebleId = inmueble.Id;

            if(reparacion.EmpleadoId != 0) 
                reparacion.Empleado = await _mockEmpleadoService.ObtenerEmpleadoPorId(reparacion.EmpleadoId);

            int initialStateId = reparacion.EmpleadoId != 0 ? 1 : 2;
            await ActualizarEstadoReparacionInterno(reparacion, initialStateId, "Reparación creada.");

            _reparaciones.Add(reparacion);
            if (inmueble.Reparaciones == null) inmueble.Reparaciones = new List<Reparacion>();
            inmueble.Reparaciones.Add(reparacion);

            // Update inmueble state to "En reparación"
            var estadoEnReparacion = (await _mockInmuebleService.ObtenerEstadosInmueble()).FirstOrDefault(e => e.Estado == "En reparación");
            if (estadoEnReparacion != null && inmueble.Estado.Id != estadoEnReparacion.Id)
            {
                inmueble.Estado = estadoEnReparacion;
            }
        }

        public async Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 1) return false; // Must be "Pendiente"

            var empleado = await _mockEmpleadoService.ObtenerEmpleadoPorId(empleadoId);
            if (empleado == null) return false;

            reparacion.EmpleadoId = empleadoId;
            reparacion.Empleado = empleado;
            reparacion.CostoEstimado = costoEstimado;
            reparacion.FechaInicio = fechaInicio;
            await ActualizarEstadoReparacionInterno(reparacion, 3, $"{empleado.TipoEmpleado} aceptó. Costo est: {costoEstimado}, Inicio: {fechaInicio.ToShortDateString()}"); // En Proceso
            return true;
        }

        public async Task<bool> RechazarReparacion(int reparacionId, int empleadoId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 1 || reparacion.EmpleadoId != empleadoId) return false;

            var empleado = await _mockEmpleadoService.ObtenerEmpleadoPorId(empleadoId);
            string empleadoNombre = empleado?.Nombre ?? $"Empleado Id {empleadoId}";

            await ActualizarEstadoReparacionInterno(reparacion, 2, $"{empleadoNombre} rechazó la solicitud."); 
            reparacion.EmpleadoId = 0; 
            reparacion.Empleado = null;
            reparacion.FechaInicio = null;
            reparacion.CostoEstimado = null;
            return true;
        }

        public async Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || (reparacion.EstadoId != 3 && reparacion.EstadoId != 4)) return false;

            detalle.Id = reparacion.Detalles.Any() ? reparacion.Detalles.Max(d => d.Id) + 1 : 1;
            detalle.Reparacion = reparacion;
            detalle.Fecha = DateTime.Now;
            detalle.ACargoDeInquilino = reparacion.Inmueble.Inquilinos != null && reparacion.Inmueble.Inquilinos.Any();
            detalle.ACargoDePropietario = !detalle.ACargoDeInquilino;
            reparacion.Detalles.Add(detalle);

            if(reparacion.EstadoId != 4)
                 await ActualizarEstadoReparacionInterno(reparacion, 3, "Detalle de reparación agregado."); 
            return true;
        }

        public async Task<bool> DisputarDetalle(int reparacionId, int detalleId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 3) return false;

            var detalle = reparacion.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle == null || detalle.Disputada) return false;

            detalle.ACargoDePropietario = true; 
            detalle.ACargoDeInquilino = false;
            detalle.Disputada = true;
            await ActualizarEstadoReparacionInterno(reparacion, 4, $"Detalle ID {detalleId} en disputa."); 
            return true;
        }

        public async Task<bool> ResolverDisputa(int reparacionId, int detalleId, bool aceptarDisputaDelInquilino)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 4) return false;

            var detalle = reparacion.Detalles.FirstOrDefault(d => d.Id == detalleId && d.Disputada);
            if (detalle == null) return false;

            detalle.Disputada = false;
            detalle.ACargoDePropietario = aceptarDisputaDelInquilino; 
            detalle.ACargoDeInquilino = !aceptarDisputaDelInquilino;

            if (!reparacion.Detalles.Any(d => d.Disputada))
            {
                await ActualizarEstadoReparacionInterno(reparacion, 3, $"Disputa del detalle ID {detalleId} resuelta."); 
            }
            return true;
        }

        public async Task<bool> FinalizarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 3 || !reparacion.Detalles.Any()) return false;

            await ActualizarEstadoReparacionInterno(reparacion, 5, "Reparación finalizada por empleado, pendiente aprobación."); 
            reparacion.FechaFin = DateTime.Now;
            reparacion.CostoFinal = reparacion.Detalles.Sum(d => d.Costo);
            return true;
        }

        public async Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 5) return false;

            await ActualizarEstadoReparacionInterno(reparacion, 6, "Reparación aprobada por administrador."); 

            if (reparacion.EmpleadoId != 0) 
            {
                calificacion.ReparacionId = reparacionId;
                calificacion.EmpleadoId = reparacion.EmpleadoId; 
                await _mockEmpleadoService.AddCalificacionAsync(calificacion);
            }
            return true;
        }

        public async Task<bool> DesaprobarReparacion(int reparacionId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 5) return false;

            await ActualizarEstadoReparacionInterno(reparacion, 3, "Reparación desaprobada, vuelve a 'En Proceso'.");
            reparacion.FechaFin = null;
            reparacion.CostoFinal = null;
            return true;
        }

        public async Task<bool> CancelarReparacion(int reparacionId, int adminId, string motivoCancelacion)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || (reparacion.EstadoId != 1 && reparacion.EstadoId != 2)) return false;

            await ActualizarEstadoReparacionInterno(reparacion, 7, $"Reparación cancelada por Admin ID {adminId}. Motivo: {motivoCancelacion}"); 
            reparacion.MotivoCancelacion = motivoCancelacion;
            reparacion.FechaCancelacion = DateTime.Now;
            reparacion.CanceladoPorId = adminId;
            return true;
        }

        public async Task<bool> AsignarEmpleado(int reparacionId, int empleadoId)
        {
            var reparacion = _reparaciones.FirstOrDefault(r => r.Id == reparacionId);
            if (reparacion == null || reparacion.EstadoId != 2) return false;

            var empleado = await _mockEmpleadoService.ObtenerEmpleadoPorId(empleadoId);
            if (empleado == null) return false;

            reparacion.EmpleadoId = empleadoId;
            reparacion.Empleado = empleado;
            await ActualizarEstadoReparacionInterno(reparacion, 1, $"Empleado {empleado.Nombre} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno} asignado por administrador."); // Pendiente (de aceptación del empleado)
            return true;
        }
    }
}
