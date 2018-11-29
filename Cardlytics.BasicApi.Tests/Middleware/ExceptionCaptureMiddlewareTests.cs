using System;
using System.Linq;
using System.Net;

using Cardlytics.BasicApi.DataAccess;
using Cardlytics.BasicApi.Middleware;
using Cardlytics.BasicApi.Services;
using Cardlytics.BasicApi.Tests.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

using StructureMap;

namespace Cardlytics.BasicApi.Tests.Middleware
{
    public class ExceptionCaptureMiddlewareTests
    {
        private TestServerFixture _testServer;

        private BasicDataFixture _dummyDataFixture;

        private ILogger<ExceptionCaptureMiddleware> _logger;

        private IHealthService _healthService;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _healthService = Substitute.For<IHealthService>();
            _logger = Substitute.For<ILogger<ExceptionCaptureMiddleware>>();
            _dummyDataFixture = new BasicDataFixture();
            _testServer = new TestServerFixture(BuildTestRegistry());
        }

        [SetUp]
        public void Setup()
        {
            _logger.ClearReceivedCalls();
            _healthService.ClearReceivedCalls();
        }

        [Test]
        public void ExceptionCapture_Invoked_ForException()
        {
            // arrange
            const string ExceptionMessage = "BOOM!";
            _healthService
                .When(x => x.CheckServiceHealth())
                .Do(d => throw new Exception(ExceptionMessage));

            // act
            var response = _testServer.Client.GetAsync(@"/v1/health/").Result;

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var loggedException = _logger
                .ReceivedCalls()
                .Single()
                .GetArguments()
                .Single(o => o is Exception) as Exception;

            loggedException.Message.Should().Contain(ExceptionMessage);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testServer.Dispose();
            _dummyDataFixture.Dispose();
        }

        private Registry BuildTestRegistry()
        {
            // inject in-memory data context
            var registry = new Registry();
            registry.For<ILogger<ExceptionCaptureMiddleware>>().Use(_logger);
            registry.For<IHealthService>().Use(_healthService);
            registry.For<BasicDbContext>().Use(_dummyDataFixture.Context);

            return registry;
        }
    }
}
