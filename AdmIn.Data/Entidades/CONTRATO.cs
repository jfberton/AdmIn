using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class CONTRATO
    {
        public int CON_ID { get; set; } // ID del contrato
        public int INQ_ID { get; set; } // ID del inquilino asociado
        public int INM_ID { get; set; } // ID del inmueble asociado
        public int ADM_ID { get; set; } // ID del administrador asociado
        public DateTime CON_FECHA_INICIO { get; set; } // Fecha de inicio del contrato
        public DateTime CON_FECHA_FIN { get; set; } // Fecha de fin del contrato
        public DateTime? CON_FECHA_CANCELACION { get; set; } // Fecha de cancelación del contrato (opcional)
        public decimal CON_MONTO_MENSUAL { get; set; } // Monto mensual del contrato
        public DateTime CON_FECHA_FIRMA { get; set; } // Fecha de firma del contrato
        public int CON_ESTADO_ID { get; set; } // ID del estado del contrato
        public string CON_COMENTARIOS { get; set; } // Comentarios sobre el contrato
    }
}

