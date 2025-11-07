using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Common.Services
{
 public interface IEmailService
 {
 Task SendAsync(string to, string subject, string htmlBody);
 string RenderTemplate(string templateFile, Dictionary<string, string> model);
 }
}