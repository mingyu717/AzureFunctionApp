using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Models;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture()]
    public class CdkCustomerServiceTest
    {
        private CdkCustomerService _underTest;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<ICdkCustomerDAL> _cdkCustomerDalMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private const string Password = "password";
        private const string EncryptedPassword = "encryptedPassword";
        private const string DecryptedPassword = "decryptedPassword";

        [SetUp]
        public void SetUp()
        {
            _passwordServiceMock = new Mock<IPasswordService>();
            _cdkCustomerDalMock = new Mock<ICdkCustomerDAL>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _passwordServiceMock.Setup(m => m.GeneratePassword()).Returns(Password);
            _underTest = new CdkCustomerService(_passwordServiceMock.Object, _cdkCustomerDalMock.Object, _encryptionServiceMock.Object);
            _cdkCustomerDalMock.Setup(m => m.UpdateCustomerToken(It.IsAny<int>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
            _cdkCustomerDalMock.Setup(m => m.AddCustomer(It.IsAny<CdkCustomer>())).Returns(Task.FromResult(11));
            _encryptionServiceMock.Setup(m => m.EncryptString(It.IsAny<string>())).Returns(EncryptedPassword);
            _encryptionServiceMock.Setup(m => m.DecryptString(It.IsAny<string>())).Returns(DecryptedPassword);
        }

        [Test]
        public void CDKAutolineService_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CdkCustomerService(null, _cdkCustomerDalMock.Object, _encryptionServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() =>
                new CdkCustomerService(_passwordServiceMock.Object, null, _encryptionServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() =>
                new CdkCustomerService(_passwordServiceMock.Object, _cdkCustomerDalMock.Object, null));
        }

        [Test]
        public void MapCdkCustomer_ArgumentNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => _underTest.MapCdkCustomer(null));
        }

        [Test]
        public void MapCdkCustomerl_Test()
        {
            var cdkCustomer = _underTest.MapCdkCustomer(TestResources.CustomerVehicleRegisterModel);
            var customerVehicleModel = TestResources.CustomerVehicleRegisterModel;
            var customerLoginId = $"{ customerVehicleModel.CommunityId }{customerVehicleModel.RoofTopId}{customerVehicleModel.CustomerNo}";
            Assert.AreEqual(customerVehicleModel.CommunityId, cdkCustomer.CommunityId);
            Assert.AreEqual(customerVehicleModel.CustomerNo, cdkCustomer.CustomerNo);
            Assert.AreEqual(customerLoginId, cdkCustomer.CustomerLoginId);
            Assert.AreEqual(Password, cdkCustomer.Password);
            Assert.IsNull(cdkCustomer.Token);
        }

        [Test]
        public void MapCdkCustomerl_NoEmailUsePhoneAsCustomerLoginId_Test()
        {
            var customerVehicleRegisterRequest = new CustomerVehicleRegisterRequest
            {
                CommunityId = "TestCommunity",
                CustomerNo = 11,
                EmailAddress = "",
                RoofTopId = "TestRoofTop"
            };
            var customerLoginId = "TestCommunityTestRoofTop11";
            var cdkCustomer = _underTest.MapCdkCustomer(customerVehicleRegisterRequest);

            Assert.AreEqual(customerVehicleRegisterRequest.CommunityId, cdkCustomer.CommunityId);
            Assert.AreEqual(customerVehicleRegisterRequest.CustomerNo, cdkCustomer.CustomerNo);
            Assert.AreEqual(customerLoginId, cdkCustomer.CustomerLoginId);
            Assert.AreEqual(Password, cdkCustomer.Password);
            Assert.IsNull(cdkCustomer.Token);
        }

        [Test]
        public void SaveCdkCustomer_ArgumentNull_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.SaveCdkCustomer(null));
        }

        [Test]
        public async Task SaveCdkCustomer_NoExisting_Test()
        {
            _cdkCustomerDalMock.Setup(m =>
                m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo)).Returns((CdkCustomer)null);
            await _underTest.SaveCdkCustomer(TestResources.CdkCustomer);
            _cdkCustomerDalMock.Verify(m => m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo), Times.Once);
            _cdkCustomerDalMock.Verify(m => m.UpdateCustomerToken(It.IsAny<int>(), It.IsAny<Guid>()), Times.Never);
            _cdkCustomerDalMock.Verify(m => m.AddCustomer(It.IsAny<CdkCustomer>()), Times.Once);
            _encryptionServiceMock.Verify(m => m.EncryptString(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task SaveCdkCustomer_HasExisting_Test()
        {
            _cdkCustomerDalMock.Setup(m =>
                m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo)).Returns(TestResources.CdkCustomer);
            await _underTest.SaveCdkCustomer(TestResources.CdkCustomer);
            _cdkCustomerDalMock.Verify(m => m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo), Times.Once);
            _cdkCustomerDalMock.Verify(m => m.UpdateCustomerToken(It.IsAny<int>(), It.IsAny<Guid>()), Times.Once);
            _cdkCustomerDalMock.Verify(m => m.AddCustomer(It.IsAny<CdkCustomer>()), Times.Never);
            _encryptionServiceMock.Verify(m => m.EncryptString(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetCdkCustomer_ArgumentNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => _underTest.GetCdkCustomer(null, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _underTest.GetCdkCustomer("communityId", -1));
        }

        [Test]
        public void GetCdkCustomer_HasExisting_Test()
        {
            _cdkCustomerDalMock.Setup(m =>
                m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo)).Returns(TestResources.CdkCustomer);
            var cdkCustomer = _underTest.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo);
            _cdkCustomerDalMock.Verify(m => m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo), Times.Once);
            Assert.AreEqual(TestResources.CdkCustomer.Password, cdkCustomer.Password);
        }

        [Test]
        public void GetCdkCustomer_NoExisting_Test()
        {
            _cdkCustomerDalMock.Setup(m =>
                m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo)).Returns((CdkCustomer)null);
            var cdkCustomer = _underTest.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo);
            _cdkCustomerDalMock.Verify(m => m.GetCdkCustomer(TestResources.CdkCustomer.CommunityId, TestResources.CdkCustomer.CustomerNo), Times.Once);
            Assert.IsNull(cdkCustomer);
        }
    }
}