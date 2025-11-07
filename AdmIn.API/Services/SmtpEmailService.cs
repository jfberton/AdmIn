using AdmIn.Common.Services;
using Microsoft.Extensions.Configuration;
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
        private const string TEST_EMAIL = "testing@example.com"; // hardcoded for testing; remove in prod

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            // For now, ignore 'to' and send to TEST_EMAIL for testing
            var recipient = TEST_EMAIL;

            var smtpHost = _config["Smtp:Host"] ?? "localhost";
            var smtpPort = int.TryParse(_config["Smtp:Port"], out var p) ? p : 25;
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];
            var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var s) ? s : false;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl
            };

            if (!string.IsNullOrEmpty(smtpUser))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            }

            var mail = new MailMessage
            {
                From = new MailAddress(_config["Smtp:From"] ?? "no-reply@example.com"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(recipient);

            await client.SendMailAsync(mail);
        }

        string AdmIn.Common.Services.IEmailService.RenderTemplate(string templateFile, System.Collections.Generic.Dictionary<string,string> model)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", templateFile);
            if (!File.Exists(path)) return string.Empty;
            var content = File.ReadAllText(path);

            foreach (var kv in model)
            {
                content = content.Replace("{{" + kv.Key + "}}", kv.Value);
            }

            // remove any unreplaced placeholders
            content = Regex.Replace(content, "\\{\\{.+?\\}\\}", string.Empty);
            return content;
        }
    }
}