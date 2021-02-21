using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLProxy.Entities;
using MLProxy.Interfaces.Handlers;
using MLProxy.Interfaces.Repositories;
using MLProxy.Interfaces.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy.Handlers
{
    public class ProxyHandler : IProxyHandler
    {
        private readonly HttpClient _httpClient;
        private readonly IRequestsControlService _requestsControlService;
        private readonly IMetricsRepository _metricsRepository;
        private readonly ILogger<ProxyHandler> _logger;

        public ProxyHandler(
            HttpClient httpClient, IRequestsControlService requestsControlService,
            IMetricsRepository metricsRepository, ILogger<ProxyHandler> logger)
        {
            _httpClient = httpClient;
            _requestsControlService = requestsControlService;
            _metricsRepository = metricsRepository;
            _logger = logger;
        }

        public async Task Handler(HttpContext context)
        {
            try
            {
                var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                var path = context.Request.Path.Value;

                InsertMetric(ip, path);

                if (!await _requestsControlService.CanMakeRequest(ip, path))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                switch (context.Request.Method)
                {
                    case "GET":
                        await ProcessGet(context);
                        break;
                    case "POST":
                        await ProcessPost(context);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao processar request", ex);
                throw;
            }
        }

        private async Task ProcessGet(HttpContext context)
        {
            var response = await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress, context.Request.Path));
            await SetResponse(context, response);
        }

        private async Task ProcessPost(HttpContext context)
        {
            using (var sr = new StreamReader(context.Request.Body))
            {
                var response = await _httpClient.PostAsync(
                    new Uri(_httpClient.BaseAddress, context.Request.Path),
                    new StringContent(await sr.ReadToEndAsync(), Encoding.UTF8, "application/json"));

                await SetResponse(context, response);
            }
        }

        private void InsertMetric(string ip, string path) =>
            Task.Run(() => _metricsRepository.Insert(new Metric(ip, path, DateTime.Now)));

        private static async Task SetResponse(HttpContext context, HttpResponseMessage response)
        {
            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
        }
    }
}
