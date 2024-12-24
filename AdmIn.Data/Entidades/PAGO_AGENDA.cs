using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class PAGO_AGENDA
    {
        public int PAGO_ID { get; set; } // ID de la agenda de pago
        public int CON_ID { get; set; } // ID del contrato
        public DateTime PAGO_FECHA_VENCIMIENTO { get; set; } // Fecha de vencimiento del pago
        public decimal PAGO_MONTO { get; set; } // Monto del pago
        public DateTime? PAGO_FECHA_NOTIFICACION { get; set; } // Fecha de notificación del pago
        public DateTime? PAGO_FECHA_REALIZADO { get; set; } // Fecha en la que se realizó el pago
        public int PAGO_ESTADO_ID { get; set; } // ID del estado del pago
    }
}
