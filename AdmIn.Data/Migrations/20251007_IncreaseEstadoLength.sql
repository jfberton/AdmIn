-- Migración: Aumentar tamaño de la columna Estado en TrabajoProveedor
-- Fecha: 2025-10-07
-- Objetivo: Evitar truncamiento de valores largos como 'Finalizado por aprobar' y futuras descripciones de estado.

ALTER TABLE TrabajoProveedor
ALTER COLUMN Estado NVARCHAR(200) NULL;

-- Nota: Ajusta NULL/NOT NULL según el esquema actual. Si la columna es NOT NULL, usar:
-- ALTER TABLE TrabajoProveedor ALTER COLUMN Estado NVARCHAR(200) NOT NULL;

-- Ejecutar en la base de datos correspondiente (desarrollo/producción) después de respaldar.
