using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class CLIENTE
    {
        public int CLI_ID { get; set; } // ID del cliente
        public int PER_ID { get; set; } // ID de la persona asociada
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
    }
}
