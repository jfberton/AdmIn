using AdmIn.Business.Entidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Permiso : IServicioBase<Permiso>
    {
        Task<DTO<IEnumerable<Permiso>>> Obtener_por_rol(int rolId);
    }

}
