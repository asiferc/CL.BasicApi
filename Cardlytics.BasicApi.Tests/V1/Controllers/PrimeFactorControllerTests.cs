using System.Net;
using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.Tests.Fixtures;
using FluentAssertions;
using NUnit.Framework;
using StructureMap;

namespace Cardlytics.BasicApi.Tests.V1.Controllers
{
    [TestFixture]
    public class PrimeFactorControllerTests
    {
        private TestServerFixture _testServer;

        private BasicDataFixture _basicDataFixture;

        [Test]
        [TestCase(10, ExpectedResult = "[2,5]")]
        [TestCase(323, ExpectedResult = "[17,19]")]
        [TestCase(1005, ExpectedResult = "[3,5,67]")]
        public string ValidPrimeNumber(int number)
        {
            // Arrange/Act
            _basicDataFixture = new BasicDataFixture();
            _testServer = new TestServerFixture(BuildTestRegistryForSuccess());

            var response = _testServer.Client.GetAsync(string.Format("/primefactor?number={0}", number)).Result;
            var body = response.Content.ReadAsStringAsync().Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _basicDataFixture.Dispose();
            _testServer.Dispose();

            return body;
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
