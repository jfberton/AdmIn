using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockEmpleadoService : IEmpleadoService // Implement updated interface
    {
        private readonly List<Empleado> _empleados;
        private readonly List<EmpleadoCalificacion> _calificaciones;
        private readonly IPersonaService _mockPersonaService; // Retained for potential future use, updated type
        private static readonly Random random = new Random();

        // Simplified constructor for mock setup: Generates its own Personas or uses a predefined simple list.
        public MockEmpleadoService(IPersonaService mockPersonaService) // Updated type
        {
            _mockPersonaService = mockPersonaService; // Store it
            _empleados = GenerarEmpleados().ToList();
            _calificaciones = new List<EmpleadoCalificacion>(); // Calificaciones are added by ReparacionService
        }

        private IEnumerable<Empleado> GenerarEmpleados()
        {
            var personasParaEmpleados = GenerarPersonasInternasParaEmpleados(5).ToList();

            for (int i = 0; i < personasParaEmpleados.Count; i++)
            {
                var persona = personasParaEmpleados[i];
                var empleado = new Empleado
                {
                    EmpleadoId = i + 1,
                    PersonaId = persona.PersonaId,
                    Nombre = persona.Nombre,
                    ApellidoPaterno = persona.ApellidoPaterno,
                    ApellidoMaterno = persona.ApellidoMaterno,
                    Rfc = persona.Rfc,
                    Email = persona.Email,
                    Nacionalidad = persona.Nacionalidad,
                    EsPersonaFisica = persona.EsPersonaFisica,
                    EsTitular = persona.EsTitular,
                    Direcciones = persona.Direcciones,
                    Telefonos = persona.Telefonos,
                    Especialidad = new EmpleadoEspecialidad { Especialidad = $"Especialidad #{i+1}" },
                    EsContratistaExterno = i % 2 == 0,
                    Agenda = new List<EmpleadoAgenda>()
                };

                empleado.Agenda = GenerarAgendaParaEmpleado(empleado).ToList();
                yield return empleado;
            }
        }

        // Helper to generate placeholder PersonaBase objects for employees to avoid complex dependencies during mock init
        private IEnumerable<PersonaBase> GenerarPersonasInternasParaEmpleados(int cantidad)
        {
             var nombres = new[] { "Eduardo", "Fernanda", "Roberto", "Gabriela", "Arturo" };
             var apellidosP = new[] { "Reyes", "Jimenez", "Navarro", "Salazar", "Vega" };
             var apellidosM = new[] { "Santos", "Peralta", "Campos", "Guerrero", "Chavez" };

            for (int i = 1; i <= cantidad; i++)
            {
                var nombre = nombres[random.Next(nombres.Length)];
                var apellidoP = apellidosP[random.Next(apellidosP.Length)];
                var apellidoM = apellidosM[random.Next(apellidosM.Length)];
                yield return new PersonaBase
                {
                    PersonaId = 100 + i, // Ensure unique IDs if combined with other personas
                    Nombre = nombre,
                    ApellidoPaterno = apellidoP,
                    ApellidoMaterno = apellidoM,
                    Rfc = $"{nombre.Substring(0,1).ToUpper()}{apellidoP.Substring(0,1).ToUpper()}{apellidoM.Substring(0,1).ToUpper()}{random.Next(1000,9999)}EM",
                    Email = $"{nombre.ToLower()}.{apellidoP.ToLower()}{100+i}@empleado.mail",
                    Nacionalidad = "Mexicana",
                    EsPersonaFisica = true,
                    Direcciones = new List<Direccion> { new Direccion { CalleNumero = "Calle Empleado", Ciudad = "Ciudad Laboral"} },
                    Telefonos = new List<Telefono> { new Telefono { Numero = "555-000-EMP" } }
                };
            }
        }

        private IEnumerable<EmpleadoAgenda> GenerarAgendaParaEmpleado(Empleado empleado)
        {
            int agendaEntries = random.Next(1, 5);
            for (int j = 0; j < agendaEntries; j++)
            {
                var fecha = DateTime.Now.AddDays(random.Next(1, 30));
                var horaInicioNum = random.Next(9, 17);
                var horaInicio = TimeSpan.FromHours(horaInicioNum);
                var horaFin = TimeSpan.FromHours(horaInicioNum + random.Next(1, 3)); // Duration 1 to 2 hours

                yield return new EmpleadoAgenda
                {
                    Id = j + 1, // Simple ID per employee's agenda
                    Fecha = fecha,
                    HoraInicio = horaInicio,
                    HoraFin = horaFin,
                    Disponible = random.Next(0, 2) == 0, // 50% chance of being available
                    Empleado = empleado, // Link back to the employee
                    EmpleadoId = empleado.EmpleadoId
                };
            }
        }


        public async Task<IEnumerable<Empleado>> ObtenerEmpleados()
        {
            await Task.CompletedTask;
            return _empleados;
        }

        public async Task<Empleado?> ObtenerEmpleadoPorId(int id)
        {
            await Task.CompletedTask;
            return _empleados.FirstOrDefault(e => e.EmpleadoId == id);
        }

        public async Task<IEnumerable<EmpleadoCalificacion>> ObtenerCalificacionesEmpleado(int empleadoId)
        {
            await Task.CompletedTask;
            // Calificaciones are added by MockReparacionService, this service just stores and returns them.
            return _calificaciones.Where(c => c.EmpleadoId == empleadoId).ToList();
        }

        public async Task AddCalificacionAsync(EmpleadoCalificacion calificacion)
        {
            // Ensure ID is unique for the new calificacion
            calificacion.Id = _calificaciones.Any() ? _calificaciones.Max(c => c.Id) + 1 : 1;
            _calificaciones.Add(calificacion);
            await Task.CompletedTask;
        }
    }
}
