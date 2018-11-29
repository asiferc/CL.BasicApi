using Cardlytics.BasicApi.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.V1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _healthService;

        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger, IHealthService healthService)
        {
            _healthService = healthService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var serviceHealth = _healthService.CheckServiceHealth();

            serviceHealth.ControllerHealthy = true;

            if (serviceHealth.ControllerHealthy && serviceHealth.ServiceHealthy && serviceHealth.DataAccessHealthy)
            {
                serviceHealth.HealthMessage = "The API is working correctly.";
            }
            else
            {
                serviceHealth.HealthMessage = "One or more components of the API are malfunctioning.";
            }

            return Ok(serviceHealth);
        }
    }
}
