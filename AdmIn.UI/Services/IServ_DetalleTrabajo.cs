using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_DetalleTrabajo : IServicioBase<DetalleTrabajo>
    {
        // Reuse base methods from IServicioBase
        Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_por_trabajo(int trabajoId);
    }
}
