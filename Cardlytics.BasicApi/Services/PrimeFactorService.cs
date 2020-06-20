using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.Services
{
    public class PrimeFactorService : IPrimeFactorService
    {
        private readonly ILogger<PrimeFactorService> _logger;

        public PrimeFactorService(ILogger<PrimeFactorService> logger)
        {
            _logger = logger;
        }

        public List<int> GetAllPrimeFactors(int number)
        {
            List<int> primeFactors = new List<int>();

            while (number % 2 == 0)
            {
                primeFactors.Add(2);
                number /= 2;
            }

            int factor = 3;
            while (factor * factor <= number)
            {
                if (number % factor == 0)
                {
                    primeFactors.Add(factor);
                    number /= factor;
                }
                else
                {
                    factor += 2;
                }
            }

            if (number > 1)
            {
                primeFactors.Add(number);
            }

            return primeFactors;
        }
    }
}