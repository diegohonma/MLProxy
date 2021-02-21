using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLProxy.Entities;
using MLProxy.Handlers;
using MLProxy.Interfaces.Repositories;
using MLProxy.Interfaces.Services;
using MLProxy.Tests.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLProxy.Tests.Handlers
{
    [ExcludeFromCodeCoverage]
    internal class ProxyHandlerTests
    {
        private DefaultHttpContext _context;
        private Mock<IRequestsControlService> _requestsControlService;
        private Mock<IMetricsRepository> _metricsRepository;
        private Mock<ILogger<ProxyHandler>> _logger;

        [SetUp]
        public void SetUp()
        {
            _context = new DefaultHttpContext();
            _context.Response.Body = new MemoryStream();
            _context.Request.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            _context.Request.Path = new PathString("/api/test");

            _metricsRepository = new Mock<IMetricsRepository>();
            _metricsRepository
                .Setup(it => it.Insert(It.IsAny<Metric>()));

            _requestsControlService = new Mock<IRequestsControlService>();
            _logger = new Mock<ILogger<ProxyHandler>>();
        }

        [TestCase("GET")]
        [TestCase("POST")]
        public async Task Should_Send_And_Receive_Response(string method)
        {
            var expectedStatusCode = HttpStatusCode.OK;
            var expectedResponse = @"{ ""test"": 'test'}";

            _context.Request.Method = method;

            _requestsControlService
                .Setup(it => it.CanMakeRequest(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var httpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };
            httpMessageHandler
                .Setup(f => f.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = expectedStatusCode,
                    Content = new StringContent(expectedResponse)
                });

            var proxyHandler = new ProxyHandler(
                new HttpClient(httpMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://localhost")
                }, _requestsControlService.Object, _metricsRepository.Object, _logger.Object);

            await proxyHandler.Handler(_context);

            _context.Response.Body.Seek(0, SeekOrigin.Begin);

            Assert.Multiple(async () =>
            {
                Assert.AreEqual((int)expectedStatusCode, _context.Response.StatusCode);
                Assert.AreEqual(expectedResponse, await new StreamReader(_context.Response.Body).ReadToEndAsync());

                _metricsRepository
                    .Verify(it => it.Insert(It.IsAny<Metric>()), Times.Once);
            });
        }

        [Test]
        public async Task Should_Not_Send_When_NotAllowedToMakeRequest()
        {
            _requestsControlService
                .Setup(it => it.CanMakeRequest(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var httpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };

            var proxyHandler = new ProxyHandler(
                new HttpClient(httpMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://localhost")
                }, _requestsControlService.Object, _metricsRepository.Object, _logger.Object);

            await proxyHandler.Handler(_context);

            Assert.Multiple(() =>
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, _context.Response.StatusCode);

                httpMessageHandler
                    .Verify(it => it.Send(It.IsAny<HttpRequestMessage>()), Times.Never);

                _metricsRepository
                    .Verify(it => it.Insert(It.IsAny<Metric>()), Times.Once);
            });
        }

        [Test]
        public void LogException_When_ErrorOccurs()
        {
            _requestsControlService
               .Setup(it => it.CanMakeRequest(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var httpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };

            var proxyHandler = new ProxyHandler(
                new HttpClient(httpMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://localhost")
                }, _requestsControlService.Object, _metricsRepository.Object, _logger.Object);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync<Exception>(
                    async () => await proxyHandler.Handler(_context));

                _logger
                    .Verify(l => l.Log(LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<object>(x => x.ToString().Equals("Erro ao processar request")),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()), Times.Once);
            });
        }
    }
}
