using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Returns status of API Gateway</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var response = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            };
            
            _logger.LogInformation("Health check called");
            return Ok(response);
        }

        /// <summary>
        /// Detailed health check endpoint
        /// </summary>
        /// <returns>Returns detailed health status</returns>
        [HttpGet("detailed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDetailed()
        {
            var response = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                services = new
                {
                    productService = "https://product-service",
                    orderService = "https://order-service"
                },
                features = new
                {
                    rateLimiting = true,
                    caching = true,
                    authentication = true,
                    authorization = true
                }
            };

            _logger.LogInformation("Detailed health check called");
            return Ok(response);
        }
    }
}
