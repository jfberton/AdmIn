using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
 public interface IServ_ReservaUi
 {
 Task<DTO<Reserva>> Crear(ReservaCreateRequest req);
 Task<DTO<Reserva>> Obtener_por_id(int id);
 Task<DTO<IEnumerable<Reserva>>> Obtener_activas_por_inmueble(int inmuebleId);
 Task<DTO<Reserva>> Actualizar(Reserva reserva);
 }
}
