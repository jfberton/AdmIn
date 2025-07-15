using AdmIn.Business.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        public HealthCheckController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new { status = "API is active", databaseConnection = "Successful" });
        }
    }
}
