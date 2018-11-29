using Cardlytics.BasicApi.V1.Models;

namespace Cardlytics.BasicApi.Services
{
    public interface IHealthService
    {
        HealthDto CheckServiceHealth();
    }
}