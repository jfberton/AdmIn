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
-- Script de Pruebas para DIRECCION y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en DIRECCION:';
EXEC sp_Direccion_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC sp_Direccion_Insertar 
    @CalleNumero = 'Avenida Siempre Viva 742', 
    @Colonia = 'Springfield', 
    @Ciudad = 'Springfield', 
    @Estado = 'Illinois', 
    @CodigoPostal = '12345', 
    @Pais = 'USA', 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Direccion_Insertar 
    @CalleNumero = 'Calle 456', 
    @Colonia = 'Centro', 
    @Ciudad = 'Ciudad X', 
    @Estado = 'Estado Y', 
    @CodigoPostal = '67890', 
    @Pais = 'Pais Z', 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en DIRECCION después de inserciones:';
EXEC sp_Direccion_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Direccion_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 1; -- Ajustar según ID existente
EXEC sp_Direccion_Actualizar 
    @Id = @ActualizarId, 
    @CalleNumero = 'Calle Actualizada', 
    @Colonia = 'Colonia Modificada', 
    @Ciudad = 'Ciudad Modificada', 
    @Estado = 'Estado Modificado', 
    @CodigoPostal = '54321', 
    @Pais = 'Pais Modificado';
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Direccion_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 1; -- Ajustar según ID existente
EXEC sp_Direccion_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Direccion_ObtenerTodos;
GO
