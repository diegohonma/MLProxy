using Microsoft.Extensions.Logging;
using MLProxy.Interfaces.Handlers;
using MLProxy.Interfaces.Repositories;
using MLProxy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLProxy.Handlers
{
    public class MetricsHandler : IMetricsHandler
    {
        private readonly IMetricsRepository _metricsRepository;
        private readonly ILogger<MetricsHandler> _logger;

        public MetricsHandler(IMetricsRepository metricsRepository, ILogger<MetricsHandler> logger)
        {
            _metricsRepository = metricsRepository;
            _logger = logger;
        }

        public async Task<List<GetMetricsResponse>> GetMetrics()
        {
            try
            {
                var metrics = await _metricsRepository.GetAllMetrics();

                return metrics
                    .GroupBy(it => it.Path)
                    .Select(it => new GetMetricsResponse(it.Key, it.Count())).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao buscar métricas", ex);
                throw;
            }
        }
    }
}
