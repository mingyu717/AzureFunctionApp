using NUnit.Framework;
using Service.Contract;
using Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using Service.Contract.Models;

namespace Service.Tests
{
    /// <summary>
    /// Test class is responsible for check password with all scenerio(s).
    /// </summary>
    [TestFixture]
    public class ValidateRequestTest
    {
        private IValidateRequest _validateRequest;

        private CustomerVerifyRequest InvalidCustomerVerifyRequest => new CustomerVerifyRequest()
        {
            CommunityId = "EBBETDEV",
            CustomerNo = 12
        };

        [SetUp]
        public void Setup()
        {
            _validateRequest = new ValidateRequest();
        }

        /// <summary>
        /// Test scenerio when input request data is null, then expected result should be "Invalid request"
        /// </summary>
        [Test]
        public void ValidateRequest_Null_Test()
        {
            IEnumerable<String> result = _validateRequest.ValidateRequestData<CustomerVerifyRequest>(null);
            Assert.AreEqual(result.ToList().Count, 1);
            Assert.AreEqual(result.ToList()[0], Constants.ExceptionMessages.InvalidRequest);
        }

        [Test]
        public void ValidateRequest_Test()
        {
            IEnumerable<String> result = _validateRequest.ValidateRequestData(TestResources.CustomerVerifyRequest);
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateRequest_InvalidRequest_Test()
        {
            IEnumerable<String> result = _validateRequest.ValidateRequestData(InvalidCustomerVerifyRequest);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ToList().Count, 1);
        }
    }
}