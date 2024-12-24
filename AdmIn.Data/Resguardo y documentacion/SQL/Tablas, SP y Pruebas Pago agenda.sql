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
-- Scripts de prueba de PAGO_AGENDA
-- ====================================================================================

-- Prueba: Insertar un registro en PAGO_AGENDA
DECLARE @NuevoId INT;
EXEC sp_Pago_Agenda_Insertar
    @CON_ID = 1, -- ID del contrato asociado
    @PAGO_CUOTA_NRO = 1, -- Número de cuota
    @PAGO_FECHA_VENCIMIENTO = '2024-12-31', -- Fecha de vencimiento
    @PAGO_MONTO = 15000.00, -- Monto del pago
    @PAGO_FECHA_NOTIFICACION = NULL, -- Fecha de notificación opcional
    @PAGO_FECHA_REALIZADO = NULL, -- Fecha de pago opcional
    @PAGO_ESTADO_ID = 1, -- Estado inicial del pago
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de PAGO_AGENDA
EXEC sp_Pago_Agenda_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Pago_Agenda_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en PAGO_AGENDA
EXEC sp_Pago_Agenda_Actualizar
    @PAGO_ID = @NuevoId, -- ID del registro a actualizar
    @CON_ID = 1, -- ID del contrato asociado
    @PAGO_CUOTA_NRO = 2, -- Nuevo número de cuota
    @PAGO_FECHA_VENCIMIENTO = '2025-01-31', -- Nueva fecha de vencimiento
    @PAGO_MONTO = 15500.00, -- Nuevo monto del pago
    @PAGO_FECHA_NOTIFICACION = '2024-12-20', -- Nueva fecha de notificación
    @PAGO_FECHA_REALIZADO = '2024-12-25', -- Nueva fecha de pago
    @PAGO_ESTADO_ID = 2; -- Nuevo estado del pago
	
-- Verificación modificacion: 
EXEC sp_Pago_Agenda_ObtenerPorId
    @Id = @NuevoId;

-- Prueba: Eliminar un registro de PAGO_AGENDA
EXEC sp_Pago_Agenda_Eliminar
    @PAGO_ID = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Pago_Agenda_ObtenerTodos;
