using Microsoft.AspNetCore.Mvc;

namespace _GoodWebsite.Controllers
{
    [ApiController]
    [Route("slow")]
    public class SlowController : ControllerBase
    {
        private readonly ILogger<SlowController> _logger;
        public SlowController(ILogger<SlowController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "")]
        public async Task<IActionResult> Index(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                _logger.LogInformation($"Waiting {i} more seconds...");
                await Task.Delay(1000);
            }
            _logger.LogInformation($"Finished.");
            return Ok($"waited {seconds} seconds before responding: 👍");
        }
    }
}