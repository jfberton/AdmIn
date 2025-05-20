using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class DetallePago
    {
        public int Id { get; set; }

        // Descripción del origen del monto (ej: "Cuota mensual", "Depósito de garantía", "Reparación")
        public string Descripcion { get; set; } = string.Empty;

        // Monto asociado a este detalle
        public decimal Monto { get; set; }
    }

    public class DetalleDistribucion
    {
        public int Id { get; set; }

        // Descripción de cómo se distribuye el monto (ej: "70% para el propietario", "20% para mantenimiento")
        public string Descripcion { get; set; } = string.Empty;

        // Monto asociado a esta distribución
        public decimal Monto { get; set; }
    }

    public class Pago
    {
        public int Id { get; set; }

        // Referencia al contrato del pago
        public Contrato Contrato { get; set; }

        // Fecha en la que vence el pago
        public DateTime FechaVencimiento { get; set; }

        // Fecha en la que se realizó el pago (puede ser null si no se ha pagado aún)
        public DateTime? FechaPago { get; set; }

        // Monto total del pago (calculado como la suma de los montos en DetallePago)
        public decimal Monto => DetallesPago.Sum(d => d.Monto);

        // Estado del pago (Pendiente, Pagado, Vencido, etc.)
        public PagoEstado Estado { get; set; }

        // Fecha en la que se notificó al inquilino (null si aún no se notificó)
        public DateTime? FechaNotificacion { get; set; }

        // Para indicar a qué se debe el pago, ej: Cuota 1 de 12, Reparación, Firma de contrato
        public string Descripcion { get; set; } = string.Empty;

        // Lista de detalles que describen cómo surge el monto a pagar
        public List<DetallePago> DetallesPago { get; set; } = new();

        // Lista de detalles que describen cómo se distribuye el monto
        public List<DetalleDistribucion> DetallesDistribucion { get; set; } = new();
    }

    public class PagoEstado
    {
        public int Id { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class Contrato
    {
        public int Id { get; set; }

        // Inquilino asociado al contrato
        public Inquilino Inquilino { get; set; }

        // Inmueble que se arrienda en el contrato
        public Inmueble Inmueble { get; set; }

        // Administrador responsable del contrato
        public Administrador Administrador { get; set; }

        // Fecha de inicio del contrato
        public DateTime FechaInicio { get; set; }

        // Fecha de finalización del contrato
        public DateTime FechaFin { get; set; }

        // Monto mensual del contrato
        public decimal MontoMensual { get; set; }

        // Estado actual del contrato
        public ContratoEstado Estado { get; set; }

        // Observacion del contrato
        public string Observacion { get; set; }

        // Lista de pagos programados (Agenda de pagos)
        public List<Pago> Pagos { get; set; } = new();
    }

    public class ContratoEstado
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
