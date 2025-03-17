using AdmIn.Business.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IServ_Rol _servicio;

        public HealthCheckController(IServ_Rol servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Intentar obtener un usuario por su ID para verificar la conexión a la base de datos
                var resultado = await _servicio.Obtener_todos();
                if (resultado.Correcto)
                {
                    return Ok(new { status = "API is active", databaseConnection = "Successful" });
                }
                else
                {
                    return StatusCode(500, new { status = "API is active", databaseConnection = $"Failed. {resultado.Mensaje}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "API is active", databaseConnection = "Failed", error = ex.Message });
            }
        }
    }
}
