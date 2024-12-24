-- Tabla Agenda de Aliados
CREATE TABLE ALIADO_AGENDA (
    ALI_AGE_ID INT IDENTITY(1,1) PRIMARY KEY,
    ALI_ID INT NOT NULL FOREIGN KEY REFERENCES ALIADO(ALI_ID),
    ALI_AGE_FECHA DATE NOT NULL,
    ALI_AGE_HORA_INICIO TIME NOT NULL,
    ALI_AGE_HORA_FIN TIME NOT NULL,
    ALI_AGE_DISPONIBLE BIT NOT NULL
);


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
-- Script de Pruebas para ALIADO_AGENDA
-- ====================================================================================

-- =============================================
-- PRIMERA PARTE: CREACIÓN DE REGISTROS BASE
-- =============================================
PRINT '*** Creación de registros base para ALIADO ***';
DECLARE @PersonaId INT, @EspecialidadId INT, @AdminId INT, @AliadoId INT;

-- Insertar registros en PERSONA
PRINT 'Creación de PERSONA';
EXEC sp_Persona_Insertar 
    @RFC = 'RFC111111',
    @Nombre = 'Carlos',
    @ApellidoPaterno = 'Gomez',
    @ApellidoMaterno = 'Martinez',
    @Email = 'carlos.gomez@example.com',
    @Nacionalidad = 'Chile',
    @EsPersona = 1,
    @EsTitular = 0,
    @NuevoId = @PersonaId OUTPUT;
PRINT 'ID de Persona: ' + CAST(@PersonaId AS NVARCHAR);

-- Insertar registros en ALIADO_ESPECIALIDAD
PRINT 'Creación de ALIADO_ESPECIALIDAD';
EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Consultor TI',
    @NuevoId = @EspecialidadId OUTPUT;
PRINT 'ID de Especialidad: ' + CAST(@EspecialidadId AS NVARCHAR);

-- Insertar registros en ADMINISTRADOR
PRINT 'Creación de ADMINISTRADOR';
EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId,
    @NuevoId = @AdminId OUTPUT;
PRINT 'ID de Administrador: ' + CAST(@AdminId AS NVARCHAR);

-- Insertar registros en ALIADO
PRINT 'Creación de ALIADO';
EXEC sp_Aliado_Insertar 
    @AdminId = @AdminId,
    @PersonaId = @PersonaId,
    @EspecialidadId = @EspecialidadId,
    @NuevoId = @AliadoId OUTPUT;
PRINT 'ID de Aliado: ' + CAST(@AliadoId AS NVARCHAR);

-- =============================================
-- SEGUNDA PARTE: PRUEBA DE INSERCIÓN EN ALIADO_AGENDA
-- =============================================
PRINT '*** Creación de registros en la tabla ALIADO_AGENDA ***';
DECLARE @AgendaId1 INT, @AgendaId2 INT;

-- Insertar registros en ALIADO_AGENDA
EXEC sp_Aliado_Agenda_Insertar 
    @AliadoId = @AliadoId,
    @Fecha = '2024-12-20',
    @HoraInicio = '09:00',
    @HoraFin = '11:00',
    @Disponible = 1,
    @NuevoId = @AgendaId1 OUTPUT;
PRINT 'ID de Agenda 1: ' + CAST(@AgendaId1 AS NVARCHAR);

EXEC sp_Aliado_Agenda_Insertar 
    @AliadoId = @AliadoId,
    @Fecha = '2024-12-21',
    @HoraInicio = '14:00',
    @HoraFin = '16:00',
    @Disponible = 0,
    @NuevoId = @AgendaId2 OUTPUT;
PRINT 'ID de Agenda 2: ' + CAST(@AgendaId2 AS NVARCHAR);

-- =============================================
-- VALIDACIÓN INICIAL DE REGISTROS
-- =============================================
PRINT '*** Validación inicial: Mostrar todos los registros existentes en ALIADO_AGENDA ***';
EXEC sp_Aliado_Agenda_ObtenerTodos;

-- =============================================
-- PRUEBA DE OBTENCIÓN POR ID
-- =============================================
PRINT '*** Prueba: Obtener registro de ALIADO_AGENDA por ID ***';
EXEC sp_Aliado_Agenda_ObtenerPorId @Id = @AgendaId1;

-- =============================================
-- PRUEBA DE ACTUALIZACIÓN
-- =============================================
PRINT '*** Prueba: Actualizar un registro en ALIADO_AGENDA ***';
DECLARE @ActualizarId INT = @AgendaId2;

EXEC sp_Aliado_Agenda_Actualizar 
    @Id = @ActualizarId,
    @AliadoId = @AliadoId,
    @Fecha = '2024-12-22',
    @HoraInicio = '10:00',
    @HoraFin = '12:00',
    @Disponible = 1;
PRINT 'Registro actualizado con ID: ' + CAST(@ActualizarId AS NVARCHAR);

-- Validar actualización
PRINT '*** Validación después de la actualización ***';
EXEC sp_Aliado_Agenda_ObtenerPorId @Id = @ActualizarId;

-- =============================================
-- PRUEBA DE ELIMINACIÓN
-- =============================================
PRINT '*** Prueba: Eliminar un registro en ALIADO_AGENDA ***';
DECLARE @EliminarId INT = @AgendaId1;

EXEC sp_Aliado_Agenda_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado con ID: ' + CAST(@EliminarId AS NVARCHAR);

-- Validar eliminación
PRINT '*** Validación después de la eliminación ***';
EXEC sp_Aliado_Agenda_ObtenerTodos;

-- =============================================
-- VALIDACIÓN FINAL
-- =============================================
PRINT '*** Validación final: Mostrar todos los registros existentes en ALIADO_AGENDA ***';
EXEC sp_Aliado_Agenda_ObtenerTodos;
