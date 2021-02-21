using MLProxy.Entities;
using System;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Repositories
{
    public interface IBlockByIpAndPathRepository
    {
        Task<BlockByIpAndPath> GetBy(string ip, string path, DateTime createDate);
    }
}
