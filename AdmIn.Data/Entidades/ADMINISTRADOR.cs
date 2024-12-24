using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class ADMINISTRADOR
    {
        public int ADM_ID { get; set; } // ID del administrador
        public int PER_ID { get; set; } // ID de la persona asociada
        public int? ADM_SUPERIOR_ID { get; set; } // ID del administrador superior (auto-referencia)
    }
}

