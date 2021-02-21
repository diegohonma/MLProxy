using Microsoft.AspNetCore.Http;
using MLProxy.Interfaces.Handlers;
using System.Threading.Tasks;

namespace MLProxy.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IProxyHandler _proxyHandler;

        public ProxyMiddleware(RequestDelegate next, IProxyHandler proxyHandler)
        {
            _next = next;
            _proxyHandler = proxyHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            switch (context.Request.Path.Value)
            {
                case "/api/metrics":
                    await _next(context);
                    break;
                default:
                    await _proxyHandler.Handler(context);
                    break;
            }
        }
    }
}
