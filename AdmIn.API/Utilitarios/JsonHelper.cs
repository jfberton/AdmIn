using System.Text.Json;

namespace AdmIn.API.Utilitarios
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(dynamic json)
        {
            try
            {
                // Convertir el dynamic a string
                string jsonString = json.ToString();

                // Configurar el deserializador con insensibilidad a mayúsculas/minúsculas
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserializar al tipo solicitado
                return JsonSerializer.Deserialize<T>(jsonString, options);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deserializando JSON al tipo {typeof(T).Name}: {ex.Message}", ex);
            }
        }
    }

}
