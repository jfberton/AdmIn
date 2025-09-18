namespace AdmIn.API.Services
{
    public interface IApiLoggerService
    {
        /// <summary>
        /// Escribe un mensaje de log al archivo centralizado
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        /// <param name="logLevel">Nivel de log (INFO, ERROR, WARNING, DEBUG)</param>
        /// <param name="source">Fuente del log (controlador, servicio, etc.)</param>
        void WriteLog(string message, string logLevel = "INFO", string source = "API");

        /// <summary>
        /// Escribe un mensaje de log async al archivo centralizado
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        /// <param name="logLevel">Nivel de log (INFO, ERROR, WARNING, DEBUG)</param>
        /// <param name="source">Fuente del log (controlador, servicio, etc.)</param>
        Task WriteLogAsync(string message, string logLevel = "INFO", string source = "API");

        /// <summary>
        /// Obtiene las últimas líneas del archivo de log
        /// </summary>
        /// <param name="maxLines">Número máximo de líneas a obtener</param>
        /// <returns>Array con las líneas del log</returns>
        string[] ReadLogLines(int maxLines = 100);

        /// <summary>
        /// Obtiene la ruta del archivo de log actual
        /// </summary>
        /// <returns>Ruta completa del archivo de log</returns>
        string GetLogFilePath();

        /// <summary>
        /// Obtiene la ruta del directorio de logs
        /// </summary>
        /// <returns>Ruta del directorio de logs</returns>
        string GetLogsDirectory();
    }
}