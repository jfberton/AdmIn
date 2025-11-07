using AdmIn.Common;
using AdmIn.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public EmailTestController(IEmailService emailService, IConfiguration config)
    {
     _emailService = emailService;
          _config = config;
     }

        [HttpPost("send")]
        [AllowAnonymous] // Para pruebas - cambiar a [Authorize] en producción
        public async Task<DTO<string>> SendTestEmail([FromBody] EmailTestRequest request)
     {
       try
      {
   Console.WriteLine("=== EMAIL TEST START ===");
                Console.WriteLine($"Destino: {request.to}");
Console.WriteLine($"Asunto: {request.subject}");
   Console.WriteLine($"Tipo: {request.testType}");

         // Obtener configuración SMTP
    var smtpHost = _config["Smtp:Host"];
   var smtpPort = _config["Smtp:Port"];
          var smtpUser = _config["Smtp:User"];
     var smtpPass = _config["Smtp:Pass"];
     var enableSsl = _config["Smtp:EnableSsl"];
     var smtpFrom = _config["Smtp:From"];

              Console.WriteLine($"SMTP Host: {smtpHost}");
        Console.WriteLine($"SMTP Port: {smtpPort}");
                Console.WriteLine($"SMTP User: {smtpUser}");
 Console.WriteLine($"SMTP Pass: {(string.IsNullOrEmpty(smtpPass) ? "NO CONFIGURADO" : "***OCULTO***")}");
         Console.WriteLine($"Enable SSL: {enableSsl}");
    Console.WriteLine($"From: {smtpFrom}");

       // Validar configuración
      if (string.IsNullOrEmpty(smtpHost))
    {
                return new DTO<string>
         {
          Correcto = false,
    Mensaje = "? SMTP Host no configurado. Revisa appsettings.json o User Secrets."
            };
    }

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
    {
   return new DTO<string>
      {
             Correcto = false,
 Mensaje = "? SMTP User o Password no configurados. Para Gmail necesitas una 'App Password'."
    };
        }

        string htmlBody;

    // Preparar el cuerpo del email según el tipo
                switch (request.testType?.ToLower())
                {
         case "password_reset":
         var resetModel = new Dictionary<string, string>
       {
  { "Name", "Usuario de Prueba" },
        { "Link", "https://localhost:7189/confirm-password-reset?token=TEST123" },
   { "ExpiryHours", "24" }
 };
         htmlBody = _emailService.RenderTemplate("PasswordReset_Request.html", resetModel);
 request.subject = "Restablecer contraseña - PRUEBA";
       break;

            case "new_user":
               var newUserModel = new Dictionary<string, string>
     {
          { "Name", "Nuevo Usuario" },
         { "Link", "https://localhost:7189/confirm-password-reset?token=NEWUSER123" },
      { "ExpiryHours", "24" }
        };
       htmlBody = _emailService.RenderTemplate("NewUser_SetPassword.html", newUserModel);
         request.subject = "Bienvenido - Configura tu contraseña - PRUEBA";
           break;

  default:
            htmlBody = $@"
   <html>
      <body style='font-family: Arial, sans-serif; padding: 20px;'>
           <h2>?? Email de Prueba</h2>
      <p>{request.body}</p>
       <hr/>
       <p style='color: #666; font-size: 12px;'>
     Este es un email de prueba enviado desde el sistema AdmIn.<br/>
      Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
   </p>
            </body>
      </html>";
  break;
         }

  if (string.IsNullOrEmpty(htmlBody))
    {
             htmlBody = $"<p>{request.body}</p>";
       }

                Console.WriteLine("Intentando enviar email...");

      // Intentar envío
                try
              {
   await _emailService.SendAsync(request.to, request.subject, htmlBody);
    
             Console.WriteLine("? Email enviado correctamente");
         
        return new DTO<string>
           {
           Correcto = true,
        Mensaje = $"? Email enviado correctamente a {request.to}\n\n" +
       $"Configuración usada:\n" +
              $"- Host: {smtpHost}\n" +
          $"- Port: {smtpPort}\n" +
                  $"- SSL: {enableSsl}\n" +
        $"- User: {smtpUser}\n" +
  $"- From: {smtpFrom}",
        Datos = "SUCCESS"
   };
       }
         catch (SmtpException smtpEx)
       {
      Console.WriteLine($"? SMTP Exception: {smtpEx.Message}");
         Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
   
            var errorMsg = $"? Error SMTP: {smtpEx.Message}\n\n";
 
          if (smtpEx.Message.Contains("Authentication") || smtpEx.StatusCode == SmtpStatusCode.AuthenticationRequired)
      {
         errorMsg += "?? SOLUCIÓN para Gmail:\n" +
         "1. Ve a https://myaccount.google.com/apppasswords\n" +
   "2. Genera una 'Contraseña de aplicación'\n" +
     "3. Usa esa contraseña (no tu contraseña normal)\n" +
              "4. Configura en User Secrets o appsettings:\n" +
          "   - Smtp:Host = smtp.gmail.com\n" +
      "   - Smtp:Port = 587\n" +
             "   - Smtp:EnableSsl = true\n" +
                "- Smtp:User = tu@gmail.com\n" +
  "   - Smtp:Pass = (contraseña de aplicación)";
             }
    
      return new DTO<string>
           {
      Correcto = false,
             Mensaje = errorMsg
             };
            }
    }
 catch (Exception ex)
            {
       Console.WriteLine($"? Exception general: {ex.Message}");
          Console.WriteLine($"Stack: {ex.StackTrace}");
         
      return new DTO<string>
                {
      Correcto = false,
         Mensaje = $"? Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}"
                };
          }
finally
            {
Console.WriteLine("=== EMAIL TEST END ===");
            }
 }

   [HttpGet("config")]
        [AllowAnonymous]
     public DTO<object> GetConfig()
        {
            try
     {
        var config = new
      {
            Host = _config["Smtp:Host"] ?? "(no configurado)",
 Port = _config["Smtp:Port"] ?? "(no configurado)",
        User = _config["Smtp:User"] ?? "(no configurado)",
       PasswordConfigured = !string.IsNullOrEmpty(_config["Smtp:Pass"]),
      EnableSsl = _config["Smtp:EnableSsl"] ?? "(no configurado)",
    From = _config["Smtp:From"] ?? "(no configurado)"
             };

     return new DTO<object>
         {
  Correcto = true,
        Datos = config
         };
  }
            catch (Exception ex)
  {
      return new DTO<object>
          {
     Correcto = false,
       Mensaje = ex.Message
                };
       }
        }
    }

    public class EmailTestRequest
  {
        public string to { get; set; } = string.Empty;
      public string subject { get; set; } = string.Empty;
public string body { get; set; } = string.Empty;
    public string testType { get; set; } = "simple";
    }
}
