using Microsoft.AspNetCore.Mvc;

namespace PingServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        // GET: api/ping
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Pong"); // Respond with "Pong" when pinged
        }
    }
}
