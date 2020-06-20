using System.Collections.Generic;

namespace Cardlytics.BasicApi.Services
{
    public interface IPrimeFactorService
    {
        List<int> GetAllPrimeFactors(int number);
    }
}