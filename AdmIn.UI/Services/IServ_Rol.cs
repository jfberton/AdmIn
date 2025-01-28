using AdmIn.Business.Entidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Rol : IServicioBase<Rol>
    {
        Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId);

        Task<DTO<IEnumerable<Rol>>> Obtener_todos();
    }

}
