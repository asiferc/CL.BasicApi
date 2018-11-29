using System;
using System.Linq;
using System.Net.Http;

using AutoMapper;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StructureMap;
using StructureMap.AspNetCore;

namespace Cardlytics.BasicApi.Tests.Fixtures
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;

        public TestServerFixture(
            Registry structureMapOverrides = null,
            Action<IServiceCollection> testServiceConfiguration = null)
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(s =>
                {
                    // remove db context from service collection
                    var existingContext = s
                        .Single(d => d.ServiceType.IsSubclassOf(typeof(DbContext)));

                    s.Remove(existingContext);

                    testServiceConfiguration?.Invoke(s);
                })
                .UseStructureMap(structureMapOverrides);

            _testServer = new TestServer(builder);

            Services = _testServer.Host.Services;
            Client = _testServer.CreateClient();
            Client.BaseAddress = new Uri(@"https://localhost");
        }

        public HttpClient Client { get; }

        public IServiceProvider Services { get; }

        public void Dispose()
        {
            _testServer.Dispose();
            Mapper.Reset();
        }
    }
}
