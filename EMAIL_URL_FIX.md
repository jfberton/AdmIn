# ?? Solución: Problema con URLs en Emails

## ? Problema Detectado

Los emails generados contenían URLs incorrectas:
- **URL Generada:** `https://localhost:7189/confirm-password-reset?token=XXX`
- **URL Correcta:** `http://localhost:7189/confirm-password-reset?token=XXX`

Además, los tokens estaban expirando porque la configuración SMTP no estaba funcionando correctamente.

---

## ? Cambios Realizados

### 1. **Corregido `appsettings.json`**
```json
{
  "Frontend": {
    "BaseUrl": "http://localhost:7189"  // Cambio: https ? http
  }
}
```

**Razón:** En desarrollo local, el UI corre en HTTP (no HTTPS), así que las URLs deben usar `http://`.

---

### 2. **Agregado Logging Detallado a `SmtpEmailService`**

Se agregó logging extensivo para debugging:
- ? Log de configuración SMTP completa
- ? Log de destinatarios (original y redirigido)
- ? Log de operaciones de renderizado de templates
- ? Log de reemplazo de placeholders
- ? Log de errores con stack trace

**Beneficios:**
- Fácil identificación de problemas de configuración
- Visibilidad del flujo completo del envío
- Debugging más rápido

---

## ?? Cómo Probar

### **Opción 1: Desde la Página de Pruebas**

1. **Navega a:** `http://localhost:7189/admin/test-email`
2. **Configura SMTP** en User Secrets (ver sección siguiente)
3. **Haz clic en "Ver Configuración SMTP"** para verificar
4. **Ingresa tu email** (será redirigido a `atp.jfbertoncini@chaco.gov.ar`)
5. **Selecciona tipo:** "new_user" o "password_reset"
6. **Envía el email**
7. **Revisa los logs** en `/diagnostic/logs/view`

### **Opción 2: Crear un Usuario Nuevo**

1. **Ve a:** `/usuarios/new`
2. **Crea un usuario** con un email válido
3. **El sistema enviará automáticamente** el email de bienvenida
4. **Revisa los logs** para ver el detalle del envío

---

## ?? Configuración SMTP (User Secrets)

Para que funcione el envío de emails, configura SMTP en **User Secrets**:

### **Para Gmail:**

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "tu-email@gmail.com",
    "Pass": "tu-app-password",
    "EnableSsl": "true",
    "From": "tu-email@gmail.com"
  }
}
```

**Importante:** Para Gmail necesitas una **App Password**, no tu contraseña normal:
1. Ve a: https://myaccount.google.com/apppasswords
2. Genera una contraseña de aplicación
3. Usa esa contraseña en `Smtp:Pass`

### **Para SMTP Local (Testing):**

```json
{
  "Smtp": {
    "Host": "localhost",
    "Port": "25",
    "User": "",
    "Pass": "",
  "EnableSsl": "false",
    "From": "test@localhost"
  }
}
```

---

## ?? Ver Logs Detallados

Los logs ahora muestran información muy detallada:

```
[INFO] ========== EMAIL SERVICE: SendAsync START ==========
[INFO] Original TO: nuevo@usuario.com
[INFO] Subject: Configura tu contraseña
[INFO] Body Length: 1523
[WARNING] ?? MODO TEST: Redirigiendo email de 'nuevo@usuario.com' a 'atp.jfbertoncini@chaco.gov.ar'
[INFO] SMTP Configuration:
[INFO]   Host: smtp.gmail.com
[INFO]   Port: 587
[INFO]   User: tu@gmail.com
[INFO]   EnableSsl: True
[INFO]   From: tu@gmail.com
[INFO] ? SMTP Credentials configured
[INFO] Attempting to send email to: atp.jfbertoncini@chaco.gov.ar
[INFO] ? Email sent successfully
[INFO] ========== EMAIL SERVICE: SendAsync END ==========
```

**Para ver los logs:**
- Opción 1: Ve a `http://localhost:7189/diagnostic/logs/view`
- Opción 2: Revisa la consola del API

---

## ?? Debugging: Verificar URLs en Templates

Los templates de email ahora loggean cada placeholder reemplazado:

```
[INFO] ========== EMAIL SERVICE: RenderTemplate START ==========
[INFO] Template File: NewUser_SetPassword.html
[INFO] Model Keys: Name, Link, ExpiryHours
[INFO] Template Path: D:\...\AdmIn.API\EmailTemplates\NewUser_SetPassword.html
[INFO] Template loaded, length: 2451
[INFO] Replacing placeholders:
[INFO]   ? Replaced {{ Name }} with: ***
[INFO]   ? Replaced {{ Link }} with: http://localhost:7189/confirm-password-reset?token=ABC123
[INFO]   ? Replaced {{ ExpiryHours }} with: ***
[INFO] ? Template rendered successfully, final length: 2398
[INFO] ========== EMAIL SERVICE: RenderTemplate END ==========
```

**Verificar que:**
- ? El `Link` empiece con `http://` (no `https://`)
- ? El puerto sea correcto (`7189`)
- ? El token no esté vacío

---

## ?? Solución de Problemas Comunes

### **Problema 1: Token Inválido**
**Síntoma:** La página dice "Token Inválido" o "Token expirado"

**Causas posibles:**
1. El token ya fue usado (`IsConsumed = 1`)
2. El token expiró (más de 24 horas)
3. El token no existe en la base de datos

**Solución:**
```sql
-- Ver tokens activos
SELECT * FROM PasswordResetTokens 
WHERE IsConsumed = 0 
AND ExpiresAt > GETDATE()
ORDER BY CreatedAt DESC;

-- Generar un nuevo token (crear usuario o usar /api/Usuario/request_reset)
```

### **Problema 2: Email No Llega**
**Síntoma:** El email no llega al destinatario

**Verificar:**
1. **Logs del API:** ¿Dice "Email sent successfully"?
2. **Configuración SMTP:** ¿Está bien configurada?
3. **Redirección TEST:** El email se envía a `atp.jfbertoncini@chaco.gov.ar`

**En `/diagnostic/logs/view` buscar:**
```
?? MODO TEST: Redirigiendo email de 'nuevo@usuario.com' a 'atp.jfbertoncini@chaco.gov.ar'
```

### **Problema 3: URL Incorrecta en Email**
**Síntoma:** El link del email no funciona

**Verificar:**
1. **appsettings.json:**
   ```json
   "Frontend": {
     "BaseUrl": "http://localhost:7189"  // ? Debe ser HTTP, no HTTPS
   }
   ```

2. **En los logs, buscar:**
   ```
   ? Replaced {{ Link }} with: http://localhost:7189/confirm-password-reset?token=XXX
   ```

3. **Si el Link muestra `https://`:**
   - Reinicia el API después de cambiar `appsettings.json`
   - Verifica que no haya otro `appsettings.Development.json` sobreescribiendo

---

## ?? Nota Importante: Modo TEST

**ACTUALMENTE EL SISTEMA ESTÁ EN MODO TEST:**
```csharp
private const string TEST_EMAIL = "atp.jfbertoncini@chaco.gov.ar";
```

**Esto significa:**
- ? Todos los emails se envían a `atp.jfbertoncini@chaco.gov.ar`
- ? No importa el email del destinatario original
- ? Perfecto para desarrollo/testing
- ?? **REMOVER EN PRODUCCIÓN**

**Para desactivar el modo TEST:**
1. Abre `AdmIn.API/Services/SmtpEmailService.cs`
2. Cambia:
   ```csharp
   // De:
   var recipient = TEST_EMAIL;
   
   // A:
   var recipient = to;
   ```
3. Elimina o comenta la constante `TEST_EMAIL`

---

## ?? Checklist de Verificación

Antes de crear un usuario y enviar email, verifica:

- [ ] `appsettings.json` tiene `"Frontend:BaseUrl": "http://localhost:7189"`
- [ ] User Secrets tiene configuración SMTP válida
- [ ] El API está corriendo
- [ ] El UI está corriendo en `http://localhost:7189`
- [ ] Puedes acceder a `/admin/test-email` (requiere rol `admin_sistema`)
- [ ] Los logs muestran "Email sent successfully"
- [ ] El link en el log muestra `http://` (no `https://`)

---

## ?? Siguiente Paso

**Prueba el flujo completo:**

1. **Crear un usuario nuevo:**
   ```
   Nombre: Usuario Test
   Email: test@example.com
   Password: (cualquiera)
   ```

2. **Revisar los logs:**
   - Ve a: `http://localhost:7189/diagnostic/logs/view`
   - Busca: `EMAIL SERVICE: SendAsync START`
   - Verifica el `Link` generado

3. **Verificar la base de datos:**
   ```sql
SELECT TOP 5 * 
   FROM PasswordResetTokens 
   ORDER BY CreatedAt DESC;
   ```

4. **Probar el link:**
   - Copia el link de los logs
   - Pégalo en el navegador
   - Debería cargar la página de confirmación

---

## ? Resultado Esperado

Cuando todo esté configurado correctamente:

1. **Email enviado:**
   ```
   [INFO] ? Email sent successfully
   ```

2. **Link correcto en logs:**
   ```
   [INFO] ? Replaced {{ Link }} with: http://localhost:7189/confirm-password-reset?token=awuP5KoPbMIkqztgvM1PwZkw-Ov4f7HzdYcDqnuNqY0
   ```

3. **Token en BD:**
   ```sql
   Token: awuP5KoPbMIkqztgvM1PwZkw-Ov4f7HzdYcDqnuNqY0
   IsConsumed: 0
   ExpiresAt: (24 horas en el futuro)
   ```

4. **Link funciona:**
   - Carga la página de confirmación
   - Muestra el nombre del usuario
   - Permite establecer nueva contraseña

---

¿Necesitas ayuda adicional con algún paso específico?
