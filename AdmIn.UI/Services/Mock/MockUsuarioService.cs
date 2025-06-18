using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockUsuarioService : IUsuarioService // Implement updated interface
    {
        private readonly List<Usuario> _usuarios;
        private readonly IPersonaService _mockPersonaService; // Updated type
        private static readonly Random random = new Random();

        public MockUsuarioService(IPersonaService mockPersonaService) // Updated type
        {
            _mockPersonaService = mockPersonaService;
            // It's generally better to make data generation async if it involves async calls.
            // However, for mocks and simplicity, .Result can be used if deadlocks are not an issue in the setup.
            // Consider an async InitializeAsync method for more robust scenarios.
            var personas = _mockPersonaService.BuscarPersonasAsync(null, null).Result;
            _usuarios = GenerarUsuarios(personas).ToList();
        }

        private IEnumerable<Usuario> GenerarUsuarios(List<PersonaBase> personas)
        {
            if (personas == null || !personas.Any())
            {
                // Fallback if no personas are available (e.g. service not ready)
                // This should ideally not happen if services are initialized in correct order.
                yield break;
            }

            for (int i = 0; i < personas.Count; i++)
            {
                var persona = personas[i];
                yield return new Usuario
                {
                    Id = i + 1,
                    Nombre = persona.Email, // Using email as username for simplicity
                    Password = "mockhashedpassword", // In real app, this would be properly hashed
                    Email = persona.Email,
                    Token = Guid.NewGuid().ToString(),
                    Activo = random.Next(10) < 9, // 90% active
                    Creacion = DateTime.Now.AddDays(-random.Next(1, 365)),
                    PersonaId = persona.PersonaId,
                    Persona = persona,
                    Roles = GenerarRolesParaUsuario(i + 1).ToList()
                };
            }
        }

        private IEnumerable<Rol> GenerarRolesParaUsuario(int usuarioId)
        {
            var roles = new List<Rol>();
            var allRoleNames = new[] { "Administrador", "GestorInmuebles", "GestorContratos", "UsuarioConsulta" };
            int numberOfRoles = random.Next(1, 3); // 1 or 2 roles per user

            var userRoleNames = new HashSet<string>();
            while(userRoleNames.Count < numberOfRoles)
            {
                userRoleNames.Add(allRoleNames[random.Next(allRoleNames.Length)]);
            }

            int rolIdCounter = 1; // simple id for roles within this user
            foreach (var roleName in userRoleNames)
            {
                var rol = new Rol
                {
                    Id = rolIdCounter++,
                    Nombre = roleName,
                    Permisos = GenerarPermisosParaRol(roleName).ToList()
                };
                roles.Add(rol);
            }
            return roles;
        }

        private IEnumerable<Permiso> GenerarPermisosParaRol(string roleName)
        {
            var permisos = new List<Permiso>();
            var permisoIdCounter = 1;

            // Example permissions based on role
            if (roleName == "Administrador")
            {
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerTodo" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "EditarTodo" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "EliminarTodo" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "GestionarUsuarios" });
            }
            else if (roleName == "GestorInmuebles")
            {
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerInmuebles" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "EditarInmuebles" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "GestionarReparaciones" });
            }
            else if (roleName == "GestorContratos")
            {
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerContratos" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "EditarContratos" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "GestionarPagos" });
            }
            else // UsuarioConsulta
            {
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerInmueblesAsignados" });
                permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerContratosAsignados" });
            }

            // Add a common permission
            permisos.Add(new Permiso { Id = permisoIdCounter++, Nombre = "VerPerfil" });

            return permisos;
        }


        public async Task<Usuario?> ObtenerUsuarioPorId(int id)
        {
            await Task.CompletedTask;
            return _usuarios.FirstOrDefault(u => u.Id == id);
        }

        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            await Task.CompletedTask;
            return _usuarios;
        }
    }
}
