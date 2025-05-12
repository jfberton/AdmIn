using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Imagen
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
    }
}
