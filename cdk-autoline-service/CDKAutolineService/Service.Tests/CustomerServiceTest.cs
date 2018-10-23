using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Response;
using Service.Implementation;
using System;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestFixture]
    public class CustomerServiceTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<IEncryptedTokenCodeService> _encryptedTokenCodeServiceMock;
        private CustomerService _unitTest;

        [SetUp]
        public void Setup()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _encryptedTokenCodeServiceMock = new Mock<IEncryptedTokenCodeService>();
            _unitTest = new CustomerService(_restApiClientMock.Object, TestResources.CdkAutolineUrl,
                _encryptedTokenCodeServiceMock.Object);
        }

        [Test]
        public void CustomerService_Constructor_Null_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerService(null, TestResources.CdkAutolineUrl, _encryptedTokenCodeServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerService(_restApiClientMock.Object, null, _encryptedTokenCodeServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerService(_restApiClientMock.Object, TestResources.CdkAutolineUrl, null));
        }

        [Test]
        public void CheckPassword_ArgumentNullTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.CheckPassword(null, TestResources.CdkCustomer, TestResources.DealerCDKConfiguration.PartnerKey));
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.CheckPassword(TestResources.CustomerVerifyRequest, null, TestResources.DealerCDKConfiguration.PartnerKey));
        }

        [Test]
        public async Task CheckPassword_Success_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() {Success = true}));
            var checkPasswordResponse = await _unitTest.CheckPassword(TestResources.CustomerVerifyRequest, TestResources.CdkCustomer, TestResources.DealerCDKConfiguration.PartnerKey);
            Assert.IsTrue(checkPasswordResponse.Success);
        }

        [Test]
        public async Task CheckPassword_Failure_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() {Success = false}));
            var checkPasswordResponse = await _unitTest.CheckPassword(TestResources.CustomerVerifyRequest, TestResources.CdkCustomer, TestResources.DealerCDKConfiguration.PartnerKey);
            Assert.IsFalse(checkPasswordResponse.Success);
        }

        [Test]
        public void RegisterCustomer_ArgumentNullTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.RegisterCustomer(null, TestResources.CdkCustomer));
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.RegisterCustomer(TestResources.CustomerVehicleRegisterModel, null));
        }

        [Test]
        public async Task RegisterCustomer_Success_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() {Success = true}));
            var registerCustomerResponse = await _unitTest.RegisterCustomer(TestResources.CustomerVehicleRegisterModel, TestResources.CdkCustomer);
            Assert.IsTrue(registerCustomerResponse.Success);
        }

        [Test]
        public async Task RegisterCustomer_Failure_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() {Success = false}));
            var registerCustomerResponse = await _unitTest.RegisterCustomer(TestResources.CustomerVehicleRegisterModel, TestResources.CdkCustomer);
            Assert.IsFalse(registerCustomerResponse.Success);
        }
    }
}