using System.Collections.Generic;
using Cardlytics.BasicApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.V1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    public class PrimeFactorController : ControllerBase
    {
        private readonly IPrimeFactorService _primeFactorService;

        private readonly ILogger<PrimeFactorController> _logger;

        public PrimeFactorController(ILogger<PrimeFactorController> logger, IPrimeFactorService primeFactorService)
        {
            _logger = logger;
            _primeFactorService = primeFactorService;
        }

        [HttpGet]
        public IActionResult Get(int number)
        {
            List<int> primeFactors = _primeFactorService.GetAllPrimeFactors(number);
            return Ok(primeFactors);
        }
    }
}