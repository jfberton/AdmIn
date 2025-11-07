using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
 public enum ReservaEstado
 {
 Activa =0,
 Convertida =1,
 Vencida =2,
 Cancelada =3
 }

 public class Reserva
 {
 public int Id { get; set; }

 // FK al inmueble reservado
 public int InmuebleId { get; set; }

 // Usuario (inquilino potencial) que realiza la reserva
 public int UsuarioReservadorId { get; set; }

 public DateTime FechaCreacion { get; set; } = DateTime.Now;

 // Por defecto vence en1 mes
 public DateTime FechaVencimiento { get; set; } = DateTime.Now.AddMonths(1);

 // Costo de la reserva
 public decimal Costo { get; set; }

 // Opcional: moneda
 public int? MonedaId { get; set; }

 // Estado de la reserva
 public ReservaEstado Estado { get; set; } = ReservaEstado.Activa;

 // Si se convirtió en contrato, referencia opcional
 public int? ContratoRentaId { get; set; }

 // Indica si el costo de la reserva fue aplicado/descontado al crear el contrato
 public bool AplicadoAlContrato { get; set; } = false;
 public decimal? MontoAplicadoAlContrato { get; set; }

 public DateTime FechaModificacion { get; set; } = DateTime.Now;
 public int? UsuarioCreadorId { get; set; }
 public int? UsuarioModificadorId { get; set; }

 #region Propiedades de navegación
 public Inmueble? Inmueble { get; set; }
 public Usuario? UsuarioReservador { get; set; }
 public ContratoRenta? ContratoRenta { get; set; }
 public Moneda? Moneda { get; set; }
 #endregion
 }
}
