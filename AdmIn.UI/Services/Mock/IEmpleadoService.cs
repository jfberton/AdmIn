using AdmIn.Business.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IEmpleadoService
    {
        Task<IEnumerable<Empleado>> ObtenerEmpleados();
        Task<Empleado?> ObtenerEmpleadoPorId(int id);
        Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId);
        Task AddCalificacionAsync(EmpleadoCalificacion calificacion);
    }
}
