Se agregó migración SQL para eliminar la columna InquilinoBloqueado y se cambió la lógica de la aplicación para usar únicamente el campo Estado con los siguientes valores:

- "Asignado Propietario"
- "Asignado Inquilino"
- "Reasignado Propietario"
- "Reasignado Inquilino" (estado terminal)

Ejecutar el script 20251006_RemoveInquilinoBloqueado.sql contra la base de datos de producción/desarrollo para sincronizar el esquema.

Nota: asegúrate de respaldar la base de datos antes de aplicar cambios estructurales.
