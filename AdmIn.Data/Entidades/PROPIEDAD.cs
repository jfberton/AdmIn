using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class PROPIEDAD
    {
        public int PRO_ID { get; set; } // ID de la propiedad
        public string PRO_DESCRIPCION { get; set; } // Descripción de la propiedad
        public int DIR_ID { get; set; } // ID de la dirección asociada
        public int? TEL_ID { get; set; } // ID del teléfono asociado (opcional)
        public string PRO_EMAIL { get; set; } // Email de la propiedad
        public int? PRO_UNIDADES { get; set; } // Número de unidades
        public int? PRO_UNIDADES_HABITADAS { get; set; } // Número de unidades habitadas
        public string PRO_COMENTARIO { get; set; } // Comentario sobre la propiedad
        public decimal PRO_VALOR { get; set; } // Valor de la propiedad
        public decimal PRO_SUPERFICIEM { get; set; } // Superficie de la propiedad
        public int ADM_ID { get; set; } // ID del administrador asociado
        public int CLI_ID { get; set; } // ID del cliente asociado
        public int PRT_ID { get; set; } // ID del tipo de propiedad
    }
}

