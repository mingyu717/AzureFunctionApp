using NUnit.Framework;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.RequestModel;
using Service.Implementation;
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
            var validateMessage = _validateRequest.ValidateRequestData<CreateAppointmentRequest>(null);
            Assert.IsTrue(string.Equals(validateMessage.ToList()[0],
                ExceptionMessages.InvalidRequest, System.StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void ValidateCreateAppointmentRequest_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(TestResources.CreateAppointmentRequest);
            Assert.AreEqual(null, validateMessage);
        }

        [Test]
        public void ValidateCreateAppointmentRequest_NullRequest_Test()
        {
            CreateAppointmentRequest input = null;
            var validateMessage = _validateRequest.ValidateRequestData(input).ToList();
            Assert.IsTrue(validateMessage.Find(x => x.Contains(ExceptionMessages.InvalidRequest)) != null);
        }

        [Test]
        public void ValidateCreateServiceBookingRequest_InvalidDealerId_Test()
        {
            var validateMessage = _validateRequest.ValidateRequestData(TestResources.CreateAppointmentRequest_InvalidDealerId).ToList();
            Assert.IsTrue(validateMessage.Find(x => x.Contains(ExceptionMessages.InvalidDealerId)) != null);
        }

        [TearDown]
        public void TearDown()
        {
            _validateRequest = null;
        }
    }
}
