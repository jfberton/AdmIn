using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class CaracteristicaInmueble
    {
        public int Id { get; set; }
        public int InmuebleID { get; set; }
        public int CaracteristicaID { get; set; }
        public string Valor { get; set; } = string.Empty;

        #region Propiedades de navegación
        public Inmueble? Inmueble { get; set; }
        public Caracteristica? Caracteristica { get; set; }
        #endregion

    }
}
