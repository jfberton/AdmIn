using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class EMPLEADO_AGENDA
    {
        public int EMP_AGE_ID { get; set; } // ID de la agenda del empleado
        public int EMP_ID { get; set; } // ID del empleado
        public DateTime EMP_AGE_FECHA { get; set; } // Fecha del evento en la agenda
        public TimeSpan EMP_AGE_HORA_INICIO { get; set; } // Hora de inicio del evento
        public TimeSpan EMP_AGE_HORA_FIN { get; set; } // Hora de fin del evento
        public bool EMP_AGE_DISPONIBLE { get; set; } // Disponibilidad del empleado
    }
}

