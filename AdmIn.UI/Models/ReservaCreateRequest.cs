using System;

namespace AdmIn.UI.Models
{
 public class ReservaCreateRequest
 {
 public int InmuebleId { get; set; }
 public int UsuarioReservadorId { get; set; }
 public decimal Costo { get; set; }
 public DateTime? FechaVencimiento { get; set; }
 public int? MonedaId { get; set; }

 public ReservaCreateRequest() { }

 public ReservaCreateRequest(int inmuebleId, int usuarioReservadorId, decimal costo, DateTime? fechaVencimiento, int? monedaId)
 {
 InmuebleId = inmuebleId;
 UsuarioReservadorId = usuarioReservadorId;
 Costo = costo;
 FechaVencimiento = fechaVencimiento;
 MonedaId = monedaId;
 }
 }
}
