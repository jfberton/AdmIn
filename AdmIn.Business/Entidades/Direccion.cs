using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Direccion
    {
        public int DireccionId { get; set; } // ID de la dirección
        public string CalleNumero { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; }

        public override string ToString()
        {
            List<string> partes = new();

            if (!string.IsNullOrWhiteSpace(CalleNumero))
                partes.Add(CalleNumero);

            if (!string.IsNullOrWhiteSpace(Colonia))
                partes.Add(Colonia);

            if (!string.IsNullOrWhiteSpace(Ciudad))
                partes.Add(Ciudad);

            if (!string.IsNullOrWhiteSpace(Estado))
                partes.Add(Estado);

            if (!string.IsNullOrWhiteSpace(CodigoPostal))
                partes.Add($"CP: {CodigoPostal}");

            if (!string.IsNullOrWhiteSpace(Pais))
                partes.Add(Pais);

            return string.Join(", ", partes);
        }
    }
}
