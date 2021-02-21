using Microsoft.AspNetCore.Http;
using MLProxy.Interfaces.Handlers;
using MLProxy.Middleware;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MLProxy.Tests.Middleware
{
    [ExcludeFromCodeCoverage]
    internal class ProxyMiddlewareTests
    {
        private Mock<IProxyHandler> _proxyHandler;
        private ProxyMiddleware _proxyMiddleware;

        [SetUp]
        public void SetUp()
        {
            _proxyHandler = new Mock<IProxyHandler>();
            _proxyMiddleware = new ProxyMiddleware(ctx => Task.CompletedTask, _proxyHandler.Object);
        }

        [Test]
        public async Task Should_Not_Call_ProxyHandler_When_Metrics()
        {
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Path = new PathString("/api/metrics");

            await _proxyMiddleware.InvokeAsync(defaultHttpContext);

            _proxyHandler
                .Verify(it => it.Handler(It.IsAny<HttpContext>()), Times.Never);
        }

        [Test]
        public async Task Should_Call_ProxyHandler_When_Metrics()
        {
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Path = new PathString("/api/test");

            await _proxyMiddleware.InvokeAsync(defaultHttpContext);

            _proxyHandler
                .Verify(it => it.Handler(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
