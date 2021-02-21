using MLProxy.Entities;
using System;
using System.Threading.Tasks;

namespace MLProxy.Interfaces.Repositories
{
    public interface IBlockByPathRepository
    {
        Task<BlockByPath> GetBy(string path, DateTime createDate);
    }
}
