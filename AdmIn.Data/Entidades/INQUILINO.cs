using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class INQUILINO
    {
        public int INQ_ID { get; set; } // ID del inquilino
        public int PER_ID { get; set; } // ID de la persona asociada
        public int INM_ID { get; set; } // ID del inmueble asociado
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
    }
}
