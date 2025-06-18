using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IReparacionService // Renamed from IMockReparacionService
    {
        Task<IEnumerable<Reparacion>> ObtenerReparaciones();
        Task<Reparacion?> ObtenerReparacionPorId(int id);
        Task<IEnumerable<ReparacionEstado>> ObtenerEstadosReparacion();
        Task<ReparacionEstado?> ObtenerEstadoReparacionPorId(int id);
        Task<IEnumerable<ReparacionCategoria>> ObtenerCategoriasReparacion();
        Task<IEnumerable<Reparacion>> ObtenerReparacionesPorInmueble(int inmuebleId);
        Task ActualizarReparacion(Reparacion reparacion);
        Task EliminarReparacion(int reparacionId);
        Task AgregarReparacion(int inmuebleId, Reparacion reparacion);
        Task<bool> AceptarReparacion(int reparacionId, int empleadoId, decimal costoEstimado, DateTime fechaInicio);
        Task<bool> RechazarReparacion(int reparacionId, int empleadoId);
        Task<bool> AgregarDetalleReparacion(int reparacionId, ReparacionDetalle detalle);
        Task<bool> DisputarDetalle(int reparacionId, int detalleId);
        Task<bool> ResolverDisputa(int reparacionId, int detalleId, bool aceptarDisputa);
        Task<bool> FinalizarReparacion(int reparacionId);
        Task<bool> AprobarReparacion(int reparacionId, EmpleadoCalificacion calificacion);
        Task<bool> DesaprobarReparacion(int reparacionId);
        Task<bool> CancelarReparacion(int reparacionId, int adminId, string motivoCancelacion);
        Task<bool> AsignarEmpleado(int reparacionId, int empleadoId);
    }
}
