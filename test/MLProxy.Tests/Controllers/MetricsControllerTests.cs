using Microsoft.AspNetCore.Mvc;
using MLProxy.Controllers;
using MLProxy.Interfaces.Handlers;
using MLProxy.Responses;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace MLProxy.Tests.Controllers
{
    [ExcludeFromCodeCoverage]
    internal class MetricsControllerTests
    {
        private Mock<IMetricsHandler> _metricsHandler;
        private MetricsController _metricsController;

        [SetUp]
        public void SetUp()
        {
            _metricsHandler = new Mock<IMetricsHandler>();
            _metricsController = new MetricsController(_metricsHandler.Object);
        }

        [Test]
        public async Task Should_Return_Metrics()
        {
            var expectedResponse = new List<GetMetricsResponse>
            {
                new GetMetricsResponse("/api/test", 2),
                new GetMetricsResponse("/api/test1", 5)
            };

            _metricsHandler
                .Setup(it => it.GetMetrics())
                .ReturnsAsync(expectedResponse);

            var response = await _metricsController.GetMetrics();

            Assert.Multiple(() =>
            {
                var objectResult = (OkObjectResult)response;
                var content = objectResult.Value as List<GetMetricsResponse>;

                Assert.IsNotNull(content);
                Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
                CollectionAssert.AreEqual(expectedResponse, content);
            });
        }
    }
}
