using Microsoft.AspNetCore.Mvc;

namespace _GoodWebsite.Controllers
{
    [ApiController]
    [Route("healthz")]
    public class HealthZController : ControllerBase
    {
        private readonly ILogger<HealthZController> _logger;
        public HealthZController(ILogger<HealthZController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        [Route("")]
        public IActionResult Index()
        {
            return Ok("👍");
        }
    }
}