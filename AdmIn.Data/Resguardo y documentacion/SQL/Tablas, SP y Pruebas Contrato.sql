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
-- Scripts de prueba de CONTRATO
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @ContratoId INT;
DECLARE @InquilinoId INT = 1; -- Ajustar con un valor válido de prueba
DECLARE @InmuebleId INT = 1; -- Ajustar con un valor válido de prueba
DECLARE @AdministradorId INT = 1; -- Ajustar con un valor válido de prueba
DECLARE @EstadoId INT = 1; -- Ajustar con un valor válido de prueba
DECLARE @FechaInicio DATE = '2024-01-01';
DECLARE @FechaFin DATE = '2025-01-01';
DECLARE @FechaCancelacion DATE = NULL;
DECLARE @MontoMensual DECIMAL(18,2) = 10000.00;
DECLARE @MontoDeposito DECIMAL(18,2) = 20000.00;
DECLARE @RentasIniciales INT = 2;
DECLARE @PorcentajeComision DECIMAL(5,4) = 0.05;
DECLARE @FechaFirma DATE = GETDATE();
DECLARE @Comentarios TEXT = 'Contrato inicial de prueba';
DECLARE @NuevosComentarios TEXT = 'Contrato actualizado con modificaciones.';

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Contrato_ObtenerTodos;

-- *** Prueba: Insertar un nuevo contrato ***
PRINT '*** Prueba: Insertar un nuevo contrato ***';
EXEC sp_Contrato_Insertar
    @INQ_ID = @InquilinoId,
    @INM_ID = @InmuebleId,
    @ADM_ID = @AdministradorId,
    @CON_FECHA_INICIO = @FechaInicio,
    @CON_FECHA_FIN = @FechaFin,
    @CON_FECHA_CANCELACION = @FechaCancelacion,
    @CON_MONTO_MENSUAL = @MontoMensual,
    @CON_MONTO_DEPOSITO = @MontoDeposito,
    @CON_RENTAS_INICIALES = @RentasIniciales,
    @CON_PORCENTAJE_COMISION = @PorcentajeComision,
    @CON_FECHA_FIRMA = @FechaFirma,
    @CON_ESTADO_ID = @EstadoId,
    @CON_COMENTARIOS = @Comentarios,
    @NuevoId = @ContratoId OUTPUT;

PRINT 'ID del nuevo contrato insertado: ' + CAST(@ContratoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Contrato_ObtenerPorId @CON_ID = @ContratoId;

-- *** Prueba: Actualizar el contrato ***
PRINT '*** Prueba: Actualizar el contrato ***';
EXEC sp_Contrato_Actualizar
    @CON_ID = @ContratoId,
    @INQ_ID = @InquilinoId,
    @INM_ID = @InmuebleId,
    @ADM_ID = @AdministradorId,
    @CON_FECHA_INICIO = @FechaInicio,
    @CON_FECHA_FIN = @FechaFin,
    @CON_FECHA_CANCELACION = @FechaCancelacion,
    @CON_MONTO_MENSUAL = @MontoMensual,
    @CON_MONTO_DEPOSITO = @MontoDeposito,
    @CON_RENTAS_INICIALES = @RentasIniciales,
    @CON_PORCENTAJE_COMISION = @PorcentajeComision,
    @CON_FECHA_FIRMA = @FechaFirma,
    @CON_ESTADO_ID = @EstadoId,
    @CON_COMENTARIOS = @NuevosComentarios;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Contrato_ObtenerPorId @CON_ID = @ContratoId;

-- *** Prueba: Eliminar el contrato ***
PRINT '*** Prueba: Eliminar el contrato ***';
EXEC sp_Contrato_Eliminar @CON_ID = @ContratoId;

-- *** Validación: Intentar obtener el contrato eliminado ***
PRINT '*** Validación: Intentar obtener el contrato eliminado ***';
EXEC sp_Contrato_ObtenerPorId @CON_ID = @ContratoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Contrato_ObtenerTodos;

