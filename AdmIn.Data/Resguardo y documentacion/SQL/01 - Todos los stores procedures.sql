-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_PROPIEDAD.
-- Entradas: @PropiedadId INT, @ReparacionId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Insertar
    @PropiedadId INT,
    @ReparacionId INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_PROPIEDAD (PRO_ID, REP_ID)
    VALUES (@PropiedadId, @ReparacionId);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_PROPIEDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_PROPIEDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_PROPIEDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_PROPIEDAD WHERE REP_PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT, @PropiedadId INT, @ReparacionId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Actualizar
    @Id INT,
    @PropiedadId INT,
    @ReparacionId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE REPARACION_PROPIEDAD
    SET PRO_ID = @PropiedadId,
        REP_ID = @ReparacionId
    WHERE REP_PRO_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM REPARACION_PROPIEDAD WHERE REP_PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_INMUEBLE.
-- Entradas: @InmuebleId INT, @ReparacionId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Insertar
    @InmuebleId INT,
    @ReparacionId INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_INMUEBLE (INM_ID, REP_ID)
    VALUES (@InmuebleId, @ReparacionId);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_INMUEBLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_INMUEBLE.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_INMUEBLE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-inmueble.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_INMUEBLE WHERE REP_INM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT, @InmuebleId INT, @ReparacionId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Actualizar
    @Id INT,
    @InmuebleId INT,
    @ReparacionId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION_INMUEBLE
    SET INM_ID = @InmuebleId,
        REP_ID = @ReparacionId
    WHERE REP_INM_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-inmueble.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION_INMUEBLE WHERE REP_INM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_DETALLE.
-- Entradas: @ReparacionId INT, @Descripcion NVARCHAR(200), @Costo DECIMAL(18,2).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Insertar
    @ReparacionId INT,
    @Descripcion NVARCHAR(200),
    @Costo DECIMAL(18,2),
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO REPARACION_DETALLE (REP_ID, REP_DET_DESCRIPCION, REP_DET_COSTO)
    VALUES (@ReparacionId, @Descripcion, @Costo);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_DETALLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_DETALLE.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION_DETALLE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT - ID del detalle de reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION_DETALLE WHERE REP_DET_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT, @ReparacionId INT, @Descripcion NVARCHAR(200), @Costo DECIMAL(18,2).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Actualizar
    @Id INT,
    @ReparacionId INT,
    @Descripcion NVARCHAR(200),
    @Costo DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION_DETALLE
    SET REP_ID = @ReparacionId,
        REP_DET_DESCRIPCION = @Descripcion,
        REP_DET_COSTO = @Costo
    WHERE REP_DET_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT - ID del detalle de reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION_DETALLE WHERE REP_DET_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION.
-- Entradas: @EstadoId INT, @Descripcion NVARCHAR(200), @Fecha DATE.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Insertar
    @REP_FECHA_SOLICITUD DATE,
    @REP_FECHA_INICIO DATE = NULL,
    @REP_FECHA_FIN DATE = NULL,
    @REP_COSTO_ESTIMADO DECIMAL(18,2) = NULL,
    @REP_COSTO_FINAL DECIMAL(18,2) = NULL,
    @REP_DESCRIPCION NVARCHAR(500) = NULL,
    @REP_EST_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO REPARACION (REP_FECHA_SOLICITUD, REP_FECHA_INICIO, REP_FECHA_FIN, REP_COSTO_ESTIMADO, REP_COSTO_FINAL, REP_DESCRIPCION, REP_EST_ID)
    VALUES (@REP_FECHA_SOLICITUD, @REP_FECHA_INICIO, @REP_FECHA_FIN, @REP_COSTO_ESTIMADO, @REP_COSTO_FINAL, @REP_DESCRIPCION, @REP_EST_ID);
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de REPARACION basado en su ID.
-- Entradas: @Id INT - ID de la reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_ObtenerPorId
    @REP_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION WHERE REP_ID = @REP_ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en REPARACION basado en su ID.
-- Entradas: @Id INT, @EstadoId INT, @Descripcion NVARCHAR(200), @Fecha DATE.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Actualizar
    @REP_ID INT,
    @REP_FECHA_SOLICITUD DATE,
    @REP_FECHA_INICIO DATE = NULL,
    @REP_FECHA_FIN DATE = NULL,
    @REP_COSTO_ESTIMADO DECIMAL(18,2) = NULL,
    @REP_COSTO_FINAL DECIMAL(18,2) = NULL,
    @REP_DESCRIPCION NVARCHAR(500) = NULL,
    @REP_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION
    SET REP_FECHA_SOLICITUD = @REP_FECHA_SOLICITUD,
        REP_FECHA_INICIO = @REP_FECHA_INICIO,
        REP_FECHA_FIN = @REP_FECHA_FIN,
        REP_COSTO_ESTIMADO = @REP_COSTO_ESTIMADO,
        REP_COSTO_FINAL = @REP_COSTO_FINAL,
        REP_DESCRIPCION = @REP_DESCRIPCION,
        REP_EST_ID = @REP_EST_ID
    WHERE REP_ID = @REP_ID;
    SELECT @REP_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de REPARACION basado en su ID.
-- Entradas: @Id INT - ID de la reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Eliminar
    @REP_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION WHERE REP_ID = @REP_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_ESTADO (REP_EST_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_ESTADO WHERE REP_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE REPARACION_ESTADO
    SET REP_EST_DESCRIPCION = @Descripcion
    WHERE REP_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM REPARACION_ESTADO WHERE REP_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ALIADO_AGENDA.
-- Entradas: @AliadoId INT, @Fecha DATE, @HoraInicio TIME, @HoraFin TIME, @Disponible BIT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Agenda_Insertar
    @AliadoId INT,
    @Fecha DATE,
    @HoraInicio TIME,
    @HoraFin TIME,
    @Disponible BIT,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ALIADO_AGENDA (ALI_ID, ALI_AGE_FECHA, ALI_AGE_HORA_INICIO, ALI_AGE_HORA_FIN, ALI_AGE_DISPONIBLE)
    VALUES (@AliadoId, @Fecha, @HoraInicio, @HoraFin, @Disponible);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ALIADO_AGENDA.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ALIADO_AGENDA.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Agenda_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_AGENDA;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ALIADO_AGENDA basado en su ID.
-- Entradas: @Id INT - ID del registro de agenda.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Agenda_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_AGENDA WHERE ALI_AGE_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ALIADO_AGENDA basado en su ID.
-- Entradas: @Id INT, @AliadoId INT, @Fecha DATE, @HoraInicio TIME, @HoraFin TIME, @Disponible BIT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Agenda_Actualizar
    @Id INT,
    @AliadoId INT,
    @Fecha DATE,
    @HoraInicio TIME,
    @HoraFin TIME,
    @Disponible BIT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ALIADO_AGENDA
    SET ALI_ID = @AliadoId,
        ALI_AGE_FECHA = @Fecha,
        ALI_AGE_HORA_INICIO = @HoraInicio,
        ALI_AGE_HORA_FIN = @HoraFin,
        ALI_AGE_DISPONIBLE = @Disponible
    WHERE ALI_AGE_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ALIADO_AGENDA basado en su ID.
-- Entradas: @Id INT - ID del registro de agenda.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Agenda_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ALIADO_AGENDA WHERE ALI_AGE_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ALIADO.
-- Entradas: @AdminId INT, @PersonaId INT, @EspecialidadId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Insertar
    @AdminId INT,
    @PersonaId INT,
    @EspecialidadId INT,
	@NuevoId INT OUTPUT 
	
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ALIADO (ADM_ID, PER_ID, ALI_ESP_ID)
    VALUES (@AdminId, @PersonaId, @EspecialidadId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ALIADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ALIADO.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ALIADO basado en su ID.
-- Entradas: @Id INT - ID del aliado.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO WHERE ALI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ALIADO basado en su ID.
-- Entradas: @Id INT, @AdminId INT, @PersonaId INT, @EspecialidadId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Actualizar
    @Id INT,
    @AdminId INT,
    @PersonaId INT,
    @EspecialidadId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ALIADO
    SET ADM_ID = @AdminId,
        PER_ID = @PersonaId,
        ALI_ESP_ID = @EspecialidadId
    WHERE ALI_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ALIADO basado en su ID.
-- Entradas: @Id INT - ID del aliado.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ALIADO WHERE ALI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ALIADO_ESPECIALIDAD.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ALIADO_ESPECIALIDAD (ALI_ESP_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ALIADO_ESPECIALIDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ALIADO_ESPECIALIDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_ESPECIALIDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT - ID de la especialidad del aliado.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_ESPECIALIDAD WHERE ALI_ESP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ALIADO_ESPECIALIDAD
    SET ALI_ESP_DESCRIPCION = @Descripcion
    WHERE ALI_ESP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT - ID de la especialidad del aliado.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ALIADO_ESPECIALIDAD WHERE ALI_ESP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PAGO_ADMINISTRADOR.
-- Entradas: @AdminId INT, @Monto DECIMAL(18,2), @FechaPago DATE.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Administrador_Insertar
    @CON_ID INT,
    @PAGO_ADM_MONTO DECIMAL(18,2),
    @PAGO_ADM_FECHA DATE,
    @PAGO_ADM_ESTADO_ID INT,
    @PAGO_ADM_COMENTARIOS TEXT = NULL,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PAGO_ADMINISTRADOR (CON_ID, PAGO_ADM_MONTO, PAGO_ADM_FECHA, PAGO_ADM_ESTADO_ID, PAGO_ADM_COMENTARIOS)
    VALUES (@CON_ID, @PAGO_ADM_MONTO, @PAGO_ADM_FECHA, @PAGO_ADM_ESTADO_ID, @PAGO_ADM_COMENTARIOS);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla PAGO_ADMINISTRADOR.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PAGO_ADMINISTRADOR.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Administrador_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM PAGO_ADMINISTRADOR;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de PAGO_ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del pago.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Administrador_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM PAGO_ADMINISTRADOR WHERE PAGO_ADM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en PAGO_ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT, @AdminId INT, @Monto DECIMAL(18,2), @FechaPago DATE.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Administrador_Actualizar
    @PAGO_ADM_ID INT,
    @CON_ID INT,
    @PAGO_ADM_MONTO DECIMAL(18,2),
    @PAGO_ADM_FECHA DATE,
    @PAGO_ADM_ESTADO_ID INT,
    @PAGO_ADM_COMENTARIOS TEXT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PAGO_ADMINISTRADOR
    SET CON_ID = @CON_ID,
        PAGO_ADM_MONTO = @PAGO_ADM_MONTO,
        PAGO_ADM_FECHA = @PAGO_ADM_FECHA,
        PAGO_ADM_ESTADO_ID = @PAGO_ADM_ESTADO_ID,
        PAGO_ADM_COMENTARIOS = @PAGO_ADM_COMENTARIOS
    WHERE PAGO_ADM_ID = @PAGO_ADM_ID;
    SELECT @PAGO_ADM_ID AS UpdatedID;
END
GO
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de PAGO_ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del pago.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Administrador_Eliminar
    @PAGO_ADM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PAGO_ADMINISTRADOR WHERE PAGO_ADM_ID = @PAGO_ADM_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PAGO_AGENDA.
-- Entradas: @AgendaId INT, @Monto DECIMAL(18,2), @FechaPago DATE.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Agenda_Insertar
    @CON_ID INT,
    @PAGO_CUOTA_NRO INT,
    @PAGO_FECHA_VENCIMIENTO DATE,
    @PAGO_MONTO DECIMAL(18,2),
    @PAGO_FECHA_NOTIFICACION DATE = NULL,
    @PAGO_FECHA_REALIZADO DATE = NULL,
    @PAGO_ESTADO_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PAGO_AGENDA (CON_ID, PAGO_CUOTA_NRO, PAGO_FECHA_VENCIMIENTO, PAGO_MONTO, PAGO_FECHA_NOTIFICACION, PAGO_FECHA_REALIZADO, PAGO_ESTADO_ID)
    VALUES (@CON_ID, @PAGO_CUOTA_NRO, @PAGO_FECHA_VENCIMIENTO, @PAGO_MONTO, @PAGO_FECHA_NOTIFICACION, @PAGO_FECHA_REALIZADO, @PAGO_ESTADO_ID);
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla PAGO_AGENDA.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PAGO_AGENDA.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Agenda_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PAGO_AGENDA;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de PAGO_AGENDA basado en su ID.
-- Entradas: @Id INT - ID del pago de agenda.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Agenda_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PAGO_AGENDA WHERE PAGO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en PAGO_AGENDA basado en su ID.
-- Entradas: @Id INT, @AgendaId INT, @Monto DECIMAL(18,2), @FechaPago DATE.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Agenda_Actualizar
    @PAGO_ID INT,
    @CON_ID INT,
    @PAGO_CUOTA_NRO INT,
    @PAGO_FECHA_VENCIMIENTO DATE,
    @PAGO_MONTO DECIMAL(18,2),
    @PAGO_FECHA_NOTIFICACION DATE = NULL,
    @PAGO_FECHA_REALIZADO DATE = NULL,
    @PAGO_ESTADO_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PAGO_AGENDA
    SET CON_ID = @CON_ID,
        PAGO_CUOTA_NRO = @PAGO_CUOTA_NRO,
        PAGO_FECHA_VENCIMIENTO = @PAGO_FECHA_VENCIMIENTO,
        PAGO_MONTO = @PAGO_MONTO,
        PAGO_FECHA_NOTIFICACION = @PAGO_FECHA_NOTIFICACION,
        PAGO_FECHA_REALIZADO = @PAGO_FECHA_REALIZADO,
        PAGO_ESTADO_ID = @PAGO_ESTADO_ID
    WHERE PAGO_ID = @PAGO_ID;
    SELECT @PAGO_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de PAGO_AGENDA basado en su ID.
-- Entradas: @Id INT - ID del pago de agenda.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Agenda_Eliminar
    @PAGO_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PAGO_AGENDA WHERE PAGO_ID = @PAGO_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PAGO_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Insertar
    @PAGO_EST_DESCRIPCION NVARCHAR(50),
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PAGO_ESTADO (PAGO_EST_DESCRIPCION)
    VALUES (@PAGO_EST_DESCRIPCION);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla PAGO_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PAGO_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PAGO_ESTADO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de pago.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_ObtenerPorId
    @PAGO_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PAGO_ESTADO WHERE PAGO_EST_ID = @PAGO_EST_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Actualizar
    @PAGO_EST_ID INT,
    @PAGO_EST_DESCRIPCION NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PAGO_ESTADO
    SET PAGO_EST_DESCRIPCION = @PAGO_EST_DESCRIPCION
    WHERE PAGO_EST_ID = @PAGO_EST_ID;
    SELECT @PAGO_EST_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de pago.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Eliminar
    @PAGO_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PAGO_ESTADO WHERE PAGO_EST_ID = @PAGO_EST_ID;
END
GO


-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Inserta un nuevo registro en la tabla CONTRATO.
-- Entradas: @INQ_ID INT, @INM_ID INT, @ADM_ID INT, @CON_FECHA_INICIO DATE, @CON_FECHA_FIN DATE, 
--          @CON_FECHA_CANCELACION DATE, @CON_MONTO_MENSUAL DECIMAL(18,2), @CON_MONTO_DEPOSITO DECIMAL(18,2),
--          @CON_RENTAS_INICIALES INT, @CON_PORCENTAJE_COMISION DECIMAL(5,4), @CON_FECHA_FIRMA DATE, 
--          @CON_ESTADO_ID INT, @CON_COMENTARIOS TEXT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Insertar
    @INQ_ID INT,
    @INM_ID INT,
    @ADM_ID INT,
    @CON_FECHA_INICIO DATE,
    @CON_FECHA_FIN DATE,
    @CON_FECHA_CANCELACION DATE = NULL,
    @CON_MONTO_MENSUAL DECIMAL(18,2),
    @CON_MONTO_DEPOSITO DECIMAL(18,2),
    @CON_RENTAS_INICIALES INT,
    @CON_PORCENTAJE_COMISION DECIMAL(5,4),
    @CON_FECHA_FIRMA DATE,
    @CON_ESTADO_ID INT,
    @CON_COMENTARIOS TEXT = NULL,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO CONTRATO (INQ_ID, INM_ID, ADM_ID, CON_FECHA_INICIO, CON_FECHA_FIN, CON_FECHA_CANCELACION, 
                          CON_MONTO_MENSUAL, CON_MONTO_DEPOSITO, CON_RENTAS_INICIALES, CON_PORCENTAJE_COMISION, 
                          CON_FECHA_FIRMA, CON_ESTADO_ID, CON_COMENTARIOS)
    VALUES (@INQ_ID, @INM_ID, @ADM_ID, @CON_FECHA_INICIO, @CON_FECHA_FIN, @CON_FECHA_CANCELACION, 
            @CON_MONTO_MENSUAL, @CON_MONTO_DEPOSITO, @CON_RENTAS_INICIALES, @CON_PORCENTAJE_COMISION, 
            @CON_FECHA_FIRMA, @CON_ESTADO_ID, @CON_COMENTARIOS);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve todos los registros de la tabla CONTRATO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de la tabla.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM CONTRATO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve un registro de la tabla CONTRATO por su ID.
-- Entradas: @CON_ID INT.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_ObtenerPorId
    @CON_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM CONTRATO WHERE CON_ID = @CON_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Actualiza un registro en la tabla CONTRATO.
-- Entradas: @CON_ID INT, @INQ_ID INT, @INM_ID INT, @ADM_ID INT, @CON_FECHA_INICIO DATE, @CON_FECHA_FIN DATE, 
--          @CON_FECHA_CANCELACION DATE, @CON_MONTO_MENSUAL DECIMAL(18,2), @CON_MONTO_DEPOSITO DECIMAL(18,2),
--          @CON_RENTAS_INICIALES INT, @CON_PORCENTAJE_COMISION DECIMAL(5,4), @CON_FECHA_FIRMA DATE, 
--          @CON_ESTADO_ID INT, @CON_COMENTARIOS TEXT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Actualizar
    @CON_ID INT,
    @INQ_ID INT,
    @INM_ID INT,
    @ADM_ID INT,
    @CON_FECHA_INICIO DATE,
    @CON_FECHA_FIN DATE,
    @CON_FECHA_CANCELACION DATE = NULL,
    @CON_MONTO_MENSUAL DECIMAL(18,2),
    @CON_MONTO_DEPOSITO DECIMAL(18,2),
    @CON_RENTAS_INICIALES INT,
    @CON_PORCENTAJE_COMISION DECIMAL(5,4),
    @CON_FECHA_FIRMA DATE,
    @CON_ESTADO_ID INT,
    @CON_COMENTARIOS TEXT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE CONTRATO
    SET INQ_ID = @INQ_ID, INM_ID = @INM_ID, ADM_ID = @ADM_ID, CON_FECHA_INICIO = @CON_FECHA_INICIO, 
        CON_FECHA_FIN = @CON_FECHA_FIN, CON_FECHA_CANCELACION = @CON_FECHA_CANCELACION, 
        CON_MONTO_MENSUAL = @CON_MONTO_MENSUAL, CON_MONTO_DEPOSITO = @CON_MONTO_DEPOSITO, 
        CON_RENTAS_INICIALES = @CON_RENTAS_INICIALES, CON_PORCENTAJE_COMISION = @CON_PORCENTAJE_COMISION, 
        CON_FECHA_FIRMA = @CON_FECHA_FIRMA, CON_ESTADO_ID = @CON_ESTADO_ID, CON_COMENTARIOS = @CON_COMENTARIOS
    WHERE CON_ID = @CON_ID;
    SELECT @CON_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Elimina un registro de la tabla CONTRATO por su ID.
-- Entradas: @CON_ID INT.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Eliminar
    @CON_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM CONTRATO WHERE CON_ID = @CON_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla CONTRATO_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO CONTRATO_ESTADO (CON_EST_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla CONTRATO_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de CONTRATO_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CONTRATO_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del contrato.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CONTRATO_ESTADO WHERE CON_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE CONTRATO_ESTADO
    SET CON_EST_DESCRIPCION = @Descripcion
    WHERE CON_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del contrato.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM CONTRATO_ESTADO WHERE CON_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INQUILINO.
-- Entradas: @PER_ID INT, @INM_ID INT, @ADM_ID INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Insertar
    @PER_ID INT,
    @INM_ID INT,
    @ADM_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO INQUILINO (PER_ID, INM_ID, ADM_ID)
    VALUES (@PER_ID, @INM_ID, @ADM_ID);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve todos los registros de la tabla INQUILINO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de la tabla.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INQUILINO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve un registro de la tabla INQUILINO por su ID.
-- Entradas: @INQ_ID INT.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_ObtenerPorId
    @INQ_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INQUILINO WHERE INQ_ID = @INQ_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Actualiza un registro en la tabla INQUILINO.
-- Entradas: @INQ_ID INT, @PER_ID INT, @INM_ID INT, @ADM_ID INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Actualizar
    @INQ_ID INT,
    @PER_ID INT,
    @INM_ID INT,
    @ADM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE INQUILINO
    SET PER_ID = @PER_ID, INM_ID = @INM_ID, ADM_ID = @ADM_ID
    WHERE INQ_ID = @INQ_ID;
    SELECT @INQ_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Elimina un registro de la tabla INQUILINO por su ID.
-- Entradas: @INQ_ID INT.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Eliminar
    @INQ_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM INQUILINO WHERE INQ_ID = @INQ_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INMUEBLE.
-- Entradas: @PRO_ID INT, @DIR_ID INT, @INM_DESCRIPCION NVARCHAR(200), @INM_COMENTARIO TEXT, @INM_VALOR DECIMAL(18,2), 
--          @INM_SUPERFICIEM DECIMAL(18,2), @INM_CONSTRUIDOM DECIMAL(18,2), @TEL_ID INT, @INM_ESTADO_ID INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Insertar
    @PRO_ID INT,
    @DIR_ID INT,
    @INM_DESCRIPCION NVARCHAR(200) = NULL,
    @INM_COMENTARIO TEXT = NULL,
    @INM_VALOR DECIMAL(18,2) = NULL,
    @INM_SUPERFICIEM DECIMAL(18,2) = NULL,
    @INM_CONSTRUIDOM DECIMAL(18,2) = NULL,
    @TEL_ID INT = NULL,
    @INM_ESTADO_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO INMUEBLE (PRO_ID, DIR_ID, INM_DESCRIPCION, INM_COMENTARIO, INM_VALOR, INM_SUPERFICIEM, INM_CONSTRUIDOM, TEL_ID, INM_ESTADO_ID)
    VALUES (@PRO_ID, @DIR_ID, @INM_DESCRIPCION, @INM_COMENTARIO, @INM_VALOR, @INM_SUPERFICIEM, @INM_CONSTRUIDOM, @TEL_ID, @INM_ESTADO_ID);
    
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve todos los registros de la tabla INMUEBLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de la tabla.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INMUEBLE;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve un registro de la tabla INMUEBLE por su ID.
-- Entradas: @INM_ID INT.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ObtenerPorId
    @INM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INMUEBLE WHERE INM_ID = @INM_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Actualiza un registro en la tabla INMUEBLE.
-- Entradas: @INM_ID INT, @PRO_ID INT, @DIR_ID INT, @INM_DESCRIPCION NVARCHAR(200), @INM_COMENTARIO TEXT, @INM_VALOR DECIMAL(18,2), 
--          @INM_SUPERFICIEM DECIMAL(18,2), @INM_CONSTRUIDOM DECIMAL(18,2), @TEL_ID INT, @INM_ESTADO_ID INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Actualizar
    @INM_ID INT,
    @PRO_ID INT,
    @DIR_ID INT,
    @INM_DESCRIPCION NVARCHAR(200) = NULL,
    @INM_COMENTARIO TEXT = NULL,
    @INM_VALOR DECIMAL(18,2) = NULL,
    @INM_SUPERFICIEM DECIMAL(18,2) = NULL,
    @INM_CONSTRUIDOM DECIMAL(18,2) = NULL,
    @TEL_ID INT = NULL,
    @INM_ESTADO_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE INMUEBLE
    SET PRO_ID = @PRO_ID, DIR_ID = @DIR_ID, INM_DESCRIPCION = @INM_DESCRIPCION, INM_COMENTARIO = @INM_COMENTARIO, 
        INM_VALOR = @INM_VALOR, INM_SUPERFICIEM = @INM_SUPERFICIEM, INM_CONSTRUIDOM = @INM_CONSTRUIDOM, 
        TEL_ID = @TEL_ID, INM_ESTADO_ID = @INM_ESTADO_ID
    WHERE INM_ID = @INM_ID;
    SELECT @INM_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Elimina un registro de la tabla INMUEBLE por su ID.
-- Entradas: @INM_ID INT.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Eliminar
    @INM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM INMUEBLE WHERE INM_ID = @INM_ID;
END
GO


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INMUEBLE_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO INMUEBLE_ESTADO (INM_EST_DESCRIPCION)
    VALUES (@Descripcion);

   	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla INMUEBLE_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de INMUEBLE_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ESTADO_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM INMUEBLE_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del inmueble.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM INMUEBLE_ESTADO WHERE INM_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE INMUEBLE_ESTADO
    SET INM_EST_DESCRIPCION = @Descripcion
    WHERE INM_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del inmueble.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM INMUEBLE_ESTADO WHERE INM_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PROPIEDAD.
-- Entradas: @ClienteId INT, @TipoId INT, @Descripcion NVARCHAR(200).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Insertar
    @ClienteId INT,
    @TipoId INT,
    @Descripcion NVARCHAR(200),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PROPIEDAD (CLI_ID, PRO_TIP_ID, PRO_DESCRIPCION)
    VALUES (@ClienteId, @TipoId, @Descripcion);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PROPIEDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PROPIEDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID de la propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD WHERE PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PROPIEDAD basado en su ID.
-- Entradas: @Id INT, @ClienteId INT, @TipoId INT, @Descripcion NVARCHAR(200).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Actualizar
    @Id INT,
    @ClienteId INT,
    @TipoId INT,
    @Descripcion NVARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PROPIEDAD
    SET CLI_ID = @ClienteId,
        PRO_TIP_ID = @TipoId,
        PRO_DESCRIPCION = @Descripcion
    WHERE PRO_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID de la propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PROPIEDAD WHERE PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PROPIEDAD_TIPO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Insertar
	@Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PROPIEDAD_TIPO (PRO_TIP_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PROPIEDAD_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PROPIEDAD_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD_TIPO WHERE PRO_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PROPIEDAD_TIPO
    SET PRO_TIP_DESCRIPCION = @Descripcion
    WHERE PRO_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PROPIEDAD_TIPO WHERE PRO_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla CLIENTE.
-- Entradas: @PersonaId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Insertar
    @PersonaId INT,
	@AdminId INT, 
    @NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CLIENTE (PER_ID, ADM_ID)
    VALUES (@PersonaId, @AdminId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla CLIENTE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de CLIENTE.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CLIENTE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de CLIENTE basado en su ID.
-- Entradas: @Id INT - ID del cliente.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CLIENTE WHERE CLI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en CLIENTE basado en su ID.
-- Entradas: @Id INT, @PersonaId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Actualizar
    @Id INT,
    @PersonaId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE CLIENTE
    SET PER_ID = @PersonaId
    WHERE CLI_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de CLIENTE basado en su ID.
-- Entradas: @Id INT - ID del cliente.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM CLIENTE WHERE CLI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA_TELEFONO.
-- Entradas: @PersonaId INT, @TelefonoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Insertar
    @PersonaId INT,
    @TelefonoId INT,
    @NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA_TELEFONO (PER_ID, TEL_ID)
    VALUES (@PersonaId, @TelefonoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA_TELEFONO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA_TELEFONO.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_TELEFONO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del registro persona-teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_TELEFONO WHERE PER_TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @TelefonoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Actualizar
    @Id INT,
    @PersonaId INT,
    @TelefonoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA_TELEFONO
    SET PER_ID = @PersonaId,
        TEL_ID = @TelefonoId
    WHERE PER_TEL_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del registro persona-teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA_TELEFONO WHERE PER_TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla TELEFONO.
-- Entradas: @Numero NVARCHAR(20), @TipoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Insertar
    @Numero NVARCHAR(20),
    @TipoId INT,
    @NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO TELEFONO (TEL_NUMERO, TEL_TIP_ID)
    VALUES (@Numero, @TipoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla TELEFONO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de TELEFONO.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO WHERE TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en TELEFONO basado en su ID.
-- Entradas: @Id INT, @Numero NVARCHAR(20), @TipoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Actualizar
    @Id INT,
    @Numero NVARCHAR(20),
    @TipoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE TELEFONO
    SET TEL_NUMERO = @Numero,
        TEL_TIP_ID = @TipoId
    WHERE TEL_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM TELEFONO WHERE TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla TELEFONO_TIPO.
-- Entradas: @Descripcion NVARCHAR(50) - Descripción del tipo de teléfono.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Insertar
    @Descripcion NVARCHAR(50),
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO TELEFONO_TIPO (TEL_TIP_DESCRIPCION)
    VALUES (@Descripcion);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla TELEFONO_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de TELEFONO_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO_TIPO WHERE TEL_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE TELEFONO_TIPO
    SET TEL_TIP_DESCRIPCION = @Descripcion
    WHERE TEL_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM TELEFONO_TIPO WHERE TEL_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA_DIRECCION.
-- Entradas: @PersonaId INT, @DireccionId INT, @TipoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Insertar
    @PersonaId INT,
    @DireccionId INT,
    @TipoId INT,
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA_DIRECCION (PER_ID, DIR_ID, DIR_TIP_ID)
    VALUES (@PersonaId, @DireccionId, @TipoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA_DIRECCION.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA_DIRECCION.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_DIRECCION;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la relación persona-dirección.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_DIRECCION WHERE PER_DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @DireccionId INT, @TipoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Actualizar
    @Id INT,
    @PersonaId INT,
    @DireccionId INT,
    @TipoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA_DIRECCION
    SET PER_ID = @PersonaId,
        DIR_ID = @DireccionId,
        DIR_TIP_ID = @TipoId
    WHERE PER_DIR_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la relación persona-dirección.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA_DIRECCION WHERE PER_DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla DIRECCION.
-- Entradas: @CalleNumero NVARCHAR(150), @Colonia NVARCHAR(100), @Ciudad NVARCHAR(100), 
--           @Estado NVARCHAR(100), @CodigoPostal NVARCHAR(10), @Pais NVARCHAR(50).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Insertar
    @CalleNumero NVARCHAR(150),
    @Colonia NVARCHAR(100),
    @Ciudad NVARCHAR(100),
    @Estado NVARCHAR(100),
    @CodigoPostal NVARCHAR(10),
    @Pais NVARCHAR(50),
    @NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO DIRECCION (
        DIR_CALLE_NUMERO, DIR_COLONIA, DIR_CIUDAD, DIR_ESTADO, DIR_CP, DIR_PAIS
    )
    VALUES (
        @CalleNumero, @Colonia, @Ciudad, @Estado, @CodigoPostal, @Pais
    );

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla DIRECCION.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de DIRECCION.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la dirección.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION WHERE DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en DIRECCION basado en su ID.
-- Entradas: @Id INT, @CalleNumero NVARCHAR(150), @Colonia NVARCHAR(100), 
--           @Ciudad NVARCHAR(100), @Estado NVARCHAR(100), @CodigoPostal NVARCHAR(10), 
--           @Pais NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Actualizar
    @Id INT,
    @CalleNumero NVARCHAR(150),
    @Colonia NVARCHAR(100),
    @Ciudad NVARCHAR(100),
    @Estado NVARCHAR(100),
    @CodigoPostal NVARCHAR(10),
    @Pais NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE DIRECCION
    SET DIR_CALLE_NUMERO = @CalleNumero,
        DIR_COLONIA = @Colonia,
        DIR_CIUDAD = @Ciudad,
        DIR_ESTADO = @Estado,
        DIR_CP = @CodigoPostal,
        DIR_PAIS = @Pais
    WHERE DIR_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la dirección.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM DIRECCION WHERE DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla DIRECCION_TIPO.
-- Entradas: @Tipo NVARCHAR(50).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Insertar
    @Tipo NVARCHAR(50),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO DIRECCION_TIPO (DIR_TIP_TIPO)
    VALUES (@Tipo);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla DIRECCION_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de DIRECCION_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de dirección.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION_TIPO WHERE DIR_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT, @Tipo NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Actualizar
    @Id INT,
    @Tipo NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE DIRECCION_TIPO
    SET DIR_TIP_TIPO = @Tipo
    WHERE DIR_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de dirección.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM DIRECCION_TIPO WHERE DIR_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ADMINISTRADOR.
-- Entradas: @PersonaId INT, @SuperiorId INT (opcional).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Insertar
    @PersonaId INT,
    @SuperiorId INT = NULL,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ADMINISTRADOR (PER_ID, ADM_SUPERIOR_ID)
    VALUES (@PersonaId, @SuperiorId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ADMINISTRADOR.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ADMINISTRADOR.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ADMINISTRADOR;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del administrador.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ADMINISTRADOR WHERE ADM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @SuperiorId INT (opcional).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Actualizar
    @Id INT,
    @PersonaId INT,
    @SuperiorId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ADMINISTRADOR
    SET PER_ID = @PersonaId,
        ADM_SUPERIOR_ID = @SuperiorId
    WHERE ADM_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del administrador.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ADMINISTRADOR WHERE ADM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA.
-- Entradas: @RFC NVARCHAR(20), @Nombre NVARCHAR(100), @ApellidoPaterno NVARCHAR(100), 
--           @ApellidoMaterno NVARCHAR(100), @Email NVARCHAR(100), 
--           @Nacionalidad NVARCHAR(50), @EsPersona BIT, @EsTitular BIT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Insertar
    @RFC NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @ApellidoPaterno NVARCHAR(100),
    @ApellidoMaterno NVARCHAR(100),
    @Email NVARCHAR(100),
    @Nacionalidad NVARCHAR(50),
    @EsPersona BIT,
    @EsTitular BIT,
    @NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA (
        PER_RFC, PER_NOMBRE, PER_APATERNO, PER_AMATERNO, 
        PER_EMAIL, PER_NACIONALIDAD, PER_ESPERSONA, PER_TITULAR
    )
    VALUES (
        @RFC, @Nombre, @ApellidoPaterno, @ApellidoMaterno, 
        @Email, @Nacionalidad, @EsPersona, @EsTitular
    );

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA basado en su ID.
-- Entradas: @Id INT - ID de la persona.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA WHERE PER_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA basado en su ID.
-- Entradas: @Id INT, @RFC NVARCHAR(20), @Nombre NVARCHAR(100), 
--           @ApellidoPaterno NVARCHAR(100), @ApellidoMaterno NVARCHAR(100), 
--           @Email NVARCHAR(100), @Nacionalidad NVARCHAR(50), 
--           @EsPersona BIT, @EsTitular BIT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Actualizar
    @Id INT,
    @RFC NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @ApellidoPaterno NVARCHAR(100),
    @ApellidoMaterno NVARCHAR(100),
    @Email NVARCHAR(100),
    @Nacionalidad NVARCHAR(50),
    @EsPersona BIT,
    @EsTitular BIT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA
    SET PER_RFC = @RFC,
        PER_NOMBRE = @Nombre,
        PER_APATERNO = @ApellidoPaterno,
        PER_AMATERNO = @ApellidoMaterno,
        PER_EMAIL = @Email,
        PER_NACIONALIDAD = @Nacionalidad,
        PER_ESPERSONA = @EsPersona,
        PER_TITULAR = @EsTitular
    WHERE PER_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA basado en su ID.
-- Entradas: @Id INT - ID de la persona.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA WHERE PER_ID = @Id;
END;
GO
