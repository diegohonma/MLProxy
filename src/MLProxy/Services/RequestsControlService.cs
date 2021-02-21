using Microsoft.Extensions.Options;
using MLProxy.Interfaces.Repositories;
using MLProxy.Interfaces.Services;
using MLProxy.Options;
using System;
using System.Threading.Tasks;

namespace MLProxy.Services
{
    public class RequestsControlService : IRequestsControlService
    {
        private readonly IBlockByIpRepository _blockByIpRepository;
        private readonly IBlockByPathRepository _blockByPathRepository;
        private readonly IBlockByIpAndPathRepository _blockByIpAndPathRepository;
        private readonly BlockOptions _blockOptions;

        public RequestsControlService(
            IBlockByIpRepository blockByIpRepository, IBlockByPathRepository blockByPathRepository,
            IBlockByIpAndPathRepository blockByIpAndPathRepository, IOptions<BlockOptions> blockOptions)
        {
            _blockByIpRepository = blockByIpRepository;
            _blockByPathRepository = blockByPathRepository;
            _blockByIpAndPathRepository = blockByIpAndPathRepository;
            _blockOptions = blockOptions.Value;
        }

        public async Task<bool> CanMakeRequest(string ip, string path)
        {
            var blockByIp = await _blockByIpRepository.GetBy(ip, DateTime.Now);

            if (blockByIp.Attempts >= _blockOptions.BlockByIpAttempts)
                return false;

            var blockByPath = await _blockByPathRepository.GetBy(path, DateTime.Now);

            if (blockByPath.Attempts >= _blockOptions.BlockByPathAttempts)
                return false;

            var blockByIpAndPath = await _blockByIpAndPathRepository.GetBy(ip, path, DateTime.Now);

            if (blockByIpAndPath.Attempts >= _blockOptions.BlockByPathAttempts)
                return false;

            return true;
        }
    }
}
