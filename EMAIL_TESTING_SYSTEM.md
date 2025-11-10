# ?? Sistema de Prueba de Emails - AdmIn

## ? Implementación Completada

Se ha implementado un sistema completo de diagnóstico y prueba de envío de emails con logging detallado.

---

## ?? Componentes Implementados

### 1. **API - HealthCheckController** (Endpoints de Diagnóstico)

#### **POST /diagnostic/testmail** 
Envía un email de prueba con logging detallado.

**Requiere:** Rol `admin_sistema`

**Request Body:**
```json
{
  "to": "destinatario@example.com",
  "subject": "Asunto opcional",
  "body": "Cuerpo opcional",
  "testType": "simple"  // Opciones: "simple", "password_reset", "new_user"
}
```

**Response Exitosa:**
```json
{
  "success": true,
  "message": "? Email enviado correctamente a destinatario@example.com",
  "timestamp": "2024-01-15T10:30:00",
  "details": {
    "to": "destinatario@example.com",
    "subject": "Email de Prueba",
    "smtpConfig": {
      "host": "smtp.gmail.com",
      "port": "587",
  "ssl": "true",
      "user": "tu@gmail.com",
      "from": "tu@gmail.com"
    },
    "bodyLength": 1234,
    "testType": "simple"
  }
}
```

**Response con Error:**
```json
{
  "success": false,
  "error": "? Error SMTP: Authentication failed",
  "smtpStatusCode": "GeneralFailure",
  "innerException": "Detalles adicionales...",
  "timestamp": "2024-01-15T10:30:00",
  "suggestions": [
    "Problema de autenticación detectado",
    "Para Gmail:",
    "1. Ve a https://myaccount.google.com/apppasswords",
    "2. Genera una 'Contraseña de aplicación'",
    "..."
  ],
  "smtpConfig": {
    "host": "smtp.gmail.com",
    "port": "587",
    "ssl": "true",
    "user": "tu@gmail.com",
    "passwordConfigured": true,
    "from": "tu@gmail.com"
  }
}
```

#### **GET /diagnostic/smtp-config**
Obtiene la configuración SMTP actual y warnings.

**Requiere:** Rol `admin_sistema`

**Response:**
```json
{
  "success": true,
  "isConfigured": true,
  "timestamp": "2024-01-15T10:30:00",
  "config": {
    "host": "smtp.gmail.com",
 "port": "587",
    "user": "tu@gmail.com",
 "passwordConfigured": true,
    "enableSsl": "true",
    "from": "tu@gmail.com"
  },
  "warnings": [
    "?? Para Gmail, asegúrate de usar una 'App Password', no tu contraseña normal"
  ]
}
```

---

### 2. **UI - Página de Prueba de Emails**

**Ruta:** `/admin/test-email`

**Requiere:** Rol `admin_sistema`

**Características:**
- ? Interfaz intuitiva con Radzen Components
- ? Validación de campos en tiempo real
- ? 3 tipos de prueba de email:
- **Simple:** Email básico con contenido personalizado
  - **Password Reset:** Usa plantilla de reseteo de contraseña
  - **New User:** Usa plantilla de bienvenida
- ? Visualización de configuración SMTP
- ? Mostrar resultados detallados del envío
- ? Sugerencias específicas según el tipo de error
- ? Link directo a logs detallados
- ? Diseño responsive (funciona en móviles)

---

## ?? Características de Diagnóstico

### **Logging Detallado:**
Todos los pasos se registran en:
- ? Archivo de log centralizado (`IApiLoggerService`)
- ? Logger de ASP.NET Core (`ILogger`)
- ? Consola de salida

**Información registrada:**
```
[2024-01-15 10:30:00.123] [INFO] [HEALTH_CHECK] ========== TEST EMAIL START - 2024-01-15 10:30:00.123 ==========
[2024-01-15 10:30:00.125] [INFO] [HEALTH_CHECK] Destinatario: prueba@example.com
[2024-01-15 10:30:00.127] [INFO] [HEALTH_CHECK] SMTP Host: smtp.gmail.com
[2024-01-15 10:30:00.129] [INFO] [HEALTH_CHECK] SMTP Port: 587
[2024-01-15 10:30:00.131] [INFO] [HEALTH_CHECK] SMTP User: tu@gmail.com
[2024-01-15 10:30:00.133] [INFO] [HEALTH_CHECK] SMTP Pass: ***CONFIGURADO***
[2024-01-15 10:30:00.135] [INFO] [HEALTH_CHECK] Enable SSL: true
[2024-01-15 10:30:00.137] [INFO] [HEALTH_CHECK] HTML Body generado - Length: 1234
[2024-01-15 10:30:00.139] [INFO] [HEALTH_CHECK] Intentando enviar email...
[2024-01-15 10:30:02.456] [ERROR] [HEALTH_CHECK] ? SMTP Exception: Authentication failed
[2024-01-15 10:30:02.458] [ERROR] [HEALTH_CHECK] Status Code: GeneralFailure
[2024-01-15 10:30:02.460] [ERROR] [HEALTH_CHECK] Stack Trace: ...
[2024-01-15 10:30:02.462] [INFO] [HEALTH_CHECK] ========== TEST EMAIL END - 2024-01-15 10:30:02.462 ==========
```

### **Detección Inteligente de Errores:**

1. **Error de Autenticación:**
   - Detecta problemas de credenciales
   - Proporciona instrucciones para Gmail App Passwords
   
2. **Timeout:**
   - Detecta problemas de conexión
   - Sugiere verificar firewall y conectividad
   
3. **SSL/TLS:**
   - Detecta problemas de certificados
   - Sugiere verificar configuración SSL

---

## ?? Cómo Usar

### **Desde la UI (Recomendado):**

1. **Acceder a la página:**
   - Inicia sesión con un usuario que tenga rol `admin_sistema`
   - Ve al menú lateral y haz clic en **"Prueba de Email"**
   - O navega directamente a: `https://tu-dominio.com/admin/test-email`

2. **Ver configuración SMTP:**
   - Haz clic en "Ver Configuración SMTP"
   - Revisa que todos los campos estén configurados
   - Presta atención a los warnings

3. **Enviar email de prueba:**
   - Ingresa tu email en "Email Destinatario"
   - Selecciona el tipo de prueba:
  - **Simple:** Para probar envío básico
     - **Password Reset:** Para probar plantilla de reseteo
  - **New User:** Para probar plantilla de bienvenida
   - (Opcional) Personaliza asunto y cuerpo
   - Haz clic en "Enviar Email de Prueba"

4. **Revisar resultados:**
   - Si es exitoso: Verás confirmación con detalles de configuración
   - Si hay error: Verás error detallado con sugerencias
   - Haz clic en "Ver Logs Detallados" para más información

### **Desde Postman/cURL:**

```bash
# 1. Login para obtener token
POST https://tu-api.com/api/Auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "tu-password"
}

# 2. Probar email (usa el token en Authorization)
POST https://tu-api.com/diagnostic/testmail
Authorization: Bearer TU_TOKEN_JWT
Content-Type: application/json

{
  "to": "prueba@example.com",
  "testType": "simple"
}

# 3. Ver configuración SMTP
GET https://tu-api.com/diagnostic/smtp-config
Authorization: Bearer TU_TOKEN_JWT
```

---

## ?? Configuración Necesaria

Para que funcione el envío de emails, debes tener configurado en `appsettings.json` o **User Secrets**:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "tu-email@gmail.com",
    "Pass": "tu-contraseña-de-aplicacion",
    "EnableSsl": "true",
    "From": "tu-email@gmail.com"
  },
  "Frontend": {
    "BaseUrl": "https://tu-dominio.com"
  }
}
```

### **Para Gmail (Recomendado):**

1. Ve a https://myaccount.google.com/apppasswords
2. Genera una "Contraseña de aplicación"
3. Usa esa contraseña en `Smtp:Pass` (NO tu contraseña normal)
4. Configuración recomendada:
   ```json
   {
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": "587",
 "EnableSsl": "true"
     }
   }
   ```

---

## ?? Acceso al Menú

El enlace "Prueba de Email" aparece en el menú lateral solo para usuarios con rol `admin_sistema`.

**Ubicación en el menú:**
```
Mi Perfil
Usuarios
Características
Tipos de Servicio
Proveedores
Monedas
Inmuebles
Reparaciones
Prueba de Email  ? NUEVO
```

---

## ?? Ver Logs Detallados

**Opción 1: Desde la página de prueba**
- Haz clic en "Ver Logs Detallados" en la página de resultados

**Opción 2: Navegar directamente**
- Ve a: `https://tu-api.com/diagnostic/logs/view`
- Requiere autenticación con rol `admin_sistema`

**Características del visor de logs:**
- ? Auto-refresh cada 10 segundos
- ? Colores por nivel de log (ERROR=rojo, WARNING=amarillo, etc.)
- ? Logs más recientes primero
- ? Información del servidor (máquina, PID, directorio)

---

## ?? Tipos de Prueba de Email

### **1. Simple**
- Email básico de prueba
- Permite personalizar asunto y cuerpo
- Útil para verificar conectividad SMTP

### **2. Password Reset**
- Usa la plantilla: `PasswordReset_Request.html`
- Simula el email de reseteo de contraseña
- Incluye link de prueba
- Útil para verificar que las plantillas HTML funcionan

### **3. New User**
- Usa la plantilla: `NewUser_SetPassword.html`
- Simula el email de bienvenida para nuevo usuario
- Incluye link para configurar contraseña
- Útil para verificar el flujo de creación de usuarios

---

## ? Ventajas del Sistema

1. **Diagnóstico Completo:**
 - Verifica configuración antes de enviar
   - Detecta problemas específicos
   - Proporciona sugerencias de solución

2. **Interfaz Amigable:**
   - No requiere herramientas externas (Postman, cURL)
   - Todo desde el navegador
   - Diseño responsive

3. **Logging Detallado:**
   - Cada paso del proceso se registra
   - Fácil identificación de problemas
   - Stack traces completos para debugging

4. **Seguridad:**
   - Solo usuarios `admin_sistema` pueden acceder
   - Contraseñas nunca se muestran en logs
   - Autenticación JWT requerida

---

## ?? Solución de Problemas Comunes

### **Error: "SMTP Host no configurado"**
- **Causa:** No hay configuración SMTP en appsettings
- **Solución:** Agrega la configuración SMTP completa

### **Error: "Authentication failed"**
- **Causa:** Usuario o contraseña incorrectos
- **Solución Gmail:** Usa App Password, no tu contraseña normal
- **Verificar:** Usuario y password están configurados correctamente

### **Error: "Connection timed out"**
- **Causa:** Firewall bloqueando puerto o host incorrecto
- **Solución:** 
  - Verifica que el puerto 587 (SMTP) esté abierto
  - Verifica que el host sea correcto (`smtp.gmail.com` para Gmail)

### **Error: "SSL/TLS"**
- **Causa:** Configuración SSL incorrecta
- **Solución:** Para Gmail debe ser `"EnableSsl": "true"`

---

## ?? Archivos Relacionados

**API:**
- `AdmIn.API/Controllers/HealthCheckController.cs` - Endpoints de diagnóstico
- `AdmIn.API/Services/SmtpEmailService.cs` - Servicio de email
- `AdmIn.API/EmailTemplates/*.html` - Plantillas HTML

**UI:**
- `AdmIn.UI/Components/Pages/Admin/TestEmail.razor` - Página de prueba
- `AdmIn.UI/Components/Layout/MainLayout.razor` - Menú de navegación
- `AdmIn.UI/Services/UtilityServices/TokenService.cs` - Manejo de tokens

---

## ?? ¡Listo para Usar!

El sistema está completamente implementado y listo para probar. Solo necesitas:

1. ? Configurar las credenciales SMTP
2. ? Tener un usuario con rol `admin_sistema`
3. ? Navegar a `/admin/test-email`
4. ? Enviar un email de prueba

¡Cualquier error será detectado y mostrado con sugerencias de solución!
