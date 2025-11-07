using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.Business.Servicios
{
 public interface IServ_Reserva
 {
 Task<DTO<Reserva>> CrearReserva(int inmuebleId, int usuarioReservadorId, decimal costo, DateTime? fechaVencimiento = null, int? monedaId = null, int usuarioCreadorId =0);
 Task<DTO<bool>> ExpirarReservas();
 Task<DTO<Reserva>> ConvertirAContrato(int reservaId, bool descontarCostoAlContrato, ContratoRenta contratoParametros, int usuarioId);
 }
}
