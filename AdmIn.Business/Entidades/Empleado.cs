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

        // Especialidad del empleado (Ej: Plomería, Electricidad, Albañilería)
        public EmpleadoEspecialidad Especialidad { get; set; }

        // Agenda de disponibilidad del empleado
        public List<EmpleadoAgenda> Agenda { get; set; } = new();
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

        // Empleado asignado al turno
        public Empleado Empleado { get; set; }
    }

}
