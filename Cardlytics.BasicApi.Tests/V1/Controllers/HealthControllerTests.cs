using System.Net;

using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.Tests.Fixtures;

using FluentAssertions;

using NUnit.Framework;

using StructureMap;

namespace Cardlytics.BasicApi.Tests.V1.Controllers
{
    [TestFixture]
    public class HealthControllerTests
    {
        private TestServerFixture _testServer;

        private BasicDataFixture _basicDataFixture;

        [Test]
        public void Health_ShouldReturnSuccessMessage_WhenDatabaseAvailable()
        {
            // Arrange/Act
            _basicDataFixture = new BasicDataFixture();
            _testServer = new TestServerFixture(BuildTestRegistryForSuccess());

            var response = _testServer.Client.GetAsync(@"/health").Result;
            var body = response.Content.ReadAsStringAsync().Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Contain("The API is working correctly.");

            _basicDataFixture.Dispose();
            _testServer.Dispose();
        }

        [Test]
        public void Health_ShouldReturnFailureMessage_WhenDatabaseUnavailable()
        {
            // arrange
            // act
            _basicDataFixture = new BasicDataFixture();
            _testServer = new TestServerFixture(BuildTestRegistryForFailure());

            var response = _testServer.Client.GetAsync(@"/health").Result;
            var body = response.Content.ReadAsStringAsync().Result;

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Contain("One or more components of the API are malfunctioning.");

            _basicDataFixture.Dispose();
            _testServer.Dispose();
        }

        private Registry BuildTestRegistryForSuccess()
        {
            // inject in-memory data context
            var registry = new Registry();
            registry.For<BasicDbContext>().Use(_basicDataFixture.Context);

            return registry;
        }

        private Registry BuildTestRegistryForFailure()
        {
            // inject in-memory data context
            var registry = new Registry();
            registry.For<BasicDbContext>().Use(_basicDataFixture.FailureContext);

            return registry;
        }
    }
}
