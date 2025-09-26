using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using AdmIn.API.Hubs;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionController : ControllerBase
    {
        private readonly IServ_Notificacion _serv;
        private readonly IHubContext<NotificationHub> _hub;
        public NotificacionController(IServ_Notificacion serv, IHubContext<NotificationHub> hub)
        {
            _serv = serv;
            _hub = hub;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<DTO<Notificacion>> Crear([FromBody] Notificacion notificacion)
        {
            var res = await _serv.Crear(notificacion);
            if (res.Correcto && res.Datos != null)
            {
                // Enviar notificación por SignalR al usuario especificado (grupo por usuario id)
                await _hub.Clients.User(notificacion.UsuarioId.ToString()).SendAsync("NewNotification", res.Datos);
            }
            return res;
        }

        [HttpPost("marcar_leida/{id}")]
        [Authorize]
        public async Task<DTO<bool>> MarcarLeida(int id)
            => await _serv.MarcarComoLeida(id);

        [HttpGet("obtener_por_usuario/{usuarioId}")]
        [Authorize]
        public async Task<DTO<IEnumerable<Notificacion>>> ObtenerPorUsuario(int usuarioId)
            => await _serv.Obtener_por_usuario(usuarioId);

        [HttpGet("contar_no_leidas/{usuarioId}")]
        [Authorize]
        public async Task<DTO<int>> ContarNoLeidas(int usuarioId)
            => await _serv.Contar_no_leidas(usuarioId);
    }
}
