using System;
using NUnit.Framework;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class RijndaelEncryptionServiceTest
    {
        private RijndaelEncryptionService _rijndaelEncryptionService;
        private const string SecretKey = "f0dd41dd-bbc9-4aac-b153-1e8795382c66";
        private const string ClearText = "@KH#fjPF";
        private const string CipherText = "nTy9iVIp1e+TUecKBbEDwDcnlPvTAOPRc0SgqtDuM5g=";

        [SetUp]
        public void SetUp()
        {
            _rijndaelEncryptionService = new RijndaelEncryptionService(SecretKey);
        }

        [Test]
        public void Test_Constructor()
        {
            Assert.Throws<ArgumentNullException>(() => new RijndaelEncryptionService(null));
        }

        [TestCase("")]
        [TestCase(null)]
        public void EncryptString_EmptyInput(string clearText)
        {
            Assert.Throws<ArgumentNullException>(() => _rijndaelEncryptionService.EncryptString(clearText));
        }

        [Test]
        public void EncryptString_ValidTextInput()
        {
            var result = _rijndaelEncryptionService.EncryptString(ClearText);

            Assert.AreEqual(CipherText, result);
        }

        [TestCase("")]
        [TestCase(null)]
        public void DecryptString_EmptyInput(string clearText)
        {
            Assert.Throws<ArgumentNullException>(() => _rijndaelEncryptionService.DecryptString(clearText));
        }

        [Test]
        public void DecryptPassword_Tests()
        {
            var result = _rijndaelEncryptionService.DecryptString(CipherText);

            Assert.AreEqual(ClearText, result);
        }
    }
}