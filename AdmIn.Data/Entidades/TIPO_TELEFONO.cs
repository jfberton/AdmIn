using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class TIPO_TELEFONO
    {
        public int TPT_ID { get; set; } // ID del tipo de teléfono
        public string TPT_DESCRIPCION { get; set; } = string.Empty; // Descripción del tipo de teléfono
    }
}

