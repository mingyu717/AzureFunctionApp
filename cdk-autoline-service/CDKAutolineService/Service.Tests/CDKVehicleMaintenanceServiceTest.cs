using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CDKVehicleMaintenanceServiceTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<ITokenService> _tokenService;
        private CDKVehicleMaintenanceService _underTest;

        [SetUp]
        public void SetUp()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _tokenService = new Mock<ITokenService>();
            _underTest = new CDKVehicleMaintenanceService(_restApiClientMock.Object, TestResources.CdkAutolineUrl, _tokenService.Object, null);
        }

        [Test]
        public void CDKVehicleMaintenanceService_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CDKVehicleMaintenanceService(null, TestResources.CdkAutolineUrl, _tokenService.Object, null));
            Assert.Throws<ArgumentNullException>(() => new CDKVehicleMaintenanceService(_restApiClientMock.Object, null, _tokenService.Object, null));
            Assert.Throws<ArgumentNullException>(() => new CDKVehicleMaintenanceService(_restApiClientMock.Object, TestResources.CdkAutolineUrl, null, null));
        }

        [Test]
        public void GetRecommendedServices_ArgumentNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.GetRecommendedServices(null));
        }

        [Test]
        public async Task GetRecommendedServices_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CDKRecommendedServiceResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse {Success = true, Result = TestResources.CdkRecommendedServiceResponse}));

            var result = await _underTest.GetRecommendedServices(TestResources.GetRecommendedServicesRequest);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PriceListData);
            Assert.AreEqual(1, result.PriceListData.Count());
        }

        [Test]
        public void GetRecommendedServices_CDKErrors_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CDKRecommendedServiceResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse {Success = false, Result = TestResources.CdkRecommendedServiceResponse}));

            Assert.ThrowsAsync<CDKAutolineException>(() => _underTest.GetRecommendedServices(TestResources.GetRecommendedServicesRequest));
        }
    }
}
