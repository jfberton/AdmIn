-- ======================================================
-- SCRIPT DE MIGRACIÓN: SEPARACIÓN DE ESTADO GEOGRÁFICO Y CONDICIÓN OPERATIVA
-- ======================================================

-- 1. Crear tabla InmuebleCondicion
CREATE TABLE InmuebleCondicion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(255) NULL,
    Color NVARCHAR(7) NOT NULL DEFAULT '#6b7280', -- Código de color hexadecimal
    Activo BIT NOT NULL DEFAULT 1,
    Orden INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioCreadorId INT NULL,
    UsuarioModificadorId INT NULL,
    
    -- Foreign Keys (si existen las tablas de Usuario)
    CONSTRAINT FK_InmuebleCondicion_UsuarioCreador 
        FOREIGN KEY (UsuarioCreadorId) REFERENCES Usuario(UsuarioID),
    CONSTRAINT FK_InmuebleCondicion_UsuarioModificador 
        FOREIGN KEY (UsuarioModificadorId) REFERENCES Usuario(UsuarioID)
);

-- 2. Insertar condiciones por defecto
INSERT INTO InmuebleCondicion (Nombre, Descripcion, Color, Orden) VALUES
('Disponible', 'El inmueble está disponible para alquiler/venta', '#10b981', 1),
('Ocupado', 'El inmueble está actualmente ocupado', '#374151', 2),
('Reservado', 'El inmueble está reservado pero no ocupado', '#3b82f6', 3),
('En reparación', 'El inmueble está en proceso de reparación', '#f59e0b', 4),
('En mantenimiento', 'El inmueble está en mantenimiento preventivo', '#6b7280', 5);

-- 3. Agregar nueva columna CondicionId a la tabla Inmueble
ALTER TABLE Inmueble 
ADD CondicionId INT NULL;

-- 4. Migrar datos existentes del campo Estado (string) a CondicionId (int)
-- Mapear los valores existentes en el campo Estado a los IDs de InmuebleCondicion
UPDATE Inmueble 
SET CondicionId = CASE 
    WHEN Estado = 'Disponible' THEN 1
    WHEN Estado = 'Ocupado' THEN 2
    WHEN Estado = 'Reservado' THEN 3
    WHEN Estado = 'En reparación' THEN 4
    WHEN Estado = 'En mantenimiento' THEN 5
    ELSE 1 -- Default: Disponible
END;

-- 5. Hacer obligatorio el campo CondicionId y agregar foreign key
ALTER TABLE Inmueble 
ALTER COLUMN CondicionId INT NOT NULL;

ALTER TABLE Inmueble 
ADD CONSTRAINT FK_Inmueble_InmuebleCondicion 
FOREIGN KEY (CondicionId) REFERENCES InmuebleCondicion(Id);

-- 6. Crear índices para mejorar rendimiento
CREATE INDEX IX_Inmueble_CondicionId ON Inmueble(CondicionId);
CREATE INDEX IX_InmuebleCondicion_Activo_Orden ON InmuebleCondicion(Activo, Orden);

-- ======================================================
-- VERIFICACIÓN DE MIGRACIÓN
-- ======================================================

-- Verificar que todos los inmuebles tienen una condición válida
SELECT 
    'Total inmuebles' as Categoria,
    COUNT(*) as Cantidad
FROM Inmueble
UNION ALL
SELECT 
    'Inmuebles con condición válida' as Categoria,
    COUNT(*) as Cantidad
FROM Inmueble i
INNER JOIN InmuebleCondicion ic ON i.CondicionId = ic.Id
UNION ALL
SELECT 
    'Condiciones disponibles' as Categoria,
    COUNT(*) as Cantidad
FROM InmuebleCondicion
WHERE Activo = 1;

-- Ver distribución de inmuebles por condición
SELECT 
    ic.Nombre as Condicion,
    ic.Descripcion,
    COUNT(i.InmuebleID) as CantidadInmuebles
FROM InmuebleCondicion ic
LEFT JOIN Inmueble i ON ic.Id = i.CondicionId
GROUP BY ic.Id, ic.Nombre, ic.Descripcion, ic.Orden
ORDER BY ic.Orden;

-- ======================================================
-- ROLLBACK PLAN (si es necesario)
-- ======================================================
/*
-- Para revertir la migración (USAR CON CUIDADO):

-- 1. Eliminar foreign key y columna CondicionId
ALTER TABLE Inmueble DROP CONSTRAINT FK_Inmueble_InmuebleCondicion;
ALTER TABLE Inmueble DROP COLUMN CondicionId;

-- 2. Eliminar tabla InmuebleCondicion
DROP TABLE InmuebleCondicion;

-- NOTA: El campo Estado original se mantiene intacto durante toda la migración
*/