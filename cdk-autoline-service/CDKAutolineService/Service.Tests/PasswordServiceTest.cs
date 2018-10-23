using System;
using NUnit.Framework;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class PasswordServiceTest
    {
        private int _passwordLength;
        private string _passwordCharacters;
        private PasswordService _unitTest;

        [SetUp]
        public void Setup()
        {
            _passwordLength = 8;
            _passwordCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()abcdefghijklmnopqrstuvwxyz";
            _unitTest = new PasswordService(_passwordLength, _passwordCharacters);
        }

        [Test]
        public void PasswordService_Constructor_Exception()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PasswordService(0, _passwordCharacters));
            Assert.Throws<ArgumentNullException>(() => new PasswordService(_passwordLength, null));
        }

        [Test]
        public void GeneratePassword_Test()
        {
            var password = _unitTest.GeneratePassword();
            Assert.IsInstanceOf(typeof(string), password);
        }
    }
}