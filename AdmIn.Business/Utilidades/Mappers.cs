using AdmIn.Business.Entidades;
using AdmIn.Data.Entidades;
using AdmIn.Data.Repositorios;
using AdmIn.Data.Repositorios.AdmIn.Data.Repositorios;
using System.Linq.Expressions;

namespace AdmIn.Business.Utilidades
{
    public static class Mappers
    {
        #region Usuario
        public static Usuario ToBusinessUsuario(this USUARIO dataUsuario)
        {
            if (dataUsuario == null) return null;
            
            Rep_ROL rr = new Rep_ROL();
            Rep_USUARIO_ROL rur = new Rep_USUARIO_ROL();

            // Crear el usuario base
            var usuario = new Usuario
            {
                Id = dataUsuario.USU_ID,
                Nombre = dataUsuario.USU_NOMBRE,
                Password = dataUsuario.USU_PASSWORD,
                Email = dataUsuario.USU_EMAIL,
                Activo = dataUsuario.USU_ACTIVO,
                Roles = new List<Rol>()        // Inicializa para evitar null
            };

            // Cargar roles
            var roles = rur.Obtener_por_usuario(usuario.Id).Result;
            if (roles.Correcto && roles.Datos != null)
            {
                var rolesDetalles = rr.Obtener_todos().Result;
                if (rolesDetalles.Correcto && rolesDetalles.Datos != null)
                {
                    usuario.Roles = roles.Datos
                        .Select(r => rolesDetalles.Datos.FirstOrDefault(rol => rol.ROL_ID == r.ROL_ID)?.ToBusinessRol())
                        .Where(r => r != null) // Excluye nulos
                        .ToList();
                }
            }

            return usuario;
        }

        // Métodos auxiliares (permiso y rol)
        public static Permiso ToBusinessPermiso(this PERMISO dataPermiso)
        {
            if (dataPermiso == null) return null;

            return new Permiso
            {
                Id = dataPermiso.PERM_ID,
                Nombre = dataPermiso.PERM_NOMBRE
            };
        }


        public static Rol ToBusinessRol(this ROL dataRol)
        {
            if (dataRol == null) return null;

            // Instancia de los repositorios necesarios
            Rep_ROL_PERMISO repRolPermiso = new Rep_ROL_PERMISO();
            Rep_PERMISO repPermiso = new Rep_PERMISO();

            // Crear la instancia del rol
            var rol = new Rol
            {
                Id = dataRol.ROL_ID,
                Nombre = dataRol.ROL_NOMBRE,
                Permisos = new List<Permiso>() // Inicializa lista vacía
            };

            // Obtener los permisos asociados al rol
            var permisosAsociados = repRolPermiso.Obtener_por_rol(dataRol.ROL_ID).Result;
            if (permisosAsociados.Correcto && permisosAsociados.Datos != null)
            {
                // Obtener todos los permisos para mapear nombres
                var todosPermisos = repPermiso.Obtener_todos().Result;
                if (todosPermisos.Correcto && todosPermisos.Datos != null)
                {
                    // Convertir los permisos a objetos de negocio
                    rol.Permisos = permisosAsociados.Datos
                        .Select(rp => todosPermisos.Datos.FirstOrDefault(p => p.PERM_ID == rp.PERM_ID))
                        .Where(p => p != null) // Excluir nulos
                        .Select(p => p.ToBusinessPermiso())
                        .ToList();
                }
            }

            return rol;
        }

        public static USUARIO ToDataUSUARIO(this Usuario data)
        {
            if (data == null) return null;

            return new USUARIO
            {
                USU_ID = data.Id,
                USU_ACTIVO = data.Activo,
                USU_EMAIL = data.Email,
                USU_FECHA_CREACION = data.Creacion,
                USU_NOMBRE = data.Nombre,
                USU_PASSWORD = data.Password
            };
        }

        public static PERMISO ToDataPERMISO(this Permiso data)
        {
            if (data == null) return null;

            return new PERMISO
            {
                PERM_ID = data.Id,
                PERM_NOMBRE = data.Nombre
            };
        }

        public static ROL ToDataROL(this Rol data)
        {
            if (data == null) return null;

            return new ROL
            {
                ROL_ID = data.Id,
                ROL_NOMBRE = data.Nombre
            };
        }

        #endregion

        #region Persona

        // Método para mapear la entidad Persona con relaciones (Direcciones y Teléfonos)
        public static PersonaBase ToBusinessPersona(this PERSONA dataPersona)
        {
            if (dataPersona == null) return null;

            // Instancia de los repositorios necesarios
            Rep_PERSONA_DIRECCION repPersonaDireccion = new Rep_PERSONA_DIRECCION();
            Rep_PERSONA_TELEFONO repPersonaTelefono = new Rep_PERSONA_TELEFONO();
            Rep_DIRECCION repDireccion = new Rep_DIRECCION();
            Rep_TELEFONO repTelefono = new Rep_TELEFONO();

            // Crear la instancia de la persona base
            var persona = new PersonaBase
            {
                PersonaId = dataPersona.PER_ID,
                Nombre = dataPersona.PER_NOMBRE,
                ApellidoMaterno = dataPersona.PER_AMATERNO,
                ApellidoPaterno = dataPersona.PER_APATERNO,
                Rfc = dataPersona.PER_RFC,
                Email = dataPersona.PER_EMAIL,
                Nacionalidad = dataPersona.PER_NACIONALIDAD,
                EsPersonaFisica = dataPersona.PER_ESPERSONA,
                EsTitular = dataPersona.PER_TITULAR,
                Direcciones = new List<Direccion>(),  // Inicializa para evitar null
                Telefonos = new List<Telefono>()     // Inicializa para evitar null
            };

            // Cargar direcciones asociadas a la persona
            var direccionesRelacionadas = repPersonaDireccion.Obtener_por_persona(dataPersona.PER_ID).Result;
            if (direccionesRelacionadas.Correcto && direccionesRelacionadas.Datos != null)
            {
                // Obtener las direcciones completas usando los IDs de las direcciones relacionadas
                foreach (var direccionRelacionada in direccionesRelacionadas.Datos)
                {
                    var direccionCompleta = repDireccion.Obtener_por_id(new DIRECCION() { DIR_ID = direccionRelacionada.DIR_ID }).Result;
                    if (direccionCompleta.Correcto && direccionCompleta.Datos != null)
                    {
                        persona.Direcciones.Add(direccionCompleta.Datos.ToBusinessDireccion());
                    }
                }
            }

            // Cargar teléfonos asociados a la persona
            var telefonosRelacionados = repPersonaTelefono.Obtener_por_persona(dataPersona.PER_ID).Result;
            if (telefonosRelacionados.Correcto && telefonosRelacionados.Datos != null)
            {
                // Obtener los teléfonos completos usando los IDs de los teléfonos relacionados
                foreach (var telefonoRelacionado in telefonosRelacionados.Datos)
                {
                    var telefonoCompleto = repTelefono.Obtener_por_id(new TELEFONO() { TEL_ID = telefonoRelacionado.TEL_ID }).Result;
                    if (telefonoCompleto.Correcto && telefonoCompleto.Datos != null)
                    {
                        persona.Telefonos.Add(telefonoCompleto.Datos.ToBusinessTelefono());
                    }
                }
            }

            return persona;
        }

        // Métodos auxiliares para mapear direcciones y teléfonos
        public static Direccion ToBusinessDireccion(this DIRECCION dataDireccion)
        {
            if (dataDireccion == null) return null;

            return new Direccion
            {
                DireccionId = dataDireccion.DIR_ID,
                CalleNumero = dataDireccion.DIR_CALLE_NUMERO,
                Colonia = dataDireccion.DIR_COLONIA,
                Ciudad = dataDireccion.DIR_CIUDAD,
                Estado = dataDireccion.DIR_ESTADO,
                CodigoPostal = dataDireccion.DIR_CP,
                Pais = dataDireccion.DIR_PAIS
            };
        }

        public static Telefono ToBusinessTelefono(this TELEFONO dataTelefono)
        {
            if (dataTelefono == null) return null;
            Rep_TIPO_TELEFONO rep_TIPO_TELEFONO = new Rep_TIPO_TELEFONO();

            var tipo_telefono = rep_TIPO_TELEFONO.ObtenerPorId(dataTelefono.TPT_ID);

            return new Telefono
            {
                TelefonoId = dataTelefono.TEL_ID,
                Tipo = tipo_telefono.ToBusinessTipoTelefono(),
                Numero = dataTelefono.TEL_NUMERO
            };
        }

        static TipoTelefono ToBusinessTipoTelefono(this TIPO_TELEFONO dataTipoTelefono)
        {
            if (dataTipoTelefono == null) return null;

            return new TipoTelefono
            {
                TipoTelefonoId = dataTipoTelefono.TPT_ID,
                Descripcion = dataTipoTelefono.TPT_DESCRIPCION
            };
        }

        // Métodos para mapear de negocio a data
        internal static PERSONA ToDataPERSONA(this PersonaBase data)
        {
            if (data == null) return null;

            return new PERSONA
            {
                PER_ID = data.PersonaId,
                PER_RFC = data.Rfc,
                PER_NOMBRE = data.Nombre,
                PER_AMATERNO = data.ApellidoMaterno,
                PER_APATERNO = data.ApellidoPaterno,
                PER_EMAIL = data.Email,
                PER_NACIONALIDAD = data.Nacionalidad,
                PER_ESPERSONA = data.EsPersonaFisica,
                PER_TITULAR = data.EsTitular
            };
        }

        public static DIRECCION ToDataDIRECCION(this Direccion data)
        {
            if (data == null) return null;

            return new DIRECCION
            {
                DIR_ID = data.DireccionId,
                DIR_CALLE_NUMERO = data.CalleNumero,
                DIR_COLONIA = data.Colonia,
                DIR_CIUDAD = data.Ciudad,
                DIR_ESTADO = data.Estado,
                DIR_CP = data.CodigoPostal,
                DIR_PAIS = data.Pais
            };
        }

        public static TELEFONO ToDataTELEFONO(this Telefono data)
        {
            if (data == null) return null;

            return new TELEFONO
            {
                TEL_ID = data.TelefonoId,
                TPT_ID = data.Tipo.TipoTelefonoId,
                TEL_NUMERO = data.Numero
            };
        }

        #endregion

        #region Administrador

        public static Administrador ToBusinessAdministrador(this ADMINISTRADOR dataAdministrador)
        {
            if (dataAdministrador == null) return null;

            // Instancia del repositorio de personas
            Rep_PERSONA repPersona = new Rep_PERSONA();

            // Obtener la persona asociada al administrador
            var personaResult = repPersona.Obtener_por_id(new PERSONA { PER_ID = dataAdministrador.PER_ID }).Result;

            if (!personaResult.Correcto || personaResult.Datos == null) return null;

            // Mapear la persona base
            var personaBase = personaResult.Datos.ToBusinessPersona();

            // Crear la instancia de Administrador con los datos de la persona base
            var administrador = new Administrador
            {
                PersonaId = personaBase.PersonaId,
                Nombre = personaBase.Nombre,
                ApellidoMaterno = personaBase.ApellidoMaterno,
                ApellidoPaterno = personaBase.ApellidoPaterno,
                Rfc = personaBase.Rfc,
                Email = personaBase.Email,
                Nacionalidad = personaBase.Nacionalidad,
                EsPersonaFisica = personaBase.EsPersonaFisica,
                EsTitular = personaBase.EsTitular,
                Direcciones = personaBase.Direcciones,
                Telefonos = personaBase.Telefonos,
                AdministradorId = dataAdministrador.ADM_ID,
                SuperiorId = dataAdministrador.ADM_SUPERIOR_ID
            };

            return administrador;
        }


        // Método para convertir Administrador a ADMINISTRADOR (Data Layer)
        internal static ADMINISTRADOR ToDataADMINISTRADOR(this Administrador data)
        {
            if (data == null) return null;

            return new ADMINISTRADOR
            {
                ADM_ID = data.AdministradorId,
                ADM_SUPERIOR_ID = data.SuperiorId,
                PER_ID = data.PersonaId
            };
        }

        #endregion

    }
}

    
