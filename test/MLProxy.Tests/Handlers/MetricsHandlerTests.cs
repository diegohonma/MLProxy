using Microsoft.Extensions.Logging;
using MLProxy.Entities;
using MLProxy.Handlers;
using MLProxy.Interfaces.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MLProxy.Tests.Handlers
{
    [ExcludeFromCodeCoverage]
    internal class MetricsHandlerTests
    {
        private Mock<IMetricsRepository> _metricsRepository;
        private Mock<ILogger<MetricsHandler>> _logger;
        private MetricsHandler _metricsHandler;

        [SetUp]
        public void SetUp()
        {
            _metricsRepository = new Mock<IMetricsRepository>();
            _logger = new Mock<ILogger<MetricsHandler>>();
            _metricsHandler = new MetricsHandler(_metricsRepository.Object, _logger.Object);
        }

        [Test]
        public async Task Should_Return_Metrics()
        {
            var metrics = new List<Metric>
            {
                new Metric("127.0.0.1", "/api/test", DateTime.Now),
                new Metric("127.0.0.1", "/api/test", DateTime.Now),
                new Metric("127.0.0.1", "/api/test1", DateTime.Now),
                new Metric("127.0.0.1", "/api/test2", DateTime.Now)
            };

            _metricsRepository
                .Setup(it => it.GetAllMetrics())
                .ReturnsAsync(metrics);

            var response = await _metricsHandler.GetMetrics();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, response.First(it => it.Path == "/api/test").RequestsCount);
                Assert.AreEqual(1, response.First(it => it.Path == "/api/test1").RequestsCount);
                Assert.AreEqual(1, response.First(it => it.Path == "/api/test2").RequestsCount);
            });
        }

        [Test]
        public void LogException_When_ErrorOccurs()
        {
            _metricsRepository
                .Setup(it => it.GetAllMetrics())
                .Throws(new Exception());

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync<Exception>(async () => await _metricsHandler.GetMetrics());

                _logger
                    .Verify(l => l.Log(LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<object>(x => x.ToString().Equals("Erro ao buscar métricas")),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()), Times.Once);
            });
        }
    }
}
