using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common
{
    public class DTO<T>
    {
        public bool Correcto { get; set; }
        public string Mensaje { get; set; }
        public T? Datos { get; set; }
    }
}
