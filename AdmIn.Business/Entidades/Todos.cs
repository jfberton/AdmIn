namespace AdmIn.Business.Entidades.todos
{
    public class Administrador : PersonaBase
    {
        public int AdministradorId { get; set; } // ID propio del Administrador
        public int? SuperiorId { get; set; } // ID del Administrador superior (puede ser null)
    }
    public class Direccion
    {
        public int DireccionId { get; set; } // ID de la dirección
        public string CalleNumero { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; }

        public override string ToString()
        {
            List<string> partes = new();

            if (!string.IsNullOrWhiteSpace(CalleNumero))
                partes.Add(CalleNumero);

            if (!string.IsNullOrWhiteSpace(Colonia))
                partes.Add(Colonia);

            if (!string.IsNullOrWhiteSpace(Ciudad))
                partes.Add(Ciudad);

            if (!string.IsNullOrWhiteSpace(Estado))
                partes.Add(Estado);

            if (!string.IsNullOrWhiteSpace(CodigoPostal))
                partes.Add($"CP: {CodigoPostal}");

            if (!string.IsNullOrWhiteSpace(Pais))
                partes.Add(Pais);

            return string.Join(", ", partes);
        }
    }
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
    public class Imagen
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Identificador único de la imagen
        public string Descripcion { get; set; } // Descripción de la imagen
        public string Url { get; set; } // URL de la imagen en tamaño completo
        public string? ThumbnailUrl { get; set; } // URL de la imagen en tamaño reducido (opcional)
    }
    public class Propiedad
    {
        public int Id { get; set; } // ID de la propiedad
        public string Descripcion { get; set; } // Descripción de la propiedad
        public Direccion Direccion { get; set; } // ID de la dirección asociada
        public Telefono? Telefono { get; set; } // ID del teléfono asociado (opcional)
        public string Email { get; set; } // Email de la propiedad
        public int? Unidades { get; set; } // Número de unidades
        public int? UnidadesHabitadas { get; set; } // Número de unidades habitadas
        public string Comentario { get; set; } // Comentario sobre la propiedad
        public decimal Valor { get; set; } // Valor de la propiedad
        public decimal Superficie { get; set; } // Superficie de la propiedad
        public Administrador Administrador { get; set; } // ID del administrador asociado
        public Propietario Propietario { get; set; } // ID del cliente asociado
        public TipoPropiedad Tipo { get; set; } // ID del tipo de propiedad
    }
    public class TipoPropiedad
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
    }
    public class Inmueble
    {
        public int Id { get; set; }
        public Direccion Direccion { get; set; }
        public string Descripcion { get; set; }
        public string Comentario { get; set; }
        public decimal Valor { get; set; }
        public decimal Superficie { get; set; }
        public decimal Construido { get; set; }
        public Telefono Telefono { get; set; }
        public EstadoInmueble Estado { get; set; }

        // Propiedad para indicar cuál es la imagen principal
        public Guid? ImagenPrincipalId { get; set; }

        // Propiedad de navegación para obtener la imagen principal
        public Imagen? ImagenPrincipal => Imagenes.FirstOrDefault(i => i.Id == ImagenPrincipalId);

        // Relaciones
        public List<Imagen> Imagenes { get; set; } = new();

        public List<Reparacion> Reparaciones { get; set; } = new();
        public List<Inquilino> Inquilinos { get; set; } = new();
        public List<Pago> Pagos { get; set; } = new();
        public List<Contrato> Contratos { get; set; } = new();
        public List<CaracteristicaInmueble> Caracteristicas { get; set; } = new();
    }
    //Habitaciones, baños, pileta, garage, etc el valor es indica cantidad o estado
    public class CaracteristicaInmueble
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
    public class EstadoInmueble
    {
        public int Id { get; set; }
        public string Estado { get; set; }
    }
    public class Inquilino : PersonaBase
    {
        public int Id { get; set; }
        public Inmueble Inmueble { get; set; } // Relación con Inmueble
    }
    public class DetallePago
    {
        public int Id { get; set; }

        // Descripción del origen del monto (ej: "Cuota mensual", "Depósito de garantía", "Reparación")
        public string Descripcion { get; set; } = string.Empty;

        // Monto asociado a este detalle
        public decimal Monto { get; set; }
    }
    public class DetalleDistribucion
    {
        public int Id { get; set; }

        // Descripción de cómo se distribuye el monto (ej: "70% para el propietario", "20% para mantenimiento")
        public string Descripcion { get; set; } = string.Empty;

        // Monto asociado a esta distribución
        public decimal Monto { get; set; }
    }
    public class Pago
    {
        public int Id { get; set; }

        // Referencia al contrato del pago
        public Contrato Contrato { get; set; }

        // Fecha en la que vence el pago
        public DateTime FechaVencimiento { get; set; }

        // Fecha en la que se realizó el pago (puede ser null si no se ha pagado aún)
        public DateTime? FechaPago { get; set; }

        // Monto total del pago (calculado como la suma de los montos en DetallePago)
        public decimal Monto => DetallesPago.Sum(d => d.Monto);

        // Estado del pago (Pendiente, Pagado, Vencido, etc.)
        public PagoEstado Estado { get; set; }

        // Fecha en la que se notificó al inquilino (null si aún no se notificó)
        public DateTime? FechaNotificacion { get; set; }

        // Para indicar a qué se debe el pago, ej: Cuota 1 de 12, Reparación, Firma de contrato
        public string Descripcion { get; set; } = string.Empty;

        // Lista de detalles que describen cómo surge el monto a pagar
        public List<DetallePago> DetallesPago { get; set; } = new();

        // Lista de detalles que describen cómo se distribuye el monto
        public List<DetalleDistribucion> DetallesDistribucion { get; set; } = new();
    }
    public class PagoEstado
    {
        public int Id { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
    public class Contrato
    {
        public int Id { get; set; }

        // Inquilino asociado al contrato
        public Inquilino Inquilino { get; set; }

        // Inmueble que se arrienda en el contrato
        public Inmueble Inmueble { get; set; }

        // Administrador responsable del contrato
        public Administrador Administrador { get; set; }

        // Fecha de inicio del contrato
        public DateTime FechaInicio { get; set; }

        // Fecha de finalización del contrato
        public DateTime FechaFin { get; set; }

        // Monto mensual del contrato
        public decimal MontoMensual { get; set; }

        // Estado actual del contrato
        public ContratoEstado Estado { get; set; }

        // Lista de pagos programados (Agenda de pagos)
        public List<Pago> Pagos { get; set; } = new();
    }
    public class ContratoEstado
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
    public class Permiso
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }

        #endregion
    }
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
    }
    public class Propietario : PersonaBase
    {
        public int PropietarioId { get; set; }
    }
    public class Reparacion
    {
        public int Id { get; set; }

        // Fecha en la que se solicita la reparación
        public DateTime FechaSolicitud { get; set; }

        // Fecha en la que inicia la reparación (puede ser null hasta que se asigne)
        public DateTime? FechaInicio { get; set; }

        // Fecha de finalización (puede ser null si aún no finalizó)
        public DateTime? FechaFin { get; set; }

        public string Descripcion { get; set; }
        public decimal? CostoEstimado { get; set; }
        public decimal? CostoFinal { get; set; }

        // Relación con el inmueble donde se realiza la reparación
        public Inmueble Inmueble { get; set; }
        public int InmuebleId { get; set; }

        // Estado de la reparación (Pendiente, En proceso, Finalizado)
        public ReparacionEstado Estado { get; set; }
        public int EstadoId { get; set; }

        // Empleado responsable de la reparación
        public Empleado Empleado { get; set; }
        public int EmpleadoId { get; set; }

        // Detalles de la reparación (Materiales, Mano de obra, etc.)
        public List<ReparacionDetalle> Detalles { get; set; } = new();
    }
    public class ReparacionEstado
    {
        public int Id { get; set; }
        public string Estado { get; set; } // Ej: Pendiente, En proceso, Finalizado
    }
    public class ReparacionDetalle
    {
        public int Id { get; set; }

        // Referencia a la reparación principal
        public Reparacion Reparacion { get; set; }

        // Descripción del trabajo realizado
        public string Descripcion { get; set; } = string.Empty;

        // Costo del trabajo realizado
        public decimal Costo { get; set; }

        // Fecha en que se realizó esta parte de la reparación
        public DateTime Fecha { get; set; }
    }
    public class Rol
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }

        #endregion

        #region Propiedades de navegación
        public List<Permiso>? Permisos { get; set; }

        #endregion
    }
    public class Telefono
    {
        public int TelefonoId { get; set; } // ID del teléfono
        public string Numero { get; set; } = string.Empty; // Número de teléfono
        public TipoTelefono Tipo { get; set; } = new(); // Asociación con el tipo de teléfono

        public override string ToString()
        {
            return $"({Tipo.Descripcion}) {this.Numero}";
        }
    }
    public class TipoTelefono
    {
        public int TipoTelefonoId { get; set; } // ID del tipo de teléfono
        public string Descripcion { get; set; } = string.Empty; // Descripción del tipo (Ej.: "Móvil", "Fijo")
    }
    public class Usuario
    {
        #region Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        public bool Activo { get; set; }

        public DateTime Creacion { get; set; }

        #endregion

        #region Propiedades de navegación
        public List<Rol>? Roles { get; set; }

        public List<Permiso> Permisos
        {
            get
            {
                HashSet<Permiso> permisos = new HashSet<Permiso>();

                foreach (Rol rol in Roles)
                {
                    foreach (Permiso permiso in rol.Permisos)
                    {
                        permisos.Add(permiso);
                    }
                }

                return permisos.ToList();
            }
        }

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

        public string PermisoString
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                {
                    return "No posee permisos asignados.-";
                }

                // Utilizamos un HashSet para evitar duplicados
                HashSet<string> permisosUnicos = new HashSet<string>();

                foreach (Rol rol in Roles)
                {
                    foreach (Permiso permiso in rol.Permisos)
                    {
                        permisosUnicos.Add(permiso.Nombre);
                    }
                }

                return string.Join(", ", permisosUnicos);
            }
        }

        #endregion

        #region Métodos privados

        #endregion
    }
}
