using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.Business.Servicios
{
    public interface IServ_Rol : IServicioBase<Rol>
    {
        Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId);
    }
}
