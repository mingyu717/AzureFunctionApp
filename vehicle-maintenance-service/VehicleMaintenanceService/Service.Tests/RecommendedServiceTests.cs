using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Models;
using Service.Implemention;

namespace Service.Tests
{
    [TestFixture]
    public class RecommendedServiceTests
    {
        private RecommendedService _underTest;
        private Mock<IDealerService> _dealerServiceMock;
        private Mock<ICDKAutolineService> _cdkAutolineServiceMock;
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _dealerServiceMock = _fixture.Freeze<Mock<IDealerService>>();
            _cdkAutolineServiceMock = _fixture.Freeze<Mock<ICDKAutolineService>>();
            _underTest = _fixture.Create<RecommendedService>();
        }

        [Test]
        public void Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new RecommendedService(null, _cdkAutolineServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new RecommendedService(_dealerServiceMock.Object, null));
        }

        [Test]
        public void Get_ArgumentNullCheck()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.Get(null));
        }

        [Test]
        public async Task Get_DealerNotExist_ShouldReturnNull()
        {
            var request = _fixture.Create<GetRecommendedServicesRequest>();
            _dealerServiceMock.Setup(m => m.GetDealerConfiguration(request.DealerId)).Returns(Task.FromResult((DealerConfigurationResponse) null));

            var result = await _underTest.Get(request);
            Assert.IsNull(result);
            _dealerServiceMock.Verify(m => m.GetDealerConfiguration(request.DealerId), Times.Once);
            _cdkAutolineServiceMock.Verify(m => m.GetRecommendedServices(request, It.IsAny<DealerConfigurationResponse>()), Times.Never);
        }

        [Test]
        public async Task Get_DealerExist_ShouldGetRecommendedServics()
        {
            var getRecommendedServicesRequest = _fixture.Create<GetRecommendedServicesRequest>();
            var dealerConfigurationResponse = _fixture.Create<DealerConfigurationResponse>();
            var expected = _fixture.Create<GetRecommendedServicesResponse>();
            _dealerServiceMock.Setup(m => m.GetDealerConfiguration(getRecommendedServicesRequest.DealerId)).Returns(Task.FromResult(dealerConfigurationResponse));
            _cdkAutolineServiceMock.Setup(m => m.GetRecommendedServices(getRecommendedServicesRequest, dealerConfigurationResponse)).Returns(Task.FromResult(expected));

            var result = await _underTest.Get(getRecommendedServicesRequest);
            Assert.IsNotNull(result);
            Assert.AreSame(expected, result);
            _dealerServiceMock.Verify(m => m.GetDealerConfiguration(getRecommendedServicesRequest.DealerId), Times.Once);
            _cdkAutolineServiceMock.Verify(m => m.GetRecommendedServices(getRecommendedServicesRequest, dealerConfigurationResponse), Times.Once);
        }
    }
}
