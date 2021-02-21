using System.Threading.Tasks;

namespace MLProxy.Interfaces.Services
{
    public interface IRequestsControlService
    {
        Task<bool> CanMakeRequest(string ip, string path);
    }
}
