# Sistema de Logging Centralizado - AdmIn API

## ?? Descripción

Se ha implementado un sistema de logging centralizado para que todos los controladores de la API escriban al mismo archivo de log y puedan ser visualizados desde el endpoint `/api/Diagnostic/logs/view`.

## ??? Arquitectura

### **Servicio Centralizado: `IApiLoggerService`**

```csharp
IApiLoggerService apiLogger = // Inyectado por DI

// Escribir log
apiLogger.WriteLog("Mensaje", "INFO", "AUTH");
await apiLogger.WriteLogAsync("Mensaje async", "ERROR", "DIAGNOSTIC");

// Leer logs
string[] logs = apiLogger.ReadLogLines(100);
string logPath = apiLogger.GetLogFilePath();
```

### **Niveles de Log Disponibles:**
- `INFO` - Información general
- `WARNING` - Advertencias
- `ERROR` - Errores
- `DEBUG` - Información de debug

### **Fuentes de Log (Source):**
- `AUTH` - Controlador de autenticación
- `DIAGNOSTIC` - Controlador de diagnóstico
- `USUARIO` - Controlador de usuarios
- `PROVEEDOR` - Controlador de proveedores
- `API` - General del API

## ?? Ubicación de Logs

### **Prioridad de ubicación:**
1. **Servidor IIS:** `C:\inetpub\logs\AdmIn\`
2. **Fallback:** `{AppContext.BaseDirectory}\Logs\`

### **Formato de archivo:**
- `AdmIn-API-2024-01-15.log` (un archivo por día)

### **Formato de entrada de log:**
```
[2024-01-15 14:30:25.123] [INFO] [AUTH] ========== INICIO PROCESO LOGIN ==========
[2024-01-15 14:30:25.125] [DEBUG] [AUTH] Datos raw recibidos: {"email":"user@test.com","password":"***"}
[2024-01-15 14:30:25.130] [ERROR] [AUTH] ERROR DESERIALIZACIÓN: Invalid JSON format
```

## ?? Uso en Controladores

### **1. Registro en Program.cs**
```csharp
builder.Services.AddScoped<IApiLoggerService, ApiLoggerService>();
```

### **2. Inyección en Controlador**
```csharp
public class AuthController : ControllerBase
{
    private readonly IApiLoggerService _apiLogger;

    public AuthController(IApiLoggerService apiLogger)
    {
        _apiLogger = apiLogger;
    }
}
```

### **3. Uso en Métodos**
```csharp
[HttpPost("login")]
public async Task<DTO<Usuario>> Login([FromBody] dynamic duser)
{
    _apiLogger.WriteLog("========== INICIO PROCESO LOGIN ==========", "INFO", "AUTH");
    
    try
    {
        _apiLogger.WriteLog("Validando credenciales...", "INFO", "AUTH");
        // ... lógica ...
        _apiLogger.WriteLog("Login exitoso", "INFO", "AUTH");
    }
    catch (Exception ex)
    {
        _apiLogger.WriteLog($"Error en login: {ex.Message}", "ERROR", "AUTH");
    }
}
```

## ?? Visualización de Logs

### **Endpoint de visualización:**
```
GET /api/Diagnostic/logs/view?lines=100
```

### **Características del visor:**
- ? **Auto-refresh** cada 10 segundos
- ? **Colores por nivel de log** (ERROR=rojo, WARNING=amarillo, etc.)
- ? **Logs más recientes primero**
- ? **Información del servidor** (máquina, PID, directorio)
- ? **Responsive design**

### **API endpoints relacionados:**
```
GET /api/Diagnostic/logs          - JSON con logs
GET /api/Diagnostic/logs/view     - Visor HTML
GET /api/Diagnostic/test-logging  - Generar logs de prueba
```

## ?? Migración de Controladores Existentes

### **Antes (método individual):**
```csharp
// Cada controlador tenía su propio método de logging
static async void LogToFile(string message) { ... }
WriteToServerLogFile($"[DIAGNOSTIC] {message}");
```

### **Después (servicio centralizado):**
```csharp
// Todos usan el mismo servicio
_apiLogger.WriteLog("Mensaje", "INFO", "CONTROLLER_NAME");
```

## ?? Beneficios

### **? Centralización:**
- Todos los logs en el mismo archivo
- Un solo punto de configuración
- Fácil visualización desde DiagnosticController

### **? Estandarización:**
- Formato uniforme con timestamp, nivel y fuente
- Niveles de log consistentes
- Threading seguro

### **? Mantenibilidad:**
- Un solo servicio para modificar
- Fácil agregar nuevas características
- Logs estructurados

### **? Diagnóstico mejorado:**
- Visor web con auto-refresh
- Filtrado visual por colores
- Información del servidor integrada

## ?? Controladores Actualizados

- ? **DiagnosticController** - Migrado completamente
- ? **Auth** - Migrado completamente
- ? **UsuarioController** - Pendiente
- ? **ProveedorController** - Pendiente
- ? **Otros controladores** - Pendiente

## ?? Próximos Pasos

1. **Migrar controladores restantes** al servicio centralizado
2. **Agregar filtros avanzados** en el visor (por nivel, por fuente)
3. **Implementar rotación de logs** (archivo por día/semana)
4. **Agregar métricas** de performance
5. **Integrar con Application Insights** (opcional)

## ?? Configuración Avanzada

### **Cambiar directorio de logs:**
Modificar `ApiLoggerService.GetLogsDirectory()` para usar una ubicación personalizada.

### **Agregar nuevo nivel de log:**
1. Modificar `WriteLog()` para manejar el nuevo nivel
2. Actualizar CSS del visor para el color correspondiente
3. Documentar el uso del nuevo nivel

### **Thread Safety:**
El servicio usa `lock` para escritura sincrónica y `async` para escritura asíncrona segura.

---

**? Sistema implementado exitosamente - Logs centralizados y listos para diagnóstico!**