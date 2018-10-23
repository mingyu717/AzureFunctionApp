using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Models;
using Service.Implemention;

namespace Service.Tests
{
    [TestFixture]
    public class DealerServiceTests
    {
        private DealerService _underTest;
        private Mock<IRestfulClient> _restfulClientMock;
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _restfulClientMock = _fixture.Freeze<Mock<IRestfulClient>>();
            _underTest = _fixture.Create<DealerService>();
        }

        [Test]
        public void Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new DealerService(null));
        }

        [Test]
        public void GetDealerConfiguration_dealerIdSmallerThan0_ShouldThrowException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _underTest.GetDealerConfiguration(-1));
        }

        [Test, AutoData]
        public async Task GetDealerConfiguration_ValidRequest(int dealerId)
        {
            var result = await _underTest.GetDealerConfiguration(dealerId);

            Assert.IsNotNull(result);
            _restfulClientMock.Verify(m=>m.GetAsync<DealerConfigurationResponse>($"dealers/{dealerId}"), Times.Once);
        }
    }
}