using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class REPARACION
    {
        public int REP_ID { get; set; } // ID de la reparación
        public int INM_ID { get; set; } // ID del inmueble
        public int INQ_ID { get; set; } // ID del inquilino
        public int EMP_ID { get; set; } // ID del empleado
        public DateTime REP_FECHA_SOLICITUD { get; set; } // Fecha de solicitud de la reparación
        public DateTime? REP_FECHA_INICIO { get; set; } // Fecha de inicio de la reparación
        public DateTime? REP_FECHA_FIN { get; set; } // Fecha de fin de la reparación
        public decimal? REP_COSTO_ESTIMADO { get; set; } // Costo estimado de la reparación
        public decimal? REP_COSTO_FINAL { get; set; } // Costo final de la reparación
        public string REP_DESCRIPCION { get; set; } // Descripción de la reparación
        public int REP_EST_ID { get; set; } // ID del estado de la reparación
    }
}

