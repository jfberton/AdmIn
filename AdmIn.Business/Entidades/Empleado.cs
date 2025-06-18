using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Entidades
{
    public class Empleado : PersonaBase
    {
        public int EmpleadoId { get; set; }
        public EmpleadoEspecialidad Especialidad { get; set; }
        public List<EmpleadoAgenda> Agenda { get; set; } = new();

        // Nuevo campo para distinguir entre interno/externo
        public bool EsContratistaExterno { get; set; }

        // Propiedad calculada para mostrar el tipo
        public string TipoEmpleado => EsContratistaExterno ? "Contratista" : "Reparador";
    }

    public class EmpleadoEspecialidad
    {
        public int Id { get; set; }
        public string Especialidad { get; set; } // Ej: Plomería, Electricidad, Albañilería
    }

    public class EmpleadoAgenda
    {
        public int Id { get; set; }

        // Fecha del turno
        public DateTime Fecha { get; set; }

        // Hora de inicio del turno
        public TimeSpan HoraInicio { get; set; }

        // Hora de finalización del turno
        public TimeSpan HoraFin { get; set; }

        public bool Disponible { get; set; } 

        // Empleado asignado al turno
        public Empleado Empleado { get; set; }
    }

    public class EmpleadoCalificacion
    {
        public int Id { get; set; }
        public int ReparacionId { get; set; }
        public int EmpleadoId { get; set; }
        public int CalificacionTrabajo { get; set; } // 1-5
        public int CalificacionComportamiento { get; set; } // 1-5
        public string Comentario { get; set; }
    }
}
