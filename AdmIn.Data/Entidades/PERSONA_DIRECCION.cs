using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class PERSONA_DIRECCION
    {
        public int PED_ID { get; set; } // ID de la relación persona-dirección
        public int PER_ID { get; set; } // ID de la persona
        public int DIR_ID { get; set; } // ID de la dirección
        public string? PED_TIPO { get; set; } // Tipo de relación (por ejemplo, 'Residencial', 'Oficina')
    }
}

