using System;

using Cardlytics.BasicApi.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.DataAccess
{
    public class HealthRepository : IHealthRepository
    {
        private readonly ILogger<HealthRepository> _logger;

        public HealthRepository(ILogger<HealthRepository> logger, BasicDbContext context)
        {
            _logger = logger;
            Context = context;
        }

        public DbContext Context { get; }

        public Health VerifyDatabaseConnection()
        {
            IDbContextTransaction testTransaction = null;
            var healthResult = new Health();

            try
            {
                testTransaction = Context.Database.BeginTransaction();

                healthResult.DatabaseConnectionVerified = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while attempting to verify database connectivity.");
                healthResult.DatabaseConnectionVerified = false;
            }
            finally
            {
                testTransaction?.Dispose();
            }

            return healthResult;
        }
    }
}
