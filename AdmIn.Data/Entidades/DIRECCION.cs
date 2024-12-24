using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class DIRECCION
    {
        public int DIR_ID { get; set; } // ID de la dirección
        public string DIR_CALLE_NUMERO { get; set; } = string.Empty; // Calle y número de la dirección
        public string? DIR_COLONIA { get; set; } // Colonia de la dirección
        public string? DIR_CIUDAD { get; set; } // Ciudad de la dirección
        public string? DIR_ESTADO { get; set; } // Estado de la dirección
        public string? DIR_CP { get; set; } // Código postal de la dirección
        public string? DIR_PAIS { get; set; } // País de la dirección
    }
}
