using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public class MockUsuarioService : IUsuarioService 
    {
        private readonly List<Usuario> _usuarios;
        private readonly IPersonaService _mockPersonaService; 
        private static readonly Random random = new Random();

        public MockUsuarioService(IPersonaService mockPersonaService)
        {
            _mockPersonaService = mockPersonaService;
            var personas = _mockPersonaService.BuscarPersonasAsync(null, null).Result;
            _usuarios = GenerarUsuarios(personas).ToList();
        }

        private IEnumerable<Usuario> GenerarUsuarios(List<PersonaBase> personas)
        {
            if (personas == null || !personas.Any())
            {
                yield break;
            }

            for (int i = 0; i < personas.Count; i++)
            {
                var persona = personas[i];
                yield return new Usuario
                {
                    Id = i + 1,
                    Nombre = persona.Email,
                    Password = "mockhashedpassword", 
                    Email = persona.Email,
                    Token = Guid.NewGuid().ToString(),
                    Activo = random.Next(10) < 9,
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
            int numberOfRoles = random.Next(1, 3); 

            var userRoleNames = new HashSet<string>();
            while(userRoleNames.Count < numberOfRoles)
            {
                userRoleNames.Add(allRoleNames[random.Next(allRoleNames.Length)]);
            }

            int rolIdCounter = 1; 
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
