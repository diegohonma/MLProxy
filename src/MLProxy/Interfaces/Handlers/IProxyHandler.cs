using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Handlers
{
    public interface IProxyHandler
    {
        Task Handler(HttpContext context);
    }
}
