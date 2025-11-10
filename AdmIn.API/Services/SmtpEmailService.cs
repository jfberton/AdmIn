using AdmIn.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace AdmIn.API.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;
        private const string TEST_EMAIL = "atp.jfbertoncini@chaco.gov.ar"; // hardcoded for testing; remove in prod

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            try
            {
                _logger.LogInformation("========== EMAIL SERVICE: SendAsync START ==========");
                _logger.LogInformation($"Original TO: {to}");
                _logger.LogInformation($"Subject: {subject}");
                _logger.LogInformation($"Body Length: {htmlBody?.Length ?? 0}");

                // For now, ignore 'to' and send to TEST_EMAIL for testing
                var recipient = TEST_EMAIL;
                _logger.LogWarning($"?? MODO TEST: Redirigiendo email de '{to}' a '{recipient}'");

                var smtpHost = _config["Smtp:Host"] ?? "localhost";
                var smtpPort = int.TryParse(_config["Smtp:Port"], out var p) ? p : 25;
                var smtpUser = _config["Smtp:User"];
                var smtpPass = _config["Smtp:Pass"];
                var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var s) ? s : false;
                var fromAddress = _config["Smtp:From"] ?? "no-reply@example.com";

                _logger.LogInformation($"SMTP Configuration:");
                _logger.LogInformation($"  Host: {smtpHost}");
                _logger.LogInformation($"  Port: {smtpPort}");
                _logger.LogInformation($"  User: {smtpUser ?? "(not configured)"}");
                _logger.LogInformation($"  EnableSsl: {enableSsl}");
                _logger.LogInformation($"  From: {fromAddress}");

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl
                };

                if (!string.IsNullOrEmpty(smtpUser))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    _logger.LogInformation("? SMTP Credentials configured");
                }
                else
                {
                    _logger.LogWarning("?? No SMTP credentials configured (anonymous)");
                }

                var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8
                };
                // Especificar explícitamente el ContentType con charset UTF-8
                mail.BodyTransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;

                mail.To.Add(recipient);

                _logger.LogInformation($"Intentando enviar email a: {recipient}");
                _logger.LogInformation($"Encoding: UTF-8");
                _logger.LogInformation($"Transfer Encoding: QuotedPrintable");

                await client.SendMailAsync(mail);

                _logger.LogInformation("? Email enviado exitosamente");
                _logger.LogInformation("========== EMAIL SERVICE: SendAsync END ==========");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"? SMTP Exception: {smtpEx.Message}");
                _logger.LogError($"Status Code: {smtpEx.StatusCode}");
                _logger.LogError($"Stack Trace: {smtpEx.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"? General Exception: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        string AdmIn.Common.Services.IEmailService.RenderTemplate(string templateFile, System.Collections.Generic.Dictionary<string, string> model)
        {
            try
            {
                _logger.LogInformation($"========== EMAIL SERVICE: RenderTemplate START ==========");
                _logger.LogInformation($"Template File: {templateFile}");
                _logger.LogInformation($"Model Keys: {string.Join(", ", model.Keys)}");

                var path = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", templateFile);
                _logger.LogInformation($"Template Path: {path}");

                if (!File.Exists(path))
                {
                    _logger.LogError($"? Template file not found: {path}");
                    return string.Empty;
                }

                var content = File.ReadAllText(path);
                _logger.LogInformation($"Template loaded, length: {content.Length}");

                _logger.LogInformation("Replacing placeholders:");
                foreach (var kv in model)
                {
                    var placeholder = "{{" + kv.Key + "}}";
                    if (content.Contains(placeholder))
                    {
                        content = content.Replace(placeholder, kv.Value);
                        _logger.LogInformation($"  ? Replaced {{{{  {kv.Key} }}}} with: {(kv.Key == "Link" ? kv.Value : "***")}");
                    }
                    else
                    {
                        _logger.LogWarning($"  ?? Placeholder {{{{{ kv.Key }}}}} not found in template");
                    }
                }

                // remove any unreplaced placeholders
                var unreplacedCount = Regex.Matches(content, "\\{\\{.+?\\}\\}").Count;
                if (unreplacedCount > 0)
                {
                    _logger.LogWarning($"?? Found {unreplacedCount} unreplaced placeholders, removing them");
                    content = Regex.Replace(content, "\\{\\{.+?\\}\\}", string.Empty);
                }

                _logger.LogInformation($"? Template rendered successfully, final length: {content.Length}");
                _logger.LogInformation("========== EMAIL SERVICE: RenderTemplate END ==========");

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error rendering template: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                return string.Empty;
            }
        }
    }
}