using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class ProveedorConEstadisticas
    {
        public Proveedor Proveedor { get; set; } = new Proveedor();
        public decimal CalificacionPromedio { get; set; } = 0m;
        public int JobsCount { get; set; } = 0;
    }
}
