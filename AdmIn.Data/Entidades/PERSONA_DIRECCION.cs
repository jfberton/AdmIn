using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class PERSONA_DIRECCION
    {
        public int PER_DIR_ID { get; set; } // ID de la relación persona-dirección
        public int PER_ID { get; set; } // ID de la persona
        public int DIR_ID { get; set; } // ID de la dirección
        public int DIR_TIP_ID { get; set; } // Tipo de direccion
    }
}

