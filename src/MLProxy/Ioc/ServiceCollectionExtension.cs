using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MLProxy.Handlers;
using MLProxy.Interfaces.Handlers;
using MLProxy.Interfaces.Services;
using MLProxy.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MLProxy.Ioc
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterHandlers(services, configuration);
            RegisterServices(services, configuration);

            return services;
        }

        private static void RegisterHandlers(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IProxyHandler, ProxyHandler>(
                client => client.BaseAddress = new Uri(configuration["Urls:ProxyUrl"]));

            services.AddScoped<IMetricsHandler, MetricsHandler>();
        }

        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRequestsControlService, RequestsControlService>();
        }
    }
}
