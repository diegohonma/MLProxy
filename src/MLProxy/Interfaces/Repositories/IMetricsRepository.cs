using MLProxy.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Repositories
{
    public interface IMetricsRepository
    {
        Task<List<Metric>> GetAllMetrics();

        Task Insert(Metric metric);
    }
}
