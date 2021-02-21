using MLProxy.Entities;
using MLProxy.Interfaces.Repositories;
using MLProxy.Options;
using MLProxy.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MLOptions = Microsoft.Extensions.Options.Options;

namespace MLProxy.Tests.Services
{
    [ExcludeFromCodeCoverage]
    internal class RequestsControlServiceTests
    {
        private Mock<IBlockByIpRepository> _blockByIpRepository;
        private Mock<IBlockByPathRepository> _blockByPathRepository;
        private Mock<IBlockByIpAndPathRepository> _blockByIpAndPathRepository;
        private RequestsControlService _requestsControlService;
        private BlockOptions _blockOptions;

        [SetUp]
        public void SetUp()
        {
            _blockByIpRepository = new Mock<IBlockByIpRepository>();
            _blockByPathRepository = new Mock<IBlockByPathRepository>();
            _blockByIpAndPathRepository = new Mock<IBlockByIpAndPathRepository>();
            _blockOptions = new BlockOptions()
            {
                BlockByIpAttempts = 1,
                BlockByPathAttempts = 2,
                BlockByIpAndPathAttempts = 3
            };

            _requestsControlService = new RequestsControlService(
                _blockByIpRepository.Object, _blockByPathRepository.Object,
                _blockByIpAndPathRepository.Object, MLOptions.Create(_blockOptions));
        }

        [Test]
        public async Task Should_ReturnFalse_When_BlockedByIp()
        {
            _blockByIpRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIp(string.Empty, _blockOptions.BlockByIpAttempts + 1, DateTime.Now));

            var canMakeRequest = await _requestsControlService.CanMakeRequest("127.0.0.1", "/api/test");

            Assert.False(canMakeRequest);
        }

        [Test]
        public async Task Should_ReturnFalse_When_BlockedByPath()
        {
            _blockByIpRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIp(string.Empty, 0, DateTime.Now));

            _blockByPathRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByPath(string.Empty, _blockOptions.BlockByPathAttempts + 1, DateTime.Now));

            var canMakeRequest = await _requestsControlService.CanMakeRequest("127.0.0.1", "/api/test");

            Assert.False(canMakeRequest);
        }

        [Test]
        public async Task Should_ReturnFalse_When_BlockedByPathAndIp()
        {
            _blockByIpRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIp(string.Empty, 0, DateTime.Now));

            _blockByPathRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByPath(string.Empty, 0, DateTime.Now));

            _blockByIpAndPathRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIpAndPath(
                    string.Empty, string.Empty, _blockOptions.BlockByIpAndPathAttempts + 1, DateTime.Now)
                    );

            var canMakeRequest = await _requestsControlService.CanMakeRequest("127.0.0.1", "/api/test");

            Assert.False(canMakeRequest);
        }


        [Test]
        public async Task Should_ReturnTrue_When_NotBlocked()
        {
            _blockByIpRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIp(string.Empty, 0, DateTime.Now));

            _blockByPathRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByPath(string.Empty, 0, DateTime.Now));

            _blockByIpAndPathRepository
                .Setup(it => it.GetBy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new BlockByIpAndPath(string.Empty, string.Empty, 0, DateTime.Now));

            var canMakeRequest = await _requestsControlService.CanMakeRequest("127.0.0.1", "/api/test");

            Assert.True(canMakeRequest);
        }
    }
}
