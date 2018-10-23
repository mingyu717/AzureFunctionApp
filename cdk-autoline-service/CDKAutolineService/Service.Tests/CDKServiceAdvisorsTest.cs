using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using Service.Implementation;
using System;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestFixture]
    public class CDKServiceAdvisorsTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<ICdkCustomerDAL> _cdkCustomerDALMock;
        private CDKServiceAdvisors _underTest;


        [SetUp]
        public void Setup()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cdkCustomerDALMock = new Mock<ICdkCustomerDAL>();
            _underTest = new CDKServiceAdvisors(_restApiClientMock.Object, _tokenServiceMock.Object, _cdkCustomerDALMock.Object,
                null, TestResources.CdkAutolineUrl);
            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(TestResources.CdkCustomer);
            _tokenServiceMock.Setup(mock => mock.GetCustomerToken(It.IsAny<CdkCustomer>(), It.IsAny<string>()))
                .Returns(Task.FromResult(TestResources.TokenResponse.Token));
            _restApiClientMock.Setup(mock => mock.Invoke<GetServiceAdvisorsResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.GetServiceAdvisorsResponse }));
        }

        [Test]
        public void CDKServiceAdvisors_Constructor_Exception_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CDKServiceAdvisors(null,
                _tokenServiceMock.Object, _cdkCustomerDALMock.Object, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKServiceAdvisors(_restApiClientMock.Object,
                null, _cdkCustomerDALMock.Object, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKServiceAdvisors(_restApiClientMock.Object,
                _tokenServiceMock.Object, null, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKServiceAdvisors(_restApiClientMock.Object,
                _tokenServiceMock.Object, _cdkCustomerDALMock.Object, null, null));
        }

        [Test]
        public void GetServiceAdvisors_InvalidCustomer_Exception_Test()
        {
            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
                .Returns((CdkCustomer)null);
            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.GetServiceAdvisors(TestResources.GetServiceAdvisorsRequest));
        }

        [Test]
        public void GetServiceAdviors_NullRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _underTest.GetServiceAdvisors(It.IsAny<GetServiceAdvisorsRequest>()));
        }

        [Test]
        public void GetServiceAdviors_NullResponse_Exception()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<GetServiceAdvisorsResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = false, Result = null }));
            Assert.ThrowsAsync<CDKAutolineException>(() =>
                _underTest.GetServiceAdvisors(TestResources.GetServiceAdvisorsRequest));
        }

        [Test]
        public async Task GetServiceAdviors_Test()
        {
            var getServiceAdvisorsResponse = await _underTest.GetServiceAdvisors(TestResources.GetServiceAdvisorsRequest);

            Assert.AreEqual(TestResources.GetServiceAdvisorsResponse.PreferredSA,
                getServiceAdvisorsResponse.PreferredSA);
            Assert.AreEqual(TestResources.GetServiceAdvisorsResponse.PreferredSAAvailToday, getServiceAdvisorsResponse.PreferredSAAvailToday);
            Assert.AreEqual(TestResources.GetServiceAdvisorsResponse.PreferredSAName, getServiceAdvisorsResponse.PreferredSAName);
            Assert.AreEqual(TestResources.GetServiceAdvisorsResponse.Results.AdvisorData.Count, getServiceAdvisorsResponse.Results.AdvisorData.Count);

            _cdkCustomerDALMock.Verify(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()));
            _restApiClientMock.Verify(mock => mock.Invoke<GetServiceAdvisorsResponse>(It.IsAny<ApiRequest>()));
        }        
    }
}
