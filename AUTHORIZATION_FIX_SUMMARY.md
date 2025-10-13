# Fix de Autorización para Trabajos de Proveedores

## Problema Identificado

El sistema permitía que cualquier usuario con rol de proveedor pudiera aceptar o rechazar trabajos, incluso si no estaban asignados a ese proveedor específico.

## Causa del Problema

La lógica de autorización estaba comparando directamente el `ProveedorId` (ID del proveedor) con el `UsuarioId` (ID del usuario), pero estos son valores diferentes. La relación correcta es:
- `Proveedor` tiene un campo `UsuarioId` que vincula al proveedor con una cuenta de usuario
- `TrabajoProveedor` tiene un campo `ProveedorId` que indica qué proveedor está asignado

## Solución Implementada

### 1. Backend - Business Layer (`AdmIn.Business\Servicios\Serv_TrabajoProveedor.cs`)

Se actualizaron los métodos que requieren autorización de proveedor:

#### `AceptarTrabajo()`
- Ahora obtiene el registro completo del `Proveedor` asignado al trabajo
- Verifica que `Proveedor.UsuarioId` coincida con el `request.UsuarioId`
- Mensaje de error descriptivo: "No está autorizado para aceptar este trabajo. Solo el proveedor asignado puede aceptarlo."

#### `RechazarTrabajo()`
- Implementa la misma validación que `AceptarTrabajo()`
- Verifica que el usuario que rechaza sea el asociado al proveedor asignado
- Mensaje de error: "No está autorizado para rechazar este trabajo. Solo el proveedor asignado puede rechazarlo."

#### `MarcarFinalizado()`
- Implementa la misma validación que `AceptarTrabajo()`
- Verifica que el usuario que marca finalizado sea el asociado al proveedor asignado
- Mensaje de error: "No está autorizado para marcar finalizado este trabajo. Solo el proveedor asignado puede hacerlo."

### 2. Frontend - Home Page (`AdmIn.UI\Components\Pages\Home.razor`)

Se agregó el método `PuedeAceptarTrabajo()` que verifica:
- El usuario actual es un proveedor
- El trabajo está asignado al proveedor del usuario (`t.ProveedorId == proveedorIdForUser`)
- El estado del trabajo permite aceptarlo (Creado o Solicitado)

Se actualizó la UI para usar este método en lugar de verificar condiciones inline.

### 3. Frontend - Mis Reparaciones Page (`AdmIn.UI\Components\Pages\AppPages\MisReparaciones.razor`)

Se actualizó la condición del botón "Aceptar" para incluir:
```csharp
t.ProveedorId == proveedorId.Value
```

Esto asegura que solo se muestre el botón cuando el trabajo esté asignado específicamente a ese proveedor.

### 4. Frontend - Trabajo Detalle Page (`AdmIn.UI\Components\Pages\AppPages\TrabajoDetalle.razor`)

Esta página ya tenía la lógica correcta:
```csharp
puedeAceptar = (trabajo.ProveedorId == compareId && (trabajo.Estado == TrabajoEstados.Creado || trabajo.Estado == TrabajoEstados.Solicitado));
```

Donde `compareId` es el `proveedorIdFromUser` obtenido consultando el proveedor por email.

## Flujo de Autorización Correcto

1. El usuario inicia sesión con su cuenta (obtiene un `UsuarioId` en los claims)
2. El sistema busca el registro de `Proveedor` asociado a ese usuario (por email o por `UsuarioId`)
3. Se obtiene el `ProveedorId` del proveedor encontrado
4. Al intentar una acción sobre un trabajo:
   - **Frontend**: Verifica que `trabajo.ProveedorId == proveedorIdDelUsuarioActual`
   - **Backend**: Obtiene el `Proveedor` con ID = `trabajo.ProveedorId` y verifica que `proveedor.UsuarioId == usuarioIdDelRequest`

## Archivos Modificados

1. `AdmIn.Business\Servicios\Serv_TrabajoProveedor.cs`
   - Métodos: `AceptarTrabajo()`, `RechazarTrabajo()`, `MarcarFinalizado()`

2. `AdmIn.UI\Components\Pages\Home.razor`
   - Agregado método `PuedeAceptarTrabajo()`
   - Actualizada lógica del botón "Aceptar"

3. `AdmIn.UI\Components\Pages\AppPages\MisReparaciones.razor`
   - Actualizada condición del botón "Aceptar" para incluir verificación de `ProveedorId`

## Pruebas Recomendadas

1. **Escenario 1**: Proveedor A intenta aceptar un trabajo asignado a Proveedor A
   - ? Debe permitir la acción

2. **Escenario 2**: Proveedor A intenta aceptar un trabajo asignado a Proveedor B
   - ? No debe mostrar el botón en la UI
   - ? Si se hace la petición directamente, el backend debe rechazarla

3. **Escenario 3**: Usuario sin rol de proveedor intenta acceder
   - ? El controlador ya tiene `[Authorize(Roles = "proveedor")]` que lo previene

4. **Escenario 4**: Verificar mensajes de error descriptivos
   - Cuando un proveedor no autorizado intenta una acción, debe recibir un mensaje claro

## Compilación

? El proyecto compila correctamente sin errores
