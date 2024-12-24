using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class TELEFONO
    {
        public int TEL_ID { get; set; } // ID del teléfono
        public string TEL_NUMERO { get; set; } = string.Empty; // Número de teléfono
        public int TPT_ID { get; set; } // ID del tipo de teléfono (FK)
    }
}