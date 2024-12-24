using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class INMUEBLE
    {
        public int INM_ID { get; set; } // ID del inmueble
        public int PRO_ID { get; set; } // ID de la propiedad asociada
        public int DIR_ID { get; set; } // ID de la dirección asociada
        public string INM_DESCRIPCION { get; set; } // Descripción del inmueble
        public string INM_COMENTARIO { get; set; } // Comentario sobre el inmueble
        public decimal INM_VALOR { get; set; } // Valor del inmueble
        public decimal INM_SUPERFICIEM { get; set; } // Superficie del inmueble
        public decimal INM_CONSTRUIDOM { get; set; } // Superficie construida del inmueble
        public int? TEL_ID { get; set; } // ID del teléfono asociado (opcional)
        public int INM_ESTADO_ID { get; set; } // ID del estado del inmueble
    }
}

