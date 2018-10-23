using NUnit.Framework;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Implementation;
using Service.Tests;
using System.Collections.Generic;
using System.Linq;

namespace Service.Tests
{
    [TestFixture]
    public class ValidateRequestTest
    {
        private IValidateRequest _validateRequest;

        [SetUp]
        public void Setup()
        {
            _validateRequest = new ValidateRequest();
        }

        [Test]
        public void ValidateRequestData_Null_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData<UpdateCustomerContactRequest>(null);
            Assert.IsTrue(string.Equals(validateMessage.ToList()[0],
                ExceptionMessages.InvalidRequest, System.StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ValidateUpdateCustomerContactRequest_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(TestResources.UpdateCustomerContactRequest);
            Assert.AreEqual(null, validateMessage);
        }

        [Test]
        public void ValidateCreateServiceBookingRequest_NullRequest_Test()
        {
            CreateServiceBookingRequest input = null;
            var validateMessage = _validateRequest.ValidateRequestData(input).ToList();
            Assert.IsTrue(validateMessage.Find(x => x.Contains(ExceptionMessages.InvalidRequest)) != null);
        }        
        
        [Test]
        public void ValidateCreateServiceBookingRequest_InvalidDealerId_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(TestResources.CreateServiceBookingRequestVersion_InvalidDealerId).ToList();
            Assert.IsTrue(validateMessage.Find(x => x.Contains(ExceptionMessages.InvalidDealerId)) != null);
        }
        [Test]
        public void ValidateCreateServiceBookingRequest_InvalidCustomerNo_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(TestResources.CreateServiceBookingRequestVersion_InvalidCustomerNo).ToList();
            Assert.IsTrue(validateMessage.Find(x => x.Contains(ExceptionMessages.InvalidCustomerNo)) != null);
        }

        [TearDown]
        public void TearDown()
        {
            _validateRequest = null;
        }
    }
}