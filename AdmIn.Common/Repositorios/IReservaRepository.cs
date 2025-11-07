using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
 public interface IReservaRepository
 {
 Task<DTO<Reserva>> Crear(Reserva reserva);
 Task<DTO<Reserva>> Obtener_por_id(int id);
 Task<DTO<Reserva>> Actualizar(Reserva reserva);
 Task<DTO<IEnumerable<Reserva>>> Obtener_activas_por_inmueble(int inmuebleId);
 Task<DTO<IEnumerable<Reserva>>> Obtener_vencidas(DateTime ahora);
 }
}
