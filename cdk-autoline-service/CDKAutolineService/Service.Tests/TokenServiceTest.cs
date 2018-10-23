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
using TokenResponse = Service.Contract.Response.TokenResponse;

namespace Service.Tests
{
    [TestFixture]
    public class TokenServiceTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<IEncryptedTokenCodeService> _encryptedTokenCodeServiceMock;
        private Mock<IAppTokenDAL> _appTokenDalMock;
        private Mock<ICdkCustomerService> _cdkCustomerServiceMock;
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<IDealerCDKConfigurationsDAL> _dealerCDKConfigurationDAL;
        private readonly string UnregisteredGuid = "6dde59ee-fe5c-4b37-885e-69440d1eeec4";

        private TokenService _unitTest;

        [SetUp]
        public void SetUp()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _appTokenDalMock = new Mock<IAppTokenDAL>();
            _cdkCustomerServiceMock = new Mock<ICdkCustomerService>();
            _customerServiceMock = new Mock<ICustomerService>();
            _encryptedTokenCodeServiceMock = new Mock<IEncryptedTokenCodeService>();
            _dealerCDKConfigurationDAL = new Mock<IDealerCDKConfigurationsDAL>();
            _unitTest = new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object, TestResources.CdkAutolineUrl,
                UnregisteredGuid, _appTokenDalMock.Object, null,
                _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object);
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.TokenResponse }));
            _dealerCDKConfigurationDAL.Setup(mock => mock.GetDealerCDKConfigurations(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(TestResources.DealerCDKConfiguration);
            _customerServiceMock.Setup(mock => mock.CheckPassword(It.IsAny<CustomerVerifyRequest>(), It.IsAny<CdkCustomer>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true }));
        }

        [Test]
        public void TokenService_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new TokenService(null, _encryptedTokenCodeServiceMock.Object,
                TestResources.CdkAutolineUrl, UnregisteredGuid, _appTokenDalMock.Object, null,
                _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, null,
                TestResources.CdkAutolineUrl, UnregisteredGuid, _appTokenDalMock.Object, null,
                _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
                null, UnregisteredGuid, _appTokenDalMock.Object, null,
                 _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));           
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
                TestResources.CdkAutolineUrl, null, _appTokenDalMock.Object, null,
                 _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
                TestResources.CdkAutolineUrl, UnregisteredGuid, null, null,
                 _cdkCustomerServiceMock.Object, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
               TestResources.CdkAutolineUrl, UnregisteredGuid, _appTokenDalMock.Object, null,
                null, _customerServiceMock.Object, _dealerCDKConfigurationDAL.Object));
            Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
               TestResources.CdkAutolineUrl, UnregisteredGuid, _appTokenDalMock.Object, null,
                _cdkCustomerServiceMock.Object, null, _dealerCDKConfigurationDAL.Object));
             Assert.Throws<ArgumentNullException>(() => new TokenService(_restApiClientMock.Object, _encryptedTokenCodeServiceMock.Object,
               TestResources.CdkAutolineUrl, UnregisteredGuid, _appTokenDalMock.Object, null,
                _cdkCustomerServiceMock.Object, _customerServiceMock.Object, null));
        }

        [Test]
        public void GetAppToken_Null_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.GetAppToken(null, null));
        }

        [Test]
        public async Task GetAppToken_FromDb()
        {
            _appTokenDalMock.Setup(mock => mock.GetAppToken(It.IsAny<string>())).Returns(TestResources.AppToken);
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.CustomerResponse }));
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.TokenResponse }));
           
            var appToken = await _unitTest.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId);
            Assert.AreEqual(TestResources.AppToken.Token.ToString(), appToken);
            _appTokenDalMock.Verify(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()), Times.Never);
            _restApiClientMock.Verify(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()), Times.Once);
        }

        [Test]
        public async Task GetAppToken_NotFromDb()
        {
            _appTokenDalMock.Setup(mock => mock.GetAppToken(It.IsAny<string>())).Returns((AppToken)null);
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.CustomerResponse }));
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.TokenResponse }));

            var appToken = await _unitTest.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId);
            Assert.AreEqual(TestResources.TokenResponse.Token, appToken);
            _appTokenDalMock.Verify(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()), Times.Once);
        }

        [Test]
        public void GetAppToken_NotFromDb_RequestTokenError()
        {
            _appTokenDalMock.Setup(mock => mock.GetAppToken(It.IsAny<string>())).Returns((AppToken)null);
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.CustomerResponse }));
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = false, Result = TestResources.TokenResponse }));
 
            Assert.ThrowsAsync<CDKAutolineException>(() => _unitTest.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId));
            _appTokenDalMock.Verify(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()), Times.Once);
        }
        [Test]
        public void GetAppToken_NotFromDb_ActiveTokenError()
        {
            _appTokenDalMock.Setup(mock => mock.GetAppToken(It.IsAny<string>())).Returns((AppToken)null);
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = false, Result = TestResources.CustomerResponse }));
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.TokenResponse }));

            Assert.ThrowsAsync<CDKAutolineException>(() => _unitTest.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId));
            _appTokenDalMock.Verify(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()), Times.Once);
            _restApiClientMock.Verify(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()), Times.Once);
        }

        [Test]
        public async Task RequestAndActivateAppToken_FromDb_ActiveTokenError()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = false, Result = TestResources.CustomerResponse }));
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.TokenResponse }));

            var appToken = await _unitTest.RequestAndActivateAppToken(TestResources.DealerCDKConfiguration,
                TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.AppToken, true);

            _restApiClientMock.Verify(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()), Times.Never);
            _restApiClientMock.Verify(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()), Times.Once);
            Assert.IsNull(appToken);
        }

        [Test]
        public async Task ActivateToken_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CustomerResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.CustomerResponse }));
            var activateTokenResponse = await _unitTest.ActivateToken(TestResources.CustomerVehicleRegisterModel.CommunityId,
                TestResources.CustomerVehicleRegisterModel.AppToken, TestResources.DealerCDKConfiguration.PartnerKey);
            var customerResponse = activateTokenResponse.Result as CustomerResponse;
            Assert.True(activateTokenResponse.Success);
            Assert.IsNotNull(customerResponse);
            Assert.AreEqual(customerResponse.ErrorCode, TestResources.CustomerResponse.ErrorCode);
        }

        [Test]
        public async Task RequestToken_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.TokenResponse }));
            var requestTokenResponse = await _unitTest.RequestToken(TestResources.DealerCDKConfiguration, TestResources.CustomerVehicleRegisterModel.CommunityId);
            var tokenResponse = requestTokenResponse.Result as TokenResponse;
            var tokenErrorCode = tokenResponse.Result as ErrorResponse;
            Assert.IsTrue(requestTokenResponse.Success);
            Assert.IsNotNull(tokenResponse);
            Assert.IsNotNull(tokenErrorCode);
            Assert.AreEqual(tokenResponse.Token, TestResources.TokenResponse.Token);
            Assert.AreEqual(tokenErrorCode.ErrorCode, TestResources.TokenResponse.Result.ErrorCode);
        }

        [Test]
        public void RequestToken_NullArgument_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.RequestToken(null, null));
        }

        [Test]
        public void GetCustomerToken_Null_Exception_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.GetCustomerToken(null, TestResources.CustomerVerifyRequest.RoofTopId));
            Assert.ThrowsAsync<ArgumentNullException>(() => _unitTest.GetCustomerToken(TestResources.CdkCustomer, null));
        }

        [Test]
        public void GetCustomerToken_RequestToken_CheckPasswordError_ShouldThrowException()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<TokenResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = false }));
            _customerServiceMock.Setup(mock => mock.CheckPassword(It.IsAny<CustomerVerifyRequest>(), It.IsAny<CdkCustomer>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ApiResponse() {Success = false}));

            Assert.ThrowsAsync<CDKAutolineException>(() =>
                _unitTest.GetCustomerToken(TestResources.CdkCustomer, TestResources.CustomerVerifyRequest.RoofTopId));
        }

        [Test]
        public void GetCustomerToken_CheckPassword_Exception_Test()
        {
            _customerServiceMock.Setup(mock =>
                    mock.CheckPassword(It.IsAny<CustomerVerifyRequest>(), It.IsAny<CdkCustomer>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = false }));

            Assert.ThrowsAsync<CDKAutolineException>(() => _unitTest.GetCustomerToken(TestResources.CdkCustomer,
                TestResources.CustomerVerifyRequest.RoofTopId));
        }

        [Test]
        public async Task GetCustomerToken_GetTokenFromDb_Test()
        {
            await _unitTest.GetCustomerToken(TestResources.CdkCustomer, TestResources.CustomerVerifyRequest.RoofTopId);
            _customerServiceMock.Verify(mock => mock.CheckPassword(It.IsAny<CustomerVerifyRequest>(),It.IsAny<CdkCustomer>(), It.IsAny<string>()));
        }
    }
}
