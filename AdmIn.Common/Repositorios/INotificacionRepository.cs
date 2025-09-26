using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Common.Repositorios
{
    public interface INotificacionRepository
    {
        Task<DTO<Notificacion>> Crear(Notificacion notificacion);
        Task<DTO<bool>> MarcarComoLeida(int notificacionId);
        Task<DTO<IEnumerable<Notificacion>>> Obtener_por_usuario(int usuarioId);
        Task<DTO<int>> Contar_no_leidas(int usuarioId);
    }
}
