using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class ALIADO
    {
        public int ALI_ID { get; set; } // ID del aliado
        public int ADM_ID { get; set; } // ID del administrador asociado
        public int PER_ID { get; set; } // ID de la persona asociada
    }
}
