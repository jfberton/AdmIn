# Refactorización: Sistema de Reseteo de Contraseñas

## Resumen de Cambios

Se ha refactorizado el sistema de reseteo de contraseñas dividiéndolo en dos páginas independientes para mejorar la experiencia del usuario y solucionar problemas de renderización.

## Páginas Creadas

### 1. RequestPasswordReset.razor (`/request-password-reset`)
**Propósito**: Solicitar un enlace de reseteo de contraseña

**Características**:
- Validación de email en tiempo real con feedback visual (Bootstrap classes `is-valid`/`is-invalid`)
- Verificación de formato de email con expresión regular
- Spinner en el botón mientras se envía la solicitud
- Mensajes de éxito/error con clases Bootstrap alert
- Redirección automática al login después de enviar la solicitud exitosamente
- Layout: `LoginLayout` (sin menús ni header principal)
- Acceso: `[AllowAnonymous]`

**Flujo**:
1. Usuario ingresa su email
2. Se valida el formato del email
3. Si es válido, se habilita el botón "Solicitar enlace"
4. Al enviar, se llama a `/api/usuario/request_reset`
5. Si el email existe, se envía un correo con el link
6. Muestra mensaje de éxito
7. Espera 2.5 segundos y redirige a login

### 2. ConfirmPasswordReset.razor (`/confirm-password-reset`)
**Propósito**: Confirmar y establecer nueva contraseña usando el token del email

**Características**:
- Validación del token en `OnInitializedAsync()`
- Tres estados de UI:
  - **Loading**: Spinner mientras valida el token
  - **Token Inválido**: Mensaje de error con opciones para ir al login o solicitar nuevo link
  - **Token Válido**: Formulario de cambio de contraseña
- Validación de coincidencia de contraseñas en tiempo real
- Campos de usuario y email deshabilitados (solo lectura)
- Feedback visual en el campo de confirmación de contraseña
- Spinner en botón mientras se actualiza
- Layout: `LoginLayout` (sin menús ni header principal)
- Acceso: `[AllowAnonymous]`

**Flujo**:
1. Usuario llega desde el link del email (`/confirm-password-reset?token=ABC123`)
2. La página valida el token con `/api/usuario/token_info/{token}`
3. Si el token es válido:
   - Muestra datos del usuario (nombre y email)
   - Permite ingresar nueva contraseña y confirmarla
   - Valida que coincidan
   - Al enviar, llama a `/api/usuario/reset_by_token`
   - Muestra mensaje de éxito
   - Espera 2 segundos y redirige al login
4. Si el token es inválido/expirado:
   - Muestra mensaje de error
- Ofrece botones para ir al login o solicitar nuevo link

## Archivos Modificados

### AdmIn.UI\Components\Pages\Login.razor
- Actualizado enlace "¿Olvidaste tu contraseña?" para apuntar a `/request-password-reset`

### AdmIn.UI\Components\Layout\MainLayout.razor
- **Actualizada lógica de autorización** en la sección `<NotAuthorized>` para reconocer las nuevas URLs como páginas públicas
- Ahora permite acceso sin autenticación a:
  - `/request-password-reset` - Solicitar reseteo
  - `/confirm-password-reset?token=XXX` - Confirmar y cambiar contraseña
- **Código actualizado** (líneas ~132-145):
```csharp
var isPublicReset = path.StartsWith("request-password-reset", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("confirm-password-reset", StringComparison.OrdinalIgnoreCase);
```

### AdmIn.Business\Servicios\Serv_Usuario.cs
- Actualizado método `Crear()` para usar `/confirm-password-reset?token=` en el link del email
- Actualizado método `GenerateAndSendPasswordResetEmail()` para usar `/confirm-password-reset?token=` en el link del email

## Archivos Eliminados

### AdmIn.UI\Pages\ResetPassword.razor
- Página antigua que mezclaba ambas funcionalidades
- Tenía problemas de renderización (FOUC - Flash of Unstyled Content)
- Reemplazada por las dos páginas nuevas

## URLs del Sistema

### Páginas Públicas (Sin Autenticación)
- `/` - Login
- `/request-password-reset` - Solicitar reseteo de contraseña
- `/confirm-password-reset?token=XXX` - Confirmar y cambiar contraseña

### API Endpoints Utilizados
- `POST /api/usuario/request_reset` - Generar token y enviar email
- `POST /api/usuario/reset_by_token` - Actualizar contraseña con token
- `GET /api/usuario/token_info/{token}` - Validar token y obtener info del usuario

## Flujo Completo del Usuario

### Escenario 1: Usuario Olvidó su Contraseña
1. En login, click en "¿Olvidaste tu contraseña?"
2. Ingresa su email en `/request-password-reset`
3. Recibe email con link al token
4. Click en el link del email
5. Llega a `/confirm-password-reset?token=XXX`
6. Ingresa y confirma nueva contraseña
7. Sistema actualiza la contraseña
8. Redirige al login para iniciar sesión

### Escenario 2: Nuevo Usuario Creado por Admin
1. Admin crea usuario desde el panel
2. Sistema genera token automáticamente
3. Nuevo usuario recibe email con link
4. Click en el link del email (template: `NewUser_SetPassword.html`)
5. Llega a `/confirm-password-reset?token=XXX`
6. Establece su contraseña inicial
7. Redirige al login para iniciar sesión por primera vez

## Ventajas de la Nueva Implementación

### Separación de Responsabilidades
- Cada página tiene un propósito claro y único
- Más fácil de mantener y depurar

### Mejor Experiencia de Usuario
- Sin flash de contenido no inicializado
- Estados claros y feedback visual inmediato
- Mensajes de error más descriptivos

### Código Más Limpio
- Sin lógica condicional compleja para mostrar diferentes formularios
- Cada página es autónoma e independiente
- Mejor legibilidad del código

### Manejo de Errores Robusto
- Validación de token antes de mostrar formulario
- Mensajes de error específicos para cada caso
- Opciones claras de recuperación ante errores

## Notas de Implementación

### Bootstrap Classes Utilizadas
- `form-control`, `form-label`, `form-text` - Estilos de formulario
- `is-valid`, `is-invalid` - Feedback visual de validación
- `invalid-feedback`, `valid-feedback` - Mensajes de validación
- `alert`, `alert-success`, `alert-danger`, `alert-info` - Mensajes de estado
- `btn`, `btn-primary`, `btn-outline-secondary` - Botones
- `spinner-border`, `spinner-border-sm` - Spinners de carga
- `d-flex`, `justify-content-end` - Layout flexbox
- `card`, `p-4`, `mb-3`, `mt-3` - Spacing y contenedores

### Seguridad
- Ambas páginas usan `[AllowAnonymous]` ya que son de acceso público
- El token se valida en el servidor antes de permitir cambio de contraseña
- Los tokens expiran después de tiempo configurado (24 horas por defecto)
- Los tokens se marcan como consumidos después de usarse

## Testing Recomendado

1. **Solicitar reseteo con email válido**: Verificar que se envía el email
2. **Solicitar reseteo con email inválido**: Verificar mensaje de error
3. **Usar link con token válido**: Verificar que muestra formulario
4. **Usar link con token expirado**: Verificar mensaje de error apropiado
5. **Usar link con token ya consumido**: Verificar que no permite reuso
6. **Cambiar contraseña exitosamente**: Verificar que se actualiza y redirige
7. **Intentar contraseñas que no coinciden**: Verificar validación
8. **Cancelar en cualquier paso**: Verificar redirección al login

## Configuración Requerida

### appsettings.json
```json
{
  "Frontend": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

Asegurarse de que `Frontend:BaseUrl` esté configurado correctamente para cada entorno (dev/prod).
