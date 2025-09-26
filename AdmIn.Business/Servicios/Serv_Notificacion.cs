using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public class Serv_Notificacion : IServ_Notificacion
    {
        private readonly INotificacionRepository _repo;
        public Serv_Notificacion(INotificacionRepository repo)
        {
            _repo = repo;
        }

        public Task<DTO<Notificacion>> Crear(Notificacion notificacion) => _repo.Crear(notificacion);
        public Task<DTO<bool>> MarcarComoLeida(int notificacionId) => _repo.MarcarComoLeida(notificacionId);
        public Task<DTO<IEnumerable<Notificacion>>> Obtener_por_usuario(int usuarioId) => _repo.Obtener_por_usuario(usuarioId);
        public Task<DTO<int>> Contar_no_leidas(int usuarioId) => _repo.Contar_no_leidas(usuarioId);
    }
}
