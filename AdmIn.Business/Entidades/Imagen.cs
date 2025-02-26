using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Imagen
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identificador único de la imagen
        public string Descripcion { get; set; } // Descripción de la imagen
        public string Url { get; set; } // URL de la imagen en tamaño completo
        public string? ThumbnailUrl { get; set; } // URL de la imagen en tamaño reducido (opcional)
    }
}
