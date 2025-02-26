using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class INMUEBLE_IMAGEN
    {
        public int INM_ID { get; set; }
        public Guid IMG_ID { get; set; }
        public Guid? IMG_PRINCIPAL { get; set; }
    }
}
