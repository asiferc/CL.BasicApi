using AutoMapper;

using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.V1.Models;

using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.Services
{
    public class HealthService : IHealthService
    {
        private readonly IHealthRepository _repository;

        private readonly ILogger<HealthService> _logger;

        public HealthService(ILogger<HealthService> logger, IHealthRepository healthRepository)
        {
            _logger = logger;
            _repository = healthRepository;
        }

        public HealthDto CheckServiceHealth()
        {
            var dataAccessHealth = _repository.VerifyDatabaseConnection();
            var healthResult = Mapper.Map<HealthDto>(dataAccessHealth);

            healthResult.ServiceHealthy = true;
            return healthResult;
        }
    }
}