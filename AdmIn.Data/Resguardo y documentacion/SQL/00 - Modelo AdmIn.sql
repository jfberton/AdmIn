-- Eliminación de las tablas existentes 
DROP TABLE IF EXISTS REPARACION_PROPIEDAD
DROP TABLE IF EXISTS REPARACION_INMUEBLE
DROP TABLE IF EXISTS REPARACION_DETALLE;
DROP TABLE IF EXISTS REPARACION;
DROP TABLE IF EXISTS REPARACION_ESTADO;
DROP TABLE IF EXISTS ALIADO_AGENDA;
DROP TABLE IF EXISTS ALIADO;
DROP TABLE IF EXISTS ALIADO_ESPECIALIDAD;
DROP TABLE IF EXISTS PAGO_ADMINISTRADOR;
DROP TABLE IF EXISTS PAGO_AGENDA;
DROP TABLE IF EXISTS PAGO_ESTADO;
DROP TABLE IF EXISTS CONTRATO;
DROP TABLE IF EXISTS CONTRATO_ESTADO;
DROP TABLE IF EXISTS INQUILINO;
DROP TABLE IF EXISTS INMUEBLE;
DROP TABLE IF EXISTS INMUEBLE_ESTADO;
DROP TABLE IF EXISTS PROPIEDAD;
DROP TABLE IF EXISTS PROPIEDAD_TIPO;
DROP TABLE IF EXISTS CLIENTE;
DROP TABLE IF EXISTS PERSONA_TELEFONO;
DROP TABLE IF EXISTS TELEFONO;
DROP TABLE IF EXISTS TELEFONO_TIPO;
DROP TABLE IF EXISTS PERSONA_DIRECCION;
DROP TABLE IF EXISTS DIRECCION;
DROP TABLE IF EXISTS DIRECCION_TIPO;
DROP TABLE IF EXISTS ADMINISTRADOR;
DROP TABLE IF EXISTS PERSONA;


-- Creación de tablas 

-- Tabla Tipo de Teléfono
CREATE TABLE TELEFONO_TIPO (
    TEL_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    TEL_TIP_DESCRIPCION NVARCHAR(50) NOT NULL
);

-- Tabla Teléfono
CREATE TABLE TELEFONO (
    TEL_ID INT IDENTITY(1,1) PRIMARY KEY,
    TEL_NUMERO NVARCHAR(20) NOT NULL,
    TEL_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES TELEFONO_TIPO(TEL_TIP_ID)
);

-- Tabla Persona
CREATE TABLE PERSONA (
    PER_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_RFC NVARCHAR(20) NOT NULL,
    PER_NOMBRE NVARCHAR(100) NOT NULL,
    PER_APATERNO NVARCHAR(100),
    PER_AMATERNO NVARCHAR(100),
    PER_EMAIL NVARCHAR(100),
    PER_NACIONALIDAD NVARCHAR(50),
    PER_ESPERSONA BIT NOT NULL,
    PER_TITULAR BIT
);

-- Tabla Persona telefono
CREATE TABLE PERSONA_TELEFONO (
    PER_TEL_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    TEL_ID INT NOT NULL FOREIGN KEY REFERENCES TELEFONO(TEL_ID)
);

-- Tabla Dirección
CREATE TABLE DIRECCION (
    DIR_ID INT IDENTITY(1,1) PRIMARY KEY,
    DIR_CALLE_NUMERO NVARCHAR(150) NOT NULL,
    DIR_COLONIA NVARCHAR(100),
    DIR_CIUDAD NVARCHAR(100),
    DIR_ESTADO NVARCHAR(100),
    DIR_CP NVARCHAR(10),
    DIR_PAIS NVARCHAR(50)
);

-- Tabla Tipo de direccion persona
CREATE TABLE DIRECCION_TIPO (
    DIR_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    DIR_TIP_TIPO NVARCHAR(50)
);

-- Tabla Relación Persona-Dirección
CREATE TABLE PERSONA_DIRECCION (
    PER_DIR_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    DIR_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION_TIPO(DIR_TIP_ID)
);

-- Tabla Administradores
CREATE TABLE ADMINISTRADOR (
    ADM_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    ADM_SUPERIOR_ID INT FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID) NULL
);

-- Tabla Especialidad de Aliados
CREATE TABLE ALIADO_ESPECIALIDAD (
    ALI_ESP_ID INT IDENTITY(1,1) PRIMARY KEY,
    ALI_ESP_DESCRIPCION NVARCHAR(100) NOT NULL
);

-- Tabla Aliados
CREATE TABLE ALIADO (
    ALI_ID INT IDENTITY(1,1) PRIMARY KEY,
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID),
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
	ALI_ESP_ID INT NOT NULL FOREIGN KEY REFERENCES ALIADO_ESPECIALIDAD(ALI_ESP_ID)
);

-- Tabla Agenda de Aliados
CREATE TABLE ALIADO_AGENDA (
    ALI_AGE_ID INT IDENTITY(1,1) PRIMARY KEY,
    ALI_ID INT NOT NULL FOREIGN KEY REFERENCES ALIADO(ALI_ID),
    ALI_AGE_FECHA DATE NOT NULL,
    ALI_AGE_HORA_INICIO TIME NOT NULL,
    ALI_AGE_HORA_FIN TIME NOT NULL,
    ALI_AGE_DISPONIBLE BIT NOT NULL
);

-- Tabla Clientes
CREATE TABLE CLIENTE (
    CLI_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID)
);

-- Tabla Propiedad Tipo
CREATE TABLE PROPIEDAD_TIPO (
    PRO_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_TIP_DESCRIPCION NVARCHAR(100) NOT NULL
);

-- Tabla Propiedades
CREATE TABLE PROPIEDAD (
    PRO_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_DESCRIPCION NVARCHAR(200) NOT NULL,
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    TEL_ID INT FOREIGN KEY REFERENCES TELEFONO(TEL_ID) NULL,
    PRO_UNIDADES INT,
    PRO_UNIDADES_ALQUILADAS INT,
    PRO_COMENTARIO TEXT,
    PRO_VALOR DECIMAL(18,2),
    PRO_SUPERFICIEM DECIMAL(18,2),
    ADM_ID INT FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID),
    CLI_ID INT NOT NULL FOREIGN KEY REFERENCES CLIENTE(CLI_ID),
    PRO_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD_TIPO(PRO_TIP_ID)
);

-- Tabla de estados de inmueble
CREATE TABLE INMUEBLE_ESTADO (
    INM_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    INM_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- Tabla Inmuebles
CREATE TABLE INMUEBLE (
    INM_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD(PRO_ID),
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    INM_DESCRIPCION NVARCHAR(200),
    INM_COMENTARIO TEXT,
    INM_VALOR DECIMAL(18,2),
    INM_SUPERFICIEM DECIMAL(18,2),
    INM_CONSTRUIDOM DECIMAL(18,2),
    TEL_ID INT FOREIGN KEY REFERENCES TELEFONO(TEL_ID) NULL,
    INM_ESTADO_ID INT FOREIGN KEY REFERENCES INMUEBLE_ESTADO(INM_EST_ID)
);

-- Tabla Inquilinos
CREATE TABLE INQUILINO (
    INQ_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    INM_ID INT NOT NULL FOREIGN KEY REFERENCES INMUEBLE(INM_ID),
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID)
);

-- Tabla de estados de contrato
CREATE TABLE CONTRATO_ESTADO (
    CON_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    CON_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- Tabla Contratos
CREATE TABLE CONTRATO (
    CON_ID INT IDENTITY(1,1) PRIMARY KEY,
    INQ_ID INT NOT NULL FOREIGN KEY REFERENCES INQUILINO(INQ_ID),
    INM_ID INT NOT NULL FOREIGN KEY REFERENCES INMUEBLE(INM_ID),
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID),
    CON_FECHA_INICIO DATE NOT NULL,
    CON_FECHA_FIN DATE NOT NULL,
    CON_FECHA_CANCELACION DATE,
    CON_MONTO_MENSUAL DECIMAL(18,2) NOT NULL,
	CON_MONTO_DEPOSITO DECIMAL(18,2) NOT NULL,
	CON_RENTAS_INICIALES INT NOT NULL,
	CON_PORCENTAJE_COMISION DECIMAL(5,4) NOT NULL,
    CON_FECHA_FIRMA DATE NOT NULL,
    CON_ESTADO_ID INT NOT NULL FOREIGN KEY REFERENCES CONTRATO_ESTADO(CON_EST_ID),
    CON_COMENTARIOS TEXT
);

-- Tabla de estados de pago
CREATE TABLE PAGO_ESTADO (
    PAGO_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    PAGO_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- Tabla Pagos Administrador
CREATE TABLE PAGO_ADMINISTRADOR (
    PAGO_ADM_ID INT IDENTITY(1,1) PRIMARY KEY,
    CON_ID INT NOT NULL FOREIGN KEY REFERENCES CONTRATO(CON_ID),
    PAGO_ADM_MONTO DECIMAL(18,2) NOT NULL,
    PAGO_ADM_FECHA DATE NOT NULL,
    PAGO_ADM_ESTADO_ID INT NOT NULL FOREIGN KEY REFERENCES PAGO_ESTADO(PAGO_EST_ID),
    PAGO_ADM_COMENTARIOS TEXT
);

-- Tabla Agenda de Pagos
CREATE TABLE PAGO_AGENDA (
    PAGO_ID INT IDENTITY(1,1) PRIMARY KEY,
    CON_ID INT NOT NULL FOREIGN KEY REFERENCES CONTRATO(CON_ID),
	PAGO_CUOTA_NRO INT NOT NULL,
    PAGO_FECHA_VENCIMIENTO DATE NOT NULL,
    PAGO_MONTO DECIMAL(18,2) NOT NULL,
    PAGO_FECHA_NOTIFICACION DATE,
    PAGO_FECHA_REALIZADO DATE,
    PAGO_ESTADO_ID INT NOT NULL FOREIGN KEY REFERENCES PAGO_ESTADO(PAGO_EST_ID)
);

-- Tabla Estado de Reparaciones
CREATE TABLE REPARACION_ESTADO (
    REP_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- Tabla Reparaciones
CREATE TABLE REPARACION (
    REP_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_FECHA_SOLICITUD DATE NOT NULL,
    REP_FECHA_INICIO DATE,
    REP_FECHA_FIN DATE,
    REP_COSTO_ESTIMADO DECIMAL(18,2),
    REP_COSTO_FINAL DECIMAL(18,2),
    REP_DESCRIPCION NVARCHAR(500),
    REP_EST_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION_ESTADO(REP_EST_ID)
);

-- Tabla Detalle de Reparaciones
CREATE TABLE REPARACION_DETALLE (
    REP_DET_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID),
    REP_DET_DESCRIPCION NVARCHAR(500),
    REP_DET_COSTO DECIMAL(18,2),
    REP_DET_FECHA DATE
);

-- Tabla Detalle de Reparaciones relacionadas con Propiedad
CREATE TABLE REPARACION_PROPIEDAD (
	REP_PRO_ID INT IDENTITY(1,1) PRIMARY KEY,
	PRO_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD(PRO_ID),
	REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID)
);

-- Tabla Detalle de Reparaciones relacionadas con Inmueble
CREATE TABLE REPARACION_INMUEBLE (
	REP_INM_ID INT IDENTITY(1,1) PRIMARY KEY,
	INM_ID INT NOT NULL FOREIGN KEY REFERENCES INMUEBLE(INM_ID),
	REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID)
);


--INSERTAR VALORES POR DEFECTO

-- Insertar valores por defecto en la tabla CON_ESTADO
INSERT INTO CONTRATO_ESTADO (CON_EST_DESCRIPCION)
VALUES 
	('Activo') -- CONTRATO VIGENTE INMUEBLE ARRENDADO
	, ('Finalizado') -- CONTRATO FINALIZADO
	, ('Cancelado'); -- CONTRATO CANCELADO ANTICIPADAMENTE

-- Insertar valores por defecto en la tabla PAGO_ESTADO
INSERT INTO PAGO_ESTADO (PAGO_EST_DESCRIPCION)
VALUES 
	('Pendiente') -- DEUDA GENERADA PENDIENTE DE NOTIFICACION
	, ('Pagado') -- DEUDA PAGADA
	, ('Notificado') -- DEUDA NOTIFICADA AL DEUDOR
	, ('Cancelado'); -- ANULACION DE LA DEUDA

-- Insertar valores por defecto en la tabla INMUEBLE_ESTADO
INSERT INTO INMUEBLE_ESTADO (INM_EST_DESCRIPCION)
VALUES 
	('Libre') -- INMUEBLE LIBRE Y EN CONDICIONES DE SER ARRENDADO
	, ('Arrendado') --INMUEBLE ARRENDADO
	, ('En Mantenimiento') --INMUEBLE SIN ARRENDAR, EN MANTENIMIENTO
	, ('Reservado'); --INMUEBLE SIN ARRENDAR, RESERVADO POR UN POSIBLE INQUILINO


-- Inserta valores por defecto en la tabla REPARACION_ESTADO
INSERT INTO REPARACION_ESTADO (REP_EST_DESCRIPCION)
VALUES 
	('Solicitada'), -- REPARACION NUEVA
    ('Pendiente'), -- REPARACION SOLICITADA PENDIENTE DE ATENCION POR PARTE DEL EMPLEADO ASOCIADO
    ('En Proceso'), --REPARACION EN PROCESO INICIADA POR EL EMPLEADO
    ('Completada'), --REPARACION COMPLETADA
    ('Cancelada'); --REPARACION CANCELADA POR ALGUNA DE LAS PARTES

-- Inserta valores por defecto en TELEFONO_TIPO
INSERT INTO TELEFONO_TIPO (TEL_TIP_DESCRIPCION)
VALUES 
    ('Celular'),  
    ('Fijo'),  
    ('Trabajo'),  
    ('Casa'),  
    ('Fax'),  
    ('Otro');  
