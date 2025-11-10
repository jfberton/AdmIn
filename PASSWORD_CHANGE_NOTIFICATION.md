# ?? Sistema de Notificación de Cambio de Contraseña

## ? Implementación Completada

Se ha implementado un sistema completo de notificación por email cuando se actualiza una contraseña.

---

## ?? **Template de Email**

**Archivo:** `AdmIn.API/EmailTemplates/PasswordChanged_Notification.html`

### **Características:**
- ? Diseño moderno y profesional
- ? Responsive (se adapta a móviles)
- ? Información del cambio (fecha, hora, dispositivo)
- ? Alerta de seguridad si no fue el usuario
- ? Botón de acción para asegurar la cuenta
- ? Consejos de seguridad integrados
- ? Branding de m3tria

### **Placeholders Utilizados:**
```
{{Name}}    - Nombre del usuario
{{Timestamp}}  - Fecha y hora del cambio
{{Device}}        - Dispositivo usado (ej: "Navegador Web")
{{Location}}   - Ubicación aproximada
{{SecurityLink}}  - Link para solicitar reseteo si no fue el usuario
{{Year}}          - Año actual para el footer
```

---

## ?? **Flujos de Envío**

### **1. Cambio de Contraseña con Token (Reset)**

**Método:** `Serv_Usuario.ResetPasswordByToken`

**Cuándo se envía:**
- Después de que el usuario completa exitosamente el formulario `/confirm-password-reset`
- Después de actualizar la contraseña en la base de datos
- Después de marcar el token como consumido

**Contenido del Email:**
```
Asunto: Tu contraseña ha sido actualizada

Información:
- Nombre del usuario
- Timestamp del cambio
- Link para asegurar cuenta si no fue el usuario
- Consejos de seguridad
```

**Características:**
- ? No falla el proceso si el email falla (try-catch)
- ? Se loggea el error si ocurre
- ? El usuario recibe confirmación aunque el email falle

---

### **2. Cambio de Contraseña desde Perfil**

**Método:** `Serv_Usuario.Modificar_contraseña`

**Cuándo se envía:**
- Después de que el usuario cambia su contraseña desde su perfil
- Después de validar la contraseña actual
- Después de actualizar la contraseña en la base de datos

**Contenido del Email:**
- Igual que el caso anterior (mismo template)

**Características:**
- ? No falla el proceso si el email falla
- ? Se loggea el error si ocurre
- ? El usuario recibe confirmación en la UI aunque el email falle

---

## ?? **Preview del Email**

El email tiene el siguiente diseño:

```
???????????????????????????????????????????
?  ?? [Header con gradiente morado]       ?
?     Contraseña Actualizada   ?
???????????????????????????????????????????
?                 ?
?  Hola José,            ?
?    ?
?  Te confirmamos que tu contraseña ha   ?
?  sido actualizada exitosamente.  ?
?      ?
?  ?????????????????????????????????????? ?
?  ? ?? Fecha: 10/11/2025 14:30:00     ? ?
?  ? ?? Dispositivo: Navegador Web     ? ?
?  ? ?? Ubicación: No disponible       ? ?
?  ?????????????????????????????????????? ?
?       ?
?  ?? ¿No fuiste tú?     ?
?  Si no realizaste este cambio...  ?
?             ?
?     [?? Asegurar mi cuenta ahora]        ?
?       ?
?  ????????????????????????????????????
?          ?
?  ?? Consejos de Seguridad                ?
?• Usa contraseñas únicas       ?
?  • Activa autenticación en dos pasos     ?
?  • Revisa tu actividad regularmente      ?
?• No compartas tu contraseña            ?
?   ?
?  Saludos,           ?
?  El equipo de m3tria     ?
?       ?
???????????????????????????????????????????
?  [Footer con información de contacto]   ?
???????????????????????????????????????????
```

---

## ?? **Configuración Necesaria**

El sistema utiliza la configuración SMTP existente en `appsettings.json` o **User Secrets**:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "tu-email@gmail.com",
    "Pass": "tu-app-password",
    "EnableSsl": "true",
    "From": "tu-email@gmail.com"
  },
  "Frontend": {
    "BaseUrl": "https://localhost:7189"
  }
}
```

---

## ?? **Ejemplo de Uso**

### **Caso 1: Usuario resetea contraseña con token**

1. Usuario solicita reset de contraseña
2. Recibe email con link de reset
3. Hace clic en el link
4. Ingresa nueva contraseña
5. ? Sistema actualiza contraseña
6. ? Sistema marca token como consumido
7. ? **Sistema envía email de notificación**
8. Usuario recibe confirmación por email

### **Caso 2: Usuario cambia contraseña desde perfil**

1. Usuario va a su perfil
2. Hace clic en "Cambiar contraseña"
3. Ingresa contraseña actual y nueva
4. ? Sistema valida contraseña actual
5. ? Sistema actualiza contraseña
6. ? **Sistema envía email de notificación**
7. Usuario recibe confirmación por email

---

## ??? **Seguridad**

### **Buenas Prácticas Implementadas:**

1. **? Notificación Obligatoria:**
   - Todo cambio de contraseña genera un email
   - El usuario siempre es notificado

2. **? Detección de Acceso No Autorizado:**
   - El usuario puede identificar cambios sospechosos
   - Link directo para asegurar cuenta

3. **? No Bloquea el Proceso:**
   - Si el email falla, el cambio se completa igual
   - Se loggea el error para debugging

4. **? Información Contextual:**
   - Timestamp exacto del cambio
   - Dispositivo y ubicación (cuando esté disponible)

5. **? Educación del Usuario:**
   - Consejos de seguridad integrados
   - Mejores prácticas de passwords

---

## ?? **Testing**

### **Probar Flujo de Reset con Token:**

1. **Solicitar reset:**
```
POST /api/Usuario/request_reset
{
  "email": "usuario@example.com"
}
```

2. **Revisar email recibido** (contiene token)

3. **Usar el link del email** para ir a `/confirm-password-reset?token=XXX`

4. **Completar formulario** con nueva contraseña

5. **Verificar:**
   - ? Contraseña actualizada en BD
   - ? Token marcado como consumido
   - ? **Email de notificación enviado**

### **Probar Flujo de Cambio desde Perfil:**

1. **Login** con usuario válido

2. **Ir a perfil** ? Cambiar contraseña

3. **Ingresar:**
   - Contraseña actual
   - Nueva contraseña
   - Confirmar nueva contraseña

4. **Verificar:**
   - ? Contraseña actualizada
   - ? **Email de notificación enviado**

---

## ?? **Logs de Ejemplo**

Cuando se envía el email correctamente:

```
[INFO] ========== EMAIL SERVICE: RenderTemplate START ==========
[INFO] Template File: PasswordChanged_Notification.html
[INFO] Model Keys: Name, Timestamp, Device, Location, SecurityLink, Year
[INFO] ? Replaced {{ Name }} with: José
[INFO] ? Replaced {{ Timestamp }} with: 10/11/2025 14:30:00
[INFO] ? Replaced {{ SecurityLink }} with: https://localhost:7189/request-password-reset
[INFO] ? Template rendered successfully
[INFO] ========== EMAIL SERVICE: SendAsync START ==========
[INFO] Original TO: jose@example.com
[INFO] Subject: Tu contraseña ha sido actualizada
[INFO] ?? MODO TEST: Redirigiendo email de 'jose@example.com' a 'atp.jfbertoncini@chaco.gov.ar'
[INFO] ? Email sent successfully
```

Cuando falla el email (no afecta el proceso):

```
[BUSINESS] ?? Error enviando email de notificación: SMTP connection failed
[INFO] Contraseña actualizada correctamente (email notification failed but change succeeded)
```

---

## ?? **Mejoras Futuras** (Opcional)

### **1. Información Avanzada del Dispositivo:**

Agregar a `HttpContext` para obtener:
- User-Agent real
- IP del cliente
- Geolocalización aproximada

```csharp
var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
```

### **2. Historial de Cambios:**

Crear tabla para registrar todos los cambios de contraseña:

```sql
CREATE TABLE PasswordChangeHistory (
    Id INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL,
    ChangedAt DATETIME2 NOT NULL,
    IpAddress NVARCHAR(45),
    UserAgent NVARCHAR(500),
    ChangeMethod NVARCHAR(50) -- 'Token', 'Profile', 'Admin'
);
```

### **3. Autenticación de Dos Factores (2FA):**

Implementar 2FA para mayor seguridad:
- Código por SMS
- Código por email
- App de autenticación (Google Authenticator, etc.)

### **4. Detección de Patrones Sospechosos:**

Algoritmo para detectar:
- Múltiples cambios en poco tiempo
- Accesos desde ubicaciones inusuales
- Cambios a horas extrañas

---

## ?? **Archivos Relacionados**

**Templates:**
- `AdmIn.API/EmailTemplates/PasswordChanged_Notification.html` - Notificación de cambio
- `AdmIn.API/EmailTemplates/PasswordReset_Request.html` - Solicitud de reset
- `AdmIn.API/EmailTemplates/NewUser_SetPassword.html` - Nuevo usuario

**Backend:**
- `AdmIn.Business/Servicios/Serv_Usuario.cs` - Lógica de negocio
- `AdmIn.API/Services/SmtpEmailService.cs` - Servicio de email
- `AdmIn.API/Controllers/UsuarioController.cs` - API endpoints

**Frontend:**
- `AdmIn.UI/Pages/ConfirmPasswordReset.razor` - Reset con token
- `AdmIn.UI/Pages/RequestPasswordReset.razor` - Solicitud de reset
- `AdmIn.UI/Components/Pages/AppPages/MiPerfil.razor` - Cambio desde perfil

---

## ? **Checklist de Implementación**

- [x] Template HTML creado
- [x] Integración en `ResetPasswordByToken`
- [x] Integración en `Modificar_contraseña`
- [x] Manejo de errores (no bloquea proceso)
- [x] Logging de errores
- [x] Compilación exitosa
- [x] Documentación completa

---

## ?? **Resultado Final**

Ahora el sistema envía automáticamente un email de notificación cada vez que se cambia una contraseña:

1. ? **Usuario resetea contraseña** ? Email enviado
2. ? **Usuario cambia desde perfil** ? Email enviado
3. ? **Proceso no se bloquea** si email falla
4. ? **Usuario siempre informado** de cambios en su cuenta
5. ? **Seguridad mejorada** con notificaciones proactivas

¡Sistema de notificaciones de seguridad completamente funcional! ??
