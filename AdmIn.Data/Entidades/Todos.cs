namespace AdmIn.Data.Entidades.Todos
{
    public class ADMINISTRADOR
    {
        public int ADM_ID { get; set; } // ID del administrador
        public int PER_ID { get; set; } // ID de la persona asociada
        public int? ADM_SUPERIOR_ID { get; set; } // ID del administrador superior (auto-referencia)
    }

    public class ALIADO
    {
        public int ALI_ID { get; set; } // ID del aliado
        public int ADM_ID { get; set; } // ID del administrador asociado
        public int PER_ID { get; set; } // ID de la persona asociada
    }

    public class CLIENTE
    {
        public int CLI_ID { get; set; } // ID del cliente
        public int PER_ID { get; set; } // ID de la persona asociada
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
    }

    public class CONTRATO
    {
        public int CON_ID { get; set; } // ID del contrato
        public int INQ_ID { get; set; } // ID del inquilino asociado
        public int INM_ID { get; set; } // ID del inmueble asociado
        public int ADM_ID { get; set; } // ID del administrador asociado
        public DateTime CON_FECHA_INICIO { get; set; } // Fecha de inicio del contrato
        public DateTime CON_FECHA_FIN { get; set; } // Fecha de fin del contrato
        public DateTime? CON_FECHA_CANCELACION { get; set; } // Fecha de cancelación del contrato (opcional)
        public decimal CON_MONTO_MENSUAL { get; set; } // Monto mensual del contrato
        public DateTime CON_FECHA_FIRMA { get; set; } // Fecha de firma del contrato
        public int CON_ESTADO_ID { get; set; } // ID del estado del contrato
        public string CON_COMENTARIOS { get; set; } // Comentarios sobre el contrato
    }

    public class CONTRATO_ESTADO
    {
        public int CON_EST_ID { get; set; } // ID del estado del contrato
        public string CON_EST_DESCRIPCION { get; set; } // Descripción del estado del contrato
    }

    public class DIRECCION
    {
        public int DIR_ID { get; set; } // ID de la dirección
        public string DIR_CALLE_NUMERO { get; set; } = string.Empty; // Calle y número de la dirección
        public string? DIR_COLONIA { get; set; } // Colonia de la dirección
        public string? DIR_CIUDAD { get; set; } // Ciudad de la dirección
        public string? DIR_ESTADO { get; set; } // Estado de la dirección
        public string? DIR_CP { get; set; } // Código postal de la dirección
        public string? DIR_PAIS { get; set; } // País de la dirección
    }

    public class DIRECCION_TIPO
    {
        public int DIR_TIP_ID { get; set; }

        public string DIR_TIP_TIPO { get; set; }
    }

    public class EMPLEADO
    {
        public int EMP_ID { get; set; } // ID del empleado
        public int PER_ID { get; set; } // ID de la persona asociada
        public decimal? EMP_HONORARIOS { get; set; } // Honorarios del empleado
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
        public int? EME_ID { get; set; } // ID de la especialidad del empleado
    }

    public class EMPLEADO_AGENDA
    {
        public int EMP_AGE_ID { get; set; } // ID de la agenda del empleado
        public int EMP_ID { get; set; } // ID del empleado
        public DateTime EMP_AGE_FECHA { get; set; } // Fecha del evento en la agenda
        public TimeSpan EMP_AGE_HORA_INICIO { get; set; } // Hora de inicio del evento
        public TimeSpan EMP_AGE_HORA_FIN { get; set; } // Hora de fin del evento
        public bool EMP_AGE_DISPONIBLE { get; set; } // Disponibilidad del empleado
    }

    public class EMPLEADO_ESPECIALIDAD
    {
        public int EME_ID { get; set; } // ID de la especialidad
        public string EME_DESCRIPCION { get; set; } // Descripción de la especialidad
    }

    public class INMUEBLE
    {
        public int INM_ID { get; set; } // ID del inmueble
        public int PRO_ID { get; set; } // ID de la propiedad asociada
        public int DIR_ID { get; set; } // ID de la dirección asociada
        public string INM_DESCRIPCION { get; set; } // Descripción del inmueble
        public string INM_COMENTARIO { get; set; } // Comentario sobre el inmueble
        public decimal INM_VALOR { get; set; } // Valor del inmueble
        public decimal INM_SUPERFICIEM { get; set; } // Superficie del inmueble
        public decimal INM_CONSTRUIDOM { get; set; } // Superficie construida del inmueble
        public int? TEL_ID { get; set; } // ID del teléfono asociado (opcional)
        public int INM_ESTADO_ID { get; set; } // ID del estado del inmueble
    }

    public class INMUEBLE_ESTADO
    {
        public int INM_EST_ID { get; set; } // ID del estado del inmueble
        public string INM_EST_DESCRIPCION { get; set; } // Descripción del estado del inmueble
    }

    public class INQUILINO
    {
        public int INQ_ID { get; set; } // ID del inquilino
        public int PER_ID { get; set; } // ID de la persona asociada
        public int INM_ID { get; set; } // ID del inmueble asociado
        public int? ADM_ID { get; set; } // ID del administrador asociado (opcional)
    }

    public class PAGO_AGENDA
    {
        public int PAGO_ID { get; set; } // ID de la agenda de pago
        public int CON_ID { get; set; } // ID del contrato
        public DateTime PAGO_FECHA_VENCIMIENTO { get; set; } // Fecha de vencimiento del pago
        public decimal PAGO_MONTO { get; set; } // Monto del pago
        public DateTime? PAGO_FECHA_NOTIFICACION { get; set; } // Fecha de notificación del pago
        public DateTime? PAGO_FECHA_REALIZADO { get; set; } // Fecha en la que se realizó el pago
        public int PAGO_ESTADO_ID { get; set; } // ID del estado del pago
    }

    public class PAGO_ESTADO
    {
        public int PAGO_ESTADO_ID { get; set; } // ID del estado del pago
        public string PAGO_ESTADO_DESCRIPCION { get; set; } // Descripción del estado del pago
    }

    public class PERMISO
    {
        public int PERM_ID { get; set; }

        public string PERM_NOMBRE { get; set; }
    }

    public class PERSONA
    {
        public int PER_ID { get; set; } // ID de la persona
        public string PER_RFC { get; set; } = string.Empty; // RFC de la persona
        public string PER_NOMBRE { get; set; } = string.Empty; // Nombre de la persona
        public string? PER_APATERNO { get; set; } // Apellido paterno de la persona
        public string? PER_AMATERNO { get; set; } // Apellido materno de la persona
        public string? PER_EMAIL { get; set; } // Email de la persona
        public string? PER_NACIONALIDAD { get; set; } // Nacionalidad de la persona
        public bool PER_ESPERSONA { get; set; } // Indica si es persona física
        public bool PER_TITULAR { get; set; } // Indica si es titular
    }

    public class PERSONA_DIRECCION
    {
        public int PER_DIR_ID { get; set; } // ID de la relación persona-dirección
        public int PER_ID { get; set; } // ID de la persona
        public int DIR_ID { get; set; } // ID de la dirección
        public int DIR_TIP_ID { get; set; } // Tipo de direccion
    }

    public class PERSONA_TELEFONO
    {
        public int PER_TEL_ID { get; set; }
        public int PER_ID { get; set; }
        public int TEL_ID { get; set; }
    }

    public class PROPIEDAD
    {
        public int PRO_ID { get; set; } // ID de la propiedad
        public string PRO_DESCRIPCION { get; set; } // Descripción de la propiedad
        public int DIR_ID { get; set; } // ID de la dirección asociada
        public int? TEL_ID { get; set; } // ID del teléfono asociado (opcional)
        public string PRO_EMAIL { get; set; } // Email de la propiedad
        public int? PRO_UNIDADES { get; set; } // Número de unidades
        public int? PRO_UNIDADES_HABITADAS { get; set; } // Número de unidades habitadas
        public string PRO_COMENTARIO { get; set; } // Comentario sobre la propiedad
        public decimal PRO_VALOR { get; set; } // Valor de la propiedad
        public decimal PRO_SUPERFICIEM { get; set; } // Superficie de la propiedad
        public int ADM_ID { get; set; } // ID del administrador asociado
        public int CLI_ID { get; set; } // ID del cliente asociado
        public int PRT_ID { get; set; } // ID del tipo de propiedad
    }

    public class PROPIEDAD_TIPO
    {
        public int PRT_ID { get; set; } // ID del tipo de propiedad
        public string PRT_DESCRIPCION { get; set; } // Descripción del tipo de propiedad
    }

    public class REPARACION
    {
        public int REP_ID { get; set; } // ID de la reparación
        public int INM_ID { get; set; } // ID del inmueble
        public int INQ_ID { get; set; } // ID del inquilino
        public int EMP_ID { get; set; } // ID del empleado
        public DateTime REP_FECHA_SOLICITUD { get; set; } // Fecha de solicitud de la reparación
        public DateTime? REP_FECHA_INICIO { get; set; } // Fecha de inicio de la reparación
        public DateTime? REP_FECHA_FIN { get; set; } // Fecha de fin de la reparación
        public decimal? REP_COSTO_ESTIMADO { get; set; } // Costo estimado de la reparación
        public decimal? REP_COSTO_FINAL { get; set; } // Costo final de la reparación
        public string REP_DESCRIPCION { get; set; } // Descripción de la reparación
        public int REP_EST_ID { get; set; } // ID del estado de la reparación
    }

    public class REPARACION_DETALLE
    {
        public int REP_DET_ID { get; set; } // ID del detalle de la reparación
        public int REP_ID { get; set; } // ID de la reparación a la que pertenece el detalle
        public string REP_DET_DESCRIPCION { get; set; } // Descripción del detalle de la reparación
        public decimal? REP_DET_COSTO { get; set; } // Costo del detalle de la reparación
        public DateTime REP_DET_FECHA { get; set; } // Fecha del detalle de la reparación
    }

    public class REPARACION_ESTADO
    {
        public int REP_ESTADO_ID { get; set; } // ID del estado de la reparación
        public string REP_ESTADO_DESCRIPCION { get; set; } // Descripción del estado de la reparación
    }

    public class ROL
    {
        public int ROL_ID { get; set; }
        public string ROL_NOMBRE { get; set; }
    }

    public class ROL_PERMISO
    {
        public int ROL_ID { get; set; }
        public int PERM_ID { get; set; }
    }

    public class TELEFONO
    {
        public int TEL_ID { get; set; } // ID del teléfono
        public string TEL_NUMERO { get; set; } = string.Empty; // Número de teléfono
        public int TPT_ID { get; set; } // ID del tipo de teléfono (FK)
    }

    public class TIPO_TELEFONO
    {
        public int TPT_ID { get; set; } // ID del tipo de teléfono
        public string TPT_DESCRIPCION { get; set; } = string.Empty; // Descripción del tipo de teléfono
    }

    public class USUARIO
    {
        public int USU_ID { get; set; }
        public string USU_NOMBRE { get; set; }
        public string USU_PASSWORD { get; set; }
        public string USU_EMAIL { get; set; }
        public DateTime USU_FECHA_CREACION { get; set; }
        public bool USU_ACTIVO { get; set; }

    }

    public class USUARIO_ROL
    {
        public int USU_ID { get; set; }
        public int ROL_ID { get; set; }
    }
}
