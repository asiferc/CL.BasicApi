using System;

using Cardlytics.BasicApi.DataAccess;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cardlytics.BasicApi.Tests.Fixtures
{
    public class BasicDataFixture : IDisposable
    {
        public BasicDataFixture()
        {
            var builder = new DbContextOptionsBuilder<BasicDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            Context = new BasicDbContext(builder.Options);
            Context.Database.EnsureCreated();

            SeedContext();

            builder = new DbContextOptionsBuilder<BasicDbContext>()
                .UseSqlServer("Crappy connection string");
            FailureContext = new BasicDbContext(builder.Options);
        }

        public BasicDbContext Context { get; }

        public BasicDbContext FailureContext { get; }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }

        private void SeedContext()
        {
            // Add any seed data here
            Context.SaveChanges();
        }
    }
}
