using Microsoft.AspNetCore.Mvc;
using MLProxy.Interfaces.Handlers;
using System.Threading.Tasks;

namespace MLProxy.Controllers
{
    [Route("api")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsHandler _metricsHandler;

        public MetricsController(IMetricsHandler metricsHandler)
        {
            _metricsHandler = metricsHandler;
        }

        [HttpGet]
        [Route("metrics")]
        public async Task <IActionResult> GetMetrics()
        {
            return Ok(await _metricsHandler.GetMetrics());
        }
    }
}
