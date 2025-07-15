using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class VerificacionInquilino
    {
        public int Id { get; set; }
        public int ContratoId { get; set; }
        public string? TipoVerificacion { get; set; }
        public string? Resultado { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }
        public string ArchivoURL { get; set; }
    }
}
