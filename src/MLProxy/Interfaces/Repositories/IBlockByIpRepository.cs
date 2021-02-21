using MLProxy.Entities;
using System;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Repositories
{
    public interface IBlockByIpRepository
    {
        Task<BlockByIp> GetBy(string ip, DateTime createDate);
    }
}
