using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class EMPLEADO
    {
        public int EMP_ID { get; set; } // ID del empleado
        public int PER_ID { get; set; } // ID de la persona asociada
        public decimal? EMP_HONORARIOS { get; set; } // Honorarios del empleado
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
        public int? EME_ID { get; set; } // ID de la especialidad del empleado
    }
}

