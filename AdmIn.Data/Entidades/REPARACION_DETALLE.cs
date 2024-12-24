using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class REPARACION_DETALLE
    {
        public int REP_DET_ID { get; set; } // ID del detalle de la reparación
        public int REP_ID { get; set; } // ID de la reparación a la que pertenece el detalle
        public string REP_DET_DESCRIPCION { get; set; } // Descripción del detalle de la reparación
        public decimal? REP_DET_COSTO { get; set; } // Costo del detalle de la reparación
        public DateTime REP_DET_FECHA { get; set; } // Fecha del detalle de la reparación
    }
}

