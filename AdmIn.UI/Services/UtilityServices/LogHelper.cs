namespace AdmIn.UI.Services.UtilityServices
{
    using Microsoft.JSInterop;
    using System;
    using System.Threading.Tasks;

    public class LogHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public LogHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Info(string message)
        {
            await Log("info", message);
        }

        public async Task Warn(string message)
        {
            await Log("warn", message);
        }

        public async Task Error(string message)
        {
            await Log("error", message);
        }

        public async Task Debug(string message)
        {
            await Log("debug", message);
        }

        private async Task Log(string logType, string message)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync($"console.{logType}", message);
            }
            catch (Exception ex)
            {
                // Manejo de errores internos del log
                Console.WriteLine($"Error al registrar el log en consola: {ex.Message}");
            }
        }
    }

}
