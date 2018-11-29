using System.Linq;
using System.Net.Http;
using System.Text;

using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.Services;
using Cardlytics.BasicApi.Tests.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

using Newtonsoft.Json;

using NSubstitute;

using NUnit.Framework;

using StructureMap;

namespace Cardlytics.BasicApi.Tests.Middleware
{
    public class RequestLogMiddlewareTests
    {
        private TestServerFixture _testServer;

        private BasicDataFixture _dummyDataFixture;

        private ILoggerFactory _loggerFactory;

        private ILogger _requestLogger;

        private IHealthService _healthService;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _requestLogger = Substitute.For<ILogger>();
            _loggerFactory.CreateLogger("RequestLogger").Returns(_requestLogger);
            _healthService = Substitute.For<IHealthService>();
            _dummyDataFixture = new BasicDataFixture();
            _testServer = new TestServerFixture(
                BuildTestRegistry(),
                s => s.Remove(s.Single(d => d.ServiceType == typeof(ILoggerFactory))));
        }

        [SetUp]
        public void Setup()
        {
            _requestLogger.ClearReceivedCalls();
        }

        [TestCase(@"/v1/health/")]
        [TestCase(@"/v1/fakeEndpoint/")]
        public void RequestLogger_LogsAllGetRequests(string resource)
        {
            // act
            var response = _testServer.Client.GetAsync(resource).Result;

            // assert
            var requestLogValues = _requestLogger
                .ReceivedCalls()
                .Single()
                .GetArguments()
                .Single(o => o is FormattedLogValues) as FormattedLogValues;
            var requestEntry = requestLogValues.Single().Value.ToString();

            requestEntry.Should().ContainAll(
                resource,
                "GET");
        }

        [TestCase(@"/v1/health/")]
        [TestCase(@"/v1/fakeEndpoint/")]
        public void RequestLogger_LogsAllPostRequests(string resource)
        {
            // arrange
            var requestBodyObject = new { Name = "something special", Id = 5 };
            var requestBody = JsonConvert.SerializeObject(requestBodyObject);
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // act
            var response = _testServer.Client.PostAsync(resource, requestContent).Result;

            // assert
            var requestLogValues = _requestLogger
                .ReceivedCalls()
                .Single()
                .GetArguments()
                .Single(o => o is FormattedLogValues) as FormattedLogValues;
            var requestEntry = requestLogValues.Single().Value.ToString();

            requestEntry.Should().ContainAll(
                requestBodyObject.Name,
                requestBodyObject.Id.ToString(),
                resource,
                "POST");
        }

        [Test]
        public void RequestLogger_LogsUnhandledException()
        {
            // arrange
            const string Resource = @"/v1/health/";
            var requestBodyObject = new { Name = "bad things", Id = 6 };
            var requestBody = JsonConvert.SerializeObject(requestBodyObject);
            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // act
            var response = _testServer.Client.PostAsync(Resource, requestContent).Result;

            // assert
            var requestLogValues = _requestLogger
                .ReceivedCalls()
                .Single()
                .GetArguments()
                .Single(o => o is FormattedLogValues) as FormattedLogValues;
            var requestEntry = requestLogValues.Single().Value.ToString();

            requestEntry.Should().ContainAll(
                requestBodyObject.Name,
                requestBodyObject.Id.ToString(),
                Resource,
                "POST");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testServer.Dispose();
            _dummyDataFixture.Dispose();
        }

        private Registry BuildTestRegistry()
        {
            var registry = new Registry();
            registry.For<ILoggerFactory>().Use(_loggerFactory);
            registry.For<BasicDbContext>().Use(_dummyDataFixture.Context);

            return registry;
        }
    }
}
