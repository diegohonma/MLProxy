using MLProxy.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Handlers
{
    public interface IMetricsHandler
    {
        Task<List<GetMetricsResponse>> GetMetrics();
    }
}
