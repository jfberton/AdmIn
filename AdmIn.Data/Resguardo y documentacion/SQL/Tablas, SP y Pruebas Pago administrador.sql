-- Tabla Pagos Administrador
CREATE TABLE PAGO_ADMINISTRADOR (
    PAGO_ADM_ID INT IDENTITY(1,1) PRIMARY KEY,
    CON_ID INT NOT NULL FOREIGN KEY REFERENCES CONTRATO(CON_ID),
    PAGO_ADM_MONTO DECIMAL(18,2) NOT NULL,
    PAGO_ADM_FECHA DATE NOT NULL,
    PAGO_ADM_ESTADO_ID INT NOT NULL FOREIGN KEY REFERENCES PAGO_ESTADO(PAGO_EST_ID),
    PAGO_ADM_COMENTARIOS TEXT
);


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
-- Scripts de prueba de PAGO_ADMINISTRADOR
-- ====================================================================================

-- Prueba: Insertar un registro en PAGO_ADMINISTRADOR
DECLARE @NuevoId INT;
EXEC sp_Pago_Administrador_Insertar
    @CON_ID = 1, -- ID del contrato
    @PAGO_ADM_MONTO = 10000.50, -- Monto del pago
    @PAGO_ADM_FECHA = '2024-12-18', -- Fecha del pago
    @PAGO_ADM_ESTADO_ID = 1, -- Estado inicial del pago
    @PAGO_ADM_COMENTARIOS = 'Primer pago registrado', -- Comentario opcional
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de PAGO_ADMINISTRADOR
EXEC sp_Pago_Administrador_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Pago_Administrador_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en PAGO_ADMINISTRADOR
EXEC sp_Pago_Administrador_Actualizar
    @PAGO_ADM_ID = @NuevoId, -- ID del registro a actualizar
    @CON_ID = 1, -- Nuevo ID del contrato
    @PAGO_ADM_MONTO = 12000.75, -- Nuevo monto del pago
    @PAGO_ADM_FECHA = '2024-12-19', -- Nueva fecha del pago
    @PAGO_ADM_ESTADO_ID = 2, -- Nuevo estado del pago
    @PAGO_ADM_COMENTARIOS = 'Pago actualizado con éxito'; -- Nuevo comentario opcional

-- Prueba: Eliminar un registro de PAGO_ADMINISTRADOR
EXEC sp_Pago_Administrador_Eliminar
    @PAGO_ADM_ID = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Pago_Administrador_ObtenerTodos;


