using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    /// <summary>
    /// Contr�leur de statut pour v�rifier que l'API fonctionne
    /// </summary>
    [ApiController]
    [Route("api/status")]
    [Produces("application/json")]
    public class StatusController : ControllerBase
    {
        /// <summary>
        /// V�rification du statut de l'API
        /// </summary>
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "OK",
                message = "API Dior fonctionne correctement",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Test de sant� de l'API
        /// </summary>
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                healthy = true,
                checks = new
                {
                    api = "OK",
                    timestamp = DateTime.UtcNow
                }
            });
        }
    }
}