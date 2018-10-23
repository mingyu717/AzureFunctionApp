using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CDKAutolineServiceTest
    {
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<ICdkCustomerService> _cdkCustomerServiceMock;
        private CDKAutolineServices _unitTest;
        private const string RegisterResponse = "Customer test@email.com registered successfully";

        [SetUp]
        public void SetUp()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cdkCustomerServiceMock = new Mock<ICdkCustomerService>();
            _cdkCustomerServiceMock.Setup(m => m.SaveCdkCustomer(It.IsAny<CdkCustomer>())).Returns(Task.FromResult(10));
            _cdkCustomerServiceMock.Setup(m => m.MapCdkCustomer(It.IsAny<CustomerVehicleRegisterRequest>())).Returns(TestResources.CdkCustomer);
            _cdkCustomerServiceMock.Setup(m => m.GetCdkCustomer(TestResources.CustomerVerifyRequest.CommunityId, TestResources.CustomerVerifyRequest.CustomerNo))
                .Returns(TestResources.CdkCustomer);
            _unitTest = new CDKAutolineServices(_customerServiceMock.Object, _tokenServiceMock.Object, _cdkCustomerServiceMock.Object, null);
        }

        [Test]
        public void CDKAutolineService_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CDKAutolineServices(null, _tokenServiceMock.Object, _cdkCustomerServiceMock.Object, null));
            Assert.Throws<ArgumentNullException>(() =>
                new CDKAutolineServices(_customerServiceMock.Object, null, _cdkCustomerServiceMock.Object, null));
            Assert.Throws<ArgumentNullException>(() =>
                new CDKAutolineServices(_customerServiceMock.Object, _tokenServiceMock.Object, null, null));
        }

        [Test]
        public async Task RegisterCustomer_RegisterSuccess_Test()
        {
            _tokenServiceMock.Setup(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId))
                .Returns(Task.FromResult(TestResources.TestAppToken));
            _customerServiceMock.Setup(mock => mock.RegisterCustomer(It.IsAny<CustomerVehicleRegisterRequest>(), It.IsAny<CdkCustomer>()))
                .Returns(Task.FromResult(new ApiResponse { Success = true, Result = TestResources.CustomerResponse }));

            var registerResponse = await _unitTest.RegisterCustomer(TestResources.CustomerVehicleRegisterModel);

            _tokenServiceMock.Verify(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId), Times.Once);
            _customerServiceMock.Verify(mock => mock.RegisterCustomer(It.IsAny<CustomerVehicleRegisterRequest>(), It.IsAny<CdkCustomer>()), Times.Once);
            _cdkCustomerServiceMock.Verify(mock => mock.MapCdkCustomer(It.IsAny<CustomerVehicleRegisterRequest>()), Times.Once);
            _cdkCustomerServiceMock.Verify(mock => mock.SaveCdkCustomer(It.IsAny<CdkCustomer>()), Times.Once);
            Assert.AreEqual(RegisterResponse, registerResponse);
        }

        [Test]
        public void RegisterCustomer_CDKRegisterApiError_Test()
        {
            _tokenServiceMock.Setup(mock => mock.GetAppToken(TestResources.CustomerVehicleRegisterModel.CommunityId, TestResources.CustomerVehicleRegisterModel.RoofTopId))
                .Returns(Task.FromResult(TestResources.TestAppToken));
            _customerServiceMock.Setup(mock => mock.RegisterCustomer(It.IsAny<CustomerVehicleRegisterRequest>(), It.IsAny<CdkCustomer>()))
                .Returns(Task.FromResult(new ApiResponse { Success = false }));

            Assert.ThrowsAsync<CDKAutolineException>(() => _unitTest.RegisterCustomer(TestResources.CustomerVehicleRegisterModel));
        }

        [Test]
        public async Task VerifyCustomer_Test()
        {
            _tokenServiceMock.Setup(mock => mock.GetCustomerToken(It.IsAny<CdkCustomer>(), TestResources.CustomerVerifyRequest.RoofTopId))
                .Returns(Task.FromResult(TestResources.TokenResponse.Token));
            var verifyCustomerResponse = await _unitTest.VerifyCustomer(TestResources.CustomerVerifyRequest);
            Assert.AreEqual(TestResources.TokenResponse.Token, verifyCustomerResponse.CDKAutolineToken);
            Assert.AreEqual(TestResources.CdkCustomer.CustomerLoginId, verifyCustomerResponse.CustomerLoginId);
            _cdkCustomerServiceMock.Verify(mock => mock.GetCdkCustomer(
                TestResources.CustomerVerifyRequest.CommunityId, TestResources.CustomerVerifyRequest.CustomerNo), Times.Once);
        }

        [Test]
        public void VerifyCustomer_CustomerNotRegisterException_Test()
        {
            _cdkCustomerServiceMock.Setup(m => m.GetCdkCustomer(TestResources.CustomerVerifyRequest.CommunityId, TestResources.CustomerVerifyRequest.CustomerNo))
                .Returns((CdkCustomer)null);

            Assert.ThrowsAsync<CustomerNotRegisterException>(() => _unitTest.VerifyCustomer(TestResources.CustomerVerifyRequest));
        }

        [Test]
        public async Task VerifyCustomer_GetCustomerToken_Null_Test()
        {
            _tokenServiceMock.Setup(mock => mock.GetCustomerToken(It.IsAny<CdkCustomer>(), TestResources.CustomerVerifyRequest.RoofTopId))
                .Returns(Task.FromResult((string)null));
            var response = await _unitTest.VerifyCustomer(TestResources.CustomerVerifyRequest);
            Assert.IsNull(response.CDKAutolineToken);
            _cdkCustomerServiceMock.Verify(mock => mock.GetCdkCustomer(
                TestResources.CustomerVerifyRequest.CommunityId, TestResources.CustomerVerifyRequest.CustomerNo), Times.Once);
        }
    }
}