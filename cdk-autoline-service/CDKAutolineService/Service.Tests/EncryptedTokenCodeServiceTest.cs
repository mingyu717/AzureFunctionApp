using NUnit.Framework;
using Service.Implementation;
using System;
using Moq;
using Service.Contract;

namespace Service.Tests
{
    [TestFixture]
    public class EncryptedTokenCodeServiceTest
    {
        private EncryptedTokenCodeService _unitTest;
        private string _unregisteredGuid = "6dde59ee-fe5c-4b37-885e-69440d1eeec4";
        private Mock<IEncryptionService> _encryptionServiceMock;

        [SetUp]
        public void Setup()
        {
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _unitTest = new EncryptedTokenCodeService(_unregisteredGuid, _encryptionServiceMock.Object);
        }

        [Test]
        public void EncrypedTokenCodeService_Constructor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new EncryptedTokenCodeService(null, _encryptionServiceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new EncryptedTokenCodeService(_unregisteredGuid, null));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GetEncryptedTokenCode_Test(bool addPassword)
        {
            Assert.IsInstanceOf(typeof(string), _unitTest.GetEncryptedTokenCode(
                TestResources.CdkCustomer.Token.ToString(), TestResources.CdkCustomer, TestResources.DealerCDKConfiguration.PartnerKey, addPassword));
        }
    }
}