using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockPersonaService : IPersonaService // Implement updated interface
    {
        private readonly List<PersonaBase> _personas;
        private static readonly Random random = new Random();

        public MockPersonaService()
        {
            _personas = GenerarPersonas().ToList();
        }

        private IEnumerable<PersonaBase> GenerarPersonas()
        {
            var nombres = new[] { "Juan", "Ana", "Carlos", "María", "Pedro", "Lucía", "Javier", "Sofía", "Miguel", "Laura" };
            var apellidosP = new[] { "Pérez", "García", "López", "Martínez", "Rodríguez", "Fernández", "Sánchez", "Ramírez", "Torres", "Flores" };
            var apellidosM = new[] { "Gómez", "Díaz", "Morales", "Vargas", "Silva", "Castro", "Ramos", "Molina", "Ortega", "Suárez" };
            var nacionalidades = new[] { "Argentina", "Uruguaya", "Chilena", "Paraguaya", "Boliviana", "Peruana", "Colombiana", "Venezolana", "Mexicana", "Española" };

            for (int i = 1; i <= 20; i++)
            {
                var nombre = nombres[random.Next(nombres.Length)];
                var apellidoP = apellidosP[random.Next(apellidosP.Length)];
                var apellidoM = apellidosM[random.Next(apellidosM.Length)];
                var rfc = $"{nombre.Substring(0, 2).ToUpper()}{apellidoP.Substring(0, 2).ToUpper()}{random.Next(100000, 999999)}";
                var email = $"{nombre.ToLower()}.{apellidoP.ToLower()}{i}@mail.com";
                var nacionalidad = nacionalidades[random.Next(nacionalidades.Length)];

                yield return new PersonaBase
                {
                    PersonaId = i,
                    Nombre = nombre,
                    ApellidoPaterno = apellidoP,
                    ApellidoMaterno = apellidoM,
                    Rfc = rfc,
                    Email = email,
                    Nacionalidad = nacionalidad,
                    EsPersonaFisica = true,
                    EsTitular = random.Next(2) == 0,
                    Direcciones = new List<Direccion>
                    {
                        new Direccion
                        {
                            DireccionId = i,
                            CalleNumero = $"Calle {random.Next(1, 200)} #{random.Next(1, 9999)}",
                            Colonia = "Centro",
                            Ciudad = "Ciudad Ejemplo",
                            Estado = "Estado Ejemplo",
                            CodigoPostal = $"{random.Next(10000, 99999)}",
                            Pais = "Argentina"
                        }
                    },
                    Telefonos = new List<Telefono>
                    {
                        new Telefono
                        {
                            TelefonoId = i,
                            Numero = $"+54 9 {random.Next(1000, 9999)}-{random.Next(100000, 999999)}",
                            Tipo = new TipoTelefono { TipoTelefonoId = 1, Descripcion = "Móvil" }
                        }
                    }
                };
            }
        }

        public async Task<List<PersonaBase>> BuscarPersonasAsync(string apellido, string rfc)
        {
            var query = _personas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(apellido))
                query = query.Where(p => (p.ApellidoPaterno ?? "").Contains(apellido, StringComparison.OrdinalIgnoreCase)
                                      || (p.ApellidoMaterno ?? "").Contains(apellido, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(rfc))
                query = query.Where(p => p.Rfc.Contains(rfc, StringComparison.OrdinalIgnoreCase));

            return await Task.FromResult(query.ToList());
        }

        public async Task<PersonaBase> CrearPersonaAsync(PersonaBase persona)
        {
            persona.PersonaId = _personas.Any() ? _personas.Max(p => p.PersonaId) + 1 : 1;
            _personas.Add(persona);
            return await Task.FromResult(persona);
        }

        public async Task<PersonaBase> ActualizarPersonaAsync(PersonaBase persona)
        {
            var existente = _personas.FirstOrDefault(p => p.PersonaId == persona.PersonaId);
            if (existente != null)
            {
                existente.Nombre = persona.Nombre;
                existente.ApellidoPaterno = persona.ApellidoPaterno;
                existente.ApellidoMaterno = persona.ApellidoMaterno;
                existente.Rfc = persona.Rfc;
                existente.Email = persona.Email;
                existente.Nacionalidad = persona.Nacionalidad;
                existente.EsPersonaFisica = persona.EsPersonaFisica;
                existente.EsTitular = persona.EsTitular;
                existente.Direcciones = persona.Direcciones;
                existente.Telefonos = persona.Telefonos;
            }
            return await Task.FromResult(existente ?? persona);
        }

        public async Task EliminarPersonaAsync(int personaId)
        {
            var persona = _personas.FirstOrDefault(p => p.PersonaId == personaId);
            if (persona != null)
                _personas.Remove(persona);
            await Task.CompletedTask;
        }

        public async Task<PersonaBase?> ObtenerPersonaPorIdAsync(int personaId)
        {
            var persona = _personas.FirstOrDefault(p => p.PersonaId == personaId);
            return await Task.FromResult(persona);
        }
    }
}
