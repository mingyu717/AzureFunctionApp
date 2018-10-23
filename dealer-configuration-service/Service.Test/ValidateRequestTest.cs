using NUnit.Framework;
using Service.Contract;
using Service.Contract.Request;
using Service.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Service.Test
{
    [TestFixture]
    public class ValidateRequestTest
    {
        private IValidateRequest _validateRequest;

        private readonly DealerConfigurationCreateRequest _dealerConfigurationRequest =
            new DealerConfigurationCreateRequest()
            {
                DealerName = "test",
                RooftopId = "EBBVW11DEV1",
                CommunityId = "EBBETDEV1",
                Address = "abc",
                PhoneNumber = "1234567890",
                EmailAddress="test@email.com",
                Latitude = 28.459497,
                Longitude = 77.026638,
                AppThemeName = "SampleTheme1",
                ShowTransportations = true,
                ShowAdvisors = true,
                ShowPrice = true,
                CommunicationMethodName = "SMS",
                EmailContent="Test EmailContent",
                SmsContent="Test SmsContent",
                EmailSubject = "Test EmailSubject",
                CsvSourceName = "email"
            };

        [SetUp]
        public void Setup()
        {
            _validateRequest = new ValidateRequest();
        }

        /// <summary>
        /// Test scenario, when validation request is null then expected result should be invalid request.
        /// </summary>
        [Test]
        public void ValidateRequestData_Null_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData<DealerConfigurationCreateRequest>(null);
            Assert.IsTrue(string.Equals(validateMessage.ToList()[0].ToString(),
                Constants.ExceptionMessages.InvalidRequest, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Test scenario, when send valid dealerconfiguration request then expected result count should be zero.
        /// </summary>
        [Test]
        public void ValidateDealerConfigurationRequest_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(_dealerConfigurationRequest);
            Assert.AreEqual(null, validateMessage);
        }

        /// <summary>
        /// Test scenario, when rooftopid is null then expected result should be "The rooftopid is required".
        /// </summary>
        [Test]
        public void Required_ValidateDealerConfigurationRequest_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(new DealerConfigurationCreateRequest());
            Assert.IsTrue(string.Equals(validateMessage.ToList()[0].ToString(),
                Constants.ExceptionMessages.DealerNameRequired, System.StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Test scenario, when dealerId is zero then expected result should be "The id field field is required"
        /// </summary>
        [Test]
        public void ValidateDealerId_Zero_Test()
        {
            string validateMessage = _validateRequest.ValidateDealerIdRequest(0);
            Assert.True(string.Equals(validateMessage.ToString(), Constants.ExceptionMessages.DealerIdRequired,
                System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Test scenario, when send valid dealerId the expected result is empty.
        /// </summary>
        [Test]
        public void ValidateDealerId_Test()
        {
            var validateMessage = _validateRequest.ValidateDealerIdRequest(1);
            Assert.AreEqual(string.Empty, validateMessage);
        }

        /// <summary>
        /// Test scenario, when send community id and rooftopId then expected result should be expected error count.
        /// </summary>
        [TestCase("samplerooftop", null, ExpectedResult = 1)]
        [TestCase(null, null, ExpectedResult = 2)]
        [TestCase(null, "samplecommunityId", ExpectedResult = 1)]
        [TestCase("samplerooftop", "samplecommunityId", ExpectedResult = 0)]
        public int ValidateDealerRoofTopIdOrCommunityId_Null_Test(string rooftopId, string communityId)
        {
            IEnumerable<string> validateMessage =
                _validateRequest.ValidateDealerRoofTopIdAndCommunityIdRequest(rooftopId, communityId);
            return validateMessage.Count();
        }

        [TearDown]
        public void TearDown()
        {
            _validateRequest = null;
        }
    }
}