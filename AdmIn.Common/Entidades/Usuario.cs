using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Entidades
{
    public class Usuario
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
        public int? PersonaId { get; set; }
        public int? EmpresaId { get; set; }
        public int MonedaId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioCreador { get; set; }
        public int? UsuarioModificador { get; set; }

        // Nuevo campo para imagen de perfil
        public Guid? ImagenPerfilId { get; set; }

        public string? Token { get; set; }

        #endregion

        #region Propiedades de navegación

        public List<Rol> Roles { get; set; } = new List<Rol>();
        public Persona? Persona { get; set; }
        public Empresa? Empresa { get; set; }
        public Moneda? Moneda { get; set; }
        
        // Nueva propiedad de navegación para la imagen de perfil
        public Imagen? ImagenPerfil { get; set; }

        #endregion

        #region Metodos públicos
        public string RolString
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return "No posee roles asignados.-";
                }

                // Utilizamos un HashSet para evitar duplicados
                HashSet<string> rolesUnicos = new HashSet<string>();

                foreach (Rol rol in Roles)
                {
                    rolesUnicos.Add(rol.Nombre);
                }

                return string.Join(", ", rolesUnicos);
            }
        }

        // Método auxiliar para obtener la URL de la imagen de perfil
        public string ObtenerUrlImagenPerfil(string baseUrl = "")
        {
            if (ImagenPerfil?.UrlThumb != null)
            {
                if (ImagenPerfil.UrlThumb.StartsWith("http"))
                    return ImagenPerfil.UrlThumb;
                
                return $"{baseUrl.TrimEnd('/')}{ImagenPerfil.UrlThumb}";
            }
            
            // Retornar null para mostrar placeholder en lugar de imagen por defecto
            return null;
        }

        #endregion

    }
}
