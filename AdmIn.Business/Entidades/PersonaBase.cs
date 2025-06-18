using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class PersonaBase
    {
        public int PersonaId { get; set; } // ID del registro en PERSONA
        public string Nombre { get; set; } = string.Empty;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Rfc { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Nacionalidad { get; set; }
        public bool EsPersonaFisica { get; set; }
        public bool EsTitular { get; set; }

        // Listas asociadas a la Persona
        public List<Direccion> Direcciones { get; set; } = new(); // Lista de direcciones
        public List<Telefono> Telefonos { get; set; } = new(); // Lista de teléfonos


        // Relación con los diferentes tipos de persona
        public Administrador? Administrador { get; set; }
        public Inquilino? Inquilino { get; set; }
        public Propietario? Propietario { get; set; }
        public Empleado? Empleado { get; set; }
    }
}
