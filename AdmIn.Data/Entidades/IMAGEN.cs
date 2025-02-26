using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class Imagen
    {
        public Guid IMG_ID { get; set; } // Identificador único de la imagen
        public string IMG_DESCRIPCION { get; set; } // Descripción de la imagen
        public string IMG_URL { get; set; } // URL de la imagen en tamaño completo
        public string? IMG_THUMBMAILURL { get; set; } // URL de la imagen en tamaño reducido (opcional)
    }
}
