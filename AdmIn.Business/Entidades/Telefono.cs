using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Telefono
    {
        public int TelefonoId { get; set; } // ID del teléfono
        public string Numero { get; set; } = string.Empty; // Número de teléfono
        public TipoTelefono Tipo { get; set; } = new(); // Asociación con el tipo de teléfono

        public override string ToString()
        {
            return $"({Tipo.Descripcion}) {this.Numero}";
        }
    }

    public class TipoTelefono
    {
        public int TipoTelefonoId { get; set; } // ID del tipo de teléfono
        public string Descripcion { get; set; } = string.Empty; // Descripción del tipo (Ej.: "Móvil", "Fijo")
    }
}
