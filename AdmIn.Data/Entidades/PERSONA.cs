using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Entidades
{
    public class PERSONA
    {
        public int PER_ID { get; set; } // ID de la persona
        public string PER_RFC { get; set; } = string.Empty; // RFC de la persona
        public string PER_NOMBRE { get; set; } = string.Empty; // Nombre de la persona
        public string? PER_APATERNO { get; set; } // Apellido paterno de la persona
        public string? PER_AMATERNO { get; set; } // Apellido materno de la persona
        public string? PER_EMAIL { get; set; } // Email de la persona
        public string? PER_NACIONALIDAD { get; set; } // Nacionalidad de la persona
        public bool PER_ESPERSONA { get; set; } // Indica si es persona física
        public bool PER_TITULAR { get; set; } // Indica si es titular
    }
}

